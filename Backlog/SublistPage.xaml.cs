using System;
using System.Collections.Generic;
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
        public SublistPage()
        {
            InitializeComponent();
        }

        private void OneBacklogPageNavButton_Click(object sender, RoutedEventArgs e)
        {
            SublistFrame.NavigationService.Navigate(new OneBacklogPage());
        }
    }
}
