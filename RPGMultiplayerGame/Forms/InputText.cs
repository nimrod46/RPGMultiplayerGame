using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RPGMultiplayerGame.Forms
{
    public partial class InputText : Form
    {
        public InputText()
        {
            InitializeComponent();
        }

        private void InputText_Load(object sender, EventArgs e)
        {
            Hide();
            
        }

        public string getText(string title)
        {
            string text = "";
            do
            {
                text = Interaction.InputBox("", title, "");
            } while (text == "");

            Close();
            return text;
        }
    }
}
