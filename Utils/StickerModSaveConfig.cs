using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader.Config;

namespace StickersTest.Utils
{
    public class StickerModSaveConfig
    {
        //储存贴纸发送UI中的贴纸的字典，其中int为索引，string为原图路径
        public Dictionary<int, string> stickers;

        public static StickerModSaveConfig LoadConfig()
        {
            var configPath = Path.Combine(Main.SavePath, "ModConfigs", "StickersConfig.json");
            if (File.Exists(configPath))
            {
                var configJson = File.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<StickerModSaveConfig>(configJson);
            }
            else
            {
                // 如果配置文件不存在，返回一个默认配置
                return new StickerModSaveConfig {
                    stickers = new Dictionary<int, string>()
                    {
                        {0, null },
                        {1, null },
                        {2, null },
                        {3, null },
                        {4, null },
                        {5, null },
                        {6, null },
                        {7, null },
                        {8, null },
                        {9, null },
                    }
                };
            }
        }

        public void SaveConfig()
        {
            var configPath = Path.Combine(Main.SavePath, "ModConfigs");
            Directory.CreateDirectory(configPath);  // 确保目录存在
            var configJson = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(Path.Combine(configPath, "StickersConfig.json"), configJson);
        }
    }
}
