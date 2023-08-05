using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using StickersTest.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace StickersTest.UIs
{
    [Autoload(Side = ModSide.Client)]
    public class StickerManagerUISystem : ModSystem
    {
        private UserInterface stickerUserInterface;
        internal StickerManagerUI stickerManagerUI;
        public Dictionary<string, UIStickerManagerButton> stickerList;
        internal StickerSenderUI stickerSenderUI;
        public bool isSendUIEnable = false;

        public override void Load()
        {
            stickerList = new Dictionary<string, UIStickerManagerButton>();
            stickerUserInterface = new UserInterface();
            stickerSenderUI = new StickerSenderUI();
        }


        public void RefreshSenderUI()
        {
            stickerSenderUI = new StickerSenderUI();
        }


        public void CloseAllUI()
        {
            stickerUserInterface.SetState(null);
        }
        public void RefreshManagerUI()
        {
            stickerManagerUI = new StickerManagerUI();
            stickerUserInterface.SetState(stickerManagerUI);
        }

        public bool IsCurrentStateNull()
        {
            return stickerUserInterface.CurrentState == null;
        }

        public bool IsCurrentStateManager()
        {
            return stickerUserInterface.CurrentState == stickerManagerUI;
        }

        public bool IsOtherUIOnState(UIState ui)
        {
            return stickerUserInterface.CurrentState != ui && stickerUserInterface.CurrentState != null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (Main.keyState.IsKeyDown(Keys.Escape) && stickerUserInterface.CurrentState != null)
            {
                stickerUserInterface.SetState(null); // 关闭UI

            }
            if (isSendUIEnable && (stickerUserInterface.CurrentState == null || stickerUserInterface.CurrentState == stickerSenderUI))
            {
                stickerUserInterface.SetState(stickerSenderUI);
            }
            if (!isSendUIEnable && stickerUserInterface.CurrentState == stickerSenderUI)
            {
                stickerUserInterface.SetState(null);
            }
            if (stickerUserInterface.CurrentState != null)
            {
                stickerUserInterface.Update(gameTime);
            }
        }


        public static Dictionary<string, UIStickerManagerButton> GetStickersList()
        {
            StickerManagerUISystem stickerManagerUISystem = ModContent.GetInstance<StickerManagerUISystem>();
            stickerManagerUISystem.stickerList.Clear();
            var mod = ModContent.GetInstance<StickersTest>();
            DirectoryInfo folderInfo = new DirectoryInfo(mod.myPathUtils.imageSavePath);
            foreach (FileInfo fi in folderInfo.GetFiles())
            {
                string filename;
                string cacheImagePath;
                var extension = Path.GetExtension(fi.FullName).ToLower();
                if (extension != ".gif" && extension != ".png" && extension != ".jpeg" && extension != ".jpg" && extension != ".webp")
                    continue;

                if (extension == ".gif")
                {
                    filename = Path.GetFileNameWithoutExtension(fi.FullName) + "_FirstFrame_Cache.png";
                    cacheImagePath = Path.Combine(mod.myPathUtils.cachePath, filename);
                    if (File.Exists(cacheImagePath))
                    {
                        continue;
                    }
                }
                else
                {
                    var filenameWithoutExtension = Path.GetFileNameWithoutExtension(fi.FullName);
                    filename = filenameWithoutExtension + "_cache.png";
                    cacheImagePath = Path.Combine(mod.myPathUtils.cachePath, filename);
                }
                // Check if cache file already exists
                if (!File.Exists(cacheImagePath))
                {
                    var newPath = ImageUtils.ConvertToPng(fi.FullName);
                    if (newPath != null)
                        cacheImagePath = ImageUtils.CropAndSaveImage(newPath, 4f / 5f);
                }

                var texture = ImageUtils.LoadAssetFromCacheImage(cacheImagePath);
                var newImage = new UIStickerManagerButton(texture, fi.FullName);
                newImage.cachePath = cacheImagePath;
                stickerManagerUISystem.stickerList.Add(Path.GetFileName(fi.FullName), newImage);
            }
            return stickerManagerUISystem.stickerList;
        }

        public void Deck(UIStickerManagerButton sticker)
        {
            stickerUserInterface.SetState(null);
            var deckUI = new StickersDeckUI(sticker);
            stickerUserInterface.SetState(deckUI);
        }




        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "StickersManager: StickerPanel",
                    delegate
                    {
                        stickerUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;
            stickerUserInterface = null;
        }
    }
}
