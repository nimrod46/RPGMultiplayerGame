using Microsoft.VisualBasic;

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
