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
                SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
                sqlConnection1.Open();
                SQLiteCommand myCommand = new SQLiteCommand("SELECT * FROM backlogs", sqlConnection1);
                SQLiteDataReader myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    // Create a button
                    Button myButton = new Button();

                    // Set properties
                    myButton.Content = myReader["name"].ToString();
                    myButton.Click += new RoutedEventHandler(myButton_Click);
                    myButton.Margin = new Thickness(100,2,100,2);

                    // Add created button to the stackpanel
                    BacklogsStackPanel.Children.Add(myButton);
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
                    SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
                    sqlConnection1.Open();
                    SQLiteCommand myCommand = new SQLiteCommand("SELECT * FROM [backlogs] WHERE [name] = @param", sqlConnection1);
                    myCommand.Parameters.Add(new SQLiteParameter("@param", win.Title.ToString()));
                    SQLiteDataReader myReader = myCommand.ExecuteReader();

                    // Create a Button for the newly inserted row
                    Button myButton = new Button();

                    // Set Button properties
                    myReader.Read();
                    myButton.Content = myReader["name"].ToString();
                    myButton.Click += new RoutedEventHandler(myButton_Click);
                    myButton.Margin = new Thickness(100, 2, 100, 2);
                    myReader.Close();

                    //// Add created DockPanel to the StackPanel
                    //DockPanel entriesDockPanel = createSublistDockPanel(myButton);
                    //sublistStackPanel.Children.Add(entriesDockPanel);

                    BacklogsStackPanel.Children.Add(myButton);

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
