using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using ReLogic.Content;
using Terraria.ModLoader;
using StickersTest.CodeReference;
using StickersTest.ModConfigs;
using System.Windows.Forms;
using Terraria.ID;
using Terraria.Localization;
using tModPorter;

namespace StickersTest.Utils
{
    public static class ImageUtils
    {

        public static Texture2D LoadAssetFromCacheImage(string imagePath)
        {
            Texture2D texture = null;
            using (var stream = File.OpenRead(imagePath))
            {
                texture = Texture2D.FromStream(Main.graphics.GraphicsDevice, stream);
            }
            return texture;
        }

        public static Texture2D LoadAssetFromCacheImageByStickerPath(string stickerPath)
        {
            return LoadAssetFromCacheImage(ModContent.GetInstance<StickersTest>().myPathUtils.GetCachePathFromOriginalImagePath(stickerPath));
        }



        public static string ConvertToPng(string path)
        {
            string outputPath = null;
            if (File.Exists(path))
            {
                try
                {
                    using (Image image = Image.Load(path))
                    {
                        // 当文件是多帧图像时（如GIF），将其第一帧抽取并在图像原始路径下保存为<OriginalFileName>_FirstFrame.PNG
                        if (image.Frames.Count > 1)
                        {
                            string directory = Path.GetDirectoryName(path);
                            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                            outputPath = Path.Combine(directory, $"{fileNameWithoutExtension}_FirstFrame.PNG");
                            if(File.Exists(outputPath))
                            {
                                return outputPath;
                            }
                            image.SaveAsPng(outputPath);
                        }
                        else
                        {
                            // 如果是静态图像且不是.png格式，则将该图像保存为.png格式并覆盖原图像
                            if (Path.GetExtension(path) != ".png")
                            {
                                outputPath = Path.ChangeExtension(path, ".png");
                                image.SaveAsPng(outputPath);

                                // 删除原始文件
                                File.Delete(path);
                            }
                            else
                            {
                                // 如果已经是.png，直接返回路径
                                outputPath = path;
                            }
                        }
                    }
                }
                catch (Exception ex) when (ex is NotSupportedException || ex is UnknownImageFormatException)
                {
                    return null;
                }
            }
            else
            {
                Console.WriteLine($"File '{path}' does not exist.");
            }
            return outputPath;
        }

        public static string CropAndSaveImage(string path, float aspectRatio)
        {
            if (Path.GetExtension(path) != ".png" && Path.GetExtension(path) != ".PNG")
            {
                throw new FormatException("Input file is not a PNG image.");
            }

            using (Image<Rgba32> image = Image.Load<Rgba32>(path))
            {
                int newWidth, newHeight;

                if (image.Width / (float)image.Height > aspectRatio)
                {
                    newHeight = image.Height;
                    newWidth = (int)(newHeight * aspectRatio);
                }
                else
                {
                    newWidth = image.Width;
                    newHeight = (int)(newWidth / aspectRatio);
                }

                int startX = (image.Width - newWidth) / 2;
                int startY = (image.Height - newHeight) / 2;

                image.Mutate(x => x.Crop(new Rectangle(startX, startY, newWidth, newHeight)));

                // Resize the image to a width of 180 pixels, height is calculated based on the aspect ratio
                int resizedHeight = (int)(180 / aspectRatio);
                image.Mutate(x => x.Resize(180, resizedHeight));
                string directory = ModContent.GetInstance<StickersTest>().myPathUtils.cachePath;
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                string newPath = Path.Combine(directory, $"{fileNameWithoutExtension}_Cache.PNG");

                image.Save(newPath);
                return newPath;
            }
        }

        public static MemoryStream GetMemoryFromImage(string path)
        {
            if (File.Exists(path))
            {
                byte[] fileBytes = File.ReadAllBytes(path);
                MemoryStream memoryStream = new MemoryStream(fileBytes);
                return memoryStream;
            }
            else
            {
                throw new FileNotFoundException("The file at path " + path + " does not exist.");
            }
        }


        /// <summary>
        /// Splits a Gif stream into an array of image streams
        /// </summary>
        private static async Task<Stream[]> ExtractGifFrames(
            Image<Rgba32> gif,
            int framesLimit)
        {
            var frames = new List<Stream>();

            using (gif)
            {
                int totalFrameCount = gif.Frames.Count;

                if (totalFrameCount < 1)
                    return null;

                // This is the canvas that will get drawn over after each iteration
                Image<Rgba32> canvas = null;
                bool isTransparent = false;

                // Will break once every frame has been extracted or the frames limit has been reached
                for (int i = 0; i < totalFrameCount && frames.Count < framesLimit; i++)
                {
                    var currFrame = gif.Frames.CloneFrame(i);

                    if (i == 0)
                    {
                        canvas = currFrame; // We set the canvas
                        isTransparent = canvas.IsTransparent();
                    }
                    else if (isTransparent)
                    {
                        canvas = currFrame;
                    }
                    else // We draw over the canvas with the new frame
                    {
                        canvas.Mutate(c => c.DrawImage(currFrame, 1));
                        currFrame.Dispose();
                    }


                    var frameStream = new MemoryStream();
                    await canvas.SaveAsGifAsync(frameStream);

                    frames.Add(frameStream);
                }

                canvas?.Dispose();
            }

            return frames.ToArray();
        }

        private static bool IsTransparent(this Image<Rgba32> image)
        {
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (image[x, y].A < 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        public static async void RenderGif(Image<Rgba32> gif, string sendBy, string imagePath)
        {
            int framesLimit = StickersTest.ClientConfig.FramesLimit;
            var gifFramesStreams = await ExtractGifFrames(gif, framesLimit);

            // Sends the Gif
            GifConverter.EnqueueGifFramesStreams(gifFramesStreams, Main.LocalPlayer.name, imagePath);
        }

        public static void SendGif(MemoryStream gifStream, Image<Rgba32> gif, string sendBy, string imagePath)
        {
            RenderGif(gif, sendBy, imagePath);
            // 多人发包
            if (Main.netMode is NetmodeID.MultiplayerClient)
            {
                ModContent.GetInstance<StickersTest>().SendGifPacket(gifStream);
            }

        }

        public static void LocalSendImage(MemoryStream imageStream, string path)
        {
            if (imageStream.Length > StickersTest.ServerConfig.MaximumFileSize * 1024 * 1024)
            {
                string warning = Language.GetTextValue("Mods.ImageChat.ImageTooLarge", StickersTest.ServerConfig.MaximumFileSize);
                if (StickersTest.ClientConfig.WindowWarning)
                {
                    MessageBox.Show(warning, Language.GetTextValue("Mods.ImageChat.Warn"));
                }
                else
                {
                    Main.NewText(warning, G: 50, B: 50);
                }

                return;
            }

            if (BasicsSystem.SendDelay > 0)
            {
                string warning = Language.GetTextValue("Mods.ImageChat.Wait", BasicsSystem.SendDelay.ToString("F1"));
                if (StickersTest.ClientConfig.WindowWarning)
                {
                    MessageBox.Show(warning, Language.GetTextValue("Mods.ImageChat.Warn"));
                }
                else
                {
                    Main.NewText(warning, G: 50, B: 50);
                }

                return;
            }

            // 设置冷却
            BasicsSystem.SendDelay = StickersTest.ServerConfig.SendCap;

            // 发送图片
            Main.NewText($"<{Main.LocalPlayer.name}>");
            RemadeChatMonitorHooks.PostToChat(imageStream, path);

            // 多人发包
            if (Main.netMode is NetmodeID.MultiplayerClient)
            {
                ModContent.GetInstance<StickersTest>().SendImagePacket(imageStream);
            }
        }

        public static byte[] StreamToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

    }
}
