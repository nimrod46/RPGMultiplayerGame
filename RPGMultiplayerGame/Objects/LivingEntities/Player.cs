using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Networking;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.InventoryObjects;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Objects.Items.Potions;
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Objects.QuestsObjects;
using static RPGMultiplayerGame.Managers.GameManager;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public class Player : Human
    {
        readonly List<Keys> currentArrowsKeysPressed = new List<Keys>();
        public delegate void LocalPlayerNameSetEventHandler(Player player);
        public event LocalPlayerNameSetEventHandler OnLocalPlayerNameSet;

       
        public bool IsInventoryVisible { get { return inventory.IsVisible; } set { inventory.IsVisible = value; } }
        private QuestsMenu playerQuests;
        private Inventory inventory;
        private Inventory usableItems;
        private Inventory equippedItems;
        private Npc interactingWith;

        public Player() : base(EntityId.Player, 0, 10, 100, GameManager.Instance.PlayerNameFont, true)
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
            inventory = new Inventory((GameManager.Instance.GetMapSize().ToVector2() / 2).ToPoint(), OriginLocationType.Centered, GameManager.INVENTORY_COLUMNS_NUMBER, GameManager.INVENTORY_ROWS_NUMBER);
            inventory.IsVisible = false;
            usableItems = new Inventory(new Point(GameManager.Instance.GetMapSize().X / 2, GameManager.Instance.GetMapSize().Y - 10), OriginLocationType.ButtomCentered, 5, 1);
            equippedItems = new Inventory(new Point(10, GameManager.Instance.GetMapSize().Y - 10), OriginLocationType.ButtomLeft, 3, 1);
            playerQuests = new QuestsMenu(new Vector2(GameManager.Instance.GetMapSize().X, 100), OriginLocationType.TopLeft);
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

        public void AddQuest(Quest quest)
        {
            InvokeCommandMethodNetworkly(nameof(AddQuest), quest);
            playerQuests.AddQuest(quest);
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

        

        public void StopInteractingWithNpc()
        {
            interactingWith = null;
        }

        public void InteractWithNpc(Npc npc)
        {
            interactingWith = npc;
        }

        private void AddItemToInventoryLocaly(Item inventoryItem)
        {
            inventory.TryAddItem(inventoryItem);
        }

        public void AddItemToInventory(int itemType, int count)
        {
            InvokeCommandMethodNetworkly(nameof(AddItemToInventory), itemType, count);
            AddItemToInventoryLocaly(ItemFactory.GetItem<Item>((ItemType)itemType, count));
        }

        public void AddItemToInventory(int itemType)
        {
            InvokeCommandMethodNetworkly(nameof(AddItemToInventory), itemType);
            AddItemToInventoryLocaly(ItemFactory.GetItem<Item>((ItemType)itemType));
        }

        public override void Update(GameTime gameTime)
        {
            if (hasAuthority)
            {
                if (IsInventoryVisible)
                {
                    if (InputManager.Instance.GetMouseLeftButtonPressed())
                    {
                        if (inventory.TryGetInventoryItemAtScreenLocation(InputManager.Instance.MouseBounds(), out Item item))
                        {
                            if (item is InteractiveItem)
                            {
                                if (usableItems.TryAddItem(item))
                                {
                                    inventory.TryRemoveItem(item);
                                }
                            }
                        }
                        else if (usableItems.TryGetInventoryItemAtScreenLocation(InputManager.Instance.MouseBounds(), out item))
                        {
                            if (item is InteractiveItem)
                            {
                                if (inventory.TryAddItem(item))
                                {
                                    usableItems.TryRemoveItem(item);
                                }
                            }
                        }
                    }
                }
                if (InputManager.Instance.KeyPressed(Keys.A))
                {
                    MoveFromUsableItemSlot(1);
                }
                else if (InputManager.Instance.KeyPressed(Keys.S))
                {
                    MoveFromUsableItemSlot(2);
                }
                else if (InputManager.Instance.KeyPressed(Keys.D))
                {
                    MoveFromUsableItemSlot(3);
                }
                else if (InputManager.Instance.KeyPressed(Keys.F))
                {
                    MoveFromUsableItemSlot(4);
                }

                if (InputManager.Instance.KeyPressed(Keys.Z))
                {
                    equippedItems.UsePotionAtSlot(1, this);
                }

                if (EquippedWeapon != null && InputManager.Instance.KeyPressed(Keys.X) && !(GetCurrentEnitytState<State>() == State.Attacking))
                {
                    Attack();
                }
                else
                {
                    if (GetCurrentEnitytState<State>() == State.Attacking)
                    {
                        if (!InputManager.Instance.KeyDown(Keys.X) || GetIsLoopAnimationFinished())
                        {
                            Instance_OnArrowsKeysStateChange(Keys.None, false);
                        }
                    }
                }
                if (interactingWith != null)
                {
                    if (InputManager.Instance.KeyPressed(Keys.D1, Keys.D9, out Keys pressedKey))
                    {
                        interactingWith.CmdChooseDialogOption(this, pressedKey - Keys.D1);
                    }
                }
            }
            base.Update(gameTime);
        }

        private void MoveFromUsableItemSlot(int slot)
        {
            if (usableItems.TryGetItemInSlot(slot, out Item item))
            {
                if (item is Weapon weapon)
                {
                    EquipeWith(weapon);
                    equippedItems.PutItemInSlot(2, EquippedWeapon);
                }
                else if(item is Potion potion)
                {
                    equippedItems.PutItemInSlot(1, potion);
                }
            }
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
            inventory.Draw(sprite);
            equippedItems.Draw(sprite);
            usableItems.Draw(sprite);
            playerQuests.Draw(sprite);
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
