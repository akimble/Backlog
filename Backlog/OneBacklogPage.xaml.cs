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
    /// Interaction logic for OneBacklogPage.xaml
    /// </summary>
    public partial class OneBacklogPage : Page
    {
        // Need global variables here
        private string thisBacklogParent;

        public OneBacklogPage(string backlogName)
        {
            InitializeComponent();
            thisBacklogParent = backlogName;
            // The Textbox in the corner should have the name of the backlog clicked on
            BacklogTitleTextbox.Text = backlogName;
            // Populate the frame components(?) with the sqlite3 database
            populateWithDatabase(backlogName);
        }

        private void BacklogsPageNavButton_Click(object sender, RoutedEventArgs e)
        {
            OneBacklogFrame.NavigationService.Navigate(new BacklogsPage());
        }

        private void NewSublistButton_Click(object sender, RoutedEventArgs e)
        {
            // Default for a new sublist name is "NewSublist" WARNING: duplicates might cause trouble
            // Idea in regards to WARNING in SublistPage.xaml: direct this method to a custom method that sends a second parameter (backlogParent) to SublistPage.xaml
            // - and make a second constructor in there that accepts two parameters
            string myValue = "NewSublist";
            OneBacklogFrame.NavigationService.Navigate(new SublistPage(myValue, thisBacklogParent));
        }

        private void populateWithDatabase(string backlogName)
        {
            try
            {
                // Create a new SQLite connection, command, and DataReader
                SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
                sqlConnection1.Open();
                Console.WriteLine("TESSSSSSSSSSSSSSSSSSSSSSSTING:" + backlogName);
                SQLiteCommand myCommand = new SQLiteCommand("SELECT * FROM [sublists] WHERE [backlogParent] = '" + backlogName + "'", sqlConnection1);
                SQLiteDataReader myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    // Create a button
                    Button myButton = new Button();
                    // Set properties
                    myButton.Content = myReader["name"].ToString();
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
            // Cast the sender object to Button and convert the Content property to string
            string mySublist = ((Button)sender).Content.ToString();
            OneBacklogFrame.NavigationService.Navigate(new SublistPage(mySublist, thisBacklogParent));
        }
    }
}
