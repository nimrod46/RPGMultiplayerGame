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
using RPGMultiplayerGame.Ui;
using static RPGMultiplayerGame.Ui.UiComponent;
using RPGMultiplayerGame.Objects.LivingEntities;

namespace RPGMultiplayerGame.Managers
{
    public class GameChat : NetworkIdentity
    {
        private GameTextBox textBox;
        public bool IsActive
        {
            get
            {

                return textBox.Active;
            }
            set
            {
                textBox.Active = value;
            }
        }

        public Player LocalPlayer { get; set; }

        private UiTextComponent chatMassages;
        private string massgaes;

        public GameChat() : base()
        {
            massgaes = "";
        }

        public void Initialize(Game game)
        {
            MonoGame_Textbox.KeyboardInput.Initialize(game, 500f, 20);
            MonoGame_Textbox.KeyboardInput.KeyPressed += KeyboardInput_KeyPressed;
            Rectangle viewport = new Rectangle(25, 25, 400, 200);
            textBox = new GameTextBox(viewport, 200, "This is a test. Move the cursor, select, delete, write...",
              game.GraphicsDevice, GraphicManager.Instance.PlayerNameFont, Color.LightGray, Color.DarkGreen, 30);
            textBox.EnterDown += TextBox_EnterDown;
            chatMassages = new UiTextComponent((g) => new Vector2(25, 50), PositionType.TopLeft, true, 0, GraphicManager.Instance.PlayerNameFont, () => massgaes, Color.Black);
        }

        internal void Update()
        {
            textBox.Update();
        }

        public void BoardcastlyAddPlayerMassage(string massage)
        {
            InvokeBroadcastMethodNetworkly(nameof(LocallyAddPlayerMassage), massage, LocalPlayer);
        }

        public void LocallyAddPlayerMassage(string massage, Player player)
        {
            System.Drawing.KnownColor color;
            if (player.hasAuthority)
            {
                color = System.Drawing.KnownColor.DarkBlue;
            }
            else
            {
                color = System.Drawing.KnownColor.DarkOrange;
            }
            massage = chatMassages.ColorToColorCode(color, player.GetName() + ": ") + massage;
            LocallyAddMassage(massage);
        }

        public void LocallyAddMassage(string massage)
        {
            massgaes = massage + "\n " + massgaes;
        }

        private void TextBox_EnterDown(object sender, MonoGame_Textbox.KeyboardInput.KeyEventArgs e)
        {
            BoardcastlyAddPlayerMassage(textBox.Text.String);
            textBox.Clear();
        }

        private void KeyboardInput_KeyPressed(object sender, MonoGame_Textbox.KeyboardInput.KeyEventArgs e, KeyboardState ks)
        {
            if (e.KeyCode == Keys.OemTilde)
            {
                IsActive = !IsActive;
            }
        }
    }
}
