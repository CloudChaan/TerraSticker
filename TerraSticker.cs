using Microsoft.Xna.Framework.Input;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using TerraSticker.CodeReference;
using TerraSticker.ModConfigs;
using TerraSticker.UIs;
using TerraSticker.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using System.Threading.Tasks;

namespace TerraSticker
{
    public enum PacketType
    {
        image0,
        image1,
        gif0,
        gif1
    }
    public class TerraSticker : Mod
    {
        // 创建一个静态的Mod实例，这样我们就可以在其他地方获取到这个实例
        public static TerraSticker Instance;
        public PathUtils myPathUtils;

        internal static List<byte> imageCachedData;
        internal static List<byte> gifCachedData;
        public static ClientConfig ClientConfig;
        public static ServerConfig ServerConfig;


        public override void Load()
        {
            myPathUtils = new PathUtils();
            imageCachedData = new List<byte>();
            gifCachedData = new List<byte>();
            Instance = this;

            if (System.IO.Directory.Exists(myPathUtils.netPackageCachePath))
            {
                // 获取路径下的所有文件
                string[] files = System.IO.Directory.GetFiles(myPathUtils.netPackageCachePath);

                // 遍历所有文件并删除
                foreach (string file in files)
                {
                    System.IO.File.Delete(file);
                }
            }


        }

        public override void Unload()
        {
            myPathUtils = null;
            imageCachedData = null;
            gifCachedData = null;
            Instance = null;
        }


        public override async void HandlePacket(BinaryReader reader, int whoAmI)
        {
            switch (reader.ReadByte())
            {
                case (byte)PacketType.image0: // 传输中
                    if (Main.netMode is NetmodeID.Server)
                    {
                        var p = GetPacket();
                        ushort length = reader.ReadUInt16();
                        var data = reader.ReadBytes(length);
                        p.Write((byte)PacketType.image0);
                        p.Write(length);
                        p.Write(data);
                        p.Send(ignoreClient: whoAmI);
                    }
                    else
                    {
                        ushort length = reader.ReadUInt16();
                        var data = reader.ReadBytes(length);
                        imageCachedData.AddRange(data);
                    }

                    break;
                case (byte)PacketType.image1: // 完成包
                    if (Main.netMode is NetmodeID.Server)
                    {
                        var p = GetPacket();
                        p.Write((byte)PacketType.image1); // 包类型
                        p.Write(reader.ReadString());
                        p.Send(ignoreClient: whoAmI);
                    }
                    else
                    {
                        string name = reader.ReadString();

                        if (!PathUtils.TryCreatingDirectory(myPathUtils.netPackageCachePath))
                            break;

                        using var stream = imageCachedData.ToArray().ToMemoryStream();

                        Main.NewText(name);
                        string fileName = Path.Combine(myPathUtils.netPackageCachePath, DateTime.Now.ToFileTime() + ".png");
                        using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                        {
                            stream.WriteTo(file);
                        }

                        RemadeChatMonitorHooks.PostToChat(stream, fileName);
                        imageCachedData.Clear();
                    }

                    break;
                case (byte)PacketType.gif0: // 传输中
                    if (Main.netMode is NetmodeID.Server)
                    {
                        var p = GetPacket();
                        ushort length = reader.ReadUInt16();
                        var data = reader.ReadBytes(length);
                        p.Write((byte)PacketType.gif0);
                        p.Write(length);
                        p.Write(data);
                        p.Send(ignoreClient: whoAmI);
                    }
                    else
                    {
                        ushort length = reader.ReadUInt16();
                        var data = reader.ReadBytes(length);
                        gifCachedData.AddRange(data);
                    }

                    break;

                case (byte)PacketType.gif1: // 完成包
                    if (Main.netMode is NetmodeID.Server)
                    {
                        var p = GetPacket();
                        p.Write((byte)PacketType.gif1); // 包类型
                        p.Write(reader.ReadString());
                        p.Send(ignoreClient: whoAmI);
                    }
                    else
                    {
                        string name = reader.ReadString();
                        Main.NewText(name);
                        if (!PathUtils.TryCreatingDirectory(myPathUtils.netPackageCachePath))
                            break;
                        string fileName = Path.Combine(myPathUtils.netPackageCachePath, DateTime.Now.ToFileTime() + ".gif");
                        using (var stream = new MemoryStream(gifCachedData.ToArray()))
                        {
                            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                            {
                                stream.WriteTo(file);
                            }
                        }
                        var gif = Image.Load<Rgba32>(fileName);
                        await Task.Run(() => ImageUtils.RenderGif(gif, Main.LocalPlayer.name, fileName)); // 使用Task.Run移动到后台线程
                        imageCachedData.Clear();
                    }

                    break;


            }
        }
        public void SendImagePacket(MemoryStream stream)
        {
            string name = $"<{Main.LocalPlayer.name}>";

            // 发包
            var imageBytes = stream.ToArray();

            const int batchSize = 50000;
            int totalBytes = imageBytes.Length;
            int startIndex = 0;

            while (startIndex < totalBytes)
            {
                int endIndex = Math.Min(startIndex + batchSize, totalBytes); // 发送[startIndex, endIndex)索引内的所有byte
                var data = imageBytes[startIndex..endIndex];

                var p = GetPacket();
                p.Write((byte)PacketType.image0); // 包类型
                p.Write((ushort)data.Length); // byte数组长度
                p.Write(data); // 数据
                p.Send();

                startIndex = endIndex;
            }

            var finishPacket = GetPacket();
            finishPacket.Write((byte)PacketType.image1); // 包类型
            finishPacket.Write(name);
            finishPacket.Send();
        }

        public void SendGifPacket(MemoryStream stream)
        {
            string name = $"<{Main.LocalPlayer.name}>";

            var imageBytes = stream.ToArray();
            const int batchSize = 50000;
            int totalBytes = imageBytes.Length;
            int startIndex = 0;

            while (startIndex < totalBytes)
            {
                int endIndex = Math.Min(startIndex + batchSize, totalBytes); // 发送[startIndex, endIndex)索引内的所有byte
                var data = imageBytes[startIndex..endIndex];

                var p = TerraSticker.Instance.GetPacket();
                p.Write((byte)PacketType.gif0); // 包类型
                p.Write((ushort)data.Length); // byte数组长度
                p.Write(data); // 数据
                p.Send();

                startIndex = endIndex;
            }

            var finishPacket = TerraSticker.Instance.GetPacket();
            finishPacket.Write((byte)PacketType.gif1); // 包类型
            finishPacket.Write(name);
            finishPacket.Send();

        }


    }
}