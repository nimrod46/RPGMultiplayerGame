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
using static RPGMultiplayerGame.Ui.UiComponent;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Graphics;
using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Ui
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

        public GameChat() : base()
        {
        }

        public void Initialize(Game game)
        {
            MonoGame_Textbox.KeyboardInput.Initialize(game, 500f, 20);
            MonoGame_Textbox.KeyboardInput.KeyPressed += KeyboardInput_KeyPressed;
            Rectangle viewport = new Rectangle(25, 25, 400, 200);
            textBox = new GameTextBox(viewport, 200, "",
              game.GraphicsDevice, GraphicManager.Instance.PlayerNameFont, Color.LightGray, Color.DarkGreen, 30);
            textBox.EnterDown += TextBox_EnterDown;
            chatMassages = new UiTextComponent((g) => new Vector2(25, 50), PositionType.TopLeft, true, 0, GraphicManager.Instance.PlayerNameFont, () => "", Color.Black);
            chatMassages.ColoredText.MaxNumberOfLines = 10;
        }

        public void Update()
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
            massage = ColoredTextRenderer.ColorToColorCode(color, player.GetName() + ": ") + massage;
            LocallyAddMassage(massage);
        }

        public void LocallyAddGeneralMassage(string massage)
        {
            massage = ColoredTextRenderer.ColorToColorCode(System.Drawing.KnownColor.Chocolate) + massage;
            LocallyAddMassage(massage);
        }

        private void LocallyAddMassage(string massage)
        {
            chatMassages.ColoredText.AppendTextLine(massage);
        }

        private void TextBox_EnterDown(object sender, MonoGame_Textbox.KeyboardInput.KeyEventArgs e)
        {
            BoardcastlyAddPlayerMassage(textBox.Text.String);
            textBox.TextSended();
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
