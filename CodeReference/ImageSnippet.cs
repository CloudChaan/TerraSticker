using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Diagnostics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI.Chat;
using Color = Microsoft.Xna.Framework.Color;

namespace TerraSticker.CodeReference;

public class ImageSnippet : TextSnippet, IImageSnippet
{
    public readonly Texture2D Texture;
    private readonly string _imagePath;

    public ImageSnippet(Stream imageStream, string imagePath)
    {
        Texture = Texture2D.FromStream(Main.graphics.GraphicsDevice, imageStream);
        _imagePath = imagePath;
        Scale = 1f;

        float availableWidth = 270;
        int width = Texture.Width;
        int height = Texture.Height;
        if (width > availableWidth || height > availableWidth)
        {
            if (width > height)
            {
                Scale = availableWidth / width;
            }
            else
            {
                Scale = availableWidth / height;
            }
        }
    }
    public ImageSnippet(Texture2D texture2D, string imagePath)
    {
        Texture = texture2D;
        _imagePath = imagePath;
        Scale = 1f;

        float availableWidth = 270;
        int width = Texture.Width;
        int height = Texture.Height;
        if (width > availableWidth || height > availableWidth)
        {
            if (width > height)
            {
                Scale = availableWidth / width;
            }
            else
            {
                Scale = availableWidth / height;
            }
        }
    }

    public override void OnHover()
    {
        if (!Main.drawingPlayerChat) return;

        //string open = Language.GetTextValue("Mods.ImageChat.OpenImage");
        //Main.instance.MouseText($"{open}");
        //Main.LocalPlayer.mouseInterface = true;
    }

    public override void OnClick()
    {
        if (!Main.drawingPlayerChat) return;
        //打开添加表情包面板
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch,
        Vector2 position = default, Color color = default, float scale = 1f)
    {
        if (!justCheckingString && color != Color.Black)
        {
            float opacity = TerraSticker.ClientConfig.Opacity;

            position += new Vector2(2f);
            spriteBatch.Draw(Texture, position, null, Color.White*opacity, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        size = Texture.Size() * scale + new Vector2(6f); // 这里拿来作间隔的，GetStringLength不知道拿来干啥的反正绘制没用
        return true;
    }

    public override float GetStringLength(DynamicSpriteFont font) => Texture.Width * Scale + 6f;

    public int GetChatYOffset()
            => (int)(Texture.Height * Scale);
}