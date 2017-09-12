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

            // Save any changes made to BacklogTitleTextbox
            ImplementTitleTextBox(backlogName);

            // Save backlogParent for NewSublistWindow to inherit
            NewSublistButton.Tag = backlogName;

            PopulateFromDB(backlogName);
        }

        private void ImplementTitleTextBox(string backlogName)
        {
            BacklogTitleTextbox.Tag = backlogName;
            BacklogTitleTextbox.LostKeyboardFocus += BacklogTitleTextbox_LostKeyboardFocus;
        }

        private void BacklogTitleTextbox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
            {
                // Create a new SQLite connection, command, and parameter
                SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
                sqlConnection1.Open();
                SQLiteCommand myCommand = new SQLiteCommand("UPDATE [backlogs] SET [name] = @param WHERE [name] = '" + BacklogTitleTextbox.Tag + "'", sqlConnection1);
                myCommand.Parameters.Add(new SQLiteParameter("@param", BacklogTitleTextbox.Text));
                myCommand.ExecuteNonQuery();

                // Reset Tag for BacklogTitleTextbox
                BacklogTitleTextbox.Tag = BacklogTitleTextbox.Text;

                // Close the connection
                sqlConnection1.Close();
            }
            catch (SQLiteException excep)
            {
                if (excep.ErrorCode == 19)
                    MessageBox.Show("Cannot be the same name as another Backlog.");
            }
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
                    myTags.Add("sublists_id", myReader["id"]);
                    myTags.Add("sublists_backlogParent", myReader["backlogParent"]);

                    // Add created DockPanel to the StackPanel
                    DockPanel entriesDockPanel = createSublistDockPanel(myButton);
                    sublistStackPanel.Children.Add(entriesDockPanel);
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

        private DockPanel createSublistDockPanel(Button myButton)
        {
            // Create a DockPanel, delete Button, and MultiTag object
            DockPanel sublistsDockPanel = new DockPanel();
            Button deleteButton = new Button();
            MultiTag myTags = (MultiTag)myButton.Tag;
            MultiTag myDeleteTags = new MultiTag();

            // Set Button properties
            deleteButton.Content = "  X  ";
            deleteButton.Background = MyColors.DarkGrey();
            deleteButton.Foreground = MyColors.Garnet();
            deleteButton.Tag = myDeleteTags;
            deleteButton.Click += DeleteButton_Click;
            // Set myDeleteTags properties
            myDeleteTags.Add("sublists_id", myTags.Get("sublists_id"));
            myDeleteTags.Add("myDockPanel", sublistsDockPanel);

            // Set Button style as the ToolBar's Button style (no border)
            Style toolbarButtonStyle = (Style)FindResource(ToolBar.ButtonStyleKey);
            deleteButton.Style = toolbarButtonStyle;

            // Add TextBox and Button to DockPanel
            deleteButton.SetValue(DockPanel.DockProperty, Dock.Right);
            myButton.SetValue(DockPanel.DockProperty, Dock.Left);
            sublistsDockPanel.Children.Add(deleteButton);
            sublistsDockPanel.Children.Add(myButton);

            return sublistsDockPanel;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Bring up a confirmation box
            var confirmResult = MessageBox.Show("Are you sure you want to delete this sublist?", "Confirm Delete", MessageBoxButton.YesNo);

            // If "Yes" is chosen, delete the sublist from the database
            if (confirmResult == MessageBoxResult.Yes)
            {
                Button deleteButton = sender as Button;
                MultiTag myTags = (MultiTag)deleteButton.Tag;
                int sublist_id = Convert.ToInt32(myTags.Get("sublists_id"));
                DockPanel myDockPanel = (DockPanel)myTags.Get("myDockPanel");

                DeleteFromDB(sublist_id, myDockPanel);

            }
        }

        private void DeleteFromDB(int sublist_id, DockPanel myDockPanel)
        {
            try
            {
                // Create a new SQLite connection, command, and parameter
                SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
                sqlConnection1.Open();
                SQLiteCommand myCommand = new SQLiteCommand("DELETE FROM [sublists] WHERE [id] =" + sublist_id, sqlConnection1);
                myCommand.ExecuteNonQuery();

                // Close the connection
                sqlConnection1.Close();

                sublistStackPanel.Children.Remove(myDockPanel);
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
            int sublist_id = Convert.ToInt32(myTags.Get("sublists_id"));
            string sublist_backlogParent = (string)(myTags.Get("sublists_backlogParent"));

            this.NavigationService.Navigate(new EntriesPage(sublist_id, sublist_Name, sublist_backlogParent));
        }

        private void NewSublistButton_Click(object sender, RoutedEventArgs e)
        {
            Button myButton = sender as Button;
            string backlogParent = myButton.Tag.ToString();

            // Create a new window to input new information about the new Sublist
            NewSublistWindow myWindow = new NewSublistWindow(backlogParent);
            myWindow.Closing += MyWindow_Closing;
            myWindow.ShowDialog();
        }

        private void MyWindow_Closing(object sender, EventArgs e)
        {
            int sublists_ID = getLatestEntries_ID();

            // Obtain the row with the latest id value (i.e. the latest entry)
            SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
            sqlConnection1.Open();
            SQLiteCommand myCommand = new SQLiteCommand("SELECT * FROM [sublists] WHERE [id] = " + sublists_ID, sqlConnection1);
            SQLiteDataReader myReader = myCommand.ExecuteReader();

            // Create a Button for the newly inserted row
            Button myButton = new Button();
            MultiTag myTags = new MultiTag();

            // Set Button properties
            myReader.Read();
            myButton.Content = myReader["name"].ToString();
            myButton.Tag = myTags;
            myButton.Click += new RoutedEventHandler(myButton_Click);
            // Set myTags properties
            myTags.Add("sublists_id", myReader["id"]);
            myTags.Add("sublists_backlogParent", myReader["backlogParent"]);
            myReader.Close();

            // Add created DockPanel to the StackPanel
            DockPanel entriesDockPanel = createSublistDockPanel(myButton);
            sublistStackPanel.Children.Add(entriesDockPanel);

            sqlConnection1.Close();
        }

        private int getLatestEntries_ID()
        {
            int entries_ID = -1;

            // Create a new SQLite connection, command, and DataReader
            SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
            sqlConnection1.Open();
            SQLiteCommand myID = new SQLiteCommand("SELECT [seq] FROM [sqlite_sequence] WHERE [name] = 'sublists'", sqlConnection1);
            SQLiteDataReader myIDReader = myID.ExecuteReader();

            // Obtain the latest auto-incremented id for the entries table
            myIDReader.Read();
            entries_ID = Convert.ToInt32(myIDReader["seq"]);
            myIDReader.Close();

            return entries_ID;
        }
    }
}
