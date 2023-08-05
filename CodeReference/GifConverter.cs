using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using StickersTest.Utils;

namespace StickersTest.CodeReference
{
    public class GifConverter : ModSystem
    {
        private static Random s_rand = new();
        private static Dictionary<uint, Queue<Stream>> s_awaitingStreams = new();
        private static Dictionary<uint, List<Texture2D>> s_awaitingGifs = new();
        private static Dictionary<uint, (string sentBy, string imagePath)> s_gifDatas = new();

        public override void PostUpdateEverything()
        {
            TrySendFirstGif();
            TryConvertFirstStream();
        }

        /// <summary>
        /// Enqueues an array of Gif frame streams for conversion. Once converted, they will be sent to chat
        /// </summary>
        public static void EnqueueGifFramesStreams(Stream[] streams, string sentBy, string imagePath)
        {
            // We generate a unique hashcode for every new Gif so that if two Gifs are received at the same time, 
            // they will be dealt with separately
            uint hashCode = (uint)(DateTime.Now.GetHashCode() ^ sentBy.GetHashCode() ^ s_rand.Next());

            s_awaitingStreams.Add(hashCode, new());
            s_awaitingGifs.Add(hashCode, new());
            s_gifDatas.Add(hashCode, (sentBy, imagePath));

            foreach (var stream in streams)
            {
                s_awaitingStreams[hashCode].Enqueue(stream);
            }
        }

        /// <summary>
        /// Converts the first awaiting frame into a Texture2D
        /// </summary>
        private void TryConvertFirstStream()
        {
            if (!s_awaitingStreams.Any())
                return;

            var awaitingStream = s_awaitingStreams.First();

            // This check might be unnecessary
            if (awaitingStream.Value == null || !awaitingStream.Value.Any())
                return;

            uint hash = awaitingStream.Key;
            var queue = awaitingStream.Value;

            using (var stream = queue.Dequeue())
            {
                try
                {
                    var tex2d = Texture2D.FromStream(Main.instance.GraphicsDevice, stream);
                    s_awaitingGifs[hash].Add(tex2d);
                }
                catch (Exception e)
                {
                    GifsChatModUtils.NewText("Failed to convert stream into Texture2D!", true);
                    GifsChatModUtils.NewText(e.GetType().ToString(), true);
                    GifsChatModUtils.NewText(e.Message, true);
                }
            }
        }

        /// <summary>
        /// Sends the first fully converted Gif in chat
        /// </summary>
        private void TrySendFirstGif()
        {
            if (!s_awaitingGifs.Any())
                return;

            // Get all Gifs that are ready to be sent. If none are found, return
            var readyGifs = s_awaitingGifs.Where(kv => !s_awaitingStreams[kv.Key].Any());
            if (!readyGifs.Any())
                return;

            var awaitingGif = readyGifs.First();

            uint hash = awaitingGif.Key;
            var queue = awaitingGif.Value;

            try
            {
                Main.NewText($"<{s_gifDatas[hash].sentBy}>");
                RemadeChatMonitorHooks.SendTexture(queue.ToArray(), s_gifDatas[hash].imagePath);
            }
            catch (Exception e)
            {
                GifsChatModUtils.NewText("Failed to send fully-converted Gif in chat!", true);
                GifsChatModUtils.NewText(e.GetType().ToString(), true);
                GifsChatModUtils.NewText(e.Message, true);
            }
            finally
            {
                // Once a gif has been successfully sent, we delete it from our caches
                s_awaitingStreams.Remove(hash);
                s_awaitingGifs.Remove(hash);
                s_gifDatas.Remove(hash);

                //Main.NewText(s_awaitingGifs.Count);
            }
        }
    }
}
