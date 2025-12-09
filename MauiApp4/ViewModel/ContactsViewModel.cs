using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp4.DTO;
using MauiApp4.Model;
using MauiApp4.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp4.ViewModel
{
#pragma warning disable
    partial class ContactsViewModel : ObservableObject
    {
        private readonly IApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<ContactDto> _contacts = new ObservableCollection<ContactDto>();
        [ObservableProperty]
        private ContactDto _selectedContact;
        [ObservableProperty]
        private string _searchText;
        [ObservableProperty]
        private bool _isRefreshing;
        [ObservableProperty]
        private bool _isModalVisible;
        [ObservableProperty]
        private ContactDto _editingContact;
        [ObservableProperty]
        private bool _isBusy;


        public ContactsViewModel(IApiService apiserv)
        {
            _apiService = apiserv;
        }

        public ContactsViewModel() { }

        private async Task LoadContacts()
        {
            try
            {
                IsBusy = true;
                // var contacts = ; извлеките данные через сервис API

                Contacts.Clear();
                //добавьте загруженные контакты в Contacts
            }
            catch(Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка",
                    $"ошибка загрузки данных {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        private void AddContact()
        {
            //реализуйте функционал добавления нового контакта через модальное окно
        }
        [RelayCommand]
        private void EditContact(ContactDto contact)
        {
            //реализуйте функционал одновления контакта
        }
        [RelayCommand]
        private void DeleteContact(ContactDto contact)
        {
            //реализуйте удаление выбранного контакта. можно через кнопку в collectionview Или пойти более сложным путем и реализовать удаление свайпом. тут понадобиться отслеживать события ввода и работа с Behaviuours
        }
        [RelayCommand]
        private async Task RefreshContacts()
        {
            IsRefreshing = true;
            var contacts = await _apiService.GetContactsAsync(SearchText);

            //одновление основного списка контактов на основе полученного из сервиса
            //не забудьте про обработку исключений
        }
        
        private void SearchContact()
        {
            //реализуйте поиск контакта / контактов по строке поиска
        }

        [RelayCommand]
        private void CloseModal()
        {
            IsModalVisible = false;
            EditingContact = null;
        }

    }
}
