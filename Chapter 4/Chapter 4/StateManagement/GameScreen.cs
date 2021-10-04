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
            get => transitionOnTime;
            set => transitionOnTime = value;
        }
        private TimeSpan transitionOnTime = TimeSpan.Zero;

        protected TimeSpan TransitionOffTime
        {
            private get => transitionOffTime;
            set => transitionOffTime = value;
        }
        private TimeSpan transitionOffTime = TimeSpan.Zero;

        // Ranges from zero (fully active, no transition)
        // to one (transitioned fully off to nothing)
        protected float TransitionPosition
        {
            get => transitionPosition;
            set => transitionPosition = value;
        }
        private float transitionPosition = 1;

        // Ranges from 1 (fully active, no transition)
        // to 0 (transitioned fully off to nothing)
        public float TransitionAlpha => 1f - TransitionPosition;

        // Gets the current screen transition state.
        public ScreenState ScreenState
        {
            get => screenState;
            protected set => screenState = value;
        }
        private ScreenState screenState = ScreenState.TransitionOn;

        // There are two possible reasons why a screen might be transitioning
        // off. It could be temporarily going away to make room for another
        // screen that is on top of it, or it could be going away for good.
        // This property indicates whether the screen is exiting for real:
        // if set, the screen will automatically remove itself as soon as the
        // transition finishes.
        public bool IsExiting
        {
            get => isExiting;
            protected internal set => isExiting = value;
        }
        private bool isExiting;

        // Checks whether this screen is active and can respond to user input.
        protected bool IsActive => !otherScreenHasFocus &&
                                   (screenState == ScreenState.TransitionOn ||
                                    screenState == ScreenState.Active);

        private bool otherScreenHasFocus;

        public ScreenManager ScreenManager
        {
            get => screenManager;
            internal set => screenManager = value;
        }
        private ScreenManager screenManager;

        // Gets the index of the player who is currently controlling this screen,
        // or null if it is accepting input from any player. This is used to lock
        // the game to a specific player profile. The main menu responds to input
        // from any connected gamepad, but whichever player makes a selection from
        // this menu is given control over all subsequent screens, so other gamepads
        // are inactive until the controlling player returns to the main menu.
        public PlayerIndex? ControllingPlayer
        {
            protected get => controllingPlayer;
            set { controllingPlayer = value; }
        }
        private PlayerIndex? controllingPlayer;

        // Gets the gestures the screen is interested in. Screens should be as specific
        // as possible with gestures to increase the accuracy of the gesture engine.
        // For example, most menus only need Tap or perhaps Tap and VerticalDrag to operate.
        // These gestures are handled by the ScreenManager when screens change and
        // all gestures are placed in the InputState passed to the HandleInput method.
        public GestureType EnabledGestures
        {
            get => enabledGestures;
            protected set
            {
                enabledGestures = value;

                // the screen manager handles this during screen changes, but
                // if this screen is active and the gesture types are changing,
                // we have to update the TouchPanel ourselves.
                if (ScreenState == ScreenState.Active)
                {
                    TouchPanel.EnabledGestures = value;
                }
            }
        }
        private GestureType enabledGestures = GestureType.None;

        // Gets whether or not this screen is serializable. If this is true,
        // the screen will be recorded into the screen manager's state and
        // its Serialize and Deserialize methods will be called as appropriate.
        // If this is false, the screen will be ignored during serialization.
        // By default, all screens are assumed to be serializable.
        public bool IsSerializable
        {
            get => isSerializable;
            protected set => isSerializable = value;
        }
        private bool isSerializable = true;
        
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
            otherScreenHasFocus = otherScreenHasFocus;

            if (isExiting)
            {
                // If the screen is going away to die, it should transition off.
                screenState = ScreenState.TransitionOff;

                if (!UpdateTransitionPosition(gameTime, transitionOffTime, 1))
                    ScreenManager.RemoveScreen(this);    // When the transition finishes, remove the screen
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                screenState = UpdateTransitionPosition(gameTime, transitionOffTime, 1)
                    ? ScreenState.TransitionOff
                    : ScreenState.Hidden;
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                screenState = UpdateTransitionPosition(gameTime, transitionOnTime, -1) 
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

            transitionPosition += transitionDelta * direction;    // Update the transition position

            // Did we reach the end of the transition?
            if (direction < 0 && transitionPosition <= 0 || direction > 0 && transitionPosition >= 1)
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
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
                isExiting = true;    // Otherwise flag that it should transition off and then exit.
        }
    }
}