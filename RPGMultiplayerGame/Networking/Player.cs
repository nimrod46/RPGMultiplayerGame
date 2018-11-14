using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Networking;
using RPGMultiplayerGame.Other;
using static RPGMultiplayerGame.Other.GameManager;

namespace RPGMultiplayerGame.Networking
{
    class Player : Entity
    {
        List<Keys> currentArrowsKeysPressed = new List<Keys>();
        public Player() : base(EntityID.Player, 1, 0, 10)
        {

        }

        public override void OnLocalPlayerInitialize()
        {
            speed *= 2;
            InputManager.Instance.OnArrowsKeysStateChange += Instance_OnArrowsKeysStateChange;
        }

        private void Instance_OnArrowsKeysStateChange(Keys key, bool isDown)
        {
            lock (currentArrowsKeysPressed)
            {
                if (isDown)
                {
                    Direction direction = (Direction)((int)key - (int)Keys.Left);
                    StartMoving(direction);
                    currentArrowsKeysPressed.Add(key);
                }
                else
                {
                    currentArrowsKeysPressed.RemoveAll(k => k == key);
                    if (currentArrowsKeysPressed.Count == 0)
                    {
                        StopMoving();
                    }
                    else
                    {
                        key = currentArrowsKeysPressed[0];
                        Direction direction = (Direction)((int)key - (int)Keys.Left);
                        StartMoving(direction);
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }  

        public override void OnDestroyed()
        {
            base.OnDestroyed();
            InputManager.Instance.OnArrowsKeysStateChange -= Instance_OnArrowsKeysStateChange;
        }

        [Command]
        public void CmdSetSpawnPoint(NetBlock spawnPoint)
        {
            SetSpawnPointLocaly(spawnPoint);
        }

        [BroadcastMethod]
        public void SetSpawnPoint(NetBlock spawnPoint)
        {
            SetSpawnPointLocaly(spawnPoint);
        }

        public void SetSpawnPointLocaly(NetBlock spawnPoint)
        {
            MapManager.Instance.spawnPoint = spawnPoint;
            SyncX = spawnPoint.SyncX;
            SyncY = spawnPoint.SyncY;
        }
    }
}
