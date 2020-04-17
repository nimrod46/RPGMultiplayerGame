using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Networking;
using RPGMultiplayerGame.Managers;
using System;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;

namespace RPGMultiplayerGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        readonly Form gameForm;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            gameForm = Control.FromHandle(Window.Handle) as Form;
            IsFixedTimeStep = true;
            InactiveSleepTime = new TimeSpan(0);
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
            GameManager.Instance.Init(this);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
          //  MapManager.Instance.LoadSpriteSheet(GraphicsDevice, textures);
            GameManager.Instance.LoadTextures(GraphicsDevice, Content);
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

            NetworkBehavior.RunActionsSynchronously();

            if (gameTime.IsRunningSlowly)
            {
                Console.WriteLine("RUNING SLOWWWW");
            }
            base.Update(gameTime);
            if (ServerManager.Instance.IsRuning != true)
            {
                GameManager.Instance.Update(graphics.GraphicsDevice, gameTime);
                InputManager.Instance.Update(gameTime);
            }
            else
            {
                ServerManager.Instance.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (ServerManager.Instance.IsRuning != true)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
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
        }

        private void Lobby_OnConnecting(Form form)
        {
            form.Hide();
            ClientManager.Instance.OnStartGame += Instance_OnStartGame;
            ClientManager.Instance.Start();
        }

        private void Instance_OnStartGame(object sender, EventArgs e)
        {
            gameForm.Show();
        }
    }
}
