using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace MonoGameRPG.StateManagement
{
    public enum MouseButtons
    {
        Left,
        Middle,
        Right
    }

    // Helper for reading input from keyboard, gamepad, and touch input. This class 
    // tracks both the current and previous state of the input devices, and implements 
    // query methods for high level input actions such as "move up through the menu"
    // or "pause the game".
    public class InputState
    {
        private const int MaxInputs = 4;

        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;
        public readonly MouseState[] CurrentMouseState;

        private readonly KeyboardState[] _lastKeyboardStates;
        private readonly GamePadState[] _lastGamePadStates;
        private readonly MouseState[] _lastMouseState;

        public readonly bool[] GamePadWasConnected;

        public TouchCollection TouchState;
        private readonly List<GestureSample> _gestures = new List<GestureSample>();
        
        public InputState()
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];
            CurrentMouseState = new MouseState[1];

            _lastKeyboardStates = new KeyboardState[MaxInputs];
            _lastGamePadStates = new GamePadState[MaxInputs];
            _lastMouseState = new MouseState[1];


            GamePadWasConnected = new bool[MaxInputs];
        }

        // Reads the latest user input state.
        public void Update()
        {
            _lastMouseState[0] = CurrentMouseState[0];
            CurrentMouseState[0] = Mouse.GetState();

            for (int i = 0; i < MaxInputs; i++)
            {
                _lastKeyboardStates[i] = CurrentKeyboardStates[i];
                _lastGamePadStates[i] = CurrentGamePadStates[i];

                CurrentKeyboardStates[i] = Keyboard.GetState();
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

                // Keep track of whether a gamepad has ever been
                // connected, so we can detect if it is unplugged.
                if (CurrentGamePadStates[i].IsConnected)
                    GamePadWasConnected[i] = true;
            }

            // Get the raw touch state from the TouchPanel
            TouchState = TouchPanel.GetState();

            // Read in any detected gestures into our list for the screens to later process
            _gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                _gestures.Add(TouchPanel.ReadGesture());
            }
        }

        // Helper for checking if a key was pressed during this update. The
        // controllingPlayer parameter specifies which player to read input for.
        // If this is null, it will accept input from any player. When a keypress
        // is detected, the output playerIndex reports which player pressed it.
        public bool IsKeyPressed(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return CurrentKeyboardStates[i].IsKeyDown(key);
            }

            // Accept input from any player.
            return IsKeyPressed(key, PlayerIndex.One, out playerIndex) ||
                   IsKeyPressed(key, PlayerIndex.Two, out playerIndex) ||
                   IsKeyPressed(key, PlayerIndex.Three, out playerIndex) ||
                   IsKeyPressed(key, PlayerIndex.Four, out playerIndex);
        }

        // Helper for checking if a button was pressed during this update.
        // The controllingPlayer parameter specifies which player to read input for.
        // If this is null, it will accept input from any player. When a button press
        // is detected, the output playerIndex reports which player pressed it.
        public bool IsButtonPressed(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return CurrentGamePadStates[i].IsButtonDown(button);
            }

            // Accept input from any player.
            return IsButtonPressed(button, PlayerIndex.One, out playerIndex) ||
                   IsButtonPressed(button, PlayerIndex.Two, out playerIndex) ||
                   IsButtonPressed(button, PlayerIndex.Three, out playerIndex) ||
                   IsButtonPressed(button, PlayerIndex.Four, out playerIndex);
        }


        // Helper for checking if a key was newly pressed during this update. The
        // controllingPlayer parameter specifies which player to read input for.
        // If this is null, it will accept input from any player. When a keypress
        // is detected, the output playerIndex reports which player pressed it.
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyDown(key) &&
                        _lastKeyboardStates[i].IsKeyUp(key));
            }

            // Accept input from any player.
            return IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                   IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                   IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                   IsNewKeyPress(key, PlayerIndex.Four, out playerIndex);
        }

        // Helper for checking if a button was newly pressed during this update.
        // The controllingPlayer parameter specifies which player to read input for.
        // If this is null, it will accept input from any player. When a button press
        // is detected, the output playerIndex reports which player pressed it.
        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return CurrentGamePadStates[i].IsButtonDown(button) &&
                       _lastGamePadStates[i].IsButtonUp(button);
            }

            // Accept input from any player.
            return IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                   IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                   IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                   IsNewButtonPress(button, PlayerIndex.Four, out playerIndex);
        }

        public bool IsNewMouseClick(MouseButtons button)
        {
            switch(button)
            {
                case MouseButtons.Left:

                    return CurrentMouseState[0].LeftButton == ButtonState.Pressed && _lastMouseState[0].LeftButton == ButtonState.Released;

                case MouseButtons.Middle:

                    return CurrentMouseState[0].MiddleButton == ButtonState.Pressed && _lastMouseState[0].MiddleButton == ButtonState.Released;

                case MouseButtons.Right:

                    return CurrentMouseState[0].RightButton == ButtonState.Pressed && _lastMouseState[0].RightButton == ButtonState.Released;
            }

            return false;
        }
    }
}