using Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Networking;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RPGMultiplayerGame
{
    class MapManager
    {
        public static MapManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MapManager();
                }
                return instance;
            }
        }
        static MapManager instance;
        public List<Texture2D> textures = new List<Texture2D>();
        public List<NetBlock> blocks = new List<NetBlock>();
        public GameMap currentGameMap { get; set; }
        MapManager()
        {
        }
        public void LoadSpriteSheet(GraphicsDevice graphicsDevice, Texture2D spriteTextures)
        {
            ImageList list = new ImageList();
            int count = 0;
            for (int y = 0; y < spriteTextures.Height; y += 16)
            {
                for (int x = 0; x < spriteTextures.Width; x += 16)
                {
                    Texture2D texture = new Texture2D(graphicsDevice, 16, 16);
                    Rectangle cloneRect = new Rectangle(x, y, 16, 16);
                    int c = cloneRect.Width * cloneRect.Height;
                    Color[] data = new Color[c];
                    spriteTextures.GetData(0, cloneRect, data, 0, c);
                    texture.SetData(data);
                    textures.Add(texture);
                    count++;
                }
            }
        }

        public void Draw(SpriteBatch sprite)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i].Draw(sprite);
            }
        }
    }
}

