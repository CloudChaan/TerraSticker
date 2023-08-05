using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StickersTest.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace StickersTest.UIs
{
    public class StickerSenderUI : UIState
    {
        public UIPanel senderPanel;


        public override void OnInitialize()
        {
            senderPanel = new UIPanel();
            senderPanel.HAlign = 0f;
            senderPanel.VAlign = 0.5f;
            senderPanel.Width.Set(0f, 0.8f);
            senderPanel.Height.Set(0f, 0.5f);
            Append(senderPanel);

            var deckList = StickerModSaveConfig.LoadConfig().stickers;
            var emptyStickerImage = ModContent.Request<Texture2D>("StickersTest/Assets/Images/non_content_sticker", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            var buttons = new Dictionary<int, UIStickerManagerButton>();
            foreach (var deck in deckList)
            {
                if (deck.Value != null)
                {
                    var sticker = new UIStickerManagerButton(ModContent.GetInstance<StickersTest>().myPathUtils.GetCachePathFromOriginalImagePath(deck.Value));
                    buttons.Add(deck.Key, sticker);
                    sticker.index = deck.Key;

                }
                else
                {
                    var sticker = new UIStickerManagerButton(emptyStickerImage);
                    buttons.Add(deck.Key, sticker);
                    sticker.index = deck.Key;
                }
            }

            senderPanel.DrawButtonsOnUIElement(buttons,0);

            foreach (var element in senderPanel.Children)
            {
                var button = element as UIStickerManagerButton;
                if (button != null)
                {
                    button.OnLeftClick += (evt, listeningElement) => StickerButton_Click(evt, listeningElement, button);
                }
            }

        }


        private void StickerButton_Click(UIMouseEvent evt, UIElement listeningElement, UIStickerManagerButton button)
        {
            //发送图片
            //首先判断图片是PNG还是GIF
            var key = button.index;
            //从StickerConfig获取文件路径
            var imagePath = StickerModSaveConfig.LoadConfig().stickers[key];
            if (!File.Exists(imagePath)) return;
            var extension = Path.GetExtension(imagePath).ToLower();
            switch (extension)
            {
                case ".png":
                    var memory = ImageUtils.GetMemoryFromImage(imagePath);
                    ImageUtils.LocalSendImage(memory, imagePath);
                    break;
                case ".gif":
                    var gifMemory = ImageUtils.GetMemoryFromImage(imagePath);
                    var gif = Image.Load<Rgba32>(imagePath);
                    ImageUtils.SendGif(gifMemory, gif, Main.LocalPlayer.name, imagePath);
                    break;
            }

        }

        public override void Update(GameTime gameTime)
        {
            // 在这里可以更新你的 UI 元素
            base.Update(gameTime);
        }

    }
}
