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
    class Ship : GameObject
    {
        private float speed;
        private float turningSpeed;
        public Vector2 velocity;

        public const float MAX_VELOCITY = 6;
        public const float ACCELERATION = 0.5f;
        //private Vector2 shipPosition;
        //private float shipRotation;
        //private Vector2 shipVelocity;

        public Ship(Texture2D texture, Vector2 Position)
        {
            this.Position = Position;
            this.Texture = texture;
            this.speed = 0.005f;
            this.turningSpeed = 0.01f;
            this.Scale = 0.14f;
            this.Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            this.Rotation = (float)Math.PI;
            this.velocity = Vector2.Zero;
            this.HitPoints = 100;

            this.boundingSphere = new BoundingSphere(new Vector3(Position.X, Position.Y, 0), (float)(Texture.Width / 2 * Scale * 0.90));
        }

        public void Update(GameTime gametime)
        {
            // Modify velocity
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Rotation -= this.turningSpeed * gametime.ElapsedGameTime.Milliseconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Rotation += this.turningSpeed * gametime.ElapsedGameTime.Milliseconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                // Accelerating
                float velocityX = (float)Math.Sin((double)Rotation) * MAX_VELOCITY;
                float velocityY = (float)Math.Cos((double)Rotation) * MAX_VELOCITY;

                this.velocity += new Vector2(velocityX * -1, velocityY) * gametime.ElapsedGameTime.Milliseconds * this.speed;
                this.velocity = this.velocity * 0.94f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                this.velocity = this.velocity * 0.90f;
            }

            this.Position += this.velocity;

            // Move to other side of the screen if necessary.
            if (this.Position.X > screenWidth + Texture.Width / 2)
            {
                this.Position = new Vector2(this.Position.X - screenWidth - Texture.Width / 2, this.Position.Y);
            }
            if (this.Position.X < 0 - Texture.Width / 2)
            {
                this.Position = new Vector2(this.Position.X + screenWidth + Texture.Width / 2, this.Position.Y);
            }
            if (this.Position.Y > screenHeight + Texture.Height / 2)
            {
                this.Position = new Vector2(this.Position.X, this.Position.Y - screenHeight - Texture.Height / 2);
            }
            if (this.Position.Y < 0 - Texture.Height / 2)
            {
                this.Position = new Vector2(this.Position.X, this.Position.Y + screenHeight + Texture.Height / 2);
            }

            this.boundingSphere.Center = new Vector3(Position.X, Position.Y, 0);
        }
    }
}
