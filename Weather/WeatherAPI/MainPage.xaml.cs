using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace WeatherAPI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        async private void JsonButton_Click(object sender, RoutedEventArgs e)
        {
            string input;
            if (JsonInputBox.Text == "") input = "guangzhou";  //default, find the weather of guangzhou
            else input = JsonInputBox.Text;

            RootObject myWeather = await OpenWeatherByJson.GetWeatherByJson(input);

            if (myWeather.result == null)   // not find the city
            {
                NotFindCity();
                return;
            }

            string result = myWeather.result[0].week + "-"
                            + myWeather.result[0].days + "\n"
                            + myWeather.result[0].citynm + "\n"
                            + myWeather.result[0].weather + "\n"
                            + myWeather.result[0].temperature + "\n"
                            + "风向： " + myWeather.result[0].wind + "\n";

            resultBlock.Text = result;

            string icon = myWeather.result[0].weather_icon;
            image.Source = new BitmapImage(new Uri(icon, UriKind.Absolute));
        }

        async private void NotFindCity()
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Not find ",
                Content = "We can't find the city, please check your input",
                CloseButtonText = "OK"
            };
            ContentDialogResult show = await dialog.ShowAsync();
        }

        async private void XmlButton_Click(object sender, RoutedEventArgs e)
        {
            string input;
            if (XmlInputBox.Text == "") input = "guangzhou";  //default, find the weather of guangzhou
            else input = XmlInputBox.Text;

            Root myWeather = await OpenWeatherByXml.GetWeatherByXml(input);

            if (myWeather.result == null)
            {
                NotFindCity();
                return;
            }

            string result = myWeather.result.week + "-"
                            + myWeather.result.days + "\n"
                            + myWeather.result.citynm + "\n"
                            + myWeather.result.weather + "\n"
                            + myWeather.result.temperature + "\n"
                            + "风向： " + myWeather.result.wind + "\n";

            resultBlock.Text = result;

            string icon = myWeather.result.weather_icon;
            image.Source = new BitmapImage(new Uri(icon, UriKind.Absolute));
        }
    }
}
