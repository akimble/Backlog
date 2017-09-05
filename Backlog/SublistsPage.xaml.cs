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
    /// Interaction logic for SublistsPage.xaml
    /// </summary>
    public partial class SublistsPage : Page
    {
        // Global variables here
        private readonly string thisBacklogParent;

        public SublistsPage(string backlogName)
        {
            InitializeComponent();

            thisBacklogParent = backlogName;

            // The Textbox at the top should have the name of the backlog clicked on
            BacklogTitleTextbox.Text = backlogName;
            // Populate the frame components(?) with the sqlite3 database
            PopulateFromDB(backlogName);
        }

        private void HomePageNavButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void NewSublistButton_Click(object sender, RoutedEventArgs e)
        {
            // Default for a new sublist name is "NewSublist" WARNING: duplicates might cause trouble
            // Idea in regards to WARNING in EntriesPage.xaml: direct this method to a custom method that sends a second parameter (backlogParent) to EntriesPage.xaml
            // - and make a second constructor in there that accepts two parameters
            string myValue = "NewSublist";
            this.NavigationService.Navigate(new EntriesPage(-1, myValue, thisBacklogParent));
        }

        private void PopulateFromDB(string backlogName)
        {
            try
            {
                // Create a new SQLite connection, command, and DataReader
                SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
                sqlConnection1.Open();
                SQLiteCommand myCommand = new SQLiteCommand("SELECT * FROM [sublists] WHERE [backlogParent] = '" + backlogName + "'", sqlConnection1);
                SQLiteDataReader myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    // Create a button
                    Button myButton = new Button();
                    // Set properties
                    myButton.Content = myReader["name"].ToString();
                    myButton.Tag = myReader.GetInt16(0);
                    myButton.Click += new RoutedEventHandler(myButton_Click);

                    // Add created button to the dockpanel
                    sublistStackPanel.Children.Add(myButton);
                }

                // Close the connection
                sqlConnection1.Close();
            }
            catch (Exception excep)
            {
                Console.WriteLine(excep.ToString());
            }
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Int16 sublist_ID = (Int16)btn.Tag;
            string sublist_Name = btn.Content.ToString();

            this.NavigationService.Navigate(new EntriesPage(sublist_ID, sublist_Name, thisBacklogParent));
        }
    }
}
