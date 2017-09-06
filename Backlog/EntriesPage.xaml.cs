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
            Create_NewEntryButton();
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
                    // Create a TextBox
                    TextBox myTextBox = new TextBox();

                    // Set TextBox properties
                    myTextBox.Text = myReader["entryLine"].ToString();
                    myTextBox.Margin = new Thickness(10, 2, 10, 2);
                    myTextBox.Tag = myReader["id"];

                    // Event if the keyboard loses focus on the TextBox
                    myTextBox.LostKeyboardFocus += MyTextBox_LostKeyboardFocus;

                    // Add created TextBox to the stackpanel
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

        private void Create_NewEntryButton()
        {
            Button newEntryButton = new Button();

            // Set Button properties
            newEntryButton.Content = " + ";
            newEntryButton.Background = (Brush)(new BrushConverter().ConvertFromString("#FF272525"));
            newEntryButton.Foreground = (Brush)(new BrushConverter().ConvertFromString("#FF24A836"));
            newEntryButton.FontWeight = FontWeights.Bold;
            newEntryButton.FontSize = 26;
            newEntryButton.Margin = new Thickness(10, 2, 10, 2);

            // Add created Button to the stackpanel
            entriesStackPanel.Children.Add(newEntryButton);
        }

        private void MyTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox myTextBox = sender as TextBox;

            InsertIntoDB(Convert.ToInt32(myTextBox.Tag), myTextBox.Text);
        }

        private void OneBacklogPageNavButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        // Update the (changed) text into the DB for the entry with the given id
        private void InsertIntoDB(int id, string insertThisText)
        {
            try
            {
                // Create a new SQLite connection, command, and parameter
                SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
                sqlConnection1.Open();
                SQLiteCommand myCommand = new SQLiteCommand("UPDATE [entries] SET [entryLine] = @param WHERE [id] =" + id, sqlConnection1);
                myCommand.Parameters.Add(new SQLiteParameter("@param", insertThisText));
                myCommand.ExecuteNonQuery();

                // Close the connection
                sqlConnection1.Close();
            }
            catch (Exception excep)
            {
                Console.WriteLine(excep.ToString());
            }
        }
    }
}
