namespace speech_to_text_winform.models
{
    //    using SendModel;
    //
    //    var sendModel = SendModel.FromJson(jsonString);

    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class SendModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("transcribe")]
        public string Transcribe { get; set; }
    }

    public partial class SendModel
    {
        public static SendModel FromJson(string json) => JsonConvert.DeserializeObject<SendModel>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this SendModel self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
