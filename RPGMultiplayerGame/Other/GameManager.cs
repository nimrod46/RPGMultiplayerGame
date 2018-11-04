using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Networking;

namespace RPGMultiplayerGame.Other
{
    class GameManager
    {
        public enum EntityID
        {
            Player,

        }

        public enum Animation
        {
            Walk
        }

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameManager();
                }
                return instance;
            }
        }

        static GameManager instance;

        public Dictionary<EntityID, Dictionary<Animation, List<Texture2D>>> animationsByEntities = new Dictionary<EntityID, Dictionary<Animation, List<Texture2D>>>();
        public List<GameObject> gameObjects = new List<GameObject>();

        private GameManager()
        {

        }

        public void LoadTextures(GraphicsDevice graphicsDevice, ContentManager content)
        {
            for (int i = 0; i < (int)Enum.GetValues(typeof(EntityID)).Cast<EntityID>().Last() + 1; i++)
            {
                Dictionary<Animation, List<Texture2D>> animations = new Dictionary<Animation, List<Texture2D>>();
                for (int j = 0; j < (int)Enum.GetValues(typeof(Animation)).Cast<Animation>().Last() + 1; j++)
                {
                    List<Texture2D> animation = new List<Texture2D>();
                    for (int k = 1; k <= 32; k++)
                    {
                        try
                        {
                            animation.Add(content.Load<Texture2D>("" + (EntityID)i + (Animation)j + k));
                            Console.WriteLine("Loaded: " + (EntityID)i + (Animation)j + k);
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                    animations.Add((Animation)i, animation);
                }
                animationsByEntities.Add(EntityID.Player, animations);
            }
        }

        internal void Update(GameTime gameTime)
        {
            lock (gameObjects)
            {
                foreach (GameObject obj in gameObjects)
                {
                    obj.Update(gameTime);
                }
            }
        }

        public void AddGameObject(GameObject gameObject)
        {
            lock (gameObjects)
            {
                gameObjects.Add(gameObject);
            }
        }
    }
}
