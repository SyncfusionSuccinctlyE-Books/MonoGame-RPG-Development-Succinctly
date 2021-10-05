using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGameRPG.Animation;
using System.Runtime.CompilerServices;

namespace MonoGameRPG
{
    public class Sprite
    {
        public Vector2 Position { get; set; }
        public Point CellSize { get; set; }
        public Point Size { get; set; }
        public Texture2D spriteTexture { get; set; }
        protected SpriteSheetAnimationPlayer _animationPlayer;
        public SpriteSheetAnimationPlayer animationPlayer
        {
            get { return _animationPlayer; }
            set
            {
                if (_animationPlayer != value && _animationPlayer != null)
                    _animationPlayer.OnAnimationStopped -= OnAnimationStopped;

                _animationPlayer = value;
                _animationPlayer.OnAnimationStopped += OnAnimationStopped;
            }
        }

        public Color Tint { get; set; }

        public Rectangle sourceRect
        {
            get
            {
                if (animationPlayer != null)
                    return new Rectangle((int)animationPlayer.CurrentCell.X, (int)animationPlayer.CurrentCell.Y, CellSize.X, CellSize.Y);
                else
                {
                    if (CellSize == Point.Zero)
                        CellSize = new Point(spriteTexture.Width, spriteTexture.Height);

                    return new Rectangle(0,0, CellSize.X, CellSize.Y);
                }
            }
        }

        public Sprite(Texture2D spriteSheetAsset, Point size, Point cellSize)
        {
            spriteTexture = spriteSheetAsset;
            Tint = Color.White;
            Size = size;
            CellSize = cellSize;
        }

        protected virtual void OnAnimationStopped(SpriteSheetAnimationClip clip) 
        { 
            
        }

        public virtual void StartAnimation(string animation)
        {
            if (animationPlayer != null)
                animationPlayer.StartClip(animation);
        }

        public virtual void StopAnimation()
        {
            if (animationPlayer != null)
                animationPlayer.StopClip();
        }

        public virtual void Update(GameTime gameTime)
        {
            if(animationPlayer != null)
                animationPlayer.Update(gameTime.ElapsedGameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteTexture, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), sourceRect, Tint);
        }
    }
}
