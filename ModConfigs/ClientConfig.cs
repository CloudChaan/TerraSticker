using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace StickersTest.ModConfigs
{
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public override void OnLoaded()
        {
            StickersTest.ClientConfig = this;
        }

        [DefaultValue(false)]
        [LabelKey("$Mods.StickersTest.Configs.StickersTestClientConfig.WindowWarningLabel")]
        public bool WindowWarning;

        [DefaultValue(true)]
        [LabelKey("$Mods.StickersTest.Configs.StickersTestClientConfig.AutoClearLabel")]
        public bool AutoClear;

        [Header("$Mods.StickersTest.Configs.StickersTestClientConfig.Gif_Appearance")]
        [LabelKey("$Mods.StickersTest.Configs.StickersTestClientConfig.WidthInChatLabel")]
        [TooltipKey("$Mods.StickersTest.Configs.StickersTestClientConfig.WidthInChatTooltip")]
        [DefaultValue(150)]
        [Range(20, 200)]
        public int WidthInChat;

        [LabelKey("$Mods.StickersTest.Configs.StickersTestClientConfig.OpacityLabel")]
        [TooltipKey("$Mods.StickersTest.Configs.StickersTestClientConfig.OpacityTooltip")]
        [DefaultValue(1f)]
        [Range(0, 1)]
        public float Opacity;

        [LabelKey("$Mods.StickersTest.Configs.StickersTestClientConfig.FramesLimitLabel")]
        [TooltipKey("$Mods.StickersTest.Configs.StickersTestClientConfig.FramesLimitTooltip")]
        [DefaultValue(150)]
        [Range(5, 150)]
        public int FramesLimit;


        [DefaultValue(10)]
        [Range(5,30)]
        public int FramesSpeed;
    }
}
