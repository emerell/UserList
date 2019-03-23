using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UserList
{
    public class PersonRegisterEditViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _surname;
        private string _email;
        private DateTime _birthDate = DateTime.Today;
        private RelayCommand _signInCommand;
        private Action<Person> _onRegisterAction;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Surname
        {
            get
            {
                return _surname;
            }
            set
            {
                _surname = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public DateTime BirthDate
        {
            get
            {
                return _birthDate;
            }
            set
            {
                _birthDate = value;
                OnPropertyChanged();
            }
        }

        //this is used to keep track of empty birthday field
        public string BirthDateText { get; set; }

        public RelayCommand RegisterCommand
        {
            get
            {
                return _signInCommand ?? (_signInCommand = new RelayCommand(RegisterImpl,
                           o => !string.IsNullOrWhiteSpace(_name) &&
                                !string.IsNullOrWhiteSpace(_surname) &&
                                !string.IsNullOrWhiteSpace(_email) &&
                                !string.IsNullOrWhiteSpace(BirthDateText)
                                ));
            }
        }

        private async void RegisterImpl(object o)
        {
            Person person = null;
            await Task.Run((() =>
            {
                try
                {
                    person = new Person(_name, _surname, _email, _birthDate);
                }
                catch (PersonCreationException e)
                {
                    MessageBox.Show(e.Message);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Erorr happened while saving profile: {e}");
                }
            }));
            if (person != null)
            {
                _onRegisterAction(person);
            }
        }

        internal PersonRegisterEditViewModel(Person person, Action<Person> onRegisterAction)
        {
            if (person != null)
            {
                Name = person.Name;
                Surname = person.Surname;
                Email = person.Email;
                BirthDate = person.Birthday;
            }
            _onRegisterAction = onRegisterAction;
        }

        #region Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
