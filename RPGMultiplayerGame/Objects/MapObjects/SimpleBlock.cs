using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.MapObjects
{
    public class SimpleBlock : Block
    {
        public SimpleBlock() : base(GraphicManager.Instance.Textures[0])
        {
        }

        public override void OnTextureIndexSet()
        {
            animationsByType = GraphicManager.Instance.Textures[SyncTextureIndex];
        }
    }
}
