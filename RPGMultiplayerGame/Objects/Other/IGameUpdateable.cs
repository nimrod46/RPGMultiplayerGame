using Microsoft.Xna.Framework;

namespace RPGMultiplayerGame.Objects.Other
{
    public interface IGameUpdateable
    {
        public bool IsDestroyed { get; set; }

        public bool IsEnabled { get; set; }

        void Update(GameTime gameTime);
    }
}
