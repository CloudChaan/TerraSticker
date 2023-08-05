using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;

namespace TerraSticker.CodeReference
{
    public interface IImageSnippet
    {
        int GetChatYOffset();
        float GetStringLength(DynamicSpriteFont font);
        bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1);
    }
}