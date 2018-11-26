using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Forms;
using RPGMultiplayerGame.Objects;
using RPGMultiplayerGame.Objects.LivingEntities;

namespace RPGMultiplayerGame.Managers
{
    class GameManager
    {
        public enum EntityID
        {
            Player,

        }

        public enum Animation
        {
            WalkLeft,
            WalkUp,
            WalkRight,
            WalkDown,
            IdleLeft,
            IdleUp,
            IdleRight,
            IdleDown,
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

        public void SetLocalPlayerName(Player player)
        {
            InputText inputText = new InputText();
            inputText.Show();
            player.SetName(inputText.getText("Name"));
        }

        public Dictionary<EntityID, Dictionary<Animation, List<Texture2D>>> animationsByEntities = new Dictionary<EntityID, Dictionary<Animation, List<Texture2D>>>();
        public Texture2D HealthBar;
        public Texture2D HealthBarBackground;
        public SpriteFont PlayerName;
        private List<GameObject> gameObjects = new List<GameObject>();

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

                            animation.Add(content.Load<Texture2D>("Entities\\" + (EntityID)i + (Animation)j + k));
                            Console.WriteLine("Loaded: " + (EntityID)i + (Animation)j + k);
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                    animations.Add((Animation)j, animation);
                }
                animationsByEntities.Add(EntityID.Player, animations);
            }

            HealthBar = content.Load<Texture2D>("HealthBar");
            HealthBarBackground = content.Load<Texture2D>("HealthBarBackground");
            PlayerName = content.Load<SpriteFont>("PlayerName");
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            lock (gameObjects)
            {
                gameObjects.Remove(gameObject);
            }
        }
        
        public void AddObject(GameObject gameObject)
        {
            lock (gameObjects)
            {
                gameObjects.Add(gameObject);
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Update(gameTime);
            }
        }

        public void Draw(SpriteBatch sprite)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Draw(sprite);
            }
        }
    }
}
