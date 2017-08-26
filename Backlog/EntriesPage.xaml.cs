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
        // Global variables here
        private readonly string thisBacklogParent;

        public EntriesPage(string sublistName, string backlogParent)
        {
            InitializeComponent();

            thisBacklogParent = backlogParent;

            // The Textbox at the top should have the name of the sublist clicked on
            SublistTitleTextbox.Text = sublistName;
            // The Button in the corner should have the name of the backlog for the sublist
            OneBacklogPageNavButton.Content = backlogParent;
        }
         
        private void OneBacklogPageNavButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
