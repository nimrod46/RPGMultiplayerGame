using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Graphics.Ui
{
    public interface IUiComponent : IGameDrawable
    {
        void Resize();
    }
}
