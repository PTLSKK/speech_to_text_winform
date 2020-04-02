using System;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Models.MediaStreams;

namespace speech_to_text_winform.services
{
    internal class YoutubeService
    {

        public async Task<string> DownloadVideoTask(string url)
        {
            try
            {
                var client = new YoutubeClient();
                var id = YoutubeClient.ParseVideoId(url);
                var video = await client.GetVideoAsync(id);
                var title = video.Title;
                var author = video.Author;
                var duration = video.Duration;
                var fileName = DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss");

                var converter = new YoutubeConverter(client, "C:\\ffmpeg-4.2.2\\bin\\ffmpeg.exe");

                await converter.DownloadVideoAsync(id, $"C:\\Temp\\{fileName}.wav");

                var path = $"C:\\Temp\\{fileName}.wav";

                Console.WriteLine("[DownloadVideoTask] - Done Download Video");

                return path;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[DownloadVideoTask] - Error {e}");

                return null;
            }
        }

        public async Task<string> DownloadVideoOptionsTask(string url)
        {
            //try
            //{
            var client = new YoutubeClient();

            var id = YoutubeClient.ParseVideoId(url);

            var video = await client.GetVideoAsync(id);

            var author = video.Author;

            var converter = new YoutubeConverter(client, "C:\\ffmpeg-4.2.2\\bin\\ffmpeg.exe");

            var mediaStreamInfoSet = await client.GetVideoMediaStreamInfosAsync(id);

            // Select video stream
            var videoStreamInfo = mediaStreamInfoSet.Video.FirstOrDefault(s => s.VideoQualityLabel == "240p");

            // Select audio stream
            var audioStreamInfo = mediaStreamInfoSet.Audio.First(s => s.Bitrate == 128);

            // Combine them into a collection
            var mediaStreamInfos = new MediaStreamInfo[] { audioStreamInfo, videoStreamInfo };

            // Download and process them into one file
            await converter.DownloadAndProcessMediaStreamsAsync(mediaStreamInfos, $"C:\\Temp\\{author}.wav", "wav");

            var path = $"C:\\Temp\\{author}.wav";

            Console.WriteLine("[DownloadVideoTask] - Done Download Video");

            return path;
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine($"[DownloadVideoTask] - Error {e}");

            //    return null;
            //}
        }
    }
}