using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Voxel2
{
    class GamePlayState
    {
        Effect effect;
        
        RasterizerState rsWire, rsSolid;
        bool rs = true;

        Camera Camera;
        public static Camera CameraInstance;

        World world;

        public static Vector3 lightPos = new Vector3(30, 30, 30);
        float ambientPower;
        float lightPower;
        Matrix lightsViewProjectionMatrix;
        float gamma = 1f;

        RenderTarget2D renderTarget;
        Texture2D shadowMap;

        VertexPositionNormalTexture[] TargetVertices;
        int[] TargetIndices;
        int faceCount;

        bool pause;
        bool useShadows = false;


        public GamePlayState()
        {
            effect = Static.Effect;
            effect.Parameters["xTexture"].SetValue(Static.TileSheet);

            PresentationParameters pp = Static.Device.PresentationParameters;
            renderTarget = new RenderTarget2D(Static.Device, pp.BackBufferWidth, pp.BackBufferHeight, true, Static.Device.DisplayMode.Format, DepthFormat.Depth24);

            rsWire = new RasterizerState();
            rsWire.FillMode = FillMode.WireFrame;
            rsSolid = new RasterizerState();
            rsSolid.FillMode = FillMode.Solid;

            Camera = new Camera(new Vector3(0, 30, 0), Vector3.Zero, 200);
            Camera.UpdateViewMatrix();
            CameraInstance = Camera;

            world = new World();
        }

        public void Update(GameTime gameTime)
        {
            ProcessInput();
            if (rs)
                Static.Device.RasterizerState = rsSolid;
            else
                Static.Device.RasterizerState = rsWire;

            if (pause)
                return;

            UpdateLightData();
            world.Update();
            Camera.Update();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (useShadows)
            {
                Static.Device.SetRenderTarget(renderTarget);

                Static.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
                DrawScene("ShadowMap");
                Static.Device.SetRenderTarget(null);
                shadowMap = (Texture2D)renderTarget;
            }
            

            Static.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

            //DrawScene("ShadowMap");
            if(useShadows)
                DrawScene("ShadowedScene");
            else
                DrawScene("Simplest");

            //spriteBatch.Begin();
            //spriteBatch.Draw(shadowMap, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 1);
            //spriteBatch.End();

            spriteBatch.Begin();
            if (pause)
            {
                spriteBatch.Draw(Static.Cursor, Input.MousePosition, Color.White);
                spriteBatch.DrawString(Static.FontBig, "PAUSE", Static.Center("PAUSE", Static.FontBig), Color.White);
            }
            spriteBatch.End();
            

            Static.Device.BlendState = BlendState.Opaque;
            Static.Device.DepthStencilState = DepthStencilState.Default;

            shadowMap = null;
        }

        void ProcessInput()
        {
            if (Input.mouseState.LeftButton == ButtonState.Pressed && Input.oldMouseState.LeftButton == ButtonState.Released)
                ModifyTerrain.ReplaceBlockCursor(0);
            if (Input.mouseState.RightButton == ButtonState.Pressed && Input.oldMouseState.RightButton == ButtonState.Released)
                ModifyTerrain.AddBlockCursor(5);

            if (Input.keyboardState.IsKeyDown(Keys.Space))
                ModifyTerrain.ReplaceBlockCursor(0);

            if (Input.keyboardState.IsKeyDown(Keys.R) && Input.oldState.IsKeyUp(Keys.R))
                rs = !rs;

            if (Input.keyboardState.IsKeyDown(Keys.T))
                lightPos.X++;
            if (Input.keyboardState.IsKeyDown(Keys.G))
                lightPos.X--;

            if (Input.keyboardState.IsKeyDown(Keys.W) && Input.oldState.IsKeyUp(Keys.W))
                if (gamma > 0.2f)
                gamma -= 0.1f;
            if (Input.keyboardState.IsKeyDown(Keys.X) && Input.oldState.IsKeyUp(Keys.X))
                gamma += 0.1f;

            if (Input.keyboardState.IsKeyDown(Keys.Escape) && Input.oldState.IsKeyUp(Keys.Escape))
            {
                pause = !pause;
                Mouse.SetPosition((int)Static.ScreenSize.X / 2, (int)Static.ScreenSize.Y / 2);
            }
        }

        private void UpdateLightData()
        {
            lightPower = 2.0f;
            ambientPower = 0.3f;

            Matrix lightsView = Matrix.CreateLookAt(lightPos, new Vector3(32, 0, 32), new Vector3(0, 1, 0));
            Matrix lightsProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 5f, 1000f);

            lightsViewProjectionMatrix = lightsView * lightsProjection;
        }

        void DrawScene(string technique)
        {
            effect.CurrentTechnique = effect.Techniques[technique];
            effect.Parameters["xWorldViewProjection"].SetValue(Camera.ViewMatrix * Camera.ProjectionMatrix);
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);

            effect.Parameters["xLightPos"].SetValue(lightPos);
            effect.Parameters["xLightPower"].SetValue(lightPower);
            effect.Parameters["xAmbient"].SetValue(ambientPower);
            effect.Parameters["xLightsWorldViewProjection"].SetValue(Matrix.Identity * lightsViewProjectionMatrix);
            effect.Parameters["xShadowMap"].SetValue(shadowMap);
            effect.Parameters["xGamma"].SetValue(gamma);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                world.Draw();
                DrawTargetCube();
            }
        }

        void DrawTargetCube()
        {
            List<Vector3> newVertices = new List<Vector3>();
            List<Vector3> newNormals = new List<Vector3>();
            List<Vector2> newUV = new List<Vector2>();
            List<int> newTriangles = new List<int>();

            newNormals.Clear();
            newTriangles.Clear();
            newUV.Clear();
            newVertices.Clear();
            faceCount = 0;
            RayCastHit hit = RayCast.RayCasting(Camera, Static.maxRayCastDistance);
            Vector3 position = hit.Position;
            position -= 0.5f * hit.Normal;

            int x = (int)Math.Floor(position.X);
            int y = (int)Math.Floor(position.Y);
            int z = (int)Math.Floor(position.Z);

            Vector2 texturePos = new Vector2(0, 0);

            //top
            newVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 1 - 0.5f));
            newVertices.Add(new Vector3(x + 1 - 0.5f, y + 0.5f, z + 1 - 0.5f));
            newVertices.Add(new Vector3(x + 1 - 0.5f, y + 0.5f, z - 0.5f));
            newVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
            for (int i = 0; i < 4; i++)
                newNormals.Add(new Vector3(0, 1, 0));
            MakeCube(texturePos, newTriangles, newUV);

            //bot
            newVertices.Add(new Vector3(x - 0.5f, y - 1 + 0.5f, z - 0.5f));
            newVertices.Add(new Vector3(x + 1 - 0.5f, y - 1 + 0.5f, z - 0.5f));
            newVertices.Add(new Vector3(x + 1 - 0.5f, y - 1 + 0.5f, z + 1 - 0.5f));
            newVertices.Add(new Vector3(x - 0.5f, y - 1 + 0.5f, z + 1 - 0.5f));
            for (int i = 0; i < 4; i++)
                newNormals.Add(new Vector3(0, 1, 0));
            MakeCube(texturePos, newTriangles, newUV);

            //west
            newVertices.Add(new Vector3(x - 0.5f, y - 1 + 0.5f, z + 1 - 0.5f));
            newVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 1 - 0.5f));
            newVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
            newVertices.Add(new Vector3(x - 0.5f, y - 1 + 0.5f, z - 0.5f));
            for (int i = 0; i < 4; i++)
                newNormals.Add(new Vector3(0, 1, 0));
            MakeCube(texturePos, newTriangles, newUV);

            //north
            newVertices.Add(new Vector3(x + 1 - 0.5f, y - 1 + 0.5f, z + 1 - 0.5f));
            newVertices.Add(new Vector3(x + 1 - 0.5f, y + 0.5f, z + 1 - 0.5f));
            newVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 1 - 0.5f));
            newVertices.Add(new Vector3(x - 0.5f, y - 1 + 0.5f, z + 1 - 0.5f));
            for (int i = 0; i < 4; i++)
                newNormals.Add(new Vector3(0, 1, 0));
            MakeCube(texturePos, newTriangles, newUV);

            //east
            newVertices.Add(new Vector3(x + 1 - 0.5f, y - 1 + 0.5f, z - 0.5f));
            newVertices.Add(new Vector3(x + 1 - 0.5f, y + 0.5f, z - 0.5f));
            newVertices.Add(new Vector3(x + 1 - 0.5f, y + 0.5f, z + 1 - 0.5f));
            newVertices.Add(new Vector3(x + 1 - 0.5f, y - 1 + 0.5f, z + 1 - 0.5f));
            for (int i = 0; i < 4; i++)
                newNormals.Add(new Vector3(0, 1, 0));
            MakeCube(texturePos, newTriangles, newUV);

            //south
            newVertices.Add(new Vector3(x - 0.5f, y - 1 + 0.5f, z - 0.5f));
            newVertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
            newVertices.Add(new Vector3(x + 1 - 0.5f, y + 0.5f, z - 0.5f));
            newVertices.Add(new Vector3(x + 1 - 0.5f, y - 1 + 0.5f, z - 0.5f));
            for (int i = 0; i < 4; i++)
                newNormals.Add(new Vector3(0, 1, 0));
            MakeCube(texturePos, newTriangles, newUV);


            if (newVertices.Count == 0)
                return;

            TargetIndices = newTriangles.ToArray();
            TargetVertices = new VertexPositionNormalTexture[newVertices.Count];

            for (int i = 0; i < newVertices.Count; i++)
            {
                TargetVertices[i].Position = newVertices[i];
                TargetVertices[i].TextureCoordinate = newUV[i];
                TargetVertices[i].Normal = newNormals[i];
            }

            Static.Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, TargetVertices, 0, TargetVertices.Length, TargetIndices, 0, (int)TargetIndices.Length / 3, VertexPositionNormalTexture.VertexDeclaration); ;
        }

        void MakeCube(Vector2 texturePos, List<int> newTriangles, List<Vector2> newUV)
        {
            float tUnit = 0.25f;

            newTriangles.Add(faceCount * 4 + 2);
            newTriangles.Add(faceCount * 4 + 1);
            newTriangles.Add(faceCount * 4);
            newTriangles.Add(faceCount * 4 + 3);
            newTriangles.Add(faceCount * 4 + 2);
            newTriangles.Add(faceCount * 4);

            newUV.Add(new Vector2(tUnit * texturePos.X + tUnit, tUnit * texturePos.Y));
            newUV.Add(new Vector2(tUnit * texturePos.X + tUnit, tUnit * texturePos.Y + tUnit));
            newUV.Add(new Vector2(tUnit * texturePos.X, tUnit * texturePos.Y + tUnit));
            newUV.Add(new Vector2(tUnit * texturePos.X, tUnit * texturePos.Y));

            faceCount++;
        }

    }
}
