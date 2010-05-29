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
    class GameObject
    {

        public Vector2 Position { get; protected set; }
        public Texture2D Texture { get; protected set; }
        public Vector2 Origin { get; protected set; }
        public float Rotation { get; protected set; }
        public float Scale { get; protected set; }
        public static int screenHeight { get; set; }
        public static int screenWidth { get; set; }
        public int HitPoints { get; set; }
        public Color ObjectColor = new Color(255, 255, 255, 255);
        public byte alpha = 255;

        public BoundingSphere boundingSphere;

        public static Random rand = new Random();

        public GameObject()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Texture, this.Position, null, ObjectColor, this.Rotation, this.Origin, this.Scale, SpriteEffects.None, 0);
        }

        public bool isCollission(GameObject gameObject)
        {
            return boundingSphere.Intersects(gameObject.boundingSphere);
        }
    }
}
