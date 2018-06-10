using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using MyList.ViewModels;
using Windows.Storage.AccessCache;
using MyList.Services;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyList
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewPage : Page
    {
        WriteableBitmap source;
        async private void OpenFileWriteBit()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/BackGroud.jpg", UriKind.RelativeOrAbsolute));
            source = await ImageTools.FileToWriteableBit(file);
        }

        public NewPage()
        {
            this.InitializeComponent();
            OpenFileWriteBit();
        }

        private bool Valid_Date(int y, int m, int d)
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            if (y < year) return false;
            else if (y > year) return true;
            if (m < month) return false;
            else if (m > month) return true;
            if (d < day) return false;
            else return true;
        }

        private void Button_Click_Create(object sender, RoutedEventArgs e)
        {
            int year = date_picker.Date.Year;
            int month = date_picker.Date.Month;
            int day = date_picker.Date.Day;

            if (title.Text == "" || detail.Text == "") Empty_Text();
            else if (!Valid_Date(year, month, day)) InValid_Date_Input();
            else
            {
                if ((string)create.Content == "Create") {
                    ListItemViewModel.GetViewModel().AddListItem(title.Text, detail.Text, new DateTime(year, month, day), source);
                    Create_Succeed();
                }
                else if ((string)create.Content == "Update")
                {
                    ListItemViewModel.GetViewModel().UpdateListItem(ListItemViewModel.GetViewModel().selectedItem.GetID(),title.Text, detail.Text, new DateTime(year, month, day), source);
                    Update_Succeed();
                }
                TileService.UpdateTiles();
                title.Text = detail.Text = "";
                date_picker.Date = DateTime.Now;
                create.Content = "Create";
                OpenFileWriteBit();
                picture.Source = new BitmapImage(new Uri("ms-appx:/Assets/BackGroud.jpg", UriKind.RelativeOrAbsolute));
                this.Frame.GoBack();
            }
        }
        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            title.Text = detail.Text = "";
            date_picker.Date = DateTime.Now;
            create.Content = "Create";
            ListItemViewModel.GetViewModel().selectedItem = null;
            OpenFileWriteBit();
            picture.Source = new BitmapImage(new Uri("ms-appx:/Assets/BackGroud.jpg", UriKind.RelativeOrAbsolute));
            this.Frame.GoBack();
        }

        private async void FileOpen_ClickAsync(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                WriteableBitmap image = await ImageTools.FileToWriteableBit(file);
                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    image.SetSource(stream);
                }
                source = image;
                picture.Source = image;
                ApplicationData.Current.LocalSettings.Values["picture"] = StorageApplicationPermissions.FutureAccessList.Add(file);
            }
        }

        private async void Create_Succeed()
        {
            ContentDialog create_succeed = new ContentDialog
            {
                Title = "Succeed",
                Content = "You have created an event.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await create_succeed.ShowAsync();
        }

        private async void Update_Succeed()
        {
            ContentDialog update_succeed = new ContentDialog
            {
                Title = "Succeed",
                Content = "You have updated the event.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await update_succeed.ShowAsync();
        }

        private async void Delete_Succeed()
        {
            ContentDialog delete_succeed = new ContentDialog
            {
                Title = "Succeed",
                Content = "You have deleted the event.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await delete_succeed.ShowAsync();
        }

        private async void InValid_Date_Input()
        {
            ContentDialog invalid_date_input = new ContentDialog
            {
                Title = "Error",
                Content = "The due date has passed!",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await invalid_date_input.ShowAsync();
        }

        private async void Empty_Text()
        {
            ContentDialog empty_text = new ContentDialog
            {
                Title = "Error",
                Content = "Title and detail can't be empty!",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await empty_text.ShowAsync();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListItemViewModel.GetViewModel().selectedItem != null)
            {
                ListItemViewModel.GetViewModel().RemoveListItem(ListItemViewModel.GetViewModel().selectedItem.GetID());
                TileService.UpdateTiles();
                Delete_Succeed();
                this.Frame.GoBack();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            if (slider != null)
            {
                picture.Width = picture.Height = 3 * slider.Value;
                if (picture.Height > 250) picture.Height = 250;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            bool suspending = ((App)App.Current).issuspend;
            if (suspending)
            {
                ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
                composite["title"] = title.Text;
                composite["detail"] = detail.Text;
                composite["date"] = date_picker.Date;
                ApplicationData.Current.LocalSettings.Values["newPage"] = composite;
            }
            base.OnNavigatedFrom(e);
        }

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ListItemViewModel.GetViewModel().selectedItem != null)
            {
                title.Text = ListItemViewModel.GetViewModel().selectedItem.Title;
                detail.Text = ListItemViewModel.GetViewModel().selectedItem.description;
                date_picker.Date = ListItemViewModel.GetViewModel().selectedItem.date;
                create.Content = "Update";
                picture.Source = ListItemViewModel.GetViewModel().selectedItem.ImageSource;
                source = picture.Source as WriteableBitmap;
                DeleteButton.Visibility = Visibility.Visible;
            }
            if (e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("newPage");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("newPage"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["newPage"] as ApplicationDataCompositeValue;
                    title.Text = (string)composite["title"];
                    detail.Text = (string)composite["detail"];
                    date_picker.Date = (DateTimeOffset)composite["date"];
                    StorageFile file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync((string)ApplicationData.Current.LocalSettings.Values["picture"]);
                    WriteableBitmap image = await ImageTools.FileToWriteableBit(file);
                    using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        image.SetSource(stream);
                    }
                    source = image;
                    picture.Source = image;
                    ApplicationData.Current.LocalSettings.Values.Remove("newPage");
                }
            }
            base.OnNavigatedTo(e);
        }
    }
}
