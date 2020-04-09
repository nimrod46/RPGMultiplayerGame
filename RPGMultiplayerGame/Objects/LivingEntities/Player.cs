using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Networking;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Weapons;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public class Player : Human
    {
        readonly List<Keys> currentArrowsKeysPressed = new List<Keys>();
        public delegate void LocalPlayerNameSetEventHandler(Player player);
        public event LocalPlayerNameSetEventHandler OnLocalPlayerNameSet;
        private Npc interactingWith;

        public Player() : base(EntityId.Player, 0, 10, 100, GameManager.Instance.PlayerName, true)
        {
            scale = 1;
            speed *= 2;
            SyncName = "null";
        }

        public override void OnNetworkInitialize()
        {
            if (hasAuthority)
            {
                Layer = GameManager.OWN_PLAYER_LAYER;
            }
            base.OnNetworkInitialize();
        }

        public void InitName()
        {
            CmdCheckName(this, TextInput.getText("Name"));
        }

        public void Init(string name)
        {
            SetName(name);
            InputManager.Instance.OnArrowsKeysStateChange += Instance_OnArrowsKeysStateChange;
        }

        private void Instance_OnArrowsKeysStateChange(Keys key, bool isDown)
        {
            lock (currentArrowsKeysPressed)
            {
                if (isDown)
                {
                    Direction direction = (Direction)((int)key - (int)Keys.Left);
                    SetCurrentEntityState((int)State.Moving, (int) direction);
                    currentArrowsKeysPressed.Insert(0, key);
                }
                else
                {
                    currentArrowsKeysPressed.RemoveAll(k => k == key);
                    if (currentArrowsKeysPressed.Count == 0)
                    {
                        SetCurrentEntityState((int)State.Idle, (int)SyncCurrentDirection);
                    }
                    else
                    {
                        key = currentArrowsKeysPressed[0];
                        Direction direction = (Direction)((int)key - (int)Keys.Left);
                        SetCurrentEntityState((int)State.Moving, (int)direction);
                    }
                }
            }
        }

        internal void StopInteractingWithNpc()
        {
            interactingWith = null;
        }

        internal void InteractWithNpc(Npc npc)
        {
            interactingWith = npc;
        }

        public override void Update(GameTime gameTime)
        {
            if (hasAuthority)
            {
                if (InputManager.Instance.KeyPressed(Keys.X) && !(GetCurrentEnitytState<State>() == State.Attacking))
                {
                    SetCurrentEntityState((int)State.Attacking, SyncCurrentDirection);
                }
                else
                {
                    if (GetCurrentEnitytState<State>() == State.Attacking)
                    {
                        if(!InputManager.Instance.KeyDown(Keys.X) || GetIsLoopAnimationFinished())
                        {
                            Instance_OnArrowsKeysStateChange(Keys.None, false);
                        }
                    }
                }
                if (interactingWith != null)
                {
                    if (InputManager.Instance.KeyPressed(Keys.D1, Keys.D9, out Keys pressedKey))
                    {
                        interactingWith.ChooseDialogOption(pressedKey - Keys.D1);
                    }
                }
            }   
            base.Update(gameTime);
        }

        public override void OnDestroyed(NetworkIdentity identity)
        {
            base.OnDestroyed(identity);
            InputManager.Instance.OnArrowsKeysStateChange -= Instance_OnArrowsKeysStateChange;
        }

        protected void CmdCheckName(Player client, string name)
        {
            InvokeCommandMethodNetworkly(nameof(CmdCheckName), client, name);
            if(!isInServer)
            {
                return;
            }

            Console.WriteLine("Checing name: " + name);
            if (ServerManager.Instance.IsNameLegal(name))
            {
                client.CmdSetName(name);
            } 
            else
            {
                client.CmdChooseNameAgain();
            }
        }

        public void CmdChooseNameAgain()
        {
            InvokeCommandMethodNetworkly(nameof(CmdChooseNameAgain));
            if (!hasAuthority)
            {
                return;
            }
            CmdCheckName(this, TextInput.getText("Name"));
        }

        public void CmdSetName(string name)
        {
            InvokeCommandMethodNetworkly(nameof(CmdSetName), name);
            if(!hasAuthority)
            {
                return;
            }
            Init(name);
            OnLocalPlayerNameSet?.Invoke(this);
            Console.WriteLine("Welcome: " + SyncName);
        }
    }
}
