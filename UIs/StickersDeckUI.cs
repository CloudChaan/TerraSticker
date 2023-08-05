using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StickersTest.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace StickersTest.UIs
{
    public class StickersDeckUI : UIState
    {
        public UIStickerManagerButton selectedSticker;
        public UIPanel deckPanel;
        public UIPanel stickersPanel;

        public StickersDeckUI(UIStickerManagerButton sticker) { 
            selectedSticker = sticker;
        }

        public override void OnInitialize()
        {
            float screenWidth = Main.screenWidth;
            float panelWidth = screenWidth - 290f;

            deckPanel = new UIPanel();
            deckPanel.Width.Set(0, 1f);
            deckPanel.Height.Set(0, 0.6f);
            deckPanel.VAlign = 0.5f;
            Append(deckPanel);

            selectedSticker.Left.Set(30, 0f);
            selectedSticker.Top.Set(30, 0f);
            deckPanel.Append(selectedSticker);

            stickersPanel = new UIPanel();
            stickersPanel.Width.Set(-290, 1f);
            stickersPanel.Height.Set(-60, 1f);
            stickersPanel.HAlign = 1f;
            stickersPanel.Left.Set(-30, 0f);
            deckPanel.Append(stickersPanel);

            var deckList = StickerModSaveConfig.LoadConfig().stickers;
            var emptyStickerImage = ModContent.Request<Texture2D>("StickersTest/Assets/Images/non_content_sticker", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            var buttons = new Dictionary<int, UIStickerManagerButton>();
            foreach ( var deck in deckList )
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
            stickersPanel.DrawButtonsOnUIElement(buttons, 0);
            foreach (var element in stickersPanel.Children)
            {
                var button = element as UIStickerManagerButton;
                if (button != null)
                {
                    button.OnLeftClick += (evt, listeningElement) => StickerButton_Click(evt, listeningElement, button);
                }
            }
        }

        /// <summary>
        /// 在这个面板中点击按钮，会将这个按钮的原图路径写入config中对应的位置
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="listeningElement"></param>
        /// <param name="button"></param>
        private void StickerButton_Click(UIMouseEvent evt, UIElement listeningElement, UIStickerManagerButton button)
        {
            var config = StickerModSaveConfig.LoadConfig();
            config.stickers[button.index] = ModContent.GetInstance<StickersTest>().myPathUtils.GetOriginalImagePathFromCachePath(selectedSticker.cachePath);
            button.SetImage(selectedSticker.GetImage());
            var ui = ModContent.GetInstance<StickerManagerUISystem>();
            ui.RefreshSenderUI();
            config.SaveConfig();
        }

        public override void Update(GameTime gameTime)
        {
            // 在这里可以更新你的 UI 元素
            base.Update(gameTime);
        }



    }
}
