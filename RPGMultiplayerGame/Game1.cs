using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Networking;
using RPGMultiplayerGame.Managers;
using System;
using System.Windows.Forms;

namespace RPGMultiplayerGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteBatch uiSpriteBatch;
        readonly Form gameForm;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            gameForm = Control.FromHandle(Window.Handle) as Form;
            IsFixedTimeStep = true;
            InactiveSleepTime = new TimeSpan(0);
            Window.AllowUserResizing = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GameManager.Instance.Init(this);
            GraphicManager.Instance.Init(this);
            UiManager.Instance.Init(this);
            base.Initialize();
            gameForm.Shown += (e, s) => gameForm.Hide();
            LobbyMenu lobby = new LobbyMenu(gameForm);
            lobby.Show();
            lobby.OnConnectionEstablished += Lobby_OnConnecting;
            lobby.OnServerOnline += Lobby_OnServerCreated; ;
            lobby.FormClosing += (e, s) => Exit();
            Window.ClientSizeChanged += (r, e) => UiManager.Instance.OnResize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            uiSpriteBatch = new SpriteBatch(GraphicsDevice);
            GraphicManager.Instance.LoadTextures(Content);
            UiManager.Instance.LoadTextures(Content);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Exit();
            if (InputManager.Instance.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width / 2;
                graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height / 2;
                graphics.IsFullScreen = false;
                graphics.ApplyChanges();
            }
            else if ((InputManager.Instance.KeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt) || InputManager.Instance.KeyDown(Microsoft.Xna.Framework.Input.Keys.RightAlt)) &&
                InputManager.Instance.KeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
                graphics.IsFullScreen = true;
                graphics.ApplyChanges();
            }

            NetworkBehavior.RunActionsSynchronously();

            if (gameTime.IsRunningSlowly)
            {
                Console.WriteLine("RUNNING SLOWWWW");
            }
            base.Update(gameTime);
            if (ServerManager.Instance.IsRunning != true)
            {
                InputManager.Instance.Update(gameTime);
            }
            GameManager.Instance.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (ServerManager.Instance.IsRunning != true)
            {
                uiSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                UiManager.Instance.Draw(uiSpriteBatch);
                uiSpriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, GameManager.Instance.Camera.Transform);
                GraphicManager.Instance.Draw(spriteBatch);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        private void Lobby_OnServerCreated(Form form)
        {
            form.Hide();
            ServerPanel panel = new ServerPanel();
            panel.Show();
        }

        private void Lobby_OnConnecting(Form form)
        {
            form.Hide();
            GameManager.Instance.OnStartGame += OnStartGame;
            ClientManager.Instance.Start();
        }

        private void OnStartGame(object sender, EventArgs e)
        {
            gameForm.Show();
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
        }
    }
}
