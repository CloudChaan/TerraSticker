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
        // ����һ����̬��Modʵ�����������ǾͿ����������ط���ȡ�����ʵ��
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
                // ��ȡ·���µ������ļ�
                string[] files = System.IO.Directory.GetFiles(myPathUtils.netPackageCachePath);

                // ���������ļ���ɾ��
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
                case (byte)PacketType.image0: // ������
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
                case (byte)PacketType.image1: // ��ɰ�
                    if (Main.netMode is NetmodeID.Server)
                    {
                        var p = GetPacket();
                        p.Write((byte)PacketType.image1); // ������
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
                case (byte)PacketType.gif0: // ������
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

                case (byte)PacketType.gif1: // ��ɰ�
                    if (Main.netMode is NetmodeID.Server)
                    {
                        var p = GetPacket();
                        p.Write((byte)PacketType.gif1); // ������
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
                        await Task.Run(() => ImageUtils.RenderGif(gif, Main.LocalPlayer.name, fileName)); // ʹ��Task.Run�ƶ�����̨�߳�
                        imageCachedData.Clear();
                    }

                    break;


            }
        }
        public void SendImagePacket(MemoryStream stream)
        {
            string name = $"<{Main.LocalPlayer.name}>";

            // ����
            var imageBytes = stream.ToArray();

            const int batchSize = 50000;
            int totalBytes = imageBytes.Length;
            int startIndex = 0;

            while (startIndex < totalBytes)
            {
                int endIndex = Math.Min(startIndex + batchSize, totalBytes); // ����[startIndex, endIndex)�����ڵ�����byte
                var data = imageBytes[startIndex..endIndex];

                var p = GetPacket();
                p.Write((byte)PacketType.image0); // ������
                p.Write((ushort)data.Length); // byte���鳤��
                p.Write(data); // ����
                p.Send();

                startIndex = endIndex;
            }

            var finishPacket = GetPacket();
            finishPacket.Write((byte)PacketType.image1); // ������
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
                int endIndex = Math.Min(startIndex + batchSize, totalBytes); // ����[startIndex, endIndex)�����ڵ�����byte
                var data = imageBytes[startIndex..endIndex];

                var p = TerraSticker.Instance.GetPacket();
                p.Write((byte)PacketType.gif0); // ������
                p.Write((ushort)data.Length); // byte���鳤��
                p.Write(data); // ����
                p.Send();

                startIndex = endIndex;
            }

            var finishPacket = TerraSticker.Instance.GetPacket();
            finishPacket.Write((byte)PacketType.gif1); // ������
            finishPacket.Write(name);
            finishPacket.Send();

        }


    }
}