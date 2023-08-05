using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TerraSticker.ModConfigs
{
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public override void OnLoaded()
        {
            TerraSticker.ClientConfig = this;
        }

        [DefaultValue(false)]
        [LabelKey("$Mods.TerraSticker.Configs.TerraStickerClientConfig.WindowWarningLabel")]
        public bool WindowWarning;

        [DefaultValue(true)]
        [LabelKey("$Mods.TerraSticker.Configs.TerraStickerClientConfig.AutoClearLabel")]
        public bool AutoClear;

        [Header("$Mods.TerraSticker.Configs.TerraStickerClientConfig.Gif_Appearance")]
        [LabelKey("$Mods.TerraSticker.Configs.TerraStickerClientConfig.WidthInChatLabel")]
        [TooltipKey("$Mods.TerraSticker.Configs.TerraStickerClientConfig.WidthInChatTooltip")]
        [DefaultValue(150)]
        [Range(20, 200)]
        public int WidthInChat;

        [LabelKey("$Mods.TerraSticker.Configs.TerraStickerClientConfig.OpacityLabel")]
        [TooltipKey("$Mods.TerraSticker.Configs.TerraStickerClientConfig.OpacityTooltip")]
        [DefaultValue(1f)]
        [Range(0, 1)]
        public float Opacity;

        [LabelKey("$Mods.TerraSticker.Configs.TerraStickerClientConfig.FramesLimitLabel")]
        [TooltipKey("$Mods.TerraSticker.Configs.TerraStickerClientConfig.FramesLimitTooltip")]
        [DefaultValue(150)]
        [Range(5, 150)]
        public int FramesLimit;


        [DefaultValue(10)]
        [Range(5,30)]
        public int FramesSpeed;
    }
}
