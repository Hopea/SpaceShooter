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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D backgroundTexture;
        private Texture2D shipTexture;
        private Texture2D bulletTexture;
        private Texture2D explosionTexture;
        private SpriteFont font;

        private Song music;
        private SoundEffect effect;

        private Background background;
        private Ship ship;
        private List<Bullet> bullets;
        private List<GameObject> gameObjects;
        private List<Explosion> explosions;
        private double timeSinceFire = 0;

        private int enemyCount = 2;
        private int killCount = 0;
        private int shotCount = 0;
        private int nextLevelCounter = 1000;
        private const int NEXT_LEVEL_WAIT = 1000;

        private int gameOverTime = 2000;
        private bool continueFading = false;
        private byte continueFade = 0;

        // 1 = Running
        // 2 = Game over
        private int GameStatus = 1;

        private Random rand;

        //const float MAX_VELOCITY = 6;
        //const float ACCELERATION = 0.5f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            //graphics.PreferredBackBufferHeight = 1200;
            //graphics.PreferredBackBufferWidth = 1920;
            //graphics.ToggleFullScreen();

            Content.RootDirectory = "Content";
            GameObject.screenWidth = graphics.PreferredBackBufferWidth;
            GameObject.screenHeight = graphics.PreferredBackBufferHeight;
            bullets = new List<Bullet>();
            gameObjects = new List<GameObject>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            rand = new Random();
            base.Initialize();

            this.background = new Background(this.backgroundTexture, Vector2.Zero);
            this.ship = new Ship(this.shipTexture, new Vector2(350, 260));
            this.explosions = new List<Explosion>();
            createEnemies();

            MediaPlayer.Play(this.music);
            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //this.backgroundTexture = Content.Load<Texture2D>("xnabackground");
            this.backgroundTexture = Content.Load<Texture2D>("Space_Planet");
            this.shipTexture = Content.Load<Texture2D>("ship");
            this.font = Content.Load<SpriteFont>("font");
            this.bulletTexture = Content.Load<Texture2D>("bullet1x1");
            this.explosionTexture = Content.Load<Texture2D>("explosion");

            this.music = Content.Load<Song>("biisi");
            this.effect = Content.Load<SoundEffect>("explode1");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (GameStatus == 2)
            {
                gameOverTime -= gameTime.ElapsedGameTime.Milliseconds;
                if (gameOverTime < 0)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        ResetGame();
                    }
                }
                // Game over, no updating
                base.Update(gameTime);
                return;
            }

            ship.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && timeSinceFire > 500)
            {
                timeSinceFire = 0;
                shotCount++;
                Fire();
            }
            else
            {
                timeSinceFire += gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            // TODO: Add your update logic here
            List<Bullet> removed = new List<Bullet>();
            foreach (Bullet bullet in bullets)
            {
                bullet.Update(gameTime);
                if (!bullet.isInScreen())
                {
                    removed.Add(bullet);
                }
            }

            foreach (Bullet bullet in removed)
            {
                bullets.Remove(bullet);
            }

            foreach (Enemy g in this.gameObjects)
            {
                g.Update(gameTime, ship.Position);
            }

            checkCollissions();

            if (gameObjects.Count == 0)
            {
                nextLevelCounter -= gameTime.ElapsedGameTime.Milliseconds;
                if (nextLevelCounter < 0)
                {
                    createEnemies();
                    nextLevelCounter = NEXT_LEVEL_WAIT;
                }
            }

            if (ship.HitPoints <= 0)
            {
                GameStatus = 2;
            }

            base.Update(gameTime);
        }

        private void ResetGame()
        {
            // Starting again.
            gameOverTime = 2000;
            GameStatus = 1;
            continueFading = false;
            continueFade = 0;

            timeSinceFire = 0;
            enemyCount = 2;
            killCount = 0;
            shotCount = 0;
            nextLevelCounter = 1000;

            gameObjects = new List<GameObject>();
            bullets = new List<Bullet>();

            this.ship = new Ship(this.shipTexture, new Vector2(350, 260));
            createEnemies();
        }

        private void createEnemies()
        {
            for (int i = 0; i <= enemyCount; i++)
            {
                int x = rand.Next(20, graphics.PreferredBackBufferWidth - 20);
                int y = rand.Next(20, graphics.PreferredBackBufferHeight - 20);
                
                this.gameObjects.Add(new Enemy(this.shipTexture, new Vector2(x, y), (rand.Next(2) == 1)));
            }

            enemyCount++;
        }

        /**
         * Checks collissions with bullets and enemies.
         */
        private void checkCollissions()
        {
            List<GameObject> collidedBullets = new List<GameObject>();
            List<GameObject> collidedEnemies = new List<GameObject>();
            foreach (Bullet bullet in bullets)
            {
                if (bullet.alpha < 170)
                    continue;
                foreach (Enemy enemy in gameObjects)
                {
                    if (!enemy.isReady())
                        continue;
                    if (enemy.isCollission(bullet))
                    {
                        if (!collidedEnemies.Contains(enemy))
                        {
                            collidedEnemies.Add(enemy);
                        }
                        if (!collidedBullets.Contains(bullet))
                        {
                            collidedBullets.Add(bullet);
                        }
                        
                    }
                }
            }

            foreach (Enemy enemy in gameObjects)
            {
                if (!enemy.isReady())
                    continue;
                if(enemy.isCollission(ship))
                {
                    collidedEnemies.Add(enemy);
                    ship.HitPoints -= 5;
                }
            }

            killCount += collidedEnemies.Count;
            foreach (GameObject obj in collidedEnemies)
            {
                gameObjects.Remove(obj);
                showExplosion(obj.Position);
            }
            foreach (Bullet bullet in collidedBullets)
            {
                bullets.Remove(bullet);
            }
            
        }

        private void showExplosion(Vector2 vector2)
        {
            this.effect.Play();

            for (int i = 0; i < 30; i++)
            {
                float velocityX = (float)(rand.NextDouble() - 0.5) * 4;
                float velocityY = (float)(rand.NextDouble() - 0.5) * 4;
                Vector2 velocity = new Vector2(velocityX, velocityY);
                int rndColor = rand.Next(2);
                Color color = Color.Red;
                if (rndColor == 1)
                {
                    color = Color.Orange;
                }

                Bullet bullet = new Bullet(bulletTexture, vector2, velocity, 500, color);
                bullets.Add(bullet);

                explosions.Add(new Explosion(explosionTexture, vector2));
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            background.Draw(spriteBatch);
            ship.Draw(spriteBatch);

            drawScore(spriteBatch);

            foreach (GameObject g in this.gameObjects)
            {
                g.Draw(spriteBatch);
            }

            foreach (Explosion explosion in this.explosions)
            {
                explosion.Draw(spriteBatch);
            }
            while (explosions.Count > 0)
                explosions.RemoveAt(0);

            spriteBatch.End();

            spriteBatch.Begin(SpriteBlendMode.Additive);

            foreach (Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }

            // Draw game over screen
            if (GameStatus == 2)
            {
                DrawGameOver(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGameOver(SpriteBatch spriteBatch)
        {
            String GameOverLine = "  Score: " + killCount + "\nGAME OVER";
            spriteBatch.DrawString(this.font, GameOverLine, new Vector2(300, 210), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);

            if (gameOverTime < 0)
            {
                // Draw continue text after wait time
                GameOverLine = "Press space to continue";
                Color color = Color.White;
                if (continueFading)
                {
                    continueFade--;
                    if (continueFade == 0)
                        continueFading = false;
                }
                else
                {
                    continueFade++;
                    if (continueFade == 255)
                        continueFading = true;
                }
                color.A = continueFade;
                spriteBatch.DrawString(this.font, GameOverLine, new Vector2(220, 300), color, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            }
        }

        private void drawScore(SpriteBatch spriteBatch)
        {
            String scoreLine = "Energy: \nKills: " + killCount + "\nShots: " + shotCount;
            spriteBatch.DrawString(this.font, scoreLine, new Vector2(5, 5), Color.White, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);

            spriteBatch.Draw(bulletTexture, new Rectangle(110, 15, 200, 20), Color.Red);
            spriteBatch.Draw(bulletTexture, new Rectangle(110, 15, 2*ship.HitPoints, 20), Color.LawnGreen);
        }

        public void Fire()
        {
            Random rand = new Random();
            for (int i = 0; i < 20; i++)
            {
                float velocityX = -1 * (float)Math.Sin((double)ship.Rotation) * 6 + (float)(rand.NextDouble() / 0.99 - 1);
                float velocityY = (float)Math.Cos((double)ship.Rotation) * 6 + (float)(rand.NextDouble() / 0.99 + 1);
                Vector2 velocity = new Vector2(velocityX, velocityY);
                velocity += ship.velocity;

                Bullet bullet = new Bullet(bulletTexture, ship.Position, velocity, 2000);
                bullets.Add(bullet);
            }
        }
    }
}
