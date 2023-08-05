using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using TerraSticker.Items;
using TerraSticker.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerraSticker.UIs
{
    public class StickerManagerUI:UIState
    {
        public UIPanel stickersManagerPanel;
        public UIPanel buttonsPanel;
        //public UITextBox searchBox;
        public UIImageButton uploadImagesButton;
        public UIImageButton refreshFolderButton;
        public UIImageButton lastPageButton;
        public UIImageButton nextPageButton;
        public UIImageButton stickerManagerUICloseButton;
        private int totalPages;
        private int pages = 0;
        private Dictionary<int, UIStickerManagerButton> buttons;
        private UIText description;
        public override void OnInitialize()
        {
            float screenWidth = Main.screenWidth;
            float screenHeight = Main.screenHeight;
            float panelWidth = screenWidth * 0.6f;
            float panelHeight = screenHeight * 0.6f;

            stickersManagerPanel = new UIPanel();
            stickersManagerPanel.HAlign = 0;
            stickersManagerPanel.VAlign = 0.5f;
            stickersManagerPanel.Width.Set(panelWidth, 0f);
            stickersManagerPanel.Height.Set(panelHeight, 0f);

            buttonsPanel = new UIPanel();
            buttonsPanel.Width.Set(0, 1f);
            buttonsPanel.Height.Set(-50, 1f);
            buttonsPanel.VAlign = 1;
            stickersManagerPanel.Append(buttonsPanel);

            lastPageButton = new UIImageButton(ModContent.Request<Texture2D>("TerraSticker/Assets/Images/leftArrow", AssetRequestMode.ImmediateLoad));
            lastPageButton.SetHoverImage(ModContent.Request<Texture2D>("TerraSticker/Assets/Images/leftArrow_border", AssetRequestMode.ImmediateLoad));
            lastPageButton.HAlign = 0;
            lastPageButton.VAlign = 1;
            lastPageButton.Left.Set(10f, 0);
            lastPageButton.Top.Set(-10f, 0);
            lastPageButton.OnLeftClick += (evt, listeningElement) => ToLastPage(evt, listeningElement);
            stickersManagerPanel.Append(lastPageButton);

            nextPageButton = new UIImageButton(ModContent.Request<Texture2D>("TerraSticker/Assets/Images/rightArrow", AssetRequestMode.ImmediateLoad));
            nextPageButton.SetHoverImage(ModContent.Request<Texture2D>("TerraSticker/Assets/Images/rightArrow_border", AssetRequestMode.ImmediateLoad));
            nextPageButton.HAlign = 1;
            nextPageButton.VAlign = 1;
            nextPageButton.Left.Set(-10f, 0);
            nextPageButton.Top.Set(-10f, 0);
            nextPageButton.OnLeftClick += (evt, listeningElement) => ToNextPage(evt, listeningElement);
            stickersManagerPanel.Append(nextPageButton);


            stickerManagerUICloseButton = new UIImageButton(ModContent.Request<Texture2D>("TerraSticker/Assets/Images/StickerManagerUICloseButton", AssetRequestMode.ImmediateLoad));
            stickerManagerUICloseButton.SetHoverImage(ModContent.Request<Texture2D>("TerraSticker/Assets/Images/StickerManagerUICloseButton_borderTexture", AssetRequestMode.ImmediateLoad));
            stickerManagerUICloseButton.HAlign= 1;
            stickerManagerUICloseButton.Left.Set(-10, 0f);
            stickerManagerUICloseButton.Top.Set(10f, 0f);
            stickerManagerUICloseButton.OnLeftClick += new MouseEvent(StickerManagerUICloseButton_Click);
            stickersManagerPanel.Append(stickerManagerUICloseButton);

            uploadImagesButton = new UIImageButton(ModContent.Request<Texture2D>("TerraSticker/Assets/Images/UploadImagesButton", AssetRequestMode.ImmediateLoad));
            uploadImagesButton.SetHoverImage(ModContent.Request<Texture2D>("TerraSticker/Assets/Images/UploadImagesButton_borderTexture", AssetRequestMode.ImmediateLoad));
            uploadImagesButton.HAlign = 1;
            uploadImagesButton.Left.Set(-stickerManagerUICloseButton.Width.Pixels - 20, 0f);
            uploadImagesButton.Top.Set(10f, 0f);
            uploadImagesButton.OnLeftClick += new MouseEvent(UploadImagesButton_Click);
            stickersManagerPanel.Append(uploadImagesButton);

            refreshFolderButton = new UIImageButton(ModContent.Request<Texture2D>("TerraSticker/Assets/Images/refresh", AssetRequestMode.ImmediateLoad));
            refreshFolderButton.SetHoverImage(ModContent.Request<Texture2D>("TerraSticker/Assets/Images/refresh_border", AssetRequestMode.ImmediateLoad));
            refreshFolderButton.HAlign= 1;
            refreshFolderButton.Left.Set(-uploadImagesButton.Width.Pixels - stickerManagerUICloseButton.Width.Pixels - 30, 0);
            refreshFolderButton.Top.Set(10f, 0);
            refreshFolderButton.OnLeftClick += new MouseEvent(RefreshManager);
            stickersManagerPanel.Append(refreshFolderButton);

            //searchBox = new UITextBox("输入图片名称以搜索");
            //searchBox.Left.Set(10f, 0f);
            //searchBox.Top.Set(10f, 0f);
            //searchBox.Width.Set(-uploadImagesButton.Width.Pixels - stickerManagerUICloseButton.Width.Pixels - 40, 1f);
            //searchBox.Height.Set(30f, 0f);
            //searchBox.BackgroundColor = new Color(73, 94, 171);
            //stickersManagerPanel.Append(searchBox);

            var dic = StickerManagerUISystem.GetStickersList();
            buttons = new Dictionary<int, UIStickerManagerButton>();
            int index = 0;
            foreach (var kvp in dic)
            {
                buttons.Add(index, kvp.Value);
                index++;
            }
            totalPages = index / 10;
            buttonsPanel.DrawButtonsOnUIElement(buttons, pages);
            foreach( var element in buttons.Values)
            {
                //注册点击事件
                if (element != null)
                {
                    element.OnLeftClick += (evt, listeningElement) => StickerButton_Click(evt, listeningElement, element);
                }
            }

            if(buttons.Count <= 0)
            {
                description = new UIText(Language.GetTextValue("Mods.TerraSticker.description",$"[i:{ItemID.Bunny}]" , $"[i:{ModContent.ItemType<StickerBubble>()}]").Replace("\\n", Environment.NewLine), 1.5f);
                description.VAlign = 0.5f;
                description.Left.Set(20f, 0);
                stickersManagerPanel.Append(description);
            }

            Append(stickersManagerPanel);
        }
        public override void Update(GameTime gameTime)
        {
            // 在这里可以更新你的 UI 元素
            base.Update(gameTime);
        }


        private void RefreshManager(UIMouseEvent evt, UIElement listeningElement)
        {
            ModContent.GetInstance<StickerManagerUISystem>().RefreshManagerUI();
        }



        private void StickerManagerUICloseButton_Click(UIMouseEvent evt, UIElement listeningElement)
        {
            // 在这里定义stickerManagerUICloseButton被点击时的行为
            StickerManagerUISystem Stickers = ModContent.GetInstance<StickerManagerUISystem>();
            if (!Stickers.IsCurrentStateNull())
            {
                Stickers.CloseAllUI();
            }
        }

        private void ToLastPage(UIMouseEvent evt, UIElement listeningElement)
        {
            if (pages <= 0) return;
            pages -= 1;
            buttonsPanel.RemoveAllChildren();
            buttonsPanel.DrawButtonsOnUIElement(buttons, pages);
        }
        private void ToNextPage(UIMouseEvent evt, UIElement listeningElement)
        {
            if (pages >= totalPages) return;
            pages += 1;
            buttonsPanel.RemoveAllChildren();
            buttonsPanel.DrawButtonsOnUIElement(buttons, pages);
        }

        private void StickerButton_Click(UIMouseEvent evt, UIElement listeningElement, UIStickerManagerButton button)
        {
            var uiSys = ModContent.GetInstance<StickerManagerUISystem>();
            uiSys.Deck(button);
        }


        private void UploadImagesButton_Click(UIMouseEvent evt, UIElement listeningElement)
        {
            string folderPath = ModContent.GetInstance<TerraSticker>().myPathUtils.imageSavePath;
            System.Diagnostics.Process.Start("explorer.exe", folderPath);
        }
    }
}
