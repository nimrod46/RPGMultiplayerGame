using Map;
using RPGMultiplayerGame.GameSaver;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            XmlManager<GameMap> xml = new XmlManager<GameMap>();
            ServerManager.Instance.LoadMap(xml.Load(openFileDialog.FileName));
        }

        private void Save_Click(object sender, EventArgs e)
        {
            XmlManager<GameSave> xml = new XmlManager<GameSave>();
            ServerManager.Instance.SaveGame();
            xml.Save(@"C:\Users\nimrod\Desktop\game save.xml", ServerManager.Instance.gameSave);
        }

        private void LoadSave_Click(object sender, EventArgs e)
        {
            XmlManager<GameSave> xml = new XmlManager<GameSave>();
            ServerManager.Instance.gameSave = xml.Load(@"C:\Users\nimrod\Desktop\game save.xml");
            ServerManager.Instance.LoadSaveGame();
        }
    }
}
