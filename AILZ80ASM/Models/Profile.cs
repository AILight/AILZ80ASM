using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AILZ80ASM.Models
{
    public class Profile
    {
        [JsonPropertyName("default-options")]
        public string[] DefaultOptions { get; set; }
        [JsonPropertyName("disable-warnings")]
        public string[] DisableWarnings { get; set; }
    }
}
