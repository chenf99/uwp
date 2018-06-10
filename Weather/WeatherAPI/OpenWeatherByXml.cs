using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WeatherAPI
{
    public class OpenWeatherByXml
    {
        async public static Task<Root> GetWeatherByXml(string location)
        {
            var http = new HttpClient();
            var response = await http.GetAsync("http://api.k780.com/?app=weather.today&weaid=" + location + "&appkey=33196&sign=4b47729adc0f4e2b9dd3dc023782a649&format=xml");
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new XmlSerializer(typeof(Root));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (Root)serializer.Deserialize(ms);

            return data;
        }
    }

    [XmlRoot(ElementName = "result")]
    public class ResultXml
    {
        [XmlElement(ElementName = "weaid")]
        public string weaid { get; set; }

        [XmlElement(ElementName = "days")]
        public string days { get; set; }

        [XmlElement(ElementName = "week")]
        public string week { get; set; }

        [XmlElement(ElementName = "cityno")]
        public string cityno { get; set; }

        [XmlElement(ElementName = "citynm")]
        public string citynm { get; set; }

        [XmlElement(ElementName = "cityid")]
        public string cityid { get; set; }

        [XmlElement(ElementName = "temperature")]
        public string temperature { get; set; }

        [XmlElement(ElementName = "temperature_curr")]
        public string temperature_curr { get; set; }

        [XmlElement(ElementName = "humidity")]
        public string humidity { get; set; }

        [XmlElement(ElementName = "aqi")]
        public string aqi { get; set; }

        [XmlElement(ElementName = "weather")]
        public string weather { get; set; }

        [XmlElement(ElementName = "weather_curr")]
        public string weather_curr { get; set; }

        [XmlElement(ElementName = "weather_icon")]
        public string weather_icon { get; set; }

        [XmlElement(ElementName = "weather_icon1")]
        public string weather_icon1 { get; set; }

        [XmlElement(ElementName = "wind")]
        public string wind { get; set; }

        [XmlElement(ElementName = "winp")]
        public string winp { get; set; }

        [XmlElement(ElementName = "temp_high")]
        public string temp_high { get; set; }

        [XmlElement(ElementName = "temp_low")]
        public string temp_low { get; set; }

        [XmlElement(ElementName = "temp_curr")]
        public string temp_curr { get; set; }

        [XmlElement(ElementName = "humi_high")]
        public string humi_high { get; set; }

        [XmlElement(ElementName = "humi_low")]
        public string humi_low { get; set; }

        [XmlElement(ElementName = "weatid")]
        public string weatid { get; set; }

        [XmlElement(ElementName = "weatid1")]
        public string weatid1 { get; set; }

        [XmlElement(ElementName = "windid")]
        public string windid { get; set; }

        [XmlElement(ElementName = "winpid")]
        public string winpid { get; set; }

        [XmlElement(ElementName = "weather_iconid")]
        public string weather_iconid { get; set; }
    }

    [XmlRoot(ElementName = "root")]
    public class Root
    {
        [XmlElement(ElementName = "success")]
        public string Success { get; set; }

        [XmlElement(ElementName = "result")]
        public ResultXml result { get; set; }
    }

}
