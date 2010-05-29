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
    class Bullet : GameObject
    {
        Vector2 velocity;
        // Use -1000 for infinite.
        int lifetime;
        //byte Alpha;

        public Bullet(Texture2D texture, Vector2 Position, Vector2 velocity, int lifetime)
        {
            this.Position = Position;
            this.Texture = texture;
            this.Scale = 1f;
            this.Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            this.Rotation = 0f;
            this.velocity = velocity;
            this.lifetime = lifetime;

            this.boundingSphere = new BoundingSphere(new Vector3(Position.X, Position.Y, 0), (float)(Texture.Width / 2));
        }

        public Bullet(Texture2D texture, Vector2 Position, Vector2 velocity, int lifetime, Color color)
        {
            this.ObjectColor = color;
            this.Position = Position;
            this.Texture = texture;
            this.Scale = 2f;
            this.Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            this.Rotation = 0f;
            this.velocity = velocity;
            this.lifetime = lifetime;

            this.boundingSphere = new BoundingSphere(new Vector3(Position.X, Position.Y, 0), (float)(Texture.Width / 2));
        }

        public void Update(GameTime gametime)
        {
            lifetime -= gametime.ElapsedGameTime.Milliseconds;
            if (lifetime < alpha)
            {
                alpha = (byte)lifetime;
                this.ObjectColor.A = alpha;
            }
            this.Position += this.velocity;
            this.boundingSphere.Center = new Vector3(Position.X, Position.Y, 0);
        }

        public Boolean isInScreen()
        {
            if (lifetime < 0 && lifetime > -1000)
            {
                return false;
            }
            Rectangle area = new Rectangle(0, 0, screenWidth, screenHeight);
            return area.Contains((int)Position.X, (int)Position.Y);
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Color shade = this.ObjectColor;
            shade.A = (byte)(shade.A / 4);
            spriteBatch.Draw(this.Texture, this.Position, null, shade, 0, Vector2.Zero, 3, SpriteEffects.None, 0);
        }
    }
}
