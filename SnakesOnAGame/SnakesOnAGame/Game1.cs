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

namespace SnakesOnAGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<Vector2> snake = new List<Vector2>();
        Vector2 food;
        Random rand = new Random();
        Texture2D snakeTexture, overTexture, pelletTexture, pelletSM, background, gtexture, stexture, backgroundSM, spaceKey, spaceDesc;
        Vector2 direction = new Vector2(0, 1);
        bool snoopmode = false; // my class wants this to be recurring
        bool gameover = false;
        bool showdirections = true;
        bool snoopmodeFinished;
        Color col, ballcol;
        string title = "Snakes on a Game";
        float snoopTime;
        float gameOverTime = 0;
        bool gameOverSoundFinished = false;
        Song song;
        SoundEffect snoop, snoopend, lose;
        SoundEffectInstance snoopi, snoopei, losei;

        int millisecondsPerFrame;
        int timeSinceLastUpdate = 0; // Accumulate the elapsed time

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            Content.RootDirectory = "Content";
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
            snake.Add(new Vector2(4, 5));
            snake.Add(new Vector2(4, 20));
            snake.Add(new Vector2(4, 35));
            snake.Add(new Vector2(4, 50));
            snake.Add(new Vector2(4, 65));
            snake.Add(new Vector2(4, 80));

            Rectangle bounds = this.Window.ClientBounds;
            int irandx = rand.Next(16, bounds.Width - 16);
            int irandy = rand.Next(16, bounds.Height - 16);

            food = new Vector2(irandx, irandy);

            Window.Title = "Snakes on a Game";

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>(@"Atari");
            backgroundSM = Content.Load<Texture2D>(@"AtariSnoopmode");
            snakeTexture = Content.Load<Texture2D>(@"Square");
            overTexture = Content.Load<Texture2D>(@"GameOver");
            pelletTexture = Content.Load<Texture2D>(@"Square");
            pelletSM = Content.Load<Texture2D>(@"PelletSnoopmode");
            gtexture = Content.Load<Texture2D>(@"Gdir");
            stexture = Content.Load<Texture2D>(@"Sdir");
            spaceKey = Content.Load<Texture2D>(@"SpaceKey");
            spaceDesc = Content.Load<Texture2D>(@"SpaceDesc");

            int randi = rand.Next(0, 8);
            switch (randi)
            {
                case 0:
                    song = Content.Load<Song>(@"chiptune1");
                    break;
                case 1:
                    song = Content.Load<Song>(@"chiptune2");
                    break;
                case 2:
                    song = Content.Load<Song>(@"chiptune3");
                    break;
                case 3:
                    song = Content.Load<Song>(@"chiptune4");
                    break;
                case 4:
                    song = Content.Load<Song>(@"chiptune5");
                    break;
                case 5:
                    song = Content.Load<Song>(@"chiptune6");
                    break;
                case 6:
                    song = Content.Load<Song>(@"chiptune7");
                    break;
                case 7:
                    song = Content.Load<Song>(@"chiptune8");
                    break;
            }
            MediaPlayer.Play(song);
            snoop = Content.Load<SoundEffect>(@"mode");
            snoopi = snoop.CreateInstance();
            snoopi.IsLooped = true;

            snoopend = Content.Load<SoundEffect>(@"modeend");
            snoopei = snoopend.CreateInstance();

            lose = Content.Load<SoundEffect>(@"lose");
            losei = lose.CreateInstance();

            // TODO: use this.Content to load your game content here
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            KeyboardState ks = Keyboard.GetState();

            int milli = (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            timeSinceLastUpdate += milli;
            snoopTime += milli;
            gameOverTime += milli;

            if (timeSinceLastUpdate >= millisecondsPerFrame)
            {
                timeSinceLastUpdate = 0;

                if (snoopmode)
                {
                    millisecondsPerFrame = 60;
                    MediaPlayer.Volume = 0.04f;
                    snoopi.Play();
                    if (snoopTime > 19615) { snoopTime = 0; snoopmode = false; snoopmodeFinished = true; } // Snoop Mode lasts ~20 seconds
                    showdirections = false;
                }
                else
                {
                    millisecondsPerFrame = 30;
                    MediaPlayer.Volume = 0.3f;
                    snoopi.Stop();
                }

                // Movement
                if (ks.IsKeyDown(Keys.Up)) direction = new Vector2(0, -1);
                else if (ks.IsKeyDown(Keys.Down)) direction = new Vector2(0, 1);
                else if (ks.IsKeyDown(Keys.Left)) direction = new Vector2(-1, 0);
                else if (ks.IsKeyDown(Keys.Right)) direction = new Vector2(1, 0);

                // Snoop Mode
                if (ks.IsKeyDown(Keys.Space) && snoopmode == false) { snoopmode = true; Window.Title = title + " [Snoop Mode]"; }
                else if ((ks.IsKeyDown(Keys.Space) && snoopmode) || snoopmodeFinished) { snoopmode = false; Window.Title = title; snoopei.Play(); snoopmodeFinished = false; }

                if (snoopmode) { col = Color.Green; ballcol = Color.GreenYellow; }
                else { col = Color.Red; ballcol = Color.Yellow; }

                // No leaving the window
                Rectangle bounds = this.Window.ClientBounds;

                if (snake.Count > 0)
                {
                    for (int i = snake.Count - 1; i > 0; i--)
                    {
                        snake[i] = snake[i - 1];
                        if (snake[i].X > bounds.Height) gameover = true;
                    }
                    for (int i = snake.Count - 1; i > 1; i--)
                        if (snake[0] == snake[i]) gameover = true;

                    snake[0] += direction;

                    if (snake[0] == food)
                    {
                        int irandx = rand.Next(16, bounds.Width - 16);
                        int irandy = rand.Next(16, bounds.Height - 16);
                        food = new Vector2(irandx, irandy);
                        snake.Add(new Vector2(snake[0].X, snake[0].Y - snake.Count * 20));
                    }
                }

                if (gameover)
                {
                    snake.Clear();
                    MediaPlayer.Stop();
                    if (gameOverSoundFinished == false) { losei.Play(); gameOverSoundFinished = true; }
                    //if (gameOverTime > 3000) Exit();
                }
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
            // TODO: Add your drawing code here
            if (snoopmode) spriteBatch.Draw(backgroundSM, new Vector2(0, 0), Color.LightGreen);
            else spriteBatch.Draw(background, new Vector2(0, 0), Color.White);

            Rectangle bounds = this.Window.ClientBounds;

            for (int i = 0; i < snake.Count; i++)
                spriteBatch.Draw(snakeTexture, new Rectangle((int)snake[i].X * 20, (int)snake[i].Y * 20, 20, 20), new Rectangle(0, 0, snakeTexture.Width, snakeTexture.Height), col);

            if (snoopmode) spriteBatch.Draw(pelletSM, food, Color.White);
            else spriteBatch.Draw(pelletTexture, food, ballcol);

            if (gameover)
            {
                Vector2 pos = new Vector2(bounds.Width / 2 - overTexture.Width / 2, bounds.Height / 2 - overTexture.Height / 2);
                spriteBatch.Draw(overTexture, pos, Color.White);
            }

            if (showdirections)
            {
                spriteBatch.Draw(spaceKey, new Vector2(bounds.Width - spaceKey.Width - 20, bounds.Height - spaceKey.Height - spaceDesc.Height - 20), Color.White);
                spriteBatch.Draw(spaceDesc, new Vector2(bounds.Width - spaceDesc.Width - 20, bounds.Height - spaceDesc.Height - 20), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public class Snake
    {
        List<Vector2> snake = new List<Vector2>();
    }
}
