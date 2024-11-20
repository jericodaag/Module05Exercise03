using System.ComponentModel;
using System.Runtime.CompilerServices;
using Module07DataAccess.Services;
using Module07DataAccess.Model;

namespace Module07DataAccess.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly EmployeeService _employeeService;

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

        private string _connectionStatus = "Not Connected";
        public string ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                _connectionStatus = value;
                OnPropertyChanged();
            }
        }

        public MainPageViewModel()
        {
            _employeeService = new EmployeeService();
            LoadCounts();
        }

        public async Task LoadCounts()
        {
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();
                TotalEmployees = employees.Count;
                TotalDepartments = employees.Select(e => e.Department).Distinct().Count();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    $"Failed to load counts: {ex.Message}", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}