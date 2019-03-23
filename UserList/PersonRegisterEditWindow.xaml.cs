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
    /// Interaction logic for PersonRegisterEditWindow.xaml
    /// </summary>
    public partial class PersonRegisterEditWindow : Window
    {
        public PersonRegisterEditWindow(Action<Person> onRegisterAction, Person person = null)
        {
            InitializeComponent();
            DataContext = new PersonRegisterEditViewModel(person, delegate (Person newPerson)
            {
                Close();
                onRegisterAction(newPerson);
            });
        }
    }
}
