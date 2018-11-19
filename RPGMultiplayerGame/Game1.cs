using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RPGMultiplayerGame.Networking;
using RPGMultiplayerGame.Other;
using System;
using System.Windows.Forms;

namespace RPGMultiplayerGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Form gameForm;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            gameForm = Control.FromHandle(Window.Handle) as Form;
            IsFixedTimeStep = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            gameForm.Shown += (e, s) => gameForm.Hide();
            LobbyMenu lobby = new LobbyMenu(gameForm);
            lobby.Show();
            lobby.OnConnectionEstablished += Lobby_OnConnecting;
            lobby.OnServerOnline += Lobby_OnServerCreated; ;
            lobby.FormClosing += (e, s) => Exit();
        }   

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D textures = Content.Load<Texture2D>("basictiles");
            MapManager.Instance.LoadSpriteSheet(GraphicsDevice, textures);
            GameManager.Instance.LoadTextures(GraphicsDevice, Content);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
            if (NetworkManager.Instance.NetBehavior?.isServer != true)
            {
                GameManager.Instance.Update(gameTime);
                InputManager.Instance.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (NetworkManager.Instance.NetBehavior?.isServer != true)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                MapManager.Instance.Draw(spriteBatch);
                GameManager.Instance.Draw(spriteBatch);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        private void Lobby_OnServerCreated(Form form)
        {
            form.Hide();
            ServerPanel panel = new ServerPanel();
            panel.Show();
            NetworkManager.Instance.Start();
        }

        private void Lobby_OnConnecting(Form form)
        {
            form.Hide();
            gameForm.Show();
            NetworkManager.Instance.Start();
        }
    }
}
