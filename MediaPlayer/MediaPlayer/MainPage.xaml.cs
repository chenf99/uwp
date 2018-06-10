using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MediaPlayer
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

        private void play_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            if (picture.Visibility == Visibility.Visible)
            {
                ellstoryBoard.Begin();
            }
        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
            if (picture.Visibility == Visibility.Visible)
            {
                ellstoryBoard.Pause();
            }
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            if (picture.Visibility == Visibility.Visible)
            {
                ellstoryBoard.Stop();
            }
        }

        private void fullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (picture.Visibility != Visibility.Visible)mediaPlayer.IsFullWindow = !mediaPlayer.IsFullWindow;
            ApplicationView view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode == true)
            {
                view.ExitFullScreenMode();
            }
            else
            {
                view.TryEnterFullScreenMode();
            }
        }

        async private void openFile_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();

            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".mp3");

            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                var stream = await file.OpenAsync(FileAccessMode.Read);
                mediaPlayer.SetSource(stream, file.ContentType);
                ellstoryBoard.Stop();
                
                if (file.FileType == ".mp3")
                {
                    picture.Visibility = Visibility.Visible;
                    mediaPlayer.Visibility = Visibility.Collapsed;
                    /*var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 500, ThumbnailOptions.UseCurrentScale);
                    BitmapImage bitmapImage = new BitmapImage();
                    InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
                    await RandomAccessStream.CopyAsync(thumbnail, randomAccessStream);
                    randomAccessStream.Seek(0);
                    bitmapImage.SetSource(randomAccessStream);
                    image.ImageSource = bitmapImage;*/
                }
                else
                {
                    picture.Visibility = Visibility.Collapsed;
                    mediaPlayer.Visibility = Visibility.Visible;
                }

                //mediaPlayer.Play();
            }
        }

        private void volumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            mediaPlayer.Volume = (double)volumeSlider.Value;
        }

        private void mediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            progressSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
        }
    }
}
