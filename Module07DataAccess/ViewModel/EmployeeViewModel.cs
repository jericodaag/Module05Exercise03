using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Module07DataAccess.Model;
using Module07DataAccess.Services;

namespace Module07DataAccess.ViewModel
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        private readonly EmployeeService _employeeService;
        public ObservableCollection<Employee> EmployeeList { get; set; }

        private int _totalEmployees;
        public int TotalEmployees
        {
            get => _totalEmployees;
            set
            {
                _totalEmployees = value;
                OnPropertyChanged();
            }
        }

        private int _totalDepartments;
        public int TotalDepartments
        {
            get => _totalDepartments;
            set
            {
                _totalDepartments = value;
                OnPropertyChanged();
            }
        }

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

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                SearchEmployees();
            }
        }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                if (_selectedEmployee != null)
                {
                    Name = _selectedEmployee.Name;
                    Department = _selectedEmployee.Department;
                    Position = _selectedEmployee.Position;
                    ContactNo = _selectedEmployee.ContactNo;
                    Email = _selectedEmployee.Email;
                    Address = _selectedEmployee.Address;
                    IsEmployeeSelected = true;
                }
                else
                {
                    IsEmployeeSelected = false;
                }
                OnPropertyChanged();
            }
        }

        private bool _isEmployeeSelected;
        public bool IsEmployeeSelected
        {
            get => _isEmployeeSelected;
            set
            {
                _isEmployeeSelected = value;
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

        // Employee properties
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _department;
        public string Department
        {
            get => _department;
            set
            {
                _department = value;
                OnPropertyChanged();
            }
        }

        private string _position;
        public string Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged();
            }
        }

        private string _contactNo;
        public string ContactNo
        {
            get => _contactNo;
            set
            {
                _contactNo = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand LoadDataCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        public ICommand UpdateEmployeeCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }
        public ICommand SearchCommand { get; }

        public EmployeeViewModel()
        {
            _employeeService = new EmployeeService();
            EmployeeList = new ObservableCollection<Employee>();

            LoadDataCommand = new Command(async () => await LoadData());
            AddEmployeeCommand = new Command(async () => await AddEmployee());
            UpdateEmployeeCommand = new Command(async () => await UpdateEmployee(), () => IsEmployeeSelected);
            DeleteEmployeeCommand = new Command(async () => await DeleteEmployee(), () => IsEmployeeSelected);
            SearchCommand = new Command<string>(async (query) => await SearchEmployees());

            LoadData();
        }

        public async Task LoadData()
        {
            if (IsBusy) return;
            IsBusy = true;
            StatusMessage = "Loading employees...";

            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();
                EmployeeList.Clear();
                foreach (var employee in employees)
                {
                    EmployeeList.Add(employee);
                }

                // Update totals
                TotalEmployees = employees.Count;
                TotalDepartments = employees.Select(e => e.Department).Distinct().Count();

                StatusMessage = "Data loaded successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Failed to load employees: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddEmployee()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Department) ||
                string.IsNullOrWhiteSpace(Position) ||
                string.IsNullOrWhiteSpace(ContactNo) ||
                string.IsNullOrWhiteSpace(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error",
                    "Please fill in all required fields", "OK");
                return;
            }

            var confirmAdd = await Application.Current.MainPage.DisplayAlert("Confirm Add",
                $"Are you sure you want to add {Name}?", "Yes", "No");

            if (!confirmAdd) return;

            IsBusy = true;
            StatusMessage = "Adding employee...";

            try
            {
                var newEmployee = new Employee
                {
                    Name = Name,
                    Department = Department,
                    Position = Position,
                    ContactNo = ContactNo,
                    Email = Email,
                    Address = Address
                };

                var success = await _employeeService.AddEmployeeAsync(newEmployee);
                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Success",
                        "Employee added successfully!", "OK");
                    ClearFields();
                    await LoadData(); // This will update the counts
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "Failed to add employee", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Error adding employee: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task UpdateEmployee()
        {
            if (IsBusy || SelectedEmployee == null) return;

            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Department) ||
                string.IsNullOrWhiteSpace(Position) ||
                string.IsNullOrWhiteSpace(ContactNo) ||
                string.IsNullOrWhiteSpace(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error",
                    "Please fill in all required fields", "OK");
                return;
            }

            var confirmUpdate = await Application.Current.MainPage.DisplayAlert("Confirm Update",
                $"Are you sure you want to update {SelectedEmployee.Name}'s information?", "Yes", "No");

            if (!confirmUpdate) return;

            IsBusy = true;
            StatusMessage = "Updating employee...";

            try
            {
                var updatedEmployee = new Employee
                {
                    ID = SelectedEmployee.ID,
                    Name = Name,
                    Department = Department,
                    Position = Position,
                    ContactNo = ContactNo,
                    Email = Email,
                    Address = Address
                };

                var success = await _employeeService.UpdateEmployeeAsync(updatedEmployee);
                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Success",
                        "Employee updated successfully!", "OK");
                    await LoadData(); // This will update the counts
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "Failed to update employee", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Error updating employee: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeleteEmployee()
        {
            if (IsBusy || SelectedEmployee == null) return;

            var confirmDelete = await Application.Current.MainPage.DisplayAlert("Confirm Delete",
                $"Are you sure you want to delete {SelectedEmployee.Name}? This action cannot be undone.",
                "Yes", "No");

            if (!confirmDelete) return;

            IsBusy = true;
            StatusMessage = "Deleting employee...";

            try
            {
                var success = await _employeeService.DeleteEmployeeAsync(SelectedEmployee.ID);
                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Success",
                        $"{SelectedEmployee.Name} has been deleted successfully!", "OK");
                    ClearFields();
                    await LoadData(); // This will update the counts
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "Failed to delete employee", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Error deleting employee: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchEmployees()
        {
            if (IsBusy) return;
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                await LoadData();
                return;
            }

            IsBusy = true;
            StatusMessage = "Searching...";

            try
            {
                var results = await _employeeService.SearchEmployeesAsync(SearchQuery);
                EmployeeList.Clear();
                foreach (var employee in results)
                {
                    EmployeeList.Add(employee);
                }

                // Update totals for search results
                TotalEmployees = results.Count;
                TotalDepartments = results.Select(e => e.Department).Distinct().Count();

                StatusMessage = $"Found {results.Count} results";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Search failed: {ex.Message}";
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Search failed: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ClearFields()
        {
            Name = string.Empty;
            Department = string.Empty;
            Position = string.Empty;
            ContactNo = string.Empty;
            Email = string.Empty;
            Address = string.Empty;
            SelectedEmployee = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}