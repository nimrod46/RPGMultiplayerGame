using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Networking;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.InventoryObjects;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Objects.Items.Potions;
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Objects.MapObjects;
using RPGMultiplayerGame.Objects.QuestsObjects;
using RPGMultiplayerGame.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static NetworkingLib.Server;
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
        public delegate void PlayerPickUpItemEventHandler(Player player);
        public event PlayerPickUpItemEventHandler OnRemotePlayerTryToPickUpItemEvent;

        public bool IsInventoryVisible { get { return inventory.IsVisible; } set { inventory.IsVisible = value; } }

        public bool SyncIsInteractingWithNpc
        {
            get => isInteractingWithNpc; set
            {
                isInteractingWithNpc = value;
                InvokeSyncVarNetworkly(nameof(SyncIsInteractingWithNpc), isInteractingWithNpc);
            }
        }

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
        private bool isInteractingWithNpc;

        public Player() : base(GraphicManager.EntityId.Player, 0, 8, 100, GraphicManager.Instance.PlayerNameFont, true, Color.DarkOrange)
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
                InputManager.Instance.OnInputStateChange += OnInputStateChange;
                InputManager.Instance.OnAnswerKeyPressed += OnAnswerKeyPressed;
                InputManager.Instance.OnAttackKeyPressed += OnAttackKeyPressed; ;
                InputManager.Instance.OnOpenCloseInventoryKeyPressed += OnOpenCloseInventoryKeyPressed;
                InputManager.Instance.OnPotionUseKeyPressed += OnPotionUseKeyPressed;
                InputManager.Instance.OnRespawnKeyPressed += OnRespawnKeyPressed;
                InputManager.Instance.OnStartInteractWithNpcKeyPressed += OnStartInteractWithNpcKeyPressed;
                InputManager.Instance.OnUsableItemSlotKeyPressed += OnUsableItemSlotKeyPressed;
                InputManager.Instance.OnPickUpItemKeyPressed += OnPickUpItemKeyPressed;
                InputManager.Instance.OnInteractWithMapElement += OnInteractWithMapElement;
            }
            base.OnNetworkInitialize();
        }

        private void OnInteractWithMapElement()
        {
            Rectangle rectangle = GetBoundingRectangle();
            rectangle.Inflate(4, 4);
            if (GameManager.Instance.Map.TryGetBlockAt(rectangle, false, out SpecialBlock specialBlock))
            {
                specialBlock.CmdEngage(this);
            }
        }

        private void OnInputStateChange(InputManager.InputState inputState)
        {
            if (inputState != InputManager.InputState.PlayerControl)
            {
                ClearArrowsState();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (hasAuthority)
            {
                Console.WriteLine(GameManager.Instance.Map.GetMapObjectAt(GetCenter())?.Isblocking());
                Console.WriteLine(GetCenter().X / 16);
                Console.WriteLine(GetCenter().Y / 16);
                if (GetCurrentEnitytState<State>() == State.Attacking)
                {
                    if (IsLoopAnimationFinished())
                    {
                        Instance_OnArrowsKeysStateChange(Keys.None, false);
                    }
                }
            }
        }

        private void CmdTryPickUpItem()
        {
            OnRemotePlayerTryToPickUpItemEvent?.Invoke(this);
        }

        public override void Respawn(float x, float y)
        {
            base.Respawn(x, y);
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

        private void UsableItems_OnItemClickedEvent(Inventory<GameItem> inv, ItemSlotUi<GameItem> itemSlotUi)
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

        private void ClearArrowsState()
        {
            currentArrowsKeysPressed.Clear();
            Instance_OnArrowsKeysStateChange(Keys.None, false);
        }

        public bool IsInteractingWith(Npc npc)
        {
            return interactingWith == npc || requestingInteraction == npc;
        }

        public void StopInteractingWithNpc()
        {
            interactingWith = null;
            requestingInteraction = null;
            SyncIsInteractingWithNpc = false;
        }

        public void InteractWithNpc(Npc npc)
        {
            SyncIsInteractingWithNpc = true;
            requestingInteraction = null;
            interactingWith = npc;
        }

        public void InteractRequestWithNpc(Npc npc)
        {
            SyncIsInteractingWithNpc = true;
            requestingInteraction = npc;
        }

        private void AddItemToInventoryLocaly(GameItem inventoryItem)
        {
            inventory.TryAddItem(inventoryItem);
        }

        public void AddItemToInventory(GameItem gameItem)
        {
            InvokeCommandMethodNetworkly(nameof(AddItemToInventory), gameItem);
            if (isInServer)
            {
                gameItem.SetAuthority(OwnerId);
            }
            if (hasAuthority)
            {
                AddItemToInventoryLocaly(gameItem);
            }
        }

        private void Inventory_OnItemRightClickedEvent(Inventory<GameItem> inventory, ItemSlotUi<GameItem> itemSlotUi)
        {
            InvokeCommandMethodNetworkly(nameof(PlayerDropItem), itemSlotUi.Item);
            inventory.DropItem(itemSlotUi, Location);
        }

        public void PlayerDropItem(GameItem item)
        {
            item.SetAuthority(EndPointId.InvalidIdentityId);
        }

        private void MoveFromUsableItemSlot(int slot)
        {
            if (usableItems.TryGetItemInSlot(slot, out GameItem item))
            {
                usableItems.TryRemoveItem(item);
                int equippedSlot = 0;
                if (item is Weapon weapon)
                {
                    EquipeWith(weapon);
                    equippedSlot = 2;
                }
                else if (item is Potion)
                {
                    equippedSlot = 1;
                }
                if (equippedItems.TryGetItemInSlot(equippedSlot, out GameItem otherItem))
                {
                    usableItems.TryAddItem(otherItem);
                }
                equippedItems.PutItemInSlot(equippedSlot, item);
            }
        }

        public override void Kill(Entity attacker)
        {
            base.Kill(attacker);
            if (hasAuthority)
            {
                InputManager.Instance.OnArrowsKeysStateChange -= Instance_OnArrowsKeysStateChange;
                ClearArrowsState();
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

        private void OnPickUpItemKeyPressed()
        {
            InvokeCommandMethodNetworkly(nameof(CmdTryPickUpItem));
        }

        private void OnUsableItemSlotKeyPressed(int slot)
        {
            MoveFromUsableItemSlot(slot);
        }

        private void OnStartInteractWithNpcKeyPressed()
        {
            if (requestingInteraction != null)
            {
                requestingInteraction.InvokeCommandMethodNetworkly(nameof(requestingInteraction.InteractionAcceptedByPlayer), this);
            }
        }

        private void OnRespawnKeyPressed()
        {
            if (SyncIsDead)
            {
                Respawn(SyncSpawnPoint.SyncX, SyncSpawnPoint.SyncY);
            }
        }

        private void OnPotionUseKeyPressed()
        {
            equippedItems.UsePotionAtSlot(1, this);
        }

        private void OnOpenCloseInventoryKeyPressed()
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

        private void OnAttackKeyPressed(bool isDown)
        {
            if (isDown)
            {
                if (!SyncIsDead && SyncEquippedWeapon != null && !(GetCurrentEnitytState<State>() == State.Attacking))
                {
                    Attack();
                }
            }
        }

        private void OnAnswerKeyPressed(int index)
        {
            if (interactingWith != null)
            {
                interactingWith.InvokeCommandMethodNetworkly(nameof(interactingWith.CmdChooseDialogOption), this, index);
            }
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
