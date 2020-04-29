using Networking;
using RPGMultiplayerGame.MapObjects;
using RPGMultiplayerGame.Objects;
using RPGMultiplayerGame.Objects.Items.Potions;
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Objects.Items.Weapons.WeaponEffects;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.QuestsObjects.Quests;
using ServerLobby;
using System.Windows.Forms;

namespace RPGMultiplayerGame.Managers
{
    public class NetworkManager<T> where T : NetworkBehavior
    {
        public T NetBehavior { get; protected set; }
        public Lobby lobby;
        public ListView LobbyList;

        protected NetworkManager()
        {
        }

        public void Init(ref ListView lobbyList)
        {
            LobbyList = lobbyList;
            lobby = new Lobby(ref LobbyList, 1331, "RPG Game");
            RegisterNetworkElements();
        }

        private void RegisterNetworkElements()
        {
            new Block();
            new Player();
            new SpawnPoint();
            new Joe();
            new Blacksmith();
            new Bat();
            new JoeKillQuest();
            new FireBall();
            new BatClaw();
            new CommonSword();
            new CommonWond();
            new CommonHealthPotion();
        }

        public void AddServer()
        {
            lobby.AddServer();
        }

        public void Refersh()
        {
            lobby.RefreshAll();
        }

        public void Remove(ListViewItem item)
        {
            lobby.Remove(item);
        }
    }
}
