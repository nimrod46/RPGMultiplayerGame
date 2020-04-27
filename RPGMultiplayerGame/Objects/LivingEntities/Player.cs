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
using RPGMultiplayerGame.Ui;
using static RPGMultiplayerGame.Managers.GameManager;
using static RPGMultiplayerGame.Ui.UiComponent;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public class Player : Human
    {
        readonly List<Keys> currentArrowsKeysPressed = new List<Keys>();
        public delegate void LocalPlayerNameSetEventHandler(Player player);
        public event LocalPlayerNameSetEventHandler OnLocalPlayerNameSet;


        public bool IsInventoryVisible { get { return inventory.IsVisible; } set { inventory.IsVisible = value; } }

        public long SyncGold
        {
            get => syncGold;
            set
            {
                syncGold = value;
                InvokeSyncVarNetworkly(nameof(SyncGold), syncGold);
            }
        }
        private Npc interactingWith;
        private Npc requestingInteraction;
        private QuestsMenu playerQuests;
        private Inventory<GameItem> inventory;
        private Inventory<GameItem> usableItems;
        private Inventory<GameItem> equippedItems;
        private UiTextComponent goldText;
        private long syncGold;
        private HealthBar uiHealthBar;

        public Player() : base(GraphicManager.EntityId.Player, 0, 10, 100, GraphicManager.Instance.PlayerNameFont, true, Color.DarkOrange)
        {
            scale = 1;
            speed *= 2;
            SyncName = "null";
        }

        public override void OnNetworkInitialize()
        {
            if (hasAuthority)
            {
                Layer = GraphicManager.OWN_PLAYER_LAYER;
                NameColor = Color.DarkBlue;

                inventory = new Inventory<GameItem>((windowSize) => windowSize.ToVector2() / 2, PositionType.Centered, false, INVENTORY_COLUMNS_NUMBER, INVENTORY_ROWS_NUMBER)
                {
                    IsVisible = false
                };
                inventory.OnItemClickedEvent += Inventory_OnItemClickedEvent;
                usableItems = new Inventory<GameItem>((windowSize) => new Vector2(windowSize.X / 2, windowSize.Y - 10), PositionType.ButtomCentered, true, 5, 1);
                usableItems.OnItemClickedEvent += UsableItems_OnItemClickedEvent;
                equippedItems = new Inventory<GameItem>((windowSize) => new Vector2(10, windowSize.Y - 10), PositionType.ButtomLeft, true, 3, 1);
                playerQuests = new QuestsMenu((windowSize) => new Vector2(windowSize.X - 10, 50), PositionType.TopRight);
                uiHealthBar = new HealthBar((windowSize) => new Vector2(equippedItems.Position.X + equippedItems.Size.X + 10, windowSize.Y - 10), PositionType.ButtomLeft, () => SyncHealth, maxHealth);
                goldText = new UiTextComponent((windowSize) =>  new Vector2(uiHealthBar.Position.X + uiHealthBar.Size.X + 20, uiHealthBar.Position.Y + uiHealthBar.Size.Y / 2), PositionType.CenteredLeft, true, UiManager.GUI_LAYER, UiManager.Instance.GoldTextFont, () => SyncGold.ToString(), Color.DarkGoldenrod);
            }
            
            base.OnNetworkInitialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (hasAuthority)
            {
                if (InputManager.Instance.KeyPressed(Keys.I))
                {
                    if (GameManager.Instance.IsMouseInteractable)
                    {
                        if (IsInventoryVisible)
                        {
                            IsInventoryVisible = false;
                            inventory.IsIntractable = false;
                            usableItems.IsIntractable = false;
                        }
                    }
                    else
                    {
                        IsInventoryVisible = true;
                        inventory.IsIntractable = true;
                        usableItems.IsIntractable = true;
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
                if (requestingInteraction != null)
                {
                    if (InputManager.Instance.KeyPressed(Keys.Q))
                    {
                        requestingInteraction.InvokeCommandMethodNetworkly(nameof(requestingInteraction.InteractionAcceptedByPlayer), this);

                    }
                }
                if (interactingWith != null)
                {
                    if (InputManager.Instance.KeyPressed(Keys.D1, Keys.D9, out Keys pressedKey))
                    {
                        interactingWith.InvokeCommandMethodNetworkly(nameof(interactingWith.CmdChooseDialogOption), this, pressedKey - Keys.D1);
                    }
                }
            }
            base.Update(gameTime);
        }

        public bool IsAbleToBuy(GameItemShop gameItemShop)
        {
            return SyncGold >= gameItemShop.Price;
        }

        private void Inventory_OnItemClickedEvent(GameItem item)
        {
            if (item is InteractiveItem)
            {
                if (usableItems.TryAddItem(item))
                {
                    inventory.TryRemoveItem(item);
                }
            }
        }

        private void UsableItems_OnItemClickedEvent(GameItem item)
        {
            if (item is InteractiveItem)
            {
                if (inventory.TryAddItem(item))
                {
                    usableItems.TryRemoveItem(item);
                }
            }
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
            playerQuests.AddQuest(quest);
        }

        public void RemoveQuest(Quest quest)
        {
            playerQuests.RemoveQuest(quest);
        }

        private void Instance_OnArrowsKeysStateChange(Keys key, bool isDown)
        {
            lock (currentArrowsKeysPressed)
            {
                if (isDown)
                {
                    Direction direction = (Direction)((int)key - (int)Keys.Left);
                    InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), (int)State.Moving, direction);
                    currentArrowsKeysPressed.Insert(0, key);
                }
                else
                {
                    currentArrowsKeysPressed.RemoveAll(k => k == key);
                    if (currentArrowsKeysPressed.Count == 0)
                    {
                        InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), (object)(int)State.Idle, SyncCurrentDirection);
                    }
                    else
                    {
                        key = currentArrowsKeysPressed[0];
                        Direction direction = (Direction)((int)key - (int)Keys.Left);
                        InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), (int)State.Moving, direction);
                    }
                }
            }
        }

        public bool IsInteractingWith(Npc npc)
        {
            return interactingWith == npc || requestingInteraction == npc;
        }

        public void StopInteractingWithNpc()
        {
            interactingWith = null;
            requestingInteraction = null;
        }

        public void InteractWithNpc(Npc npc)
        {
            requestingInteraction = null;
            interactingWith = npc;
        }

        public void InteractRequestWithNpc(Npc npc)
        {
            requestingInteraction = npc;
        }

        private void AddItemToInventoryLocaly(GameItem inventoryItem)
        {
            inventory.TryAddItem(inventoryItem);
        }

        public void AddItemToInventory(ItemType itemType, int count)
        {
            InvokeCommandMethodNetworkly(nameof(AddItemToInventory), itemType, count);
            if (hasAuthority)
            {
                AddItemToInventoryLocaly(ItemFactory.GetItem<GameItem>(itemType, count));
            }
        }

        public void AddItemToInventory(ItemType itemType)
        {
            InvokeCommandMethodNetworkly(nameof(AddItemToInventory), itemType);
            if (hasAuthority)
            {
                AddItemToInventoryLocaly(ItemFactory.GetItem<GameItem>(itemType));
            }
        }

        private void MoveFromUsableItemSlot(int slot)
        {
            if (usableItems.TryGetItemInSlot(slot, out GameItem item))
            {
                if (item is Weapon weapon)
                {
                    EquipeWith(weapon);
                    equippedItems.PutItemInSlot(2, EquippedWeapon);
                }
                else if (item is Potion potion)
                {
                    equippedItems.PutItemInSlot(1, potion);
                }
            }
        }

        public override void Kill(Entity attacker)
        {
            //base.Kill(attacker); //TODO: RETURN
        }

        public override void OnDestroyed(NetworkIdentity identity)
        {
            base.OnDestroyed(identity);
            InputManager.Instance.OnArrowsKeysStateChange -= Instance_OnArrowsKeysStateChange;
        }

        protected void CmdCheckName(Player client, string name)
        {
            InvokeCommandMethodNetworkly(nameof(CmdCheckName), client, name);
            if (!isInServer)
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
            if (!hasAuthority)
            {
                return;
            }
            Init(name);
            OnLocalPlayerNameSet?.Invoke(this);
            Console.WriteLine("Welcome: " + SyncName);
        }
    }
}
