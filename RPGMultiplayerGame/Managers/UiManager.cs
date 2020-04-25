using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Managers
{
    class UiManager
    {
        private static UiManager instance;

        public static UiManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UiManager();
                }
                return instance;
            }
        }

        private readonly List<UiComponent> uiComponents = new List<UiComponent>();
        private Game1 game;

        public void Init(Game1 game)
        {
            this.game = game;
        }

        public Point GetScreenSize()
        {
            return game.GraphicsDevice.PresentationParameters.Bounds.Size;
        }

        public void Draw(SpriteBatch sprite)
        {
            for (int i = 0; i < uiComponents.Count; i++)
            {
                uiComponents[i].Draw(sprite);
            }
        }

        public void OnResize()
        {
            lock (uiComponents)
            {
                foreach (var uiComponent in uiComponents)
                {
                    uiComponent.Resize();
                }
            }
        }

        public void AddUiComponent(UiComponent uiComponent)
        {
            lock (uiComponents)
            {
                uiComponents.Add(uiComponent);
            }
        }

        public void RemoveUiComponent(UiComponent uiComponent)
        {
            lock (uiComponents)
            {
                uiComponents.Remove(uiComponent);
            }
        }

    }
}
