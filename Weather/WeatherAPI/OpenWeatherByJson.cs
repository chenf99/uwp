using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace WeatherAPI
{
    public class OpenWeatherByJson
    {
        async public static Task<RootObject> GetWeatherByJson(string location)
        {
            var http = new HttpClient();
            var response = await http.GetAsync("http://api.k780.com/?app=weather.future&weaid=" + location + "&&appkey=33196&sign=4b47729adc0f4e2b9dd3dc023782a649");
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(RootObject));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (RootObject)serializer.ReadObject(ms);

            return data;
        }
    }

    [DataContract]
    public class Result
    {
        [DataMember]
        public string weaid { get; set; }

        [DataMember]
        public string days { get; set; }

        [DataMember]
        public string week { get; set; }

        [DataMember]
        public string cityno { get; set; }

        [DataMember]
        public string citynm { get; set; }

        [DataMember]
        public string cityid { get; set; }

        [DataMember]
        public string temperature { get; set; }

        [DataMember]
        public string humidity { get; set; }

        [DataMember]
        public string weather { get; set; }

        [DataMember]
        public string weather_icon { get; set; }

        [DataMember]
        public string weather_icon1 { get; set; }

        [DataMember]
        public string wind { get; set; }

        [DataMember]
        public string winp { get; set; }

        [DataMember]
        public string temp_high { get; set; }

        [DataMember]
        public string temp_low { get; set; }

        [DataMember]
        public string humi_high { get; set; }

        [DataMember]
        public string humi_low { get; set; }

        [DataMember]
        public string weatid { get; set; }

        [DataMember]
        public string weatid1 { get; set; }

        [DataMember]
        public string windid { get; set; }

        [DataMember]
        public string winpid { get; set; }

        [DataMember]
        public string weather_iconid { get; set; }

        [DataMember]
        public string weather_iconid1 { get; set; }
    }

    [DataContract]
    public class RootObject
    {
        [DataMember]
        public string success { get; set; }

        [DataMember]
        public List<Result> result { get; set; }
    }
}
