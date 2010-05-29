using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Game
{
    class Background : GameObject
    {
        public Background(Texture2D texture, Vector2 position)
        {
            this.Position = position;
            this.Texture = texture;
            this.Origin = Vector2.Zero;
            this.Scale = 1f;
            this.Rotation = 0f;
        }
    }
}
