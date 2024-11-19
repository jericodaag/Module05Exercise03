using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Module07DataAccess.Services;
using Module07DataAccess.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Security.Cryptography.X509Certificates;

namespace Module07DataAccess.ViewModel
{
    public class PersonalViewModel : INotifyPropertyChanged
    {
        private readonly PersonalService _personalService;

        public ObservableCollection<Personal> PersonalList { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private Personal _selectedPersonal;
        public Personal SelectedPersonal
        {
            get => _selectedPersonal;
            set
            {
                _selectedPersonal = value;
                if (_selectedPersonal != null)
                {
                    NewPersonalName = _selectedPersonal.Name;
                    NewPersonalGender = _selectedPersonal.Gender;
                    NewPersonalContactNo = _selectedPersonal.ContactNo;
                    NewPersonalAddress = _selectedPersonal.Address; 
                    NewPersonalEmail = _selectedPersonal.Email; 
                    IsPersonSelected = true;
                }
                else
                {
                    IsPersonSelected = false;
                }
                OnPropertyChanged();
            }
        }

        private bool _isPersonSelected;
        public bool IsPersonSelected
        {
            get => _isPersonSelected;
            set
            {
                _isPersonSelected = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        // Personal properties
        private string _newPersonalName;
        public string NewPersonalName
        {
            get => _newPersonalName;
            set
            {
                _newPersonalName = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalGender;
        public string NewPersonalGender
        {
            get => _newPersonalGender;
            set
            {
                _newPersonalGender = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalContactNo;
        public string NewPersonalContactNo
        {
            get => _newPersonalContactNo;
            set
            {
                _newPersonalContactNo = value;
                OnPropertyChanged();
            }
        }

        // New properties for Address and Email
        private string _newPersonalAddress;
        public string NewPersonalAddress
        {
            get => _newPersonalAddress;
            set
            {
                _newPersonalAddress = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalEmail;
        public string NewPersonalEmail
        {
            get => _newPersonalEmail;
            set
            {
                _newPersonalEmail = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand LoadDataCommand { get; }
        public ICommand AddPersonalCommand { get; }
        public ICommand SelectedPersonCommand { get; }
        public ICommand DeletePersonCommand { get; }
        public ICommand UpdatePersonalCommand { get; }

        // Constructor
        public PersonalViewModel()
        {
            _personalService = new PersonalService();
            PersonalList = new ObservableCollection<Personal>();

            LoadDataCommand = new Command(async () => await LoadData());
            AddPersonalCommand = new Command(async () => await AddPerson());
            SelectedPersonCommand = new Command<Personal>(person => SelectedPersonal = person);
            DeletePersonCommand = new Command(async () => await DeletePersonal(),
                                           () => SelectedPersonal != null);
            UpdatePersonalCommand = new Command(async () => await UpdatePerson(),
                                             () => SelectedPersonal != null);

            LoadData();
        }

        public async Task LoadData()
        {
            if (IsBusy) return;
            IsBusy = true;
            StatusMessage = "Loading personal data...";

            try
            {
                var personals = await _personalService.GetAllPersonalAsync();
                PersonalList.Clear();
                foreach (var personal in personals)
                {
                    PersonalList.Add(personal);
                }
                StatusMessage = "Data loaded successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddPerson()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(NewPersonalName) ||
                string.IsNullOrWhiteSpace(NewPersonalGender) ||
                string.IsNullOrWhiteSpace(NewPersonalContactNo) ||
                string.IsNullOrWhiteSpace(NewPersonalAddress) ||
                string.IsNullOrWhiteSpace(NewPersonalEmail)) 
            {
                StatusMessage = "Please fill in all the fields before adding";
                return;
            }

            if (!IsValidEmail(NewPersonalEmail))
            {
                StatusMessage = "Please enter a valid email address";
                return;
            }

            IsBusy = true;
            StatusMessage = "Adding new person...";

            try
            {
                var newPerson = new Personal
                {
                    Name = NewPersonalName,
                    Gender = NewPersonalGender,
                    ContactNo = NewPersonalContactNo,
                    Address = NewPersonalAddress,
                    Email = NewPersonalEmail 
                };

                var isSuccess = await _personalService.AddPersonalAsync(newPerson);
                if (isSuccess)
                {
                    ClearFields();
                    StatusMessage = "New person added successfully";
                }
                else
                {
                    StatusMessage = "Failed to add the new person";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to add the new person: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
                await LoadData();
            }
        }

        private async Task UpdatePerson()
        {
            if (SelectedPersonal == null) return;

            if (string.IsNullOrWhiteSpace(NewPersonalName) ||
                string.IsNullOrWhiteSpace(NewPersonalGender) ||
                string.IsNullOrWhiteSpace(NewPersonalContactNo) ||
                string.IsNullOrWhiteSpace(NewPersonalAddress) ||
                string.IsNullOrWhiteSpace(NewPersonalEmail))
            {
                StatusMessage = "Please fill in all fields before updating";
                return;
            }

            if (!IsValidEmail(NewPersonalEmail))
            {
                StatusMessage = "Please enter a valid email address";
                return;
            }

            IsBusy = true;
            StatusMessage = "Updating person...";

            try
            {
                var updatedPerson = new Personal
                {
                    ID = SelectedPersonal.ID,
                    Name = NewPersonalName,
                    Gender = NewPersonalGender,
                    ContactNo = NewPersonalContactNo,
                    Address = NewPersonalAddress,
                    Email = NewPersonalEmail
                };

                var success = await _personalService.UpdatePersonalAsync(updatedPerson);
                if (success)
                {
                    StatusMessage = "Person updated successfully";
                    await LoadData();
                }
                else
                {
                    StatusMessage = "Failed to update person";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error updating person: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeletePersonal()
        {
            if (SelectedPersonal == null) return;

            var answer = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete {SelectedPersonal.Name}?",
                "Yes", "No");

            if (!answer) return;

            IsBusy = true;
            StatusMessage = "Deleting person...";

            try
            {
                var success = await _personalService.DeletePersonalAsync(SelectedPersonal.ID);
                StatusMessage = success ? "Person deleted successfully!" : "Failed to delete person";

                if (success)
                {
                    PersonalList.Remove(SelectedPersonal);
                    SelectedPersonal = null;
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting person: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ClearFields()
        {
            NewPersonalName = string.Empty;
            NewPersonalGender = string.Empty;
            NewPersonalContactNo = string.Empty;
            NewPersonalAddress = string.Empty;
            NewPersonalEmail = string.Empty;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}