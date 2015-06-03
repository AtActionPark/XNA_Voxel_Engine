using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Voxel2
{
    public static class Static
    {
        public static Vector2 ScreenSize = new Vector2(800, 480);
        public static float maxRayCastDistance = 20;
        public static Texture2D TileSheet{get;private set;}
        public static Texture2D Cursor { get; private set; }
        public static Effect Effect{get;private set;}
        public static GraphicsDevice Device { get; private set; }
        public static SpriteFont FontBig { get; private set; }

        public static void Load(ContentManager Content, GraphicsDevice device)
        {
            TileSheet = Content.Load<Texture2D>("tileSheet");
            Cursor = Content.Load<Texture2D>("Cursor");
            Effect = Content.Load<Effect>("Effect1");
            Device = device;
            FontBig = Content.Load<SpriteFont>("FontBig");
        }
        public static Vector2 Center(string str, SpriteFont font)
        {
           Vector2 textWidth = font.MeasureString(str);
           float x = ScreenSize.X / 2 - textWidth.X / 2;
           float y = ScreenSize.Y / 2 - textWidth.Y / 2;
            return new Vector2(x,y);
           
        }

    }
}
