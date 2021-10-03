using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGameRPG.StateManagement;

namespace MonoGameRPG
{
    public interface IInventoryContainer
    {
        /// <summary>
        /// Maximum number of slots. NOTE this is a nullable int, a null means that there is no limit
        /// </summary>
        int? MaxVolume { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        void AddItem(IInventoryItem item);
        void RemoveItem(IInventoryItem item);
        void HandleInput(GameTime gameTime, PlayerIndex? playerIndex, InputState input);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
