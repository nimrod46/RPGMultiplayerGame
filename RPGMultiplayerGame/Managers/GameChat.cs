using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame_Textbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;
using RPGMultiplayerGame.Graphics.Ui;

namespace RPGMultiplayerGame.Managers
{
    public class GameChat : NetworkIdentity, IUiComponent
    {
        private GameTextBox textBox;
        public bool IsActive { get => textBox.Active; set => textBox.Active = value; }

        public GameChat()
        {
        }

        public void Initialize(Game game)
        {
            MonoGame_Textbox.KeyboardInput.Initialize(game, 500f, 20);
            MonoGame_Textbox.KeyboardInput.KeyPressed += KeyboardInput_KeyPressed;
            Rectangle viewport = new Rectangle(50, 50, 400, 200);
            textBox = new GameTextBox(viewport, 200, "This is a test. Move the cursor, select, delete, write...",
              game.GraphicsDevice, GraphicManager.Instance.PlayerNameFont, Color.LightGray, Color.DarkGreen, 30);
            textBox.EnterDown += TextBox_EnterDown;
            UiManager.Instance.AddUiComponent(this);
        }

        internal void Update()
        {
            textBox.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            textBox.Draw(spriteBatch);
        }

        public void BoardcastAddMassage(string massage)
        {
            InvokeBroadcastMethodNetworkly(nameof(BoardcastAddMassage), massage);
            Console.WriteLine(massage);
        }

        private void TextBox_EnterDown(object sender, MonoGame_Textbox.KeyboardInput.KeyEventArgs e)
        {
            BoardcastAddMassage(textBox.Text.String);
            textBox.Clear();
        }


        private void KeyboardInput_KeyPressed(object sender, MonoGame_Textbox.KeyboardInput.KeyEventArgs e, KeyboardState ks)
        {
            if (e.KeyCode == Microsoft.Xna.Framework.Input.Keys.OemTilde)
            {
                textBox.Active = !textBox.Active;
            }
        }

        public void Resize()
        {
            //throw new NotImplementedException();
        }
    }
}
