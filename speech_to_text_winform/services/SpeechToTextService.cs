using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Speech.V1;

namespace speech_to_text_winform.services
{
    class SpeechToTextService
    {        
        // used to pass messages back to UI for processing
        public delegate void OnMessagesReceived(string messages);
        public event OnMessagesReceived OnStatusChanged;
        public event OnMessagesReceived OnTranscribeDone;

        public string SyncRecognize(string filePath)
        {
            try
            {
                var text = "";
                var speech = SpeechClient.Create();
                var response = speech.Recognize(new RecognitionConfig()
                {
                    LanguageCode = "id"
                }, RecognitionAudio.FromFile(filePath));

                foreach (var result in response.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        Console.WriteLine(alternative.Transcript);
                        text += alternative.Transcript;
                    }

                }

                return text;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SyncRecognize] - {e}");

                return null;
            }
        }

        public string LongRunningRecognize(string filePath)
        {
            try
            {
                var text = "";
                var speech = SpeechClient.Create();
                var longOperation = speech.LongRunningRecognize(new RecognitionConfig()
                {
                    LanguageCode = "id"
                }, RecognitionAudio.FromFile(filePath));

                longOperation = longOperation.PollUntilCompleted();

                var response = longOperation.Result;

                foreach (var result in response.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        Console.WriteLine(alternative.Transcript);
                        text += alternative.Transcript;
                    }
                }

                return text;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[LongRunningRecognize] - {e}");

                return null;
            }
        }



        public async Task<string> AsyncRecognizeMultipleChannelsGcs(string storageUri)
        {
            try
            {
                OnStatusChanged("Transcribe");

                Console.WriteLine(DateTime.Now.ToString());
                var text = "";
                var speech = await SpeechClient.CreateAsync();
                var longOperation = await speech.LongRunningRecognizeAsync(new RecognitionConfig()
                {
                    LanguageCode = "id",
                    EnableSeparateRecognitionPerChannel = true,
                    AudioChannelCount = 2,
                    EnableAutomaticPunctuation = true,
                }, RecognitionAudio.FromStorageUri(storageUri));

                longOperation = await longOperation.PollUntilCompletedAsync();

                var response = longOperation.Result;

                foreach (var result in response.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        Console.WriteLine($"Transcript: { alternative.Transcript}");
                        Console.WriteLine("\n");

                        text += alternative.Transcript;
                    }
                }

                Console.WriteLine(DateTime.Now.ToString());

                OnStatusChanged("Idle");
                OnTranscribeDone(text);

                return text;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[LongRunningRecognize] - {e}");

                OnStatusChanged("Idle");

                return null;
            }
        }

        public string SyncRecognizeMultipleChannelsGcs(string storageUri)
        {
            try
            {
                Console.WriteLine(DateTime.Now.ToString());
                var text = "";
                var speech = SpeechClient.Create();
                var longOperation = speech.LongRunningRecognize(new RecognitionConfig()
                {
                    LanguageCode = "id",
                    EnableSeparateRecognitionPerChannel = true,
                    AudioChannelCount = 2,
                    EnableAutomaticPunctuation = true,
                }, RecognitionAudio.FromStorageUri(storageUri));

                longOperation = longOperation.PollUntilCompleted();

                var response = longOperation.Result;

                foreach (var result in response.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        Console.WriteLine($"Transcript: { alternative.Transcript}");
                        text += alternative.Transcript;
                    }
                }

                Console.WriteLine(DateTime.Now.ToString());
                return text;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[LongRunningRecognize] - {e}");

                return null;
            }
        }

        public string AsyncRecognizeGcs(string storageUri)
        {
            try
            {
                var text = "";
                var speech = SpeechClient.Create();
                var longOperation = speech.LongRunningRecognize(new RecognitionConfig()
                {
                    LanguageCode = "id",
                }, RecognitionAudio.FromStorageUri(storageUri));

                longOperation = longOperation.PollUntilCompleted();

                var response = longOperation.Result;

                foreach (var result in response.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        Console.WriteLine($"Transcript: { alternative.Transcript}");
                        text += alternative.Transcript;
                    }
                }

                return text;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[LongRunningRecognize] - {e}");

                return null;
            }
        }
    }
}
