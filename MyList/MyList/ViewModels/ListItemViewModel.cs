using MyList.Models;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyList.ViewModels
{
    public class ListItemViewModel
    {
        private ObservableCollection<Models.ListItem> allItems = new ObservableCollection<Models.ListItem>();
        public ObservableCollection<Models.ListItem> AllItems { get { return this.allItems; } }
        public Models.ListItem selectedItem;

        private static ListItemViewModel instance;

        async public void ImportDB()
        {
            allItems.Clear();
            ListItem item = null;
            using (var conn = new SQLiteConnection("MyListDB.db"))
            {
                using (var statement = conn.Prepare("SELECT Id, Title, Detail, Date, Complete, Image FROM ListItemTable "))
                {
                    while (SQLiteResult.ROW == statement.Step())
                    {
                        string id = (string)statement[0];
                        string title = (string)statement[1];
                        string detail = (string)statement[2];
                        DateTime date = Convert.ToDateTime((string)statement[3]);
                        string complete = (string)statement[4];
                        byte[] image = (byte[])statement[5];
                        ImageSource source = await ImageTools.SaveToImageSource(image);
                        item = new ListItem(id, title, detail, date, complete, source);
                        allItems.Add(item);
                    }
                }
            }
        }
        private ListItemViewModel() { ImportDB(); }
        public static ListItemViewModel GetViewModel()
        {
            if (instance == null)
            {
                instance = new ListItemViewModel();
            }
            return instance;
        }

        async public void AddListItem(string title, string description, DateTime date, WriteableBitmap imageSource)
        {
            ListItem item = new Models.ListItem(title, description, date, imageSource);
            this.allItems.Add(item);
            using (var conn = new SQLiteConnection("MyListDB.db"))
            {
                using (var statement = conn.Prepare("INSERT INTO ListItemTable (Id, Title, Detail, Date, Complete, Image) VALUES (?, ?, ?, ?, ?, ?)"))
                {
                    statement.Bind(1, item.GetID());
                    statement.Bind(2, title);
                    statement.Bind(3, description);
                    statement.Bind(4, date.ToString());
                    statement.Bind(5, item.Completed.ToString());
                    statement.Bind(6, await ImageTools.SaveToBytesAsync(item.ImageSource));
                    statement.Step();
                }
            }
        }

        public void RemoveListItem(string id)
        {
            for (int i = 0; i < allItems.Count; ++i)
            {
                if (allItems[i].GetID() == id)
                {
                    using (var conn = new SQLiteConnection("MyListDB.db"))
                    {
                        using (var statement = conn.Prepare("DELETE FROM ListItemTable WHERE Id = ?"))
                        {
                            statement.Bind(1, id);
                            statement.Step();
                        }
                    }
                    allItems.RemoveAt(i);break;
                }
            }
            selectedItem = null;
        }

        public void RemoveAllListItem()
        {
            int count = allItems.Count;
            for (int i = 0; i < count; ++i)
            {
                RemoveListItem(allItems[0].GetID());
            }
            //allItems.Clear();
        }

        async public void UpdateListItem(string id, string title, string description, DateTime date, WriteableBitmap imageSource)
        {
            for (int i = 0; i < allItems.Count; ++i)
            {
                if (allItems[i].GetID() == id)
                {
                    allItems[i].Title = title;
                    allItems[i].description = description;
                    allItems[i].date = date;
                    allItems[i].ImageSource = imageSource;
                    using (var conn = new SQLiteConnection("MyListDB.db"))
                    {
                        using (var statement = conn.Prepare("UPDATE ListItemTable SET Title = ?, Detail = ?, Date = ?, Complete = ?, Image = ? WHERE Id = ?"))
                        {
                            statement.Bind(1, title);
                            statement.Bind(2, description);
                            statement.Bind(3, date.ToString());
                            statement.Bind(4, allItems[i].Completed.ToString());
                            statement.Bind(5, await ImageTools.SaveToBytesAsync(allItems[i].ImageSource));
                            statement.Bind(6, id);
                            statement.Step();
                        }
                    }
                }
            }
            selectedItem = null;
        }

        async public Task UpdateListItem(string id)           //该方法用于实时更新complete属性
        {
            for (int i = 0; i < allItems.Count; ++i)
            {
                if (allItems[i].GetID() == id)
                {
                    using (var conn = new SQLiteConnection("MyListDB.db"))
                    {
                        using ( var statement = conn.Prepare("UPDATE ListItemTable SET Complete = ? WHERE Id = ?"))
                        {
                            statement.Bind(1, allItems[i].Completed.ToString());
                            statement.Bind(2, id);
                            statement.Step();
                        }
                    }
                }
            }
            await Task.Delay(0);
        }
    }
}
