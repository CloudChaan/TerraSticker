using Microsoft.Xna.Framework.Input;
using StickersTest.UIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;
using Terraria;
using Terraria.ModLoader;
using StickersTest.Items;
using Terraria.UI;
using StickersTest.ModSystems;
using StickersTest.Utils;

namespace StickersTest.Players
{
    public class StickerPlayer: ModPlayer
    {

        private StickerManagerUISystem uISystem;
        public PathUtils myPath;
        internal StickerManagerUI stickerManagerUI;
        internal StickerSenderUI stickerSenderUI;


        public override void Load()
        {
            uISystem = ModContent.GetInstance<StickerManagerUISystem>();
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            uISystem = ModContent.GetInstance<StickerManagerUISystem>();
            if (KeybindSystem.stickerKeybind.JustPressed ) // 当快捷键按下时
            {
                uISystem.isSendUIEnable = true; // 打开UI
            }
            else if (!KeybindSystem.stickerKeybind.Current) // 当快捷键释放时
            {
                uISystem.isSendUIEnable = false; // 关闭UI
            }
        }
        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            // 创建 mod 物品
            Item item = new Item();
            item.SetDefaults(ModContent.ItemType<StickerBubble>()); // 你的 mod 物品的类名
            item.stack = 1; // 设置物品的数量

            // 返回包含你的物品的列表
            return new[] { item };
        }



        public override bool CanUseItem(Item item)
        {
            if (!uISystem.IsCurrentStateNull())
            {
                return false; // 禁止玩家使用物品
            }
            return base.CanUseItem(item);
        }


    }
}
