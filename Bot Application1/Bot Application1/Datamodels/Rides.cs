using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application1.Datamodels
{
    public class Rides
    {
            [JsonProperty(PropertyName = "id")]
            public string ID { get; set; }

            [JsonProperty(PropertyName = "Type")]
            public string TYPE { get; set; }

            [JsonProperty(PropertyName = "Price")]
            public string PRICE { get; set; }
        }    
}