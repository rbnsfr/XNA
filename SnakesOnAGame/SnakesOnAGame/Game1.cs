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
        Texture2D snakeTexture, overTexture, pelletTexture;
        Vector2 direction = new Vector2(0, 1);
        bool snoopmode = false; // my class wants this to be recurring
        bool gameover = false;
        Color col, ballcol;

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
            snake.Add(new Vector2(4, 6));
            snake.Add(new Vector2(4, 7));
            snake.Add(new Vector2(4, 8));
            snake.Add(new Vector2(4, 9));

            Rectangle bounds = this.Window.ClientBounds;
            int irandx = rand.Next(bounds.Left, bounds.Right);
            int irandy = rand.Next(bounds.Top, bounds.Bottom);

            food = new Vector2(irandx, irandy);

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

            snakeTexture = Content.Load<Texture2D>(@"SQUARE");
            overTexture = Content.Load<Texture2D>(@"GameOver");
            pelletTexture = Content.Load<Texture2D>(@"SQUARE");

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

            // Movement
            if (ks.IsKeyDown(Keys.Up)) direction = new Vector2(0, -1);
            else if (ks.IsKeyDown(Keys.Down)) direction = new Vector2(0, 1);
            else if (ks.IsKeyDown(Keys.Left)) direction = new Vector2(-1, 0);
            else if (ks.IsKeyDown(Keys.Right)) direction = new Vector2(1, 0);

            // Snoop Mode
            if (ks.IsKeyDown(Keys.Space) && snoopmode == false) snoopmode = true;
            else if (ks.IsKeyDown(Keys.Space) && snoopmode) snoopmode = false;

            if (snoopmode) { col = Color.Green; ballcol = Color.GreenYellow; direction *= new Vector2(0.5f, 0.5f); Window.Title += "[Snoop Mode]"; }
            else if (snoopmode == false) { col = Color.Red; ballcol = Color.Yellow; direction = new Vector2(direction.X, direction.Y); }

            // No leaving the window
            Rectangle bounds = this.Window.ClientBounds;

            if (snake.Count > 0)
            {
                if (snake[0].Y > bounds.Bottom) gameover = true;

                for (int i = snake.Count - 1; i > 0; i--)
                {
                    snake[i] = snake[i - 1];
                }

                snake[0] += direction;

                if (snake[0] == food)
                {
                    int irandx = rand.Next(bounds.Left, bounds.Right);
                    int irandy = rand.Next(bounds.Top, bounds.Bottom);
                    food = new Vector2(irandx, irandy);
                    snake.Add(new Vector2(snake[0].X, snake[0].Y - snake.Count * 20));
                }
            }

            if (gameover)
            {
                snake.Clear();
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
            for (int i = 0; i < snake.Count; i++)
                spriteBatch.Draw(snakeTexture, new Rectangle((int)snake[i].X * 20, (int)snake[i].Y * 20, 20, 20), new Rectangle(0, 0, snakeTexture.Width, snakeTexture.Height), col);

            if (gameover)
            {
                Rectangle bounds = this.Window.ClientBounds;
                Vector2 pos = new Vector2(bounds.Width/2, bounds.Height/2) - new Vector2(overTexture.Width/2, overTexture.Height/2);
                spriteBatch.Draw(overTexture, pos, Color.White);
            }


            spriteBatch.Draw(pelletTexture, food, ballcol);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
