using RPGMultiplayerGame.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RPGMultiplayerGame
{
    public partial class LobbyMenu : Form
    {
        Form gameForm;
        public delegate void OnConnectingEvent(Form form);
        public event OnConnectingEvent OnConnectionEstablished;
        public LobbyMenu(Form gameForm)
        {
            InitializeComponent();
            this.gameForm = gameForm;
            NetworkManager.Instance.Init(ref LobbyList);
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            if (NetworkManager.Instance.Connect())
            {
                OnConnectionEstablished.Invoke(this);
            }
        }

        private void AddServer_Click(object sender, EventArgs e)
        {

            NetworkManager.Instance.AddServer();
        }

        private void Refresh_Click(object sender, EventArgs e)
        {

            NetworkManager.Instance.Refersh();
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            if (LobbyList.SelectedItems.Count > 0)
            {
                ListViewItem item = LobbyList.SelectedItems[0];
                NetworkManager.Instance.Remove(item);
            }
        }

        private void StartServer_Click(object sender, EventArgs e)
        {
            NetworkManager.Instance.StartServer();
        }

        private void LobbyMenu_Shown(object sender, EventArgs e)
        {
            NetworkManager.Instance.lobby.LoadFromFile();
        }
    }
}
