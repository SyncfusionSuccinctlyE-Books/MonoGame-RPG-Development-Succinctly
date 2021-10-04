using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace MonoGameRPG.StateManagement
{
    // A screen is a single layer that has update and draw logic, and which
    // can be combined with other layers to build up a complex menu system.
    // For instance the main menu, the options menu, the "are you sure you
    // want to quit" message box, and the main game itself are all implemented
    // as screens.
    public abstract class GameScreen
    {
        // Normally when one screen is brought up over the top of another,
        // the first screen will transition off to make room for the new
        // one. This property indicates whether the screen is only a small
        // popup, in which case screens underneath it do not need to bother
        // transitioning off.
        public bool IsPopup { get; protected set; }

        protected TimeSpan TransitionOnTime
        {
            get => _transitionOnTime;
            set => _transitionOnTime = value;
        }
        private TimeSpan _transitionOnTime = TimeSpan.Zero;

        protected TimeSpan TransitionOffTime
        {
            private get => _transitionOffTime;
            set => _transitionOffTime = value;
        }
        private TimeSpan _transitionOffTime = TimeSpan.Zero;

        // Ranges from zero (fully active, no transition)
        // to one (transitioned fully off to nothing)
        protected float TransitionPosition
        {
            get => _transitionPosition;
            set => _transitionPosition = value;
        }
        private float _transitionPosition = 1;

        // Ranges from 1 (fully active, no transition)
        // to 0 (transitioned fully off to nothing)
        public float TransitionAlpha => 1f - TransitionPosition;

        // Gets the current screen transition state.
        public ScreenState ScreenState
        {
            get => _screenState;
            protected set => _screenState = value;
        }
        private ScreenState _screenState = ScreenState.TransitionOn;

        // There are two possible reasons why a screen might be transitioning
        // off. It could be temporarily going away to make room for another
        // screen that is on top of it, or it could be going away for good.
        // This property indicates whether the screen is exiting for real:
        // if set, the screen will automatically remove itself as soon as the
        // transition finishes.
        public bool IsExiting
        {
            get => _isExiting;
            protected internal set => _isExiting = value;
        }
        private bool _isExiting;

        // Checks whether this screen is active and can respond to user input.
        protected bool IsActive => !_otherScreenHasFocus &&
                                   (_screenState == ScreenState.TransitionOn ||
                                    _screenState == ScreenState.Active);

        private bool _otherScreenHasFocus;

        public ScreenManager ScreenManager
        {
            get => _screenManager;
            internal set => _screenManager = value;
        }
        private ScreenManager _screenManager;

        // Gets the index of the player who is currently controlling this screen,
        // or null if it is accepting input from any player. This is used to lock
        // the game to a specific player profile. The main menu responds to input
        // from any connected gamepad, but whichever player makes a selection from
        // this menu is given control over all subsequent screens, so other gamepads
        // are inactive until the controlling player returns to the main menu.
        public PlayerIndex? ControllingPlayer
        {
            protected get => _controllingPlayer;
            set { _controllingPlayer = value; }
        }
        private PlayerIndex? _controllingPlayer;

        // Gets the gestures the screen is interested in. Screens should be as specific
        // as possible with gestures to increase the accuracy of the gesture engine.
        // For example, most menus only need Tap or perhaps Tap and VerticalDrag to operate.
        // These gestures are handled by the ScreenManager when screens change and
        // all gestures are placed in the InputState passed to the HandleInput method.
        public GestureType EnabledGestures
        {
            get => _enabledGestures;
            protected set
            {
                _enabledGestures = value;

                // the screen manager handles this during screen changes, but
                // if this screen is active and the gesture types are changing,
                // we have to update the TouchPanel ourselves.
                if (ScreenState == ScreenState.Active)
                {
                    TouchPanel.EnabledGestures = value;
                }
            }
        }
        private GestureType _enabledGestures = GestureType.None;

        // Gets whether or not this screen is serializable. If this is true,
        // the screen will be recorded into the screen manager's state and
        // its Serialize and Deserialize methods will be called as appropriate.
        // If this is false, the screen will be ignored during serialization.
        // By default, all screens are assumed to be serializable.
        public bool IsSerializable
        {
            get => _isSerializable;
            protected set => _isSerializable = value;
        }
        private bool _isSerializable = true;
        
        // Activates the screen. Called when the screen is added to the screen manager or if the game resumes
        // from being paused or tombstoned.
        // instancePreserved is true if the game was preserved during deactivation, false if the screen is
        // just being added or if the game was tombstoned. On Xbox and Windows this will always be false.
        public virtual void Activate(bool instancePreserved) { }

        // Deactivates the screen. Called when the game is being deactivated due to pausing or tombstoning.
        protected virtual void Deactivate() { }

        // Unload content for the screen. Called when the screen is removed from the screen manager.
        public virtual void Unload() { }

        // Unlike HandleInput, this method is called regardless of whether the screen
        // is active, hidden, or in the middle of a transition.
        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            _otherScreenHasFocus = otherScreenHasFocus;

            if (_isExiting)
            {
                // If the screen is going away to die, it should transition off.
                _screenState = ScreenState.TransitionOff;

                if (!UpdateTransitionPosition(gameTime, _transitionOffTime, 1))
                    ScreenManager.RemoveScreen(this);    // When the transition finishes, remove the screen
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                _screenState = UpdateTransitionPosition(gameTime, _transitionOffTime, 1)
                    ? ScreenState.TransitionOff
                    : ScreenState.Hidden;
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                _screenState = UpdateTransitionPosition(gameTime, _transitionOnTime, -1) 
                    ? ScreenState.TransitionOn 
                    : ScreenState.Active;
            }
        }

        private bool UpdateTransitionPosition(GameTime gameTime, TimeSpan time, int direction)
        {
            float transitionDelta;    // How much should we move by?

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

            _transitionPosition += transitionDelta * direction;    // Update the transition position

            // Did we reach the end of the transition?
            if (direction < 0 && _transitionPosition <= 0 || direction > 0 && _transitionPosition >= 1)
            {
                _transitionPosition = MathHelper.Clamp(_transitionPosition, 0, 1);
                return false;
            }

            return true;    // Otherwise we are still busy transitioning
        }

        // Unlike Update, this method is only called when the screen is active,
        // and not when some other screen has taken the focus.
        public virtual void HandleInput(GameTime gameTime, InputState input) { }
        public virtual void Draw(GameTime gameTime) { }

        // Unlike ScreenManager.RemoveScreen, which instantly kills the screen, this method respects
        // the transition timings and will give the screen a chance to gradually transition off.
        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
                ScreenManager.RemoveScreen(this);    // If the screen has a zero transition time, remove it immediately
            else
                _isExiting = true;    // Otherwise flag that it should transition off and then exit.
        }
    }
}