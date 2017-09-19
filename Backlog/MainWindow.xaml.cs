using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Backlog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // If the DB doesn't exist, create it
            checkDBExistence();

            // Load the HomePage immediately
            myFrame.NavigationService.Navigate(new HomePage());
        }

        private void checkDBExistence()
        {
            if (!System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\backlogs.db"))
            {
                string connectionString = @"Data Source=" + System.IO.Directory.GetCurrentDirectory() + "\\backlogs.db";

                using (var sqlConnection1 = new SQLiteConnection(connectionString))
                {
                    Console.WriteLine("IN USING KEYWORD");
                    sqlConnection1.Open();
                    SQLiteCommand createBacklogsTable = new SQLiteCommand("CREATE TABLE \"backlogs\" ( `name` TEXT, `summary` TEXT, PRIMARY KEY(`name`) )", sqlConnection1);
                    SQLiteCommand createSublistsTable = new SQLiteCommand("CREATE TABLE \"sublists\" ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT, `backlogParent` TEXT NOT NULL, `summary` TEXT, FOREIGN KEY(`backlogParent`) REFERENCES `backlogs`(`name`) )", sqlConnection1);
                    SQLiteCommand createEntriesTable = new SQLiteCommand("CREATE TABLE \"entries\" ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `entryLine` TEXT NOT NULL, `marked` BOOLEAN NOT NULL DEFAULT 0 CHECK(marked IN ( 0 , 1 )), `sublistParent` INTEGER NOT NULL, FOREIGN KEY(`sublistParent`) REFERENCES `sublists`(`ID`) )", sqlConnection1);
                    createBacklogsTable.ExecuteNonQuery();
                    createSublistsTable.ExecuteNonQuery();
                    createEntriesTable.ExecuteNonQuery();
                }
            }
        }
    }
}
