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
        public SublistsPage(string backlogName)
        {
            InitializeComponent();

            // The Textbox at the top should have the name of the backlog clicked on
            BacklogTitleTextbox.Text = backlogName;

            PopulateFromDB(backlogName);
        }

        private void HomePageNavButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new HomePage());
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
                    // Create a Button and then a MultiTag object for it
                    Button myButton = new Button();
                    MultiTag myTags = new MultiTag();

                    // Set Button properties
                    myButton.Content = myReader["name"].ToString();
                    myButton.Tag = myTags;
                    myButton.Click += new RoutedEventHandler(myButton_Click);
                    // Set myTags properties
                    myTags.Add("sublist_id", myReader["id"]);
                    myTags.Add("sublist_backlogParent", myReader["backlogParent"]);

                    // Add created button to the dockpanel
                    sublistStackPanel.Children.Add(myButton);
                }

                // Close the reader and connection
                myReader.Close();
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
            string sublist_Name = btn.Content.ToString();

            MultiTag myTags = (MultiTag)btn.Tag;
            int sublist_id = Convert.ToInt32(myTags.Get("sublist_id"));
            string sublist_backlogParent = (string)(myTags.Get("sublist_backlogParent"));

            this.NavigationService.Navigate(new EntriesPage(sublist_id, sublist_Name, sublist_backlogParent));
        }
    }
}
