using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Networking;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.InventoryObjects;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Objects.Items.Potions;
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Objects.QuestsObjects;
using RPGMultiplayerGame.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static RPGMultiplayerGame.Managers.GameManager;
using static RPGMultiplayerGame.Ui.UiComponent;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public class Player : Human
    {
        readonly List<Keys> currentArrowsKeysPressed = new List<Keys>();
        public delegate void LocalPlayerNameSetEventHandler(Player player);
        public event LocalPlayerNameSetEventHandler OnLocalPlayerNameSetEvent;
        public delegate void SyncPlayerSaveEventHandler(Player player);
        public event SyncPlayerSaveEventHandler OnSyncPlayerSaveEvent;


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
            Scale = 1;
            SyncSpeed *= 2;
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
                inventory.OnItemLeftClickedEvent += Inventory_OnItemClickedEvent;
                inventory.OnItemRightClickedEvent += Inventory_OnItemRightClickedEvent;
                usableItems = new Inventory<GameItem>((windowSize) => new Vector2(windowSize.X / 2, windowSize.Y - 10), PositionType.ButtomCentered, true, 5, 1);
                usableItems.OnItemLeftClickedEvent += UsableItems_OnItemClickedEvent;
                equippedItems = new Inventory<GameItem>((windowSize) => new Vector2(10, windowSize.Y - 10), PositionType.ButtomLeft, true, 3, 1);
                playerQuests = new QuestsMenu((windowSize) => new Vector2(windowSize.X - 10, 50), PositionType.TopRight);
                uiHealthBar = new HealthBar((windowSize) => new Vector2(equippedItems.Position.X + equippedItems.Size.X + 10, windowSize.Y - 10), PositionType.ButtomLeft, () => SyncHealth, maxHealth);
                goldText = new UiTextComponent((windowSize) => new Vector2(uiHealthBar.Position.X + uiHealthBar.Size.X + 20, uiHealthBar.Position.Y + uiHealthBar.Size.Y / 2), PositionType.CenteredLeft, true, UiManager.GUI_LAYER, UiManager.Instance.GoldTextFont, () => SyncGold.ToString(), Color.DarkGoldenrod);
            }
            base.OnNetworkInitialize();
        }

        private void Inventory_OnItemRightClickedEvent(Inventory<GameItem> inventory, ItemSlotUi<GameItem> item)
        {
            inventory.DropItem(item, Location);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (hasAuthority)
            {
                if(SyncIsDead)
                {
                    if (InputManager.Instance.KeyPressed(Keys.R))
                    {
                        Respawn(SyncSpawnPoint.SyncX, SyncSpawnPoint.SyncY);
                    }
                    return;
                }
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

                if (InputManager.Instance.KeyPressed(Keys.C))
                {
                    InvokeCommandMethodNetworkly(nameof(CmdTryPickUpItem));
                }

                if (EquippedWeapon != null && InputManager.Instance.KeyPressed(Keys.X) && !(GetCurrentEnitytState<State>() == State.Attacking))
                {
                    Attack();
                }
                else
                {
                    if (GetCurrentEnitytState<State>() == State.Attacking)
                    {
                        if (!InputManager.Instance.KeyDown(Keys.X) || IsLoopAnimationFinished())
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
        }

        private void CmdTryPickUpItem()
        {
            Console.WriteLine(GetBoundingRectangle());
            foreach (var item in ServerManager.Instance.GetGameItems())
            {
                Console.WriteLine(item.GetBoundingRectangle() + " " + item.SyncName);
            }
            GameItem gameItem = ServerManager.Instance.GetGameItems().FirstOrDefault(g => IsIntersectingWith(g));
            if (gameItem != null)
            {
                AddItemToInventory(gameItem);
            }
        }

        public override void Respawn(float x, float y)
        {
            base.Respawn(x,y);
            InputManager.Instance.OnArrowsKeysStateChange += Instance_OnArrowsKeysStateChange;
            usableItems.IsVisible = true;
            equippedItems.IsVisible = true;
            playerQuests.IsVisible = true;
            uiHealthBar.IsVisible = true;
            goldText.IsVisible = true;
            InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), NetworkingLib.Server.NetworkInterfaceType.UDP, false, State.Idle, Direction.Down);
        }

        public bool IsAbleToBuy(GameItemShop gameItemShop)
        {
            return SyncGold >= gameItemShop.Price;
        }

        private void Inventory_OnItemClickedEvent(Inventory<GameItem> inv, ItemSlotUi<GameItem> itemSlotUi)
        {
            if (itemSlotUi.Item is InteractiveItem)
            {
                if (usableItems.TryAddItem(itemSlotUi.Item))
                {
                    inventory.TryRemoveItem(itemSlotUi.Item);
                }
            }
        }

        private void UsableItems_OnItemClickedEvent(Inventory<GameItem> inv,ItemSlotUi<GameItem> itemSlotUi)
        {
            if (itemSlotUi.Item is InteractiveItem)
            {
                if (inventory.TryAddItem(itemSlotUi.Item))
                {
                    usableItems.TryRemoveItem(itemSlotUi.Item);
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
            InvokeCommandMethodNetworkly(nameof(CmdSync));
        }

        protected void CmdSync()
        {
            OnSyncPlayerSaveEvent.Invoke(this);
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
                    InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), NetworkingLib.Server.NetworkInterfaceType.UDP, false, (int)State.Moving, direction);
                    currentArrowsKeysPressed.Insert(0, key);
                }
                else
                {
                    currentArrowsKeysPressed.RemoveAll(k => k == key);
                    if (currentArrowsKeysPressed.Count == 0)
                    {
                        InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), NetworkingLib.Server.NetworkInterfaceType.UDP, false, (object)(int)State.Idle, SyncCurrentDirection);
                    }
                    else
                    {
                        key = currentArrowsKeysPressed[0];
                        Direction direction = (Direction)((int)key - (int)Keys.Left);
                        InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), NetworkingLib.Server.NetworkInterfaceType.UDP, false, (int)State.Moving, direction);
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

        public void AddItemToInventory(GameItem gameItem)
        {
            InvokeCommandMethodNetworkly(nameof(AddItemToInventory), gameItem);
            if (hasAuthority)
            {
                AddItemToInventoryLocaly(gameItem);
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
            base.Kill(attacker);
            if (hasAuthority)
            {
                InputManager.Instance.OnArrowsKeysStateChange -= Instance_OnArrowsKeysStateChange;
                currentArrowsKeysPressed.Clear();
                inventory.IsVisible = false;
                usableItems.IsVisible = false; 
                equippedItems.IsVisible = false;
                playerQuests.IsVisible = false;
                uiHealthBar.IsVisible = false;
                goldText.IsVisible = false;
                inventory.IsIntractable = false;
                usableItems.IsIntractable = false;
            }
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
            OnLocalPlayerNameSetEvent?.Invoke(this);
            Console.WriteLine("Welcome: " + SyncName);
        }
    }
}
