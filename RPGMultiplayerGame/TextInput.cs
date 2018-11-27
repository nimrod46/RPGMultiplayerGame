using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame
{
    public class TextInput
    {
        public static string getText(string title)
        {
            string text = "";
            do
            {
                text = Interaction.InputBox("", title, "").Trim();
            } while (text == "");

            return text;
        }
    }
}
