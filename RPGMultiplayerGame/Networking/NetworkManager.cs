﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;
using ServerLobby;
using System.Windows.Forms;
using System.Threading;
using Map;
using RPGMultiplayerGame.Other;

namespace RPGMultiplayerGame.Networking
{
    public class NetworkManager
    {
        public NetworkBehavior NetBehavior { get; private set; }
        public Lobby lobby;
        public ListView LobbyList;
        public static NetworkManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NetworkManager();
                }
                return instance;
            }
        }
        static NetworkManager instance;
        Player client;
        NetworkManager()
        {
        }

        public void Init(ref ListView lobbyList)
        {
            LobbyList = lobbyList;
            client = new Player();
            lobby = new Lobby(client, ref LobbyList, 1331, "RPG Game");
            RegisterNetworkElements();
        }

        private void RegisterNetworkElements()
        {
            new NetBlock();
            new Player();
        }

        public void LoadMap(GameMap gameMap)
        {
            MapManager.Instance.map = gameMap;
            foreach (Block block in gameMap.blocks)
            {
                NetBlock netBlock = new NetBlock
                {
                    SyncTextureIndex = block.ImageIndex,
                    SyncX = block.Rectangle.X,
                    SyncY = block.Rectangle.Y,
                    SyncLayer = block.Layer
                };
                NetBehavior.spawnWithServerAuthority(typeof(NetBlock), netBlock);
            }
        }

        public void Start()
        {
            NetBehavior.player.Synchronize();
        }

        public void StartServer()
        {
            NetBehavior = new NetworkBehavior(new Player(), 1331);
            NetBehavior.StartServer();
        }

        public bool Connect()
        {
            NetBehavior = lobby.Connect();
            if (NetBehavior != null)
            {
                return true;
            }
            return false;
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
