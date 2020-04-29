using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RPGMultiplayerGame.Managers
{
    public class InputManager
    {
        KeyboardState currentKeyState, prevKeyState;
        MouseState currentMouseState, prevMouseState;
        private static InputManager instance;


        public static InputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InputManager();
                }
                return instance;
            }
        }

        public delegate void ArrowsKeysStateChange(Keys key, bool isDown);
        public event ArrowsKeysStateChange OnArrowsKeysStateChange;

        public void Update(GameTime gameTime)
        {
            prevKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
            prevMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            if (prevKeyState.IsKeyUp(Keys.Up) && currentKeyState.IsKeyDown(Keys.Up) || prevKeyState.IsKeyDown(Keys.Up) && currentKeyState.IsKeyUp(Keys.Up))
            {
                OnArrowsKeysStateChange?.Invoke(Keys.Up, currentKeyState.IsKeyDown(Keys.Up));
            }
            if (prevKeyState.IsKeyUp(Keys.Down) && currentKeyState.IsKeyDown(Keys.Down) || prevKeyState.IsKeyDown(Keys.Down) && currentKeyState.IsKeyUp(Keys.Down))
            {
                OnArrowsKeysStateChange?.Invoke(Keys.Down, currentKeyState.IsKeyDown(Keys.Down));
            }
            if (prevKeyState.IsKeyUp(Keys.Right) && currentKeyState.IsKeyDown(Keys.Right) || prevKeyState.IsKeyDown(Keys.Right) && currentKeyState.IsKeyUp(Keys.Right))
            {
                OnArrowsKeysStateChange?.Invoke(Keys.Right, currentKeyState.IsKeyDown(Keys.Right));
            }
            if (prevKeyState.IsKeyUp(Keys.Left) && currentKeyState.IsKeyDown(Keys.Left) || prevKeyState.IsKeyDown(Keys.Left) && currentKeyState.IsKeyUp(Keys.Left))
            {
                OnArrowsKeysStateChange?.Invoke(Keys.Left, currentKeyState.IsKeyDown(Keys.Left));
            }
        }

        public bool GetMouseLeftButtonPressed()
        {
            return prevMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed;
        }

        public Rectangle MouseBounds()
        {
            return new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);
        }

        public Keys KeyPressed(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (KeyPressed(key))
                {
                    return key;
                }
            }
            return Keys.None;
        }

        public bool KeyReleased(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (KeyReleased(key))
                {
                    return true;
                }
            }
            return false;
        }

        public bool KeyDown(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (KeyDown(key))
                {
                    return true;
                }
            }
            return false;
        }

        public bool KeyPressed(Keys firstKey, Keys lastKey, out Keys pressedKey)
        {
            pressedKey = Keys.None;
            for (int i = (int)firstKey; i <= (int)lastKey; i++)
            {
                if (KeyPressed((Keys)i))
                {
                    pressedKey = (Keys)i;
                    return true;
                }
            }
            return false;
        }


        public bool KeyPressed(Keys key)
        {
            return currentKeyState.IsKeyDown(key) && prevKeyState.IsKeyUp(key);
        }

        public bool KeyReleased(Keys key)
        {

            return currentKeyState.IsKeyUp(key) && prevKeyState.IsKeyDown(key);
        }

        public bool KeyDown(Keys key)
        {
            return currentKeyState.IsKeyDown(key);
        }
    }
}
