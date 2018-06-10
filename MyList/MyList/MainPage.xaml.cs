using MyList.ViewModels;
using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using MyList.Models;
using Windows.Storage.AccessCache;
using MyList.Services;
using Windows.UI.Notifications;
using Windows.ApplicationModel.DataTransfer;
using System.Text;
using SQLitePCL;
using System.Collections.ObjectModel;
using Windows.UI;
using System.Threading.Tasks;
using Windows.Storage.Streams;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MyList
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /*int _count;
        private void UpdateBadge(object sender, RoutedEventArgs e)
        {
            _count++;
            TileService.SetBadgeCountOnTile(_count);
        }
        private void UpdatePrimaryTile(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var xmlDoc = TileService.CreateTiles(new PrimaryTile());

            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            TileNotification notification = new TileNotification(xmlDoc);
            updater.Update(notification);
            TileService.UpdateTiles();
        }
        */
        private ListItemViewModel ViewModel = ListItemViewModel.GetViewModel();
        WriteableBitmap source;
        async private void OpenFileWriteBit()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/BackGroud.jpg", UriKind.RelativeOrAbsolute));
            source = await ImageTools.FileToWriteableBit(file);
        }

        public MainPage()
        {
            this.InitializeComponent();
            listView.DataContext = ViewModel.AllItems;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            this.SizeChanged += (s, e) =>
            {
                var state = "VisualStateMin0";
                if (e.NewSize.Width > 800)
                    state = "VisualStateMin800";
                else if (e.NewSize.Width > 600)
                    state = "VisualStateMin600";
                VisualStateManager.GoToState(this, state, true);
            };
            OpenFileWriteBit();
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth <= 800) this.Frame.Navigate(typeof(NewPage));
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListItemViewModel.GetViewModel().selectedItem = (ListItem)e.ClickedItem;
            if (this.ActualWidth <= 800)
            {
                this.Frame.Navigate(typeof(NewPage));
            }
            else
            {
                title.Text = ListItemViewModel.GetViewModel().selectedItem.Title;
                detail.Text = ListItemViewModel.GetViewModel().selectedItem.description;
                date_picker.Date = ListItemViewModel.GetViewModel().selectedItem.date;
                picture.Source = ListItemViewModel.GetViewModel().selectedItem.ImageSource;
                source = picture.Source as WriteableBitmap;
                create.Content = "Update";
            }
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
                if ((string)create.Content == "Create")
                {
                    ListItemViewModel.GetViewModel().AddListItem(title.Text, detail.Text, new DateTime(year, month, day), source);
                    Create_Succeed();
                }
                else if ((string)create.Content == "Update")
                {
                    ListItemViewModel.GetViewModel().UpdateListItem(ListItemViewModel.GetViewModel().selectedItem.GetID(), title.Text, detail.Text, new DateTime(year, month, day), source);
                    Update_Succeed();
                }
                TileService.UpdateTiles();
                title.Text = detail.Text = "";
                date_picker.Date = DateTime.Now;
                create.Content = "Create";
                //source = null;
                OpenFileWriteBit();
                picture.Source = new BitmapImage(new Uri("ms-appx:/Assets/BackGroud.jpg", UriKind.RelativeOrAbsolute));
            }
        }
        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            title.Text = detail.Text = "";
            date_picker.Date = DateTime.Now;
            create.Content = "Create";
            ListItemViewModel.GetViewModel().selectedItem = null;
            //source = null;
            OpenFileWriteBit();
            picture.Source = new BitmapImage(new Uri("ms-appx:/Assets/BackGroud.jpg", UriKind.RelativeOrAbsolute));
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
                picture.Source = image;
                source = image;
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

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            if (slider != null)
            {
                picture.Width = picture.Height = 3 * slider.Value;
                if (picture.Height > 250) picture.Height = 250;
            }
        }

        async private void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog DeleteAllConfirm = new ContentDialog
            {
                Title = "Warning",
                Content = "If you delete all the items, you won't be able to recover it. You sure to delete all?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel"
            };
            ContentDialogResult result = await DeleteAllConfirm.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (ListItemViewModel.GetViewModel().AllItems.Count == 0)
                {
                    NoneEventError();
                }
                else
                {
                    ListItemViewModel.GetViewModel().RemoveAllListItem();
                    DeleteAll_Succeed();
                    ListItemViewModel.GetViewModel().selectedItem = null;
                    title.Text = detail.Text = "";
                    date_picker.Date = DateTime.Now;
                    create.Content = "Create";
                    //source = null;
                    OpenFileWriteBit();
                    picture.Source = new BitmapImage(new Uri("ms-appx:/Assets/BackGroud.jpg", UriKind.RelativeOrAbsolute));
                    TileService.UpdateTiles();
                }
            }
            else
            {

            }
        }

        private async void DeleteAll_Succeed()
        {
            ContentDialog deleteAll_succeed = new ContentDialog
            {
                Title = "Succeed",
                Content = "You have deleted all the events.",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await deleteAll_succeed.ShowAsync();
        }

        private async void NoneEventError()
        {
            ContentDialog noneEventError = new ContentDialog
            {
                Title = "Error",
                Content = "You haven't created any events!",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noneEventError.ShowAsync();
        }

        private void MenuFlyoutItemEdit_Click(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext;
            var item = listView.ContainerFromItem(data) as ListViewItem;
            ViewModel.selectedItem = item.Content as Models.ListItem;
            if (this.ActualWidth <= 800)
            {
                this.Frame.Navigate(typeof(NewPage));
            }
            else
            {
                title.Text = ListItemViewModel.GetViewModel().selectedItem.Title;
                detail.Text = ListItemViewModel.GetViewModel().selectedItem.description;
                date_picker.Date = ListItemViewModel.GetViewModel().selectedItem.date;
                picture.Source = ListItemViewModel.GetViewModel().selectedItem.ImageSource;
                source = picture.Source as WriteableBitmap;
                create.Content = "Update";
            }
        }

        private void MenuFlyoutItemDelete_Click(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext;
            var item = listView.ContainerFromItem(data) as ListViewItem;
            ViewModel.selectedItem = item.Content as Models.ListItem;
            if (ListItemViewModel.GetViewModel().selectedItem != null)
            {
                ListItemViewModel.GetViewModel().RemoveListItem(ListItemViewModel.GetViewModel().selectedItem.GetID());
                ListItemViewModel.GetViewModel().selectedItem = null;
                title.Text = detail.Text = "";
                date_picker.Date = DateTime.Now;
                create.Content = "Create";
                //source = null;
                OpenFileWriteBit();
                picture.Source = new BitmapImage(new Uri("ms-appx:/Assets/BackGroud.jpg", UriKind.RelativeOrAbsolute));
                Delete_Succeed();
                TileService.UpdateTiles();
            }
        }

        private ListItem shareItem;
        private void MenuFlyoutItemShare_Click(object sender, RoutedEventArgs e)
        {
            var s = sender as FrameworkElement;
            shareItem = (Models.ListItem)s.DataContext;
            App.title = shareItem.Title;
            App.detail = shareItem.description;

            DataTransferManager.ShowShareUI();
        }

        async void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            var deferral = args.Request.GetDeferral();
            request.Data.Properties.Title = App.title;
            request.Data.Properties.Description = App.detail;
            request.Data.SetText(App.detail);

            InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
            await stream.WriteAsync((await ImageTools.SaveToBytesAsync(shareItem.ImageSource)).AsBuffer());
            stream.Seek(0);
            RandomAccessStreamReference image = RandomAccessStreamReference.CreateFromStream(stream);
            request.Data.SetBitmap(image);
            /*var photoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/backgroud.jpg"));
            request.Data.SetStorageItems(new List<StorageFile> { photoFile });*/
            deferral.Complete();
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
                for (int i = 0; i < ViewModel.AllItems.Count; ++i)
                {
                    composite["checked" + i] = ViewModel.AllItems[i].Completed;
                }
                ApplicationData.Current.LocalSettings.Values["mainPage"] = composite;
            }
            DataTransferManager.GetForCurrentView().DataRequested -= OnShareDataRequested;
            base.OnNavigatedFrom(e);
        }

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ListItemViewModel.GetViewModel().selectedItem == null)
            {
                title.Text = detail.Text = "";
                date_picker.Date = DateTime.Now;
                create.Content = "Create";
                source = null;
                picture.Source = new BitmapImage(new Uri("ms-appx:/Assets/BackGroud.jpg", UriKind.RelativeOrAbsolute));
            }
            if (e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("mainPage");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("mainPage"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["mainPage"] as ApplicationDataCompositeValue;
                    title.Text = (string)composite["title"];
                    detail.Text = (string)composite["detail"];
                    date_picker.Date = (DateTimeOffset)composite["date"];
                    if (ApplicationData.Current.LocalSettings.Values.ContainsKey("picture"))
                    {
                        StorageFile file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync((string)ApplicationData.Current.LocalSettings.Values["picture"]);
                        if (file != null)
                        {
                            WriteableBitmap image = await ImageTools.FileToWriteableBit(file);
                            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                image.SetSource(stream);
                            }
                            source = image;
                            picture.Source = image;
                        }
                    }
                    for (int i = 0; i < ViewModel.AllItems.Count; ++i)
                    {
                        ViewModel.AllItems[i].Completed = (bool)composite["checked" + i];
                    }
                    ApplicationData.Current.LocalSettings.Values.Remove("mainPage");
                    if (ApplicationData.Current.LocalSettings.Values.ContainsKey("picture"))
                    {
                        ApplicationData.Current.LocalSettings.Values.Remove("picture");
                    }
                }
            }
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
            base.OnNavigatedTo(e);
        }

        async private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var s = sender as FrameworkElement;
            var item = (Models.ListItem)s.DataContext;
            await ViewModel.UpdateListItem(item.GetID());
        }

        private ObservableCollection<string> suggestions = new ObservableCollection<string>();
        async private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            ViewModel.AllItems.Clear();
            suggestions.Clear();
            /*
            int count = ViewModel.AllItems.Count;
            for (int i = 0, j = 0; i < count; ++i)
            {
                if (ViewModel.AllItems[j].Title.Contains(sender.Text) || ViewModel.AllItems[j].description.Contains(sender.Text) || ViewModel.AllItems[j].date.ToString().Contains(sender.Text))
                {
                    suggestions.Add(ViewModel.AllItems[j].Title);
                    j++;
                }
                else ViewModel.AllItems.RemoveAt(j);
            }*/
            
            using (var conn = new SQLiteConnection("MyListDB.db"))
            {
                using (var statement = conn.Prepare("SELECT Id, Title, Detail, Date, Complete, Image FROM ListItemTable "))
                {
                    while (SQLiteResult.ROW == statement.Step())
                    {
                        string id = (string)statement[0];
                        string title = (string)statement[1];
                        string detail = (string)statement[2];
                        string date = (string)statement[3];
                        string complete = (string)statement[4];
                        byte[] image = (byte[])statement[5];
                        DateTime time = Convert.ToDateTime(date);
                        ImageSource source = await ImageTools.SaveToImageSource(image);
                        var item = new ListItem(id, title, detail, time, complete, source);
                        if (title.Contains(sender.Text) || detail.Contains(sender.Text) || date.Contains(sender.Text))
                        {
                            suggestions.Add(title);
                            ViewModel.AllItems.Add(item);
                        }
                    }
                }
            }
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = suggestions;
            }
        }

        async private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ViewModel.AllItems.Clear();
            ViewModel.ImportDB();
            StringBuilder result = new StringBuilder("");
            result.Capacity = 50;
            String input = (string)sender.Text;
            bool find = false;
            using (var conn = new SQLiteConnection("MyListDB.db"))
            {
                using (var statement = conn.Prepare("SELECT Title, Detail, Date FROM ListItemTable"))
                {
                    while (SQLiteResult.ROW == statement.Step())
                    {
                        string title = (string)statement[0];
                        string detail = (string)statement[1];
                        string date = (string)statement[2];
                        if ((input != "") && (title.Contains(input) || detail.Contains(input) || date.Contains(input))) {
                            result.Append("Title: " + title + " ");
                            result.Append("Description: " + detail + " ");
                            result.Append("Time: " + date + "\n");
                            find = true;
                        }
                    }
                }
            }
            /*
            for (int i = 0; i < ViewModel.AllItems.Count; ++i)
            {
                if ((input != "") && (ViewModel.AllItems[i].Title.Contains(input) || ViewModel.AllItems[i].description.Contains(input) || ViewModel.AllItems[i].date.ToString().Contains(input))) {
                    result.Append("Title: " + ViewModel.AllItems[i].Title + " ");
                    result.Append("Description: " + ViewModel.AllItems[i].description + " ");
                    result.Append("Time: " + ViewModel.AllItems[i].date.ToString() + "\n");
                    find = true;
                }
            }*/
            if (!find) result.Append("No result.");
            
            ContentDialog output = new ContentDialog
            {
                Title = "Result",
                Content = result,
                CloseButtonText = "Ok"
            };
            //sender.Text = "";
            ContentDialogResult _result = await output.ShowAsync();
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem == null) return;
            sender.Text = (string)args.SelectedItem;
        }
    }
}
