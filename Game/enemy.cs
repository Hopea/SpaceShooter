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
    class Enemy : GameObject
    {
        private float speed;
        private Vector2 targetPosition;
        private Vector2 velocity;
        private int elapsedTime = 0;

        private float frictionFactor;
        private float speedFactor;
        private bool isSlow;

        public Enemy(Texture2D texture, Vector2 position, bool isSlow)
        {
            if (isSlow)
            {
                frictionFactor = 0.06f;
                speedFactor = 0.1f;
            }
            else
            {
                frictionFactor = 0.000001f;
                speedFactor = 0.98f;
            }
            alpha = 0;
            ObjectColor = new Color(0, 128, 128, 0);
            this.ObjectColor.A = alpha;
            this.Position = position;
            this.Texture = texture;
            this.speed = (float)(rand.Next(15, 50) * frictionFactor);
            //this.speed = 0.00002f;
            this.Scale = 0.14f;
            this.Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            this.Rotation = 0f;
            this.velocity = Vector2.Zero;
            this.HitPoints = 5;
            this.isSlow = isSlow;

            this.boundingSphere = new BoundingSphere(new Vector3(Position.X, Position.Y, 0), (float)(Texture.Width / 2 * Scale * 0.90));


        }

        public bool isReady()
        {
            return alpha == 255;
        }

        public void Update(GameTime gametime, Vector2 targetPosition)
        {
            elapsedTime += gametime.ElapsedGameTime.Milliseconds;
            if (alpha < 255)
            {
                // 2 second spawn time.
                int tmpAlpha = (int)(elapsedTime / 3.0);
                
                if (tmpAlpha >= 255)
                {
                    //Starting movement.
                    alpha = 255;
                    this.ObjectColor.A = alpha;
                }
                else
                {
                    alpha = (byte)tmpAlpha;
                    this.ObjectColor.A = alpha;
                    // Continuing appearing
                    return;
                }
            }
            this.targetPosition = targetPosition;

            if (isSlow)
            {
                float angle = (float)Math.Atan2(-(double)(this.targetPosition.X - this.Position.X), (double)(this.targetPosition.Y - this.Position.Y));
                this.Rotation = angle;
                this.velocity = new Vector2(this.targetPosition.X - this.Position.X, this.targetPosition.Y - this.Position.Y);
                double length = (float)(velocity.Y / Math.Cos(angle));
                length = (length < 0) ? -1 * length : length;
                this.velocity = velocity * (float)(speedFactor / length * (float)gametime.ElapsedGameTime.Milliseconds);
                this.velocity = this.velocity * speed;
                this.Position += this.velocity;
            }
            else
            {
                float angle = (float)Math.Atan2(-(double)(this.targetPosition.X - this.Position.X), (double)(this.targetPosition.Y - this.Position.Y));
                this.Rotation = angle;
                this.velocity += new Vector2(this.targetPosition.X - this.Position.X, this.targetPosition.Y - this.Position.Y) * gametime.ElapsedGameTime.Milliseconds * this.speed;
                this.velocity = this.velocity * speedFactor;
                this.Position += this.velocity;
            }
            this.boundingSphere.Center = new Vector3(Position.X, Position.Y, 0);
        }
    }
}
