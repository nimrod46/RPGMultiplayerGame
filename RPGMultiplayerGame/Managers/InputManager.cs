using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace RPGMultiplayerGame.Managers
{
    public class InputManager
    {
       
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

        public enum InputState
        {
            PlayerControl,
            Chat
        }

        public delegate void ArrowsKeysStateChange(Keys key, bool isDown);
        public event ArrowsKeysStateChange OnArrowsKeysStateChange;
        public delegate void PlayerContorlKeyPressed();
        public event PlayerContorlKeyPressed OnRespawnKeyPressed;
        public event PlayerContorlKeyPressed OnOpenCloseInventoryKeyPressed;
        public event PlayerContorlKeyPressed OnPotionUseKeyPressed;
        public event PlayerContorlKeyPressed OnStartInteractWithNpcKeyPressed;
        public event PlayerContorlKeyPressed OnPickUpItemKeyPressed;

        public delegate void PlayerContorlIndexKeyPressed(int index);
        public event PlayerContorlIndexKeyPressed OnUsableItemSlotKeyPressed;
        public event PlayerContorlIndexKeyPressed OnAnswerKeyPressed;

        public delegate void PlayerContorlKeyStateChanged(bool isDown);
        public event PlayerContorlKeyStateChanged OnAttackKeyPressed;

        public delegate void CharEnteredHandler(char character);
        public event CharEnteredHandler OnCharPressed;
        public delegate void KeyEventHandler(Keys KeyCode);
        public event KeyEventHandler OnKeyPressed;
        public event KeyEventHandler OnKeyDown;
        public event KeyEventHandler OnKeyUp;
        public delegate void InputStateEventHandler(InputState inputState);
        public event InputStateEventHandler OnInputStateChange;


        private readonly char[] SPECIAL_CHARACTERS = { '\a', '\b', '\n', '\r', '\f', '\t', '\v', '`', '|', '~' };
        private Game game;
        private KeyboardState currentKeyState, prevKeyState;
        private MouseState currentMouseState, prevMouseState;
        private static Keys? repChar;
        private static DateTime downSince = DateTime.Now;
        private static float timeUntilRepInMillis;
        private static int repsPerSec;
        private static DateTime lastRep = DateTime.Now;
        private static bool filterSpecialCharacters;
        private InputState inputState;

        public void Initialize(Game g, float timeUntilRepInMilliseconds, int repsPerSecond,
            bool filterSpecialCharactersFromCharPressed = true)
        {
            inputState = InputState.PlayerControl;
            game = g;
            timeUntilRepInMillis = timeUntilRepInMilliseconds;
            repsPerSec = repsPerSecond;
            filterSpecialCharacters = filterSpecialCharactersFromCharPressed;
            game.Window.TextInput += TextEntered;
        }

        public bool ShiftDown
        {
            get
            {
                KeyboardState state = Keyboard.GetState();
                return state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift);
            }
        }

        public bool CtrlDown
        {
            get
            {
                KeyboardState state = Keyboard.GetState();
                return state.IsKeyDown(Keys.LeftControl) || state.IsKeyDown(Keys.RightControl);
            }
        }

        public bool AltDown
        {
            get
            {
                KeyboardState state = Keyboard.GetState();
                return state.IsKeyDown(Keys.LeftAlt) || state.IsKeyDown(Keys.RightAlt);
            }
        }

        private void TextEntered(object sender, TextInputEventArgs e)
        {
            if (OnCharPressed != null)
            {
                if (!filterSpecialCharacters || !SPECIAL_CHARACTERS.Contains(e.Character))
                {
                    OnCharPressed(e.Character);
                }
            }
        }

        public void Update(GameTime _)
        {
            prevKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
            prevMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            if (KeyPressed(Keys.OemTilde))
            {
                inputState = inputState == InputState.Chat ? InputState.PlayerControl : InputState.Chat;
                OnInputStateChange?.Invoke(inputState);
            }

            foreach (Keys key in (Keys[])Enum.GetValues(typeof(Keys)))
            {
                if (KeyPressed(key))
                {
                    OnKeyDown?.Invoke(key);
                    if (OnKeyPressed != null)
                    {
                        downSince = DateTime.Now;
                        repChar = key;
                        OnKeyPressed(key);
                    }
                }
                else if (KeyReleased(key))
                {
                    if (OnKeyUp != null)
                    {
                        if (repChar == key)
                        {
                            repChar = null;
                        }
                        OnKeyUp(key);
                    }
                }

                if (repChar != null && repChar == key && currentKeyState.IsKeyDown(key))
                {
                    DateTime now = DateTime.Now;
                    TimeSpan downFor = now.Subtract(downSince);
                    if (downFor.CompareTo(TimeSpan.FromMilliseconds(timeUntilRepInMillis)) > 0)
                    {
                        // Should repeat since the wait time is over now.
                        TimeSpan repeatSince = now.Subtract(lastRep);
                        if (repeatSince.CompareTo(TimeSpan.FromMilliseconds(1000f / repsPerSec)) > 0)
                        {
                            // Time for another key-stroke.
                            if (OnKeyPressed != null)
                            {
                                lastRep = now;
                                OnKeyPressed(key);
                            }
                        }
                    }
                }
            }


            if (inputState == InputState.PlayerControl)
            {
                if (KeyPressed(Keys.Up) || KeyReleased(Keys.Up))
                {
                    OnArrowsKeysStateChange?.Invoke(Keys.Up, currentKeyState.IsKeyDown(Keys.Up));
                }
                if (KeyPressed(Keys.Down) || KeyReleased(Keys.Down))
                {
                    OnArrowsKeysStateChange?.Invoke(Keys.Down, currentKeyState.IsKeyDown(Keys.Down));
                }
                if (KeyPressed(Keys.Right) || KeyReleased(Keys.Right))
                {
                    OnArrowsKeysStateChange?.Invoke(Keys.Right, currentKeyState.IsKeyDown(Keys.Right));
                }
                if (KeyPressed(Keys.Left) || KeyReleased(Keys.Left))
                {
                    OnArrowsKeysStateChange?.Invoke(Keys.Left, currentKeyState.IsKeyDown(Keys.Left));
                }
                if (KeyPressed(Keys.R))
                {
                    OnRespawnKeyPressed?.Invoke();
                }
                if (KeyPressed(Keys.I))
                {
                    OnOpenCloseInventoryKeyPressed?.Invoke();
                }

                if (KeyPressed(Keys.A))
                {
                    OnUsableItemSlotKeyPressed?.Invoke(1);
                }
                else if (KeyPressed(Keys.S))
                {
                    OnUsableItemSlotKeyPressed?.Invoke(2);
                }
                else if (KeyPressed(Keys.D))
                {
                    OnUsableItemSlotKeyPressed?.Invoke(3);
                }
                else if (KeyPressed(Keys.F))
                {
                    OnUsableItemSlotKeyPressed?.Invoke(4);
                }

                if (KeyPressed(Keys.Z))
                {
                    OnPotionUseKeyPressed?.Invoke();
                }

                if (KeyPressed(Keys.C))
                {
                    OnPickUpItemKeyPressed?.Invoke();
                }

                if (KeyPressed(Keys.X) || KeyReleased(Keys.X))
                {
                    OnAttackKeyPressed(KeyDown(Keys.X));
                }
                if (KeyPressed(Keys.Q))
                {
                    OnStartInteractWithNpcKeyPressed?.Invoke();
                }
                if (KeyPressed(Keys.D1, Keys.D9, out Keys pressedKey))
                {
                    OnAnswerKeyPressed(pressedKey - Keys.D1);
                }
            }
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

        public bool GetMouseRightButtonPressed()
        {
            return prevMouseState.RightButton == ButtonState.Released && currentMouseState.RightButton == ButtonState.Pressed;
        }

        public bool GetMouseLeftButtonPressed()
        {
            return prevMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed;
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
