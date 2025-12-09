using MauiApp4.Services;
using MauiApp4.ViewModel;

namespace MauiApp4;

public partial class ContactsPage : ContentPage
{
	public ContactsPage(IApiService apiService)
	{
		InitializeComponent();
        BindingContext = new ContactsViewModel(apiService);
    }
}