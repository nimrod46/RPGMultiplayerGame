using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;
using ServerLobby;
using System.Windows.Forms;
using System.Threading;
using Map;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects;
using RPGMultiplayerGame.MapObjects;
using Microsoft.Xna.Framework;

namespace RPGMultiplayerGame.Managers
{
    public class NetworkManager<T> where T : NetworkBehavior 
    {
        public T NetBehavior { get; protected set; }
        public Lobby lobby;
        public ListView LobbyList;
        public bool isServer;

        protected NetworkManager()
        {
            isServer = false;
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
            new Waypoint();
            new Joe();
            new Blacksmith();
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
