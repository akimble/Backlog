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
using System.Windows.Shapes;

namespace Backlog
{
    /// <summary>
    /// Interaction logic for NewSublistWindow.xaml
    /// </summary>
    public partial class NewSublistWindow : Window
    {
        public NewSublistWindow(string backlogParent)
        {
            InitializeComponent();
            Save.Tag = backlogParent;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Button myButton = sender as Button;
            string backlogParent = myButton.Tag.ToString();

            InsertIntoDB(NewSublistName.Text, backlogParent, NewSublistSummary.Text);
        }

        private void InsertIntoDB(string name, string backlogParent, string summary)
        {
            try
            {
                // Create a new SQLite connection, command, and parameter
                SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
                sqlConnection1.Open();
                SQLiteCommand myCommand = new SQLiteCommand("INSERT INTO sublists (name, backlogParent, summary) VALUES (@param1, '" + backlogParent + "', @param2)", sqlConnection1);
                myCommand.Parameters.Add(new SQLiteParameter("@param1", name));
                myCommand.Parameters.Add(new SQLiteParameter("@param2", summary));
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
