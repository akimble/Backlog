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
    /// Interaction logic for NewBacklogWindow.xaml
    /// </summary>
    public partial class NewBacklogWindow : Window
    {
        public NewBacklogWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            InsertIntoDB(NewBacklogName.Text, NewBacklogSummary.Text);
        }

        private void InsertIntoDB(string name, string summary)
        {
            try
            {
                // Create a new SQLite connection, command, and parameters
                SQLiteConnection sqlConnection1 = new SQLiteConnection("Data Source=C:\\Users\\Andrew\\Documents\\Visual Studio 2017\\Projects\\Backlog\\backlogs.db;Version=3;");
                sqlConnection1.Open();
                SQLiteCommand myCommand = new SQLiteCommand("INSERT INTO backlogs (name, summary) VALUES (@param1, @param2)", sqlConnection1);
                myCommand.Parameters.Add(new SQLiteParameter("@param1", name));
                myCommand.Parameters.Add(new SQLiteParameter("@param2", summary));
                myCommand.ExecuteNonQuery();

                // If everything went ok (no UNIQUE constraints errors), then set the Tag to TRUE
                this.Tag = true;
                this.Title = name;

                // Close the connection
                sqlConnection1.Close();
            }
            catch (SQLiteException excep)
            {
                if (excep.ErrorCode == 19)
                {
                    this.Tag = false;
                    MessageBox.Show("Cannot be the same name as another Backlog.");
                }
            }
        }
    }
}
