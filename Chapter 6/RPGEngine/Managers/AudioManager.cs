using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using RPGEngine.Interfaces.Audio;

namespace RPGEngine.Managers
{
    /// <summary>
    /// This class is used to manage audio. 
    /// It is derived from BaseGameComponent
    /// </summary>
    public class AudioManager : GameComponent, IAudioManager
    {
        /// <summary>
        /// Current song loaded.
        /// </summary>
        public Song CurrentSong { get; set; }

        /// <summary>
        /// Toggle looping of song
        /// </summary>
        public bool loopCurrenSong { get; set; }

        /// <summary>
        /// Current sound effect loaded.
        /// </summary>
        public SoundEffectInstance CurrentSFXInstance { get; set; }

        /// <summary>
        /// Property to flag if Music is currently playing.
        /// </summary>
        public bool IsMusicPlaying { get { return MediaPlayer.State == MediaState.Playing; } }

        /// <summary>
        /// Property to flag if Music is currently stopped.
        /// </summary>
        public bool IsMusicStopped { get { return MediaPlayer.State == MediaState.Stopped; } }

        /// <summary>
        /// Property to flag if Music is currently paused.
        /// </summary>
        public bool IsMusicPaused { get { return MediaPlayer.State == MediaState.Paused; } }

        string _CurrentSongAsset;
        public string CurrentSongAsset { get { return _CurrentSongAsset; } }

        /// <summary>
        /// Current Media player state.
        /// </summary>
        public MediaState MediaState { get { return MediaPlayer.State; } }

        protected float _MasterVolume = 1;
        public float MasterVolume
        {
            get { return _MasterVolume; }
            set
            {
                float v = MathHelper.Max(0, MathHelper.Min(1, value));
                if (_MasterVolume != v)
                {
                    _MasterVolume = v;

                    // Trigger the SFX and music volumes to change with the master volume..
                    SFXVolume = _SFXVolume;
                    MusicVolume = _MusicVolume;
                }
            }
        }

        protected float _SFXVolume = 1;
        public float SFXVolume
        {
            get { return _SFXVolume; }
            set
            {
                _SFXVolume = MathHelper.Max(0, MathHelper.Min(1, value));
                if (CurrentSFXInstance != null)
                    CurrentSFXInstance.Volume = SFXVolume * MasterVolume;
            }
        }
        protected float _MusicVolume = 1;
        public float MusicVolume
        {
            get { return _MusicVolume; }
            set
            {
                _MusicVolume = MathHelper.Max(0, MathHelper.Min(1, value));
                MediaPlayer.Volume = MusicVolume * MasterVolume;
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        public AudioManager(Game game, float masterVolume = 1, float musicVolume = 1, float sfxVolume = 1) : base(game)
        {
            SFXVolume = sfxVolume;
            MusicVolume = musicVolume;
            MasterVolume = masterVolume;

            game.Services.AddService<IAudioManager>(this);
        }

        /// <summary>
        /// Method to load and play a song based on it's asset name
        /// </summary>
        /// <param name="songAsset">Song asset to play</param>
        /// <param name="volume">Volume 0-1 (default 1)</param>
        /// <param name="loop">Loop song?</param>
        public void PlaySong(string songAsset, float volume = 1, bool loop = true)
        {
            _CurrentSongAsset = songAsset;
            CurrentSong = Game.Content.Load<Song>(songAsset);

            MediaPlayer.Volume = volume * MusicVolume * MasterVolume;
            loopCurrenSong = loop;

            PlaySong(CurrentSong);
        }

        /// <summary>
        /// Method to play a Song
        /// </summary>
        /// <param name="song">Song to play</param>
        public void PlaySong(Song song)
        {
            if (MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Stop();

            MediaPlayer.Play(song);
        }

        /// <summary>
        /// Update call
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (CurrentSong != null && loopCurrenSong && IsMusicStopped)
                PlaySong(CurrentSong);
        }

        /// <summary>
        /// Method to play a sound effect asset
        /// </summary>
        /// <param name="sfxAsset">SFX asset</param>
        /// <param name="volume">Volume 0-1 (default 1)</param>
        /// <param name="emitter">Source of the sound if 3D sound is required</param>
        /// <param name="listener">Listener of the sound, if 3D sound is required</param>
        /// <param name="pitch">Pitch 0-1 (default .5)</param>
        /// <param name="pan">Pan 0-1 (default .5)</param>
        public void PlaySFX(string sfxAsset, float volume = 1, AudioListener listener = null, AudioEmitter emitter = null, float pitch = 0, float pan = 0)
        {
            SoundEffectInstance sfx = Game.Content.Load<SoundEffect>(sfxAsset).CreateInstance();
            sfx.Volume = volume * SFXVolume * MasterVolume;
            sfx.Pitch = pitch;
            sfx.Pan = pan;

            if (listener != null && emitter != null)
                sfx.Apply3D(listener, emitter);

            sfx.Play();
        }

        /// <summary>
        /// Method to play a sound effect
        /// </summary>
        /// <param name="sfxAsset">SFX asset</param>
        /// <param name="volume">Volume 0-1 (default 1)</param>m>
        /// <param name="emitter">Source of the sound if 3D sound is required</param>
        /// <param name="listener">Listener of the sound, if 3D sound is required</param>
        /// <param name="loop">Loop SFX?</param>
        /// <param name="pitch">Pitch 0-1 (default .5)</param>
        /// <param name="pan">Pan 0-1 (default .5)</param>
        public void PlaySound(string sfxAsset, float volume = 1, AudioListener listener = null, AudioEmitter emitter = null, bool loop = false, float pitch = 0, float pan = 0)
        {
            CurrentSFXInstance = Game.Content.Load<SoundEffect>(sfxAsset).CreateInstance();
            CurrentSFXInstance.IsLooped = loop;
            CurrentSFXInstance.Pan = pan;
            CurrentSFXInstance.Pitch = pitch;
            CurrentSFXInstance.Volume = volume * SFXVolume * MasterVolume;

            if (listener != null && emitter != null)
                CurrentSFXInstance.Apply3D(listener, emitter);

            CurrentSFXInstance.Play();
        }

        /// <summary>
        /// Method to stop current SFX
        /// </summary>
        public void StopSound()
        {
            if (CurrentSFXInstance.State != SoundState.Stopped)
                CurrentSFXInstance.Stop(true);
        }

        /// <summary>
        /// Method to stop Media Player
        /// </summary>
        public void StopMusic()
        {
            if (!IsMusicStopped)
            {
                loopCurrenSong = false;
                MediaPlayer.Stop();
            }
        }

        /// <summary>
        /// Method to pause Media Player
        /// </summary>
        /// <param name="audio"></param>
        public void PauseMusic(string audio)
        {
            if (IsMusicPlaying)
                MediaPlayer.Pause();
        }

        /// <summary>
        /// Method to Pause SFX
        /// </summary>
        public void PauseSound()
        {
            if (CurrentSFXInstance.State == SoundState.Playing)
                CurrentSFXInstance.Pause();
        }

        /// <summary>
        /// Method to resume Media Player
        /// </summary>
        /// <param name="audio"></param>
        public void ResumeMusic()
        {
            if (IsMusicPaused)
                MediaPlayer.Resume();
        }

        /// <summary>
        /// Method to resume current SFX
        /// </summary>
        public void ResumeSound()
        {
            if (CurrentSFXInstance.State == SoundState.Paused)
                CurrentSFXInstance.Play();
        }

        /// <summary>
        /// Method to clean up object when disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!IsMusicStopped)
                    MediaPlayer.Stop();
            }
            base.Dispose(disposing);
        }
    }
}
