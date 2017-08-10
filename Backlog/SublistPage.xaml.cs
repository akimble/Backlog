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
    /// Interaction logic for SublistPage.xaml
    /// </summary>
    public partial class SublistPage : Page
    {
        // Need global variables here
        private readonly string thisSublistName;
        private string thisBacklogParent;

        public SublistPage(string sublistName)
        {
            InitializeComponent();
            // The Textbox in the corner should have the name of the sublist clicked on
            SublistTitleTextbox.Text = sublistName;

            // If the new sublist button was not clicked, start findBacklogParent() WARNING: The newSublist button "goes back" to an empty OneBacklogPage.xaml bc can't get backlogParent
            thisSublistName = sublistName;
            if (sublistName != "NewSublist")
                findBacklogParent();
        }

        private void findBacklogParent()
        {
            try
            {
                // Create a new SQLite connection, command, and DataReader
                SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
                sqlConnection1.Open();
                SQLiteCommand myCommand = new SQLiteCommand("SELECT [backlogParent] FROM [sublists] WHERE [name] = '" + thisSublistName + "'", sqlConnection1);
                SQLiteDataReader myReader = myCommand.ExecuteReader();

                myReader.Read();
                thisBacklogParent = myReader.GetString(0);
                OneBacklogPageNavButton.Content = thisBacklogParent;

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
            SublistFrame.NavigationService.Navigate(new OneBacklogPage(thisBacklogParent));
        }
    }
}
