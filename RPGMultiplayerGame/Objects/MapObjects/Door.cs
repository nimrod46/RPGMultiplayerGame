using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.MapObjects
{
    public abstract class Door : CloseOpenBlock
    {
        public Door(Dictionary<int, List<GameTexture>> animationsByType) : base(animationsByType)
        {
        }


        public override void Close()
        {
            if (!GameManager.Instance.GetEntitiesAt(this).Any())
            {
                base.Close();
            }
        }

        public override bool Isblocking()
        {
            return SyncCurrentState != State.Open;
        }
    }
}
