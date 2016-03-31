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

namespace ZombiePong
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D background, spritesheet, scribblesheet1, paddleSheet;
        Random rand = new Random(System.Environment.TickCount);

        Song song;

        Sprite paddle1, paddle2, ball;

        int playerScore = 0;
            int AIScore = 0;

        List<Sprite> zombies = new List<Sprite>();

        float AITimerMax = 200f;
        float AITimer = 0f;
        float ballspeed = 500f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();
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

            base.Initialize();
        }

        public void UpdateScore()
        {
            // Do nothing until I am programmed
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("background");
            spritesheet = Content.Load<Texture2D>("spritesheet");
            scribblesheet1 = Content.Load<Texture2D>("scribblesheet1");
            paddleSheet = Content.Load<Texture2D>("paddle1");

            paddle1 = new Sprite(new Vector2(20, 20), paddleSheet, new Rectangle(0, 0, 25, 150), Vector2.Zero);
            paddle2 = new Sprite(new Vector2(970, 20), spritesheet, new Rectangle(32, 516, 25, 150), Vector2.Zero);
            ball = new Sprite(new Vector2(400, 350), scribblesheet1, new Rectangle(0, 0, 30, 30), new Vector2(120, 0));

            for (int i = 1; i < 5; i++)
            {
                paddle1.AddFrame(new Rectangle(25 * i, 0, 25, 150));
            }

            SpawnZombie(new Vector2(400, 400), new Vector2(-40, 0));

            song = Content.Load<Song>(@"Sound\PictoChat_-_Super_Smash_Bros");

            MediaPlayer.Play(song);

            

            for (int i = 1; i < 5; i++)
            {
                ball.AddFrame(new Rectangle(ball.BoundingBoxRect.Width * i, 0, ball.BoundingBoxRect.Width, ball.BoundingBoxRect.Height));
            }
            
            MouseState ms = Mouse.GetState();
            paddle1.Location = new Vector2(paddle1.Location.X, ms.Y);

            SpawnZombie(new Vector2(400, 400), new Vector2(-20, 0));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void SpawnZombie(Vector2 location, Vector2 velocity)
        {
            Sprite zombie = new Sprite(location, spritesheet, new Rectangle(0, 25, 160, 150), velocity);

            for (int i = 1; i < 10; i++)
            {
                zombie.AddFrame(new Rectangle(i * 165, 25, 160, 150));
            }

            zombies.Add(zombie);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();


            // TODO: Add your update logic here
            MediaPlayer.IsRepeating = true;

            ball.Update(gameTime);
            paddle1.Update(gameTime);
            paddle2.Update(gameTime);

            for (int i = zombies.Count - 1; i >= 0; i--)
            {
                zombies[i].Update(gameTime);
              


                // Zombie logic goes here..
                zombies[i].FlipHorizontal = false;
                zombies[i].FlipHorizontal = zombies[i].Velocity.X > 0;

                if (zombies[i].Location.X <= 0)
                {
                    zombies[i].Velocity = new Vector2(40, 0);
                }
                if (zombies[i].Location.X > 900)
                {
                    zombies[i].Velocity = new Vector2(-40, 0);
                }


                zombies[i].CollisionRadius = 40;
                if (zombies[i].IsCircleColliding(ball.Center, 10))
                {
                    zombies.RemoveAt(i);
                    ball.Velocity *= -1;
                   
                    if (ball.Velocity.X < -10)
                        AIScore += 1;
                    else 
                    if (ball.Velocity.X > 10)
                        playerScore += 1;

                    UpdateScore();
                    for (int zam = 0; i < 5; i++)
                    {
                        Vector2 location = new Vector2(rand.Next(150, 700), rand.Next(100, 540));
                        Vector2 velocity = new Vector2(40, 0);

                        if (zombies.Count < 10)
                            SpawnZombie(location, velocity);
                        //if (zombies.Count >= 4)
                        //    zombies.RemoveAt(zam);
                    }


                    continue;
                }

                if (zombies[i].Location.X < 0)
                {
                    zombies[i].FlipHorizontal = true;
                }

            }

            // Paddle 2 AI
            MouseState Ms = Mouse.GetState();
            paddle1.Location = new Vector2(paddle1.Location.X, Ms.Y);

            Vector2 diff2 = ball.Center - paddle2.Center;


            AITimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (AITimer > AITimerMax)
            {
                float dist = 1 / (diff2.X / (float)this.Window.ClientBounds.Width);
                float yvel = -dist * 3f * diff2.Y;

                paddle2.Velocity = new Vector2(0, MathHelper.Clamp(yvel, -300, 300));

                AITimer = 0;
            }


            if (paddle2.IsBoxColliding(ball.BoundingBoxRect))
            {
                ball.Velocity *= new Vector2(-2, -1);
              


                float diffpct2 = diff2.Y / (float)ball.BoundingBoxRect.Height;

                if (diff2.Y < -10)
                    ball.Velocity = new Vector2(-500, 200 * diffpct2);
                else 
                    if (diff2.Y > -10 && diff2.Y < 10)
                    ball.Velocity = new Vector2(-500, 0);
                else 
                     if (diff2.Y > 10)
                    ball.Velocity = new Vector2(-500, -200 * diffpct2);

                ballspeed += 10;
            }

            if (paddle1.IsBoxColliding(ball.BoundingBoxRect))
            {
                ball.Velocity = new Vector2((float)Math.Abs(ball.Velocity.X), -ball.Velocity.Y);

                Vector2 diff = ball.Center - paddle1.Center;

                float diffpct = diff.Y / (float)ball.BoundingBoxRect.Height;

                if (diff.Y < -10)
                    ball.Velocity = new Vector2(500, 200 * diffpct);
                else if (diff.Y > -10 && diff.Y < 10)
                    ball.Velocity *= new Vector2(1, 1 / 2);
                else if (diff.Y > 10)
                    ball.Velocity = new Vector2(500, 200 * diffpct);

                ballspeed += 10;
            }

            if (paddle1.Location.Y < 0)
            {
                paddle1.Location = new Vector2(paddle1.Location.X, 0);
            }
            else if (paddle1.Location.Y > 620)
            {
                paddle1.Location = new Vector2(paddle1.Location.X, 620);
            }

            if (ball.Center.X < 0)
            {
                ball.Location = new Vector2(700, 350);
                AIScore += 2;

                UpdateScore();
            }
            else if (ball.Center.X > 1000)
            {
                ball.Location = new Vector2(700, 350);

                playerScore += 2;

                UpdateScore();
            }

            if (ball.Center.Y < 0)
                ball.Velocity *= new Vector2(1, -1);
            else if (ball.Center.Y > 750)
                ball.Velocity *= new Vector2(1, -1);

            Vector2 vel = ball.Velocity;
            vel.Normalize();
            vel *= ballspeed;
            ball.Velocity = vel;

            if (paddle2.Location.Y < 0)
            {
                paddle2.Location = new Vector2(paddle2.Location.X, 0);
            }
            else if (paddle2.Location.Y > 620)
            {
                paddle2.Location = new Vector2(paddle2.Location.X, 620);
            }




            base.Update(gameTime);
        }
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            
            spriteBatch.Draw(background, Vector2.Zero, Color.White);

            paddle1.Draw(spriteBatch);
            paddle2.Draw(spriteBatch);
            ball.Draw(spriteBatch);

            for (int i = 0; i < zombies.Count; i++)
            {
                zombies[i].Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
