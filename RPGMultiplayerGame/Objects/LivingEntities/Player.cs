using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Networking;
using RPGMultiplayerGame.Managers;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    class Player : Human
    {
        List<Keys> currentArrowsKeysPressed = new List<Keys>();
        public Player() : base(EntityID.Player, 0, 10, 100, GameManager.Instance.PlayerName)
        {
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            speed *= 2;
            layer -= 0.01f;
        }

        public override void OnLocalPlayerInitialize()
        {
            if (isServerAuthority)
            {
                return;
            }
            InputManager.Instance.OnArrowsKeysStateChange += Instance_OnArrowsKeysStateChange;
            layer = 0f;
            GameManager.Instance.SetLocalPlayerName(this);
            Console.WriteLine("Welcome: " + syncName);
        }

        private void Instance_OnArrowsKeysStateChange(Keys key, bool isDown)
        {
            lock (currentArrowsKeysPressed)
            {
                if (isDown)
                {
                    Direction direction = (Direction)((int)key - (int)Keys.Left);
                    StartMoving(direction);
                    currentArrowsKeysPressed.Insert(0, key);
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
    }
}
