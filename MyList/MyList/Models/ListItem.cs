using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyList.Models
{
    public class ListItem : INotifyPropertyChanged
    {
        private string id;

        public event PropertyChangedEventHandler PropertyChanged;

        private ImageSource imageSource;
        public ImageSource ImageSource
        {
            get { return this.imageSource; }
            set
            {
                this.imageSource = value;
                INotifyPropertyChanged("ImageSource");
            }
        }

        private string title;
        public string Title {
            get { return this.title; }
            set
            {
                this.title = value;
                INotifyPropertyChanged("Title");
            }
        }
        public string description { get; set; }

        private bool completed;
        public bool Completed
        {
            get { return this.completed; }
            set
            {
                this.completed = value;
                INotifyPropertyChanged("Completed");
            }
        }

        /*private Visibility show_up;
        public Visibility Show_up
        {
            get { return this.show_up; }
            set
            {
                this.show_up = value;
                INotifyPropertyChanged("Show_up");
            }
        }*/

        public void INotifyPropertyChanged(string propertyName)
        {
            if (propertyName != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public DateTime date { get; set; }

        /*public void Checkbox_Click()
        {
            if (this.Completed == true) this.Completed = false;
            else this.Completed = true;
            if (this.Completed)
            {
                this.Show_up = Visibility.Visible;
            }
            else
            {
                this.Show_up = Visibility.Collapsed;
            }
        }*/

        public ListItem(string title, string description, DateTime date, WriteableBitmap imageSource)
        {
            this.id = Guid.NewGuid().ToString();
            this.title = title;
            this.description = description;
            this.completed = false;
            this.date = date;
            /*this.show_up = Visibility.Collapsed;*/
            this.imageSource = imageSource;
        }

        public ListItem(string id, string title, string description, DateTime date, string complete, ImageSource imageSource)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.completed = (complete == "True") ? true : false;
            this.date = date;
            this.imageSource = imageSource;
        }

        public string GetID(){ return id;}
    }

    public class CompletedToVisibilityrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Visibility visibility;
            if ((bool)value == true) visibility = Visibility.Visible;
            else visibility = Visibility.Collapsed;
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool completed;
            if ((Visibility)value == Visibility.Visible) completed = true;
            else completed = false;
            return completed;
        }
    }

    public class NullableBooleanToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool?)
            {
                return (bool)value;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
                return (bool?)value;
            return false;
        }
    }
}
