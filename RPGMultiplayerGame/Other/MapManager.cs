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

namespace RPGMultiplayerGame.Other
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
        public GameMap map;
        public List<Texture2D> textures = new List<Texture2D>();
        public NetBlock spawnPoint;
        private List<GameObject> gameObjects = new List<GameObject>();
        MapManager()
        {
            map = new GameMap();
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

        public void RemoveGameObject(GameObject gameObject)
        {
            lock (gameObjects)
            {
                gameObjects.Remove(gameObject);
            }
        }

        public void AddGameObject(GameObject gameObject)
        {
            lock (gameObjects)
            {
                gameObjects.Add(gameObject);
            }
        }

        public void Draw(SpriteBatch sprite)
        {
            lock (gameObjects)
            {
                foreach (GameObject obj in gameObjects)
                {
                    obj.Draw(sprite);
                }
            }
        }
    }
}

