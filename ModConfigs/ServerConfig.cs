using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace TerraSticker.ModConfigs
{
    public class ServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public override void OnLoaded()
        {
            TerraSticker.ServerConfig = this;
        }

        [LabelKey("$Mods.TerraSticker.Configs.TerraStickerServerConfig.MaximumFileSizeLabel")]
        [TooltipKey("$Mods.TerraSticker.Configs.TerraStickerServerConfig.MaximumFileSizeTooltip")]
        [DefaultValue(1f)]
        [Range(0.5f, 10f)]
        [Increment(0.1f)]
        public float MaximumFileSize;

        [LabelKey("$Mods.TerraSticker.Configs.TerraStickerServerConfig.SendCapLabel")]
        [TooltipKey("$Mods.TerraSticker.Configs.TerraStickerServerConfig.SendCapTooltip")]
        [DefaultValue(1)]
        [Range(1, 60)]
        public int SendCap;

        [Header("Performance")]
        [LabelKey("$Mods.TerraSticker.Configs.TerraStickerServerConfig.GifLifetimeLabel")]
        [TooltipKey("$Mods.TerraSticker.Configs.TerraStickerServerConfig.GifLifetimeTooltip")]
        [DefaultValue(15)]
        [Range(15, 120)]
        public int GifLifetime;

        [LabelKey("$Mods.TerraSticker.Configs.TerraStickerServerConfig.GifSendDelayLabel")]
        [TooltipKey("$Mods.TerraSticker.Configs.TerraStickerServerConfig.GifSendDelayTooltip")]
        [DefaultValue(3)]
        [Range(3, 15)]
        public int GifSendDelay;
    }
}
