using System.Collections.Generic;
using Newtonsoft.Json;

namespace Reader
{
    public class Classification
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("words")] public List<Word> words = new List<Word>();
        public int currentWeight { get; set; }
    }
}