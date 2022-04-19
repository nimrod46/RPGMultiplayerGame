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
using static RPGMultiplayerGame.Graphics.Ui.UiComponent;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Graphics;
using RPGMultiplayerGame.Graphics.Ui.TextBoxObjects;
using RPGMultiplayerGame.Managers;
using static NetworkingLib.Server;

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
            InputManager.Instance.OnInputStateChange += OnInputStateChange;
            Rectangle viewport = new Rectangle(25, 25, 400, 200);
            textBox = new GameTextBox(viewport, 200, "",
              game.GraphicsDevice, GraphicManager.Instance.PlayerNameFont, Color.LightGray, Color.DarkGreen, 30);
            textBox.EnterDown += TextBox_EnterDown;
            chatMassages = new UiTextComponent((g) => new Vector2(25, 50), PositionType.TopLeft, true, 0, GraphicManager.Instance.PlayerNameFont, () => "", Color.Black);
            chatMassages.ColoredText.MaxNumberOfLines = 10;
        }

        private void OnInputStateChange(InputManager.InputState inputState)
        {
            IsActive = inputState == InputManager.InputState.Chat;
        }

        public void Update()
        {
            textBox.Update();
        }

        public void BoardcastlyAddPlayerMassage(string massage)
        {
            InvokeBroadcastMethodNetworkly(nameof(LocallyAddPlayerMassage), LocalPlayer, massage);
        }

        public void LocallyAddPlayerMassage(Player player, string massage)
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

        public void BoardcastlyAddGeneralMassage(string massage)
        {
            InvokeBroadcastMethodNetworkly(nameof(LocallyAddGeneralMassage), massage);
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

        private void TextBox_EnterDown(object sender, EventArgs eventArgs)
        {
            if (!string.IsNullOrWhiteSpace(textBox.Text.String))
            {
                BoardcastlyAddPlayerMassage(textBox.Text.String);
                textBox.TextSended();
            }
        }
    }
}
