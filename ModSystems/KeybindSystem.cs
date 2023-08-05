using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace StickersTest.ModSystems
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind stickerKeybind { get; private set; }


        public override void Load()
        {
            stickerKeybind = KeybindLoader.RegisterKeybind(Mod, "LongPressToOpenStickerSendPanel", Keys.T);
        }

        public override void Unload()
        {
            stickerKeybind = null;
        }
    }
}
