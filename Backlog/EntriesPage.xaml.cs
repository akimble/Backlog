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

            // Go back to SublistPage with the right parameter
            OneBacklogPageNavButton.Tag = backlogParent;

            // The Textbox at the top should have the name of the sublist clicked on
            SublistTitleTextBox.Text = sublist_Name;

            // Save any changes made to SublistTitleTextBox
            ImplementTitleTextBox(sublist_ID);

            // The Button in the corner should have the name of the backlog for the sublist
            OneBacklogPageNavButton.Content = backlogParent;

            // Fill the page with entries from the database
            PopulateFromDB(sublist_ID);
            Create_NewEntryButton(sublist_ID);
        }

        private void ImplementTitleTextBox(int sublist_ID)
        {
            SublistTitleTextBox.Tag = sublist_ID;
            SublistTitleTextBox.LostKeyboardFocus += SublistTitleTextbox_LostKeyboardFocus;
        }

        private void SublistTitleTextbox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
            {
                // Create a new SQLite connection, command, and parameter
                SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
                sqlConnection1.Open();
                SQLiteCommand myCommand = new SQLiteCommand("UPDATE [sublists] SET [name] = @param WHERE [id] = " + SublistTitleTextBox.Tag, sqlConnection1);
                myCommand.Parameters.Add(new SQLiteParameter("@param", SublistTitleTextBox.Text));
                myCommand.ExecuteNonQuery();

                // Close the connection
                sqlConnection1.Close();
            }
            catch (Exception excep)
            {
                Console.WriteLine(excep.ToString());
            }
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

                    // Add created DockPanel to the StackPanel
                    DockPanel entriesDockPanel = createEntryDockPanel(myTextBox);
                    entriesStackPanel.Children.Add(entriesDockPanel);
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

        private DockPanel createEntryDockPanel(TextBox myTextBox)
        {
            // Create a DockPanel, delete Button, and MultiTag object
            DockPanel entriesDockPanel = new DockPanel();
            Button deleteButton = new Button();
            MultiTag myTags = new MultiTag();

            // Set Button properties
            deleteButton.Content = "  X  ";
            deleteButton.Background = (Brush)(new BrushConverter().ConvertFromString("#FF272525"));
            deleteButton.Foreground = (Brush)(new BrushConverter().ConvertFromString("#bc010b"));
            deleteButton.Tag = myTags;
            deleteButton.Click += DeleteButton_Click;
            // Set myTags properties
            myTags.Add("entries_id", myTextBox.Tag);
            myTags.Add("myDockPanel", entriesDockPanel);

            // Set Button style as the ToolBar's Button style (no border)
            Style toolbarButtonStyle = (Style)FindResource(ToolBar.ButtonStyleKey);
            deleteButton.Style = toolbarButtonStyle;

            // Add TextBox and Button to DockPanel
            deleteButton.SetValue(DockPanel.DockProperty, Dock.Right);
            myTextBox.SetValue(DockPanel.DockProperty, Dock.Left);
            entriesDockPanel.Children.Add(deleteButton);
            entriesDockPanel.Children.Add(myTextBox);

            return entriesDockPanel;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Bring up a confirmation box
            var confirmResult = MessageBox.Show("Are you sure you want to delete this entry?", "Confirm Delete", MessageBoxButton.YesNo);

            // If "Yes" is chosen, delete the entry from the database
            if (confirmResult == MessageBoxResult.Yes)
            {
                Button deleteButton = sender as Button;
                MultiTag myTags = (MultiTag)deleteButton.Tag;
                int entries_id = Convert.ToInt32(myTags.Get("entries_id"));
                DockPanel myDockPanel = (DockPanel)myTags.Get("myDockPanel");

                DeleteFromDB(entries_id, myDockPanel);

            }
        }

        private void Create_NewEntryButton(int sublist_ID)
        {
            Button newEntryButton = new Button();

            // Set Button properties
            newEntryButton.Content = " + ";
            newEntryButton.Background = (Brush)(new BrushConverter().ConvertFromString("#FF272525"));
            newEntryButton.Foreground = (Brush)(new BrushConverter().ConvertFromString("#FF24A836"));
            newEntryButton.FontWeight = FontWeights.Bold;
            newEntryButton.FontSize = 26;
            newEntryButton.Margin = new Thickness(10, 2, 10, 2);
            newEntryButton.Click += NewEntryButton_Click;
            newEntryButton.Tag = sublist_ID;

            // Add created Button to the stackpanel
            entriesStackPanel.Children.Add(newEntryButton);
        }

        private void NewEntryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button newEntryButton = sender as Button;
                int sublist_ID = (int)newEntryButton.Tag;
                
                // Insert a new entry. During insertion, the id is autoincremented by 1 for the new entry
                InsertIntoDB(sublist_ID, "");

                // Create a new SQLite connection, command, and DataReader
                SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
                sqlConnection1.Open();
                SQLiteCommand myID = new SQLiteCommand("SELECT [seq] FROM [sqlite_sequence] WHERE [name] = 'entries'", sqlConnection1);
                SQLiteDataReader myIDReader = myID.ExecuteReader();

                // Obtain the latest auto-incremented id for the entries table
                myIDReader.Read();
                int entries_ID = Convert.ToInt32(myIDReader["seq"]);
                myIDReader.Close();

                // Obtain the row with the latest id value (i.e. the latest entry)
                SQLiteCommand myCommand = new SQLiteCommand("SELECT * FROM [entries] WHERE [id] = " + entries_ID, sqlConnection1);
                SQLiteDataReader myReader = myCommand.ExecuteReader();

                // Create a TextBox for the newly inserted row
                TextBox myTextBox = new TextBox();

                // Set TextBox properties
                myReader.Read();
                myTextBox.Text = myReader["entryLine"].ToString();
                myTextBox.Margin = new Thickness(10, 2, 10, 2);
                myTextBox.Tag = myReader["id"];
                myReader.Close();

                // Event if the keyboard loses focus on the TextBox
                myTextBox.LostKeyboardFocus += MyTextBox_LostKeyboardFocus;

                // Remove newEntryButton from the stackpanel
                entriesStackPanel.Children.Remove(newEntryButton);
                // Add newly created DockPanel (of a TextBox and delete Button) to the StackPanel
                DockPanel entriesDockPanel = createEntryDockPanel(myTextBox);
                entriesStackPanel.Children.Add(entriesDockPanel);
                // Create the newEntryButton again so it is placed at the bottom of the stackpanel below the new entry
                entriesStackPanel.Children.Add(newEntryButton);

                // Close the connection
                sqlConnection1.Close();
            }
            catch (Exception excep)
            {
                Console.WriteLine(excep.ToString());
            }
        }

        private void MyTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox myTextBox = sender as TextBox;

            UpdateIntoDB(Convert.ToInt32(myTextBox.Tag), myTextBox.Text);
        }

        private void OneBacklogPageNavButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SublistsPage((string)OneBacklogPageNavButton.Tag));
        }

        private void UpdateIntoDB(int entries_id, string updateThisText)
        {
            try
            {
                // Create a new SQLite connection, command, and parameter
                SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
                sqlConnection1.Open();
                SQLiteCommand myCommand = new SQLiteCommand("UPDATE [entries] SET [entryLine] = @param WHERE [id] =" + entries_id, sqlConnection1);
                myCommand.Parameters.Add(new SQLiteParameter("@param", updateThisText));
                myCommand.ExecuteNonQuery();

                // Close the connection
                sqlConnection1.Close();
            }
            catch (Exception excep)
            {
                Console.WriteLine(excep.ToString());
            }
        }

        private void InsertIntoDB(int sublist_ID, string insertThisText)
        {
            try
            {
                // Create a new SQLite connection, command, and parameter
                SQLiteConnection sqlConnection2 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
                sqlConnection2.Open();
                SQLiteCommand myCommand = new SQLiteCommand("INSERT INTO entries (entryLine, sublistParent) VALUES (@param, " + sublist_ID + ")", sqlConnection2);
                myCommand.Parameters.Add(new SQLiteParameter("@param", insertThisText));
                myCommand.ExecuteNonQuery();

                // Close the connection
                sqlConnection2.Close();
            }
            catch (Exception excep)
            {
                Console.WriteLine(excep.ToString());
            }
        }

        private void DeleteFromDB(int entries_id, DockPanel myDockPanel)
        {
            try
            {
                // Create a new SQLite connection, command, and parameter
                SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
                sqlConnection1.Open();
                SQLiteCommand myCommand = new SQLiteCommand("DELETE FROM [entries] WHERE [id] =" + entries_id, sqlConnection1);
                myCommand.ExecuteNonQuery();

                // Close the connection
                sqlConnection1.Close();

                entriesStackPanel.Children.Remove(myDockPanel);
            }
            catch (Exception excep)
            {
                Console.WriteLine(excep.ToString());
            }
        }
    }
}
