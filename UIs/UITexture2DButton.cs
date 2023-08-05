using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using StickersTest.Utils;
using System.IO;

namespace StickersTest.UIs
{
    public class UITexture2DButton : UIImageButton
    {
        protected Texture2D texture2d;

        protected string imagePath { get; set; }

        private float _visibilityActive = 1f;

        private float _visibilityInactive = 0.6f;

        protected Texture2D borderTexture;

        public UITexture2DButton(Texture2D texture, string path) : base(ModContent.Request<Texture2D>("StickersTest/Assets/Images/StickerBubble1x"))
        {
            imagePath = path;
            texture2d = texture;
            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);
        }
        public UITexture2DButton(string path) : base(ModContent.Request<Texture2D>("StickersTest/Assets/Images/StickerBubble1x"))
        {
        }
        public UITexture2DButton(Texture2D texture) : base(ModContent.Request<Texture2D>("StickersTest/Assets/Images/StickerBubble1x"))
        {
            texture2d = texture;
            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);
        }


        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            spriteBatch.Draw(texture2d, dimensions.Position(), Color.White * (IsMouseHovering ? _visibilityActive : _visibilityInactive));
            if (borderTexture != null && IsMouseHovering)
            {
                spriteBatch.Draw(borderTexture, dimensions.Position(), Color.White);
            }
        }

        public void SetHoverTexture(Texture2D texture)
        {
            borderTexture = texture;
        }

        public void SetTexture(Texture2D texture)
        {
            texture2d = texture;
            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);
        }

    }
}
