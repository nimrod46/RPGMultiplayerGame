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
            LobbyList.OwnerDraw = true;
            Console.WriteLine("LobbyMenu");
            LobbyList.DrawItem += LobbyList_DrawItem;
            LobbyList.DrawSubItem += LobbyList_DrawSubItem; ;
            LobbyList.DrawColumnHeader += LobbyList_DrawColumnHeader;
            this.gameForm = gameForm;
            NetworkManager.Instance.Init(ref LobbyList);
        }

        private void LobbyList_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void LobbyList_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void LobbyList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = false;
            e.Graphics.FillRectangle(Brushes.Aquamarine, e.Bounds);
            e.DrawText();
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
