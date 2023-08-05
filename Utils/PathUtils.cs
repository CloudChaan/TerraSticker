using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace StickersTest.Utils
{
    public class PathUtils
    {
        public string imageSavePath;
        public string cachePath;
        public string netPackageCachePath;
        public string relativeImageSavePath = "";

        public PathUtils() {
            Initialize(ModContent.GetInstance<StickersTest>());
        }
        public void Initialize(StickersTest Instance)
        {
            imageSavePath = GetModSavePath(Instance);
            cachePath = GetModCachePath(Instance);
            netPackageCachePath = GetModNetPackageCachePath(Instance);
        }

        public static string GetModSavePath(StickersTest Instance)
        {
            if (Instance != null)
            {
                string path = Path.Combine(Main.SavePath, "ModLoader", "Mods", Instance.Name, "Stickers");
                path = path.Replace('/', '\\');
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
            else
            {
                return null;
            }
        }
        public static string GetModCachePath(StickersTest Instance)
        {
            if (Instance != null)
            {
                string path = Path.Combine(Main.SavePath, "ModLoader", "Mods", Instance.Name, "StickersCache");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
            else
            {
                return null;
            }
        }
        public static string GetModNetPackageCachePath(StickersTest Instance)
        {
            if (Instance != null)
            {
                string path = Path.Combine(Main.SavePath, "ModLoader", "Mods", Instance.Name, "StickersNetPackageCache");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
            else
            {
                return null;
            }
        }

        public string GetCachePathFromOriginalImagePath(string stickerPath)
        {
            if(!File.Exists(stickerPath))
                return null;
            string filename;
            var extension = Path.GetExtension(stickerPath).ToLower();
            if (extension == ".gif")
            {
                filename = Path.GetFileNameWithoutExtension(stickerPath) + "_FirstFrame_Cache.png";
                return Path.Combine(cachePath, filename);
            }
            else
            {
                filename = Path.GetFileNameWithoutExtension(stickerPath) + "_cache.png";
                return Path.Combine(cachePath, filename);
            }
        }

        public string GetOriginalImagePathFromCachePath(string cachePath)
        {
            string filename;
            var filenameWithExtension = Path.GetFileName(cachePath);

            if (filenameWithExtension.EndsWith("_FirstFrame_Cache.png", StringComparison.OrdinalIgnoreCase))
            {
                filename = filenameWithExtension.Replace("_FirstFrame_Cache.png", "", StringComparison.OrdinalIgnoreCase);
                return Path.Combine(imageSavePath, filename + ".gif");
            }
            else if (filenameWithExtension.EndsWith("_cache.png", StringComparison.OrdinalIgnoreCase))
            {
                filename = filenameWithExtension.Replace("_cache.png", "", StringComparison.OrdinalIgnoreCase);
                return Path.Combine(imageSavePath, filename + ".png");
            }
            else
            {
                throw new ArgumentException("Invalid cache path");
            }
        }


        public static bool TryCreatingDirectory(string folderName)
        {
            try
            {
                // 如果目录不存在，则创建目录
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }
                return true;
            }
            catch
            {
                // 如果出现任何异常，返回false
                return false;
            }
        }

    }
}
