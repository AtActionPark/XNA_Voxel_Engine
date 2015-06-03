using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Voxel2
{
    public static class Input
    {
        public static KeyboardState keyboardState, oldState;
        public static MouseState mouseState, oldMouseState;
        public static Vector2 MousePosition;

        public static void Update()
        {
            oldMouseState = mouseState;
            oldState = keyboardState;
            mouseState = Mouse.GetState();
            keyboardState = Keyboard.GetState();
            MousePosition = new Vector2((int)mouseState.X, (int)mouseState.Y);
        }

    }
}
