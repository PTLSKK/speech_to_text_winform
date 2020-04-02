using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Cloud.Storage.V1;
using speech_to_text_winform.services;

namespace speech_to_text_winform
{
    public partial class Form1 : Form
    {
        private delegate void ShowMessageDelegate(string message);

        private SpeechToTextService _textService;
        private MqttService _mqttService;
        private YoutubeService _youtubeService;
        private CloudStorageService _cloudStorageService;

        private string bucketName = "speech-to-text-resources-bucket";

        public Form1()
        {
            InitializeComponent();

            // Initialize class
            InitClass();

            // Connect to server
            ConnectMqtt();

            // Download Video
            //DownloadVideo();

        }
        private void InitClass()
        {
            _mqttService = new MqttService(this);
            _youtubeService = new YoutubeService();
            _textService = new SpeechToTextService();
            _cloudStorageService = new CloudStorageService();

            _textService.OnStatusChanged += _textService_OnStatusChanged;
            _textService.OnTranscribeDone += _textService_OnTranscribeDone;
        }

        private void _textService_OnTranscribeDone(string messages)
        {
            if (this.tbResultManual.InvokeRequired)
            {
                tbResultManual.Invoke(new MethodInvoker(delegate { tbResultManual.AppendText(messages + Environment.NewLine); }));
            }
            else
            {

                tbResultManual.AppendText(messages + Environment.NewLine);
            }
        }

        private void _textService_OnStatusChanged(string messages)
        {
            var show = new ShowMessageDelegate(UpdateStatus);
            Invoke(show, messages);
        }

        private void UpdateStatus(string message)
        {
            lblSpeech.Text = message;
        }

        private void ConnectMqtt()
        {
            Task.Run(() =>
                _mqttService.ConnectBrokerAsync());

            _mqttService.OnConnectionChanges += _mqttService_OnConnectionChanges;
            _mqttService.OnMessagesArrive += _mqttService_OnMessagesArrive;
        }

        private void _mqttService_OnMessagesArrive(string messages)
        {
            var show = new ShowMessageDelegate(UpdateOnMessageReceived);
            Invoke(show, messages);
        }

        private void UpdateOnMessageReceived(string message)
        {
            tbResult.AppendText(message + Environment.NewLine);
        }

        private void _mqttService_OnConnectionChanges(string messages)
        {
            var show = new ShowMessageDelegate(UpdateConnection);
            Invoke(show, messages);
        }

        private void UpdateConnection(string message)
        {
            lblMqtt.Text = message;
        }

        public async void DownloadVideo(string url)
        {
            var data = await _youtubeService.DownloadVideoTask(url);

            if (data != null)
            {
                var uploadBucket = await _cloudStorageService.AsyncUploadFile(bucketName, data);

                if (uploadBucket != null)
                {

                    var text = await _textService.AsyncRecognizeMultipleChannelsGcs($"gs://speech-to-text-resources-bucket/{uploadBucket}");
                    Console.WriteLine($"ini speech {text}");
                }
            }
        }

        private void btnTranscribe_Click(object sender, EventArgs e)
        {
            DownloadVideo(tbLink.Text);
        }
    }
}
