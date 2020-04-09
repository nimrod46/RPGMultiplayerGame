using Map;
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
            if (!ServerManager.Instance.IsRuning)
            {
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xml|*.xml";
            if (!(openFileDialog.ShowDialog() == DialogResult.OK))
            {
                return;
            }
            XmlManager<GameMap> xml = new XmlManager<GameMap>();
            ServerManager.Instance.LoadMap(xml.Load(openFileDialog.FileName));
        }
    }
}
