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
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private readonly string connectionString = @"Data Source=" + System.IO.Directory.GetCurrentDirectory() + "\\backlogs.db";

        public HomePage()
        {
            InitializeComponent();
            PopulateFromDB();
        }

        private void PopulateFromDB()
        {
            try
            {
                // Create a new SQLite connection, command, and DataReader
                SQLiteConnection sqlConnection1 = new SQLiteConnection(connectionString);
                sqlConnection1.Open();
                SQLiteCommand myCommand = new SQLiteCommand("SELECT * FROM backlogs", sqlConnection1);
                SQLiteDataReader myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    // Create a button
                    Button myButton = new Button();

                    // Set properties
                    myButton.Style = Application.Current.FindResource("BacklogButton") as Style;
                    myButton.Content = myReader["name"].ToString();
                    myButton.Tag = myReader["summary"].ToString();
                    myButton.Click += new RoutedEventHandler(myButton_Click);

                    // Add created DockPanel to the StackPanel
                    DockPanel myDockPanel = createBacklogDockPanel(myButton);
                    BacklogsStackPanel.Children.Add(myDockPanel);
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

        private DockPanel createBacklogDockPanel(Button myButton)
        {
            // Create a DockPanel, delete Button, and MultiTag object
            DockPanel myDockPanel = new DockPanel();
            Button deleteButton = new Button();
            MultiTag myDeleteTags = new MultiTag();

            // Set Button properties
            deleteButton.Style = Application.Current.FindResource("DeleteButton") as Style;
            deleteButton.Tag = myDeleteTags;
            deleteButton.Click += DeleteButton_Click;
            // Set myDeleteTags properties
            myDeleteTags.Add("name", myButton.Content);
            myDeleteTags.Add("myDockPanel", myDockPanel);

            // Add both buttons to DockPanel
            deleteButton.SetValue(DockPanel.DockProperty, Dock.Right);
            myButton.SetValue(DockPanel.DockProperty, Dock.Left);
            myDockPanel.Children.Add(deleteButton);
            myDockPanel.Children.Add(myButton);

            return myDockPanel;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Bring up a confirmation box
            var confirmResult = MessageBox.Show("Are you sure you want to delete this backlog?", "Confirm Delete", MessageBoxButton.YesNo);

            // If "Yes" is chosen, delete the backlog from the database
            if (confirmResult == MessageBoxResult.Yes)
            {
                Button deleteButton = sender as Button;
                MultiTag myTags = (MultiTag)deleteButton.Tag;
                string name = myTags.Get("name").ToString();
                DockPanel myDockPanel = (DockPanel)myTags.Get("myDockPanel");

                DeleteFromDB(name, myDockPanel);
            }
        }

        private void DeleteFromDB(string name, DockPanel myDockPanel)
        {
            try
            {
                // Create a new SQLite connection, command, and parameter
                SQLiteConnection sqlConnection1 = new SQLiteConnection(connectionString);
                sqlConnection1.Open();
                SQLiteCommand myCommand1 = new SQLiteCommand("DELETE FROM [entries] WHERE [sublistParent] = (SELECT [id] FROM [sublists] WHERE [backlogParent] = '" + name + "')", sqlConnection1);
                SQLiteCommand myCommand2 = new SQLiteCommand("DELETE FROM [sublists] WHERE [backlogParent] = '" + name + "'", sqlConnection1);
                SQLiteCommand myCommand3 = new SQLiteCommand("DELETE FROM [backlogs] WHERE [name] = '" + name + "'", sqlConnection1);
                myCommand1.ExecuteNonQuery();
                myCommand2.ExecuteNonQuery();
                myCommand3.ExecuteNonQuery();

                // Close the connection
                sqlConnection1.Close();

                BacklogsStackPanel.Children.Remove(myDockPanel);
            }
            catch (Exception excep)
            {
                Console.WriteLine(excep.ToString());
            }
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            // Cast the sender object to Button and convert the Content property to string
            string myValue = ((Button)sender).Content.ToString();
            this.NavigationService.Navigate(new SublistsPage(myValue));
        }

        private void createBacklogButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a new window to input new information about the new Backlog
            NewBacklogWindow myWindow = new NewBacklogWindow();
            myWindow.Closing += MyWindow_Closing;
            myWindow.ShowDialog();
        }

        private void MyWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NewBacklogWindow win = sender as NewBacklogWindow;

            // If the Tag is TRUE (no UNIQUE constraints errors) then proceed as normal
            if ((Boolean)win.Tag)
            {
                try
                {
                    // Obtain the newly created row
                    SQLiteConnection sqlConnection1 = new SQLiteConnection(connectionString);
                    sqlConnection1.Open();
                    SQLiteCommand myCommand = new SQLiteCommand("SELECT * FROM [backlogs] WHERE [name] = @param", sqlConnection1);
                    myCommand.Parameters.Add(new SQLiteParameter("@param", win.Title.ToString()));
                    SQLiteDataReader myReader = myCommand.ExecuteReader();

                    // Create a Button for the newly inserted row
                    Button myButton = new Button();

                    // Set Button properties
                    myReader.Read();
                    myButton.Style = Application.Current.FindResource("BacklogButton") as Style;
                    myButton.Content = myReader["name"].ToString();
                    myButton.Tag = myReader["summary"].ToString();
                    myButton.Click += new RoutedEventHandler(myButton_Click);
                    myReader.Close();

                    // Add created DockPanel to the StackPanel
                    DockPanel myDockPanel = createBacklogDockPanel(myButton);
                    BacklogsStackPanel.Children.Add(myDockPanel);

                    sqlConnection1.Close();
                }
                catch (Exception excep)
                {
                    Console.WriteLine(excep.ToString());
                }
            }
        }
    }
}
