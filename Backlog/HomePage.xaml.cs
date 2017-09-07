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
            string x = "";
            this.NavigationService.Navigate(new SublistsPage(x));
        }
    }
}
