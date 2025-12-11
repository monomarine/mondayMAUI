using MauiApp4.Services;
using MauiApp4.ViewModel;

namespace MauiApp4
{
    public partial class ContactsPage : ContentPage
    {
        public ContactsPage()
        {
            InitializeComponent();

            try
            {
                //создаем сервис и ViewModel
                var apiService = new ApiService();
                var viewModel = new ContactsViewModel(apiService);

                //устанавливаем BindingContext
                BindingContext = viewModel;
            }
            catch (Exception ex)
            {
                DisplayAlert("Ошибка", $"Ошибка создания страницы: {ex.Message}", "OK");
            }
        }
    }
}