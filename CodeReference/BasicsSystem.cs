using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace StickersTest.CodeReference
{
    public class BasicsSystem : ModSystem
    {
        internal static float SendDelay;
        internal static ModKeybind ScrnshotKeybind { get; private set; }
        internal static BasicsSystem Instance;

        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            SendDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

    }
}
