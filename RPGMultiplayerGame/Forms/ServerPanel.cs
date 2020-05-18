using Map;
using RPGMultiplayerGame.Managers;
using System;
using System.Windows.Forms;

namespace RPGMultiplayerGame
{
    public partial class ServerPanel : Form
    {
        public ServerPanel()
        {
            InitializeComponent();
        }

        private void LoadMap_Click(object sender, EventArgs e)
        {
            if (!ServerManager.Instance.IsRunning)
            {
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "xml|*.xml"
            };
            if (!(openFileDialog.ShowDialog() == DialogResult.OK))
            {
                return;
            }
            XmlManager<GameMapLib> xml = new XmlManager<GameMapLib>();
            ServerManager.Instance.LoadMap(xml.Load(openFileDialog.FileName));
        }

        private void Save_Click(object sender, EventArgs e)
        {
            ServerManager.Instance.SaveGame();
        }

        private void LoadSave_Click(object sender, EventArgs e)
        {
            ServerManager.Instance.LoadSaveGame();
        }
    }
}
