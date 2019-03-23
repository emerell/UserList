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
using System.Windows.Shapes;

namespace UserList
{
    /// <summary>
    /// Interaction logic for PersonsWindow.xaml
    /// </summary>
    public partial class PersonsWindow : Window
    {
        public PersonsWindow()
        {
            InitializeComponent();
            DataContext = new PersonsViewModel(
                delegate () { Dispatcher.Invoke(PersonsDataGrid.Items.Refresh); },
                delegate (string s) { Dispatcher.Invoke(() => { return UserShortTextBlock.Text = s; }); });
        }
    }
}
