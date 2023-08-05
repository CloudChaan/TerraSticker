using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace StickersTest.ModConfigs
{
    public class ServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public override void OnLoaded()
        {
            StickersTest.ServerConfig = this;
        }

        [LabelKey("$Mods.StickersTest.Configs.StickersTestServerConfig.MaximumFileSizeLabel")]
        [TooltipKey("$Mods.StickersTest.Configs.StickersTestServerConfig.MaximumFileSizeTooltip")]
        [DefaultValue(1f)]
        [Range(0.5f, 10f)]
        [Increment(0.1f)]
        public float MaximumFileSize;

        [LabelKey("$Mods.StickersTest.Configs.StickersTestServerConfig.SendCapLabel")]
        [TooltipKey("$Mods.StickersTest.Configs.StickersTestServerConfig.SendCapTooltip")]
        [DefaultValue(1)]
        [Range(1, 60)]
        public int SendCap;

        [Header("Performance")]
        [LabelKey("$Mods.StickersTest.Configs.StickersTestServerConfig.GifLifetimeLabel")]
        [TooltipKey("$Mods.StickersTest.Configs.StickersTestServerConfig.GifLifetimeTooltip")]
        [DefaultValue(15)]
        [Range(15, 120)]
        public int GifLifetime;

        [LabelKey("$Mods.StickersTest.Configs.StickersTestServerConfig.GifSendDelayLabel")]
        [TooltipKey("$Mods.StickersTest.Configs.StickersTestServerConfig.GifSendDelayTooltip")]
        [DefaultValue(3)]
        [Range(3, 15)]
        public int GifSendDelay;
    }
}
