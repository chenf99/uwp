using MyList.Models;
using SQLite.Net.Platform.WinRT;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyList
{
    public static class AppDatabase
    {

        /*
        //定义只读SQL语句常量
        public readonly static string DB_NAME = "MyListDB.db";
        public readonly static string TABLE_NAME = "ListItemTable";
        public readonly static string SQL_CREATE_TABLE = "CREATE TABLE IF NOT EXIST " + TABLE_NAME + " (Id TEXT, Title TEXT, Detail TEXT, Date TEXT)";
        public readonly static string SQL_QUERY_VALUE = "SELECT Title, Detail, Date FROM " + TABLE_NAME + " WHERE Id = ?";
        */
        
        public static void LoadDatabase()
        {
            //连接数据库，若不存在则创建
            var conn = new SQLiteConnection("MyListDB.db");
            string sql = @"CREATE TABLE IF NOT EXISTS
                                  ListItemTable (Id TEXT PRIMARY KEY NOT NULL,
                                                 Title TEXT,
                                                 Detail TEXT,
                                                 Date TEXT,
                                                 Complete TEXT,
                                                 Image BLOB
                                                 );";
            using (var statement = conn.Prepare(sql))
            {
                statement.Step();
            }
        }
    }
}
