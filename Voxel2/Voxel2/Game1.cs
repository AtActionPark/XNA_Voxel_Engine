using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Voxel2
{
    public enum State
    {
        GamePlayState,
        StartState
    }

    class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        float updateFPS, drawFPS;
        public static GraphicsDevice Device;

        GamePlayState GamePlayState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            graphics.PreferredBackBufferWidth = (int)Static.ScreenSize.X;
            graphics.PreferredBackBufferHeight = (int)Static.ScreenSize.Y;
            graphics.ApplyChanges();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Device = graphics.GraphicsDevice;


            Static.Load(Content, Device);
            GamePlayState = new GamePlayState();
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            GamePlayState.Update(gameTime);
            
            if (gameTime.ElapsedGameTime.Milliseconds != 0)
                updateFPS = 1000 / gameTime.ElapsedGameTime.Milliseconds;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GamePlayState.Draw(gameTime, spriteBatch);

            if (gameTime.ElapsedGameTime.Milliseconds != 0)
                drawFPS = 1000 / gameTime.ElapsedGameTime.Milliseconds;
            this.Window.Title = string.Concat("Update : " + updateFPS + " fps   " + "Draw : " + drawFPS + " fps");

            base.Draw(gameTime);
        }
    }
}
