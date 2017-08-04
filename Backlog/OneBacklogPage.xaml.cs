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
    /// Interaction logic for OneBacklogPage.xaml
    /// </summary>
    public partial class OneBacklogPage : Page
    {
        public OneBacklogPage()
        {
            InitializeComponent();
        }

        private void BacklogsPageNavButton_Click(object sender, RoutedEventArgs e)
        {
            OneBacklogFrame.NavigationService.Navigate(new BacklogsPage());
        }
    }
}
