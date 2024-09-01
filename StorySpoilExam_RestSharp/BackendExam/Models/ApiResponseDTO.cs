using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendExam.Models
{
    internal class ApiResponseDTO
    {
        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("storyId")]
        public string StoryId { get; set; }

    }
}
