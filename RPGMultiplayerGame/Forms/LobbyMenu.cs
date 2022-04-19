using System;
using System.Windows.Forms;
using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Forms
{
    public partial class LobbyMenu : Form
    {
        Form gameForm;
        public delegate void ConnectingEvent(Form form);
        public event ConnectingEvent? OnConnectionEstablished;
        public delegate void ServerOnline(Form form);
        public event ConnectingEvent? OnServerOnline;

        public LobbyMenu(Form gameForm)
        {
            InitializeComponent();
            this.gameForm = gameForm;
            ClientManager.Instance.Init(ref LobbyList);
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            if (ClientManager.Instance.Connect())
            {
                OnConnectionEstablished?.Invoke(this);
            }
        }

        private void AddServer_Click(object sender, EventArgs e)
        {

            ClientManager.Instance.AddServer();
        }

        private void Refresh_Click(object sender, EventArgs e)
        {

            ClientManager.Instance.Refersh();
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            if (LobbyList.SelectedItems.Count > 0)
            {
                ListViewItem item = LobbyList.SelectedItems[0];
                ClientManager.Instance.Remove(item);
            }
        }

        private void StartServer_Click(object sender, EventArgs e)
        {
            ServerManager.Instance.StartServer();
            OnServerOnline?.Invoke(this);
        }

        private void LobbyMenu_Shown(object sender, EventArgs e)
        {
            ClientManager.Instance.lobby.LoadFromFile();
        }
    }
}
