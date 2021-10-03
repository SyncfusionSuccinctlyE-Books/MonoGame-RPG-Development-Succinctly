using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameRPG.StateManagement;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;




namespace MonoGameRPG.Screens
{
    public class AudioOptionsScreen : MenuScreen
    {
        
        MenuEntry masterVolume;
        MenuEntry musicVolume;
        MenuEntry sfxVolume;

        InputAction sliderUp;
        InputAction sliderDown;

        protected float volumeDelta = .005f;

        public AudioOptionsScreen() : base("Audio Options")
        {            
            MenuEntry back = new MenuEntry("Back");

            masterVolume = new MenuEntry($"Master Volume");
            musicVolume = new MenuEntry($"Music Volume");
            sfxVolume = new MenuEntry($"SFX Volume");

            masterVolume.Selected += SetMasterVolume;
            back.Selected += OnCancel;

            MenuEntries.Add(masterVolume);
            MenuEntries.Add(musicVolume);
            MenuEntries.Add(sfxVolume);
            MenuEntries.Add(back);

            sliderUp = new InputAction(
               new[] { Buttons.DPadRight, Buttons.LeftThumbstickRight },
               new[] { Keys.Right }, false);

            sliderDown = new InputAction(
               new[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft },
               new[] { Keys.Left }, false);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            // save the audio settings.
            ScreenManager.SaveAudioSettings();

            base.OnCancel(playerIndex);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            base.HandleInput(gameTime, input);
            PlayerIndex playerIndex;

            if (sliderDown.Occurred(input, ControllingPlayer, out playerIndex))
            {
                if(_selectedEntry == 0)
                    ScreenManager.audioManager.MasterVolume = MathF.Max(0f, ScreenManager.audioManager.MasterVolume - volumeDelta);
                else if(_selectedEntry == 1)
                    ScreenManager.audioManager.MusicVolume = MathF.Max(0f, ScreenManager.audioManager.MusicVolume - volumeDelta);
                else if(_selectedEntry == 2)
                    ScreenManager.audioManager.SFXVolume = MathF.Max(0f, ScreenManager.audioManager.SFXVolume - volumeDelta);
            }

            if (sliderUp.Occurred(input, ControllingPlayer, out playerIndex))
            {
                if (_selectedEntry == 0)
                    ScreenManager.audioManager.MasterVolume = MathF.Max(0f, ScreenManager.audioManager.MasterVolume + volumeDelta);
                else if (_selectedEntry == 1)
                    ScreenManager.audioManager.MusicVolume = MathF.Max(0f, ScreenManager.audioManager.MusicVolume + volumeDelta);
                else if (_selectedEntry == 2)
                    ScreenManager.audioManager.SFXVolume = MathF.Max(0f, ScreenManager.audioManager.SFXVolume + volumeDelta);
            }
        }

        protected void SetMasterVolume(object sender, PlayerIndexEventArgs args)
        {
            // do nowt..
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            masterVolume.Text = $"Master Volume {(int)(ScreenManager.audioManager.MasterVolume*100f)}%";
            musicVolume.Text = $"Music Volume {(int)(ScreenManager.audioManager.MusicVolume * 100f)}%";
            sfxVolume.Text = $"SFX Volume {(int)(ScreenManager.audioManager.SFXVolume * 100f)}%";

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            
        }
    }
}
