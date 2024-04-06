using Newtonsoft.Json;

namespace Reader
{
    public class Word
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("weight")]
        public int weight { get; set; }
    }
}