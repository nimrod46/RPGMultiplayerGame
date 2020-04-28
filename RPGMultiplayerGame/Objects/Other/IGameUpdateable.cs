using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Other
{
    public interface IGameUpdateable
    {
        public bool IsDestroyed { get; set; }

        void Update(GameTime gameTime);
    }
}
