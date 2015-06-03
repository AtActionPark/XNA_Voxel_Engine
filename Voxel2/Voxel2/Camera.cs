using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Voxel2
{
    class Camera
    {
        public static Camera Instance;

        private float moveScale = 1f;
        public Vector3 Position;
        public Vector3 target;

        public Matrix ViewMatrix, ProjectionMatrix;

        private float leftRightRot = -3*(float)Math.PI/4;
        private float upDownRot = -(float)Math.PI/4;
        private float rotationSpeed = 0.005f;

        private MouseState originalMouseState;
        Vector3 up = Vector3.Up;
        private float sensibility = 0.5f;
        public Matrix cameraRotation;

        public BoundingFrustum boundingFrustrum;


        public Camera(Vector3 position, Vector3 target, float farDistance)
        {
            this.Position = position;
            this.target = target;

            Mouse.SetPosition((int)Static.ScreenSize.X / 2, (int)Static.ScreenSize.Y / 2);
            originalMouseState = Mouse.GetState();

            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Static.Device.Viewport.AspectRatio, 1f, farDistance);
            boundingFrustrum = new BoundingFrustum(ViewMatrix * ProjectionMatrix);
            Instance = this;
        }

        public void Update()
        {
            KeyboardState keyState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();

            if (currentMouseState != originalMouseState)
            {
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;
                leftRightRot -= rotationSpeed * xDifference;
                upDownRot -= rotationSpeed * yDifference;
                Mouse.SetPosition((int)Static.ScreenSize.X / 2, (int)Static.ScreenSize.Y / 2);
                UpdateViewMatrix();
                currentMouseState = originalMouseState;
            }

            #region keys
            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.Z))      //Forward
            {
                AddToCameraPosition(new Vector3(0, 0, -sensibility));

            }
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))    //Backward
            {
                AddToCameraPosition(new Vector3(0, 0, sensibility));

            }
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))   //Right
            {
                AddToCameraPosition(new Vector3(sensibility, 0, 0));

            }
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.Q))    //Left
            {
                AddToCameraPosition(new Vector3(-sensibility, 0, 0));

            }
            if (keyState.IsKeyDown(Keys.P))                                     //Up
            {
                AddToCameraPosition(new Vector3(0, sensibility, 0));

            }
            if (keyState.IsKeyDown(Keys.M))                                     //Down
            {
                AddToCameraPosition(new Vector3(0, -sensibility, 0));

            }
            #endregion
        }
        public void UpdateViewMatrix()
        {
            cameraRotation = Matrix.CreateRotationX(upDownRot) * Matrix.CreateRotationY(leftRightRot);
            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            target = Position + cameraRotatedTarget;
            up = Vector3.Transform(cameraOriginalUpVector, cameraRotation);
            ViewMatrix = Matrix.CreateLookAt(Position, target, up);
            boundingFrustrum = new BoundingFrustum(ViewMatrix * ProjectionMatrix);
        }

        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(upDownRot) * Matrix.CreateRotationY(leftRightRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            Position += moveScale * rotatedVector;
            UpdateViewMatrix();
        }
    }
}
