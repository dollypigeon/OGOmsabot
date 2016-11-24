using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application1.Datamodels
{
    public class Customer
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "Cname")]
        public string CNAME { get; set; }

        [JsonProperty(PropertyName = "Phone")]
        public string PHONE { get; set; }

        [JsonProperty(PropertyName = "CAge")]
        public string CAGE { get; set; }

        [JsonProperty(PropertyName ="Date")]
        public string DBDATE { get; set; }

        [JsonProperty(PropertyName = "Bookingtime")]
        public string TIME { get; set; }

    }
}