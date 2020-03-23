using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Networking;
namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public class Waypoint : GameObject
    {
        [SyncVar]
        public double SyncTime { get; set; }
        [SyncVar(hook = "OnNpcIdSet")]
        public int SyncNpcId { get; set; }

        protected void OnNpcIdSet()
        {
            if (SyncNpcId != 0)
            {
                (NetworkBehavior.GetNetworkIdentityById(SyncNpcId) as Npc).AddWaypoint(this);
            }
            else
            {
                Console.WriteLine("Warning: call to OnNpcIdSet with no SyncNpcId");
            }
        }

        public override int GetHashCode()
        {
            var hashCode = -1581547725;
            hashCode = hashCode * -1521134295 + SyncTime.GetHashCode();
            hashCode = hashCode * -1521134295 + SyncNpcId.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is Waypoint wayPoint)
            {
                return wayPoint.SyncX == SyncX && wayPoint.SyncY == SyncY && wayPoint.SyncTime == SyncTime && wayPoint.SyncNpcId == SyncNpcId;
            }
            return false;
        }
    }
}
