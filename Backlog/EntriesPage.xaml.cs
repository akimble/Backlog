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
    /// Interaction logic for EntriesPage.xaml
    /// </summary>
    public partial class EntriesPage : Page
    {

        public EntriesPage(int sublist_ID, string sublist_Name, string backlogParent)
        {
            InitializeComponent();

            // The Textbox at the top should have the name of the sublist clicked on
            SublistTitleTextbox.Text = sublist_Name;
            // The Button in the corner should have the name of the backlog for the sublist
            OneBacklogPageNavButton.Content = backlogParent;

            PopulateFromDB(sublist_ID);
        }

        private void PopulateFromDB(int sublist_ID)
        {
            try
            {
                // Create a new SQLite connection, command, and DataReader
                SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
                sqlConnection1.Open();
                SQLiteCommand myCommand = new SQLiteCommand("SELECT * FROM [entries] WHERE [sublistParent] = " + sublist_ID, sqlConnection1);
                SQLiteDataReader myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    // Create a button
                    TextBox myTextBox = new TextBox();

                    // Set TextBox properties
                    myTextBox.Text = myReader["entryLine"].ToString();
                    myTextBox.Margin = new Thickness(100, 2, 100, 2);

                    // Add created button to the stackpanel
                    entriesStackPanel.Children.Add(myTextBox);
                }

                // Close the connection
                sqlConnection1.Close();
            }
            catch (Exception excep)
            {
                Console.WriteLine(excep.ToString());
            }
        }

        private void OneBacklogPageNavButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
