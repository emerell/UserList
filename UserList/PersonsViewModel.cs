using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UserList
{
    class PersonsViewModel : INotifyPropertyChanged
    {
        private Person _selectedPerson;
        private readonly Action _updateUsersGrid;
        private readonly Action<string> _updateUserAction;
        private RelayCommand _edit;
        private RelayCommand _delete;
        private RelayCommand _register;
        private RelayCommand _sort;

        private List<Person> _personsList;

        public List<Person> PersonsListToShow =>
            (string.IsNullOrEmpty(SelectedSoftFilterProperty) || string.IsNullOrEmpty(FilterQuery))
                ? _personsList
                : _personsList.FilterBy(SelectedSoftFilterProperty, FilterQuery);

        private static CollectionView _sortFilterOptionsCollection;

        public static CollectionView SortFilterOptions => _sortFilterOptionsCollection ??
                                                          (_sortFilterOptionsCollection =
                                                              new CollectionView(SortExtension.Options));

        private string _filterQuery;

        public string SelectedSoftFilterProperty { get; set; }

        public string FilterQuery
        {
            get => _filterQuery;
            set
            {
                _filterQuery = value;
                SelectedPerson = null;
                _updateUsersGrid();
            }
        }

        public Person SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                _selectedPerson = value;
                if (_selectedPerson != null)
                    _updateUserAction($"{_selectedPerson.Name} {_selectedPerson.Surname}");
            }
        }

        public RelayCommand Delete =>
            _delete ?? (_delete = new RelayCommand(DeleteImpl, o => _selectedPerson != null));

        public RelayCommand Edit =>
            _edit ?? (_edit = new RelayCommand(EditImpl, o => _selectedPerson != null));

        public RelayCommand Register =>
            _register ?? (_register = new RelayCommand(RegisterImpl, o => true));


        public RelayCommand Sort =>
            _sort ?? (_sort =
                new RelayCommand(SortImpl, o => !string.IsNullOrEmpty(SelectedSoftFilterProperty)));


        private async void DeleteImpl(object o)
        {
            await Task.Run((() =>
            {
                _personsList.Remove(SelectedPerson);
                _updateUsersGrid();
            }));
        }

        private void EditImpl(object o)
        {
            var personToEdit = _selectedPerson;
            var editWindow = new PersonRegisterEditWindow(delegate (Person edited)
            {
                personToEdit.CopyFrom(edited);
                _updateUsersGrid();
            }, _selectedPerson);
            editWindow.Show();
        }

        private void RegisterImpl(object o)
        {
            var registerWindow = new PersonRegisterEditWindow(delegate (Person newPerson)
            {
                PersonsListToShow.Add(newPerson);
                _updateUsersGrid();
            });
            registerWindow.Show();
        }

        private async void SortImpl(object o)
        {
            await Task.Run(() =>
            {
                _personsList = _personsList.SortBy(SelectedSoftFilterProperty);
                _updateUsersGrid();
            });
        }

        public PersonsViewModel(Action updateGrid, Action<string> updateUserInfo)
        {
            _personsList = new List<Person>();
            Person.LoadAllInto(PersonsListToShow, updateGrid);

            _updateUsersGrid = () =>
            {
                Person.SaveAll(_personsList);
                OnPropertyChanged($"PersonsListToShow");
                updateGrid();
            };
            _updateUserAction = updateUserInfo;
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
