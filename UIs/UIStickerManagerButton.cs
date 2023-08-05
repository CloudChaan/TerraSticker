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
    public class UIStickerManagerButton : UITexture2DButton
    {
        public int borderWidth = 20;
        public int borderHeight = 25;
        private float _visibilityActive = 1f;
        private float _visibilityInactive = 0.7f;
        public Texture2D borderBlue = ModContent.Request<Texture2D>("StickersTest/Assets/Images/border_blue", AssetRequestMode.ImmediateLoad).Value;
        public Texture2D borderRed = ModContent.Request<Texture2D>("StickersTest/Assets/Images/border_red", AssetRequestMode.ImmediateLoad).Value;
        public string cachePath;
        public int index;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="path">此处应当填入暂存封面的路径</param>
        public UIStickerManagerButton(Texture2D texture, string path) : base(texture, path)
        {
            imagePath = path;
            texture2d = texture;
            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);
        }
        /// <summary>
        /// 允许直接使用cache路径创建button
        /// 若cache文件无法找到则返回空白贴图button
        /// </summary>
        /// <param name="path">cache路径</param>
        public UIStickerManagerButton(string path) : base(path)
        {
            Texture2D texture; 
            if (File.Exists(path))
            {
                texture = ImageUtils.LoadAssetFromCacheImage(path);
                imagePath = path;
            }
            else
            {
                texture = ModContent.Request<Texture2D>("StickersTest/Assets/Images/non_content_sticker", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                imagePath = null;
            }
            texture2d = texture;
            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);
        }
        public UIStickerManagerButton(Texture2D texture) : base(texture)
        {
            texture2d = texture;
            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);
        }


        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            CalculatedStyle innerDimensions = new CalculatedStyle(dimensions.X + borderWidth / 2, dimensions.Y + borderHeight / 2, dimensions.Width - borderWidth, dimensions.Height - borderHeight);
            spriteBatch.Draw(texture2d, innerDimensions.Position(), Color.White * (IsMouseHovering ? _visibilityActive : _visibilityInactive));
            spriteBatch.Draw(borderBlue, dimensions.Position(), Color.White );

            if (IsMouseHovering)
            {
                spriteBatch.Draw(borderRed, dimensions.Position(), Color.White);
            }
        }

        public void SetImage(Texture2D texture)
        {
            texture2d = texture;
            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);
        }

        public Texture2D GetImage()
        {
            return texture2d;
        }

    }
}
