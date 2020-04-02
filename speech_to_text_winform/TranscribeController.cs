using speech_to_text_winform.services;
using System;
using System.Threading.Tasks;

namespace speech_to_text_winform
{
    class TranscribeController
    {
        private SpeechToTextService _textService;
        private YoutubeService _youtubeService;
        private CloudStorageService _cloudStorageService;


        private string bucketName = "speech-to-text-resources-bucket";

        public TranscribeController()
        {
            _textService = new SpeechToTextService();
            _youtubeService = new YoutubeService();
            _cloudStorageService = new CloudStorageService();
        }

        public async Task<String> DownloadVideo(string url)
        {
            var data = await _youtubeService.DownloadVideoTask(url);

            if (data != null)
            {
                var uploadBucket = await _cloudStorageService.AsyncUploadFile(bucketName, data);

                if (uploadBucket != null)
                {

                    var text = await _textService.AsyncRecognizeMultipleChannelsGcs($"gs://speech-to-text-resources-bucket/{uploadBucket}");

                    Console.WriteLine($"ini speech {text}");

                    return text;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
