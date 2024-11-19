using Module07DataAccess.ViewModel;
using Module07DataAccess.Model;

namespace Module07DataAccess.View
{
    public partial class ViewEmployee : ContentPage
    {
        public ViewEmployee()
        {
            InitializeComponent();
            BindingContext = new EmployeeViewModel();
        }
    }
}