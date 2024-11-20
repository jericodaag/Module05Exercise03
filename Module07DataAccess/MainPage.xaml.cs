using Module07DataAccess.Services;
using Module07DataAccess.View;
using Module07DataAccess.ViewModel;
using MySql.Data.MySqlClient;

namespace Module07DataAccess
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseConnectionService _dbConnectionService;
        private readonly MainPageViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _dbConnectionService = new DatabaseConnectionService();
            _viewModel = new MainPageViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadCounts();
        }

        private async void OnTestConnectionClicked(object sender, EventArgs e)
        {
            var connectionString = _dbConnectionService.GetConnectionString();
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    _viewModel.ConnectionStatus = "Connection Successful";
                    await DisplayAlert("Success", "Database connection successful!", "OK");
                }
            }
            catch (Exception ex)
            {
                _viewModel.ConnectionStatus = $"Connection Failed: {ex.Message}";
                await DisplayAlert("Error", $"Connection failed: {ex.Message}", "OK");
            }
        }

        private async void OpenEmployeeManagement(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ViewEmployee");
        }
    }
}