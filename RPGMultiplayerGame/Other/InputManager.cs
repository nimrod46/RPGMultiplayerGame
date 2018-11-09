using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Other
{
    public class InputManager
    {
        KeyboardState currentKeyState, prevKeyState;
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
            if (prevKeyState.IsKeyUp(Keys.Up) && currentKeyState.IsKeyDown(Keys.Up) || prevKeyState.IsKeyDown(Keys.Up) && currentKeyState.IsKeyUp(Keys.Up))
            {
                OnArrowsKeysStateChange.Invoke(Keys.Up, currentKeyState.IsKeyDown(Keys.Up));
            }
            if (prevKeyState.IsKeyUp(Keys.Down) && currentKeyState.IsKeyDown(Keys.Down) || prevKeyState.IsKeyDown(Keys.Down) && currentKeyState.IsKeyUp(Keys.Down))
            {
                OnArrowsKeysStateChange.Invoke(Keys.Down, currentKeyState.IsKeyDown(Keys.Down));
            }
            if (prevKeyState.IsKeyUp(Keys.Right) && currentKeyState.IsKeyDown(Keys.Right) || prevKeyState.IsKeyDown(Keys.Right) && currentKeyState.IsKeyUp(Keys.Right))
            {
                OnArrowsKeysStateChange.Invoke(Keys.Right, currentKeyState.IsKeyDown(Keys.Right));
            }
            if (prevKeyState.IsKeyUp(Keys.Left) && currentKeyState.IsKeyDown(Keys.Left) || prevKeyState.IsKeyDown(Keys.Left) && currentKeyState.IsKeyUp(Keys.Left))
            {
                OnArrowsKeysStateChange.Invoke(Keys.Left, currentKeyState.IsKeyDown(Keys.Left));
            }
        }

        public bool KeyPressed(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (currentKeyState.IsKeyDown(key) && prevKeyState.IsKeyUp(key))
                {
                    return true;
                }
            }
            return false;
        }

        public bool KeyReleased(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (currentKeyState.IsKeyUp(key) && prevKeyState.IsKeyDown(key))
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
                if (currentKeyState.IsKeyDown(key))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
