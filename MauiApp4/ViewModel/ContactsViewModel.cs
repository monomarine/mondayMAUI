using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp4.DTO;
using MauiApp4.Services;
using System.Collections.ObjectModel;

namespace MauiApp4.ViewModel
{
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
        private ContactDto _editingContact = new ContactDto();

        [ObservableProperty]
        private bool _isBusy;

        public ContactsViewModel(IApiService apiserv)
        {
            _apiService = apiserv;
            _ = LoadContacts();
        }

        public ContactsViewModel()
        {
            _apiService = null;
        }

        [RelayCommand]
        private async Task LoadContacts()
        {
            if (_apiService == null) return;

            try
            {
                IsBusy = true;
                var contacts = await _apiService.GetContactsAsync(SearchText);

                Contacts.Clear();
                foreach (var contact in contacts)
                {
                    Contacts.Add(contact);
                }
            }
            catch (Exception ex)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка",
                        $"Ошибка загрузки данных: {ex.Message}", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void AddContact()
        {
            EditingContact = new ContactDto();
            IsModalVisible = true;
        }

        [RelayCommand]
        private void EditContact(ContactDto contact)
        {
            if (contact != null)
            {
                EditingContact = new ContactDto
                {
                    Id = contact.Id,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    Phone = contact.Phone,
                    Email = contact.Email,
                    Address = contact.Address
                };
                IsModalVisible = true;
            }
        }

        [RelayCommand]
        private async Task DeleteContact(ContactDto contact)
        {
            if (_apiService == null || contact == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Удаление контакта",
                $"Вы уверены, что хотите удалить контакт {contact.FullName}?",
                "Да", "Нет");

            if (confirm)
            {
                try
                {
                    await _apiService.DeleteContactAsync(contact.Id);
                    Contacts.Remove(contact);
                    await Application.Current.MainPage.DisplayAlert(
                        "Успех", "Контакт удален", "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Ошибка", $"Не удалось удалить контакта: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        private async Task RefreshContacts()
        {
            if (_apiService == null) return;

            IsRefreshing = true;
            try
            {
                var contacts = await _apiService.GetContactsAsync(SearchText);

                Contacts.Clear();
                foreach (var contact in contacts)
                {
                    Contacts.Add(contact);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка", $"Ошибка обновления: {ex.Message}", "OK");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task SearchContact()
        {
            await LoadContacts();
        }

        [RelayCommand]
        private async Task SaveContact()
        {
            if (_apiService == null || EditingContact == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка", "Сервис недоступен", "OK");
                return;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(EditingContact.FirstName) ||
                    string.IsNullOrWhiteSpace(EditingContact.LastName) ||
                    string.IsNullOrWhiteSpace(EditingContact.Phone) ||
                    string.IsNullOrWhiteSpace(EditingContact.Email))
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Ошибка", "Заполните все обязательные поля (*)", "OK");
                    return;
                }

                if (EditingContact.Id == 0)
                {
                    var createDto = new CreateContactDto
                    {
                        FirstName = EditingContact.FirstName,
                        LastName = EditingContact.LastName,
                        Phone = EditingContact.Phone,
                        Email = EditingContact.Email,
                        Address = EditingContact.Address ?? string.Empty
                    };

                    var newContact = await _apiService.CreateContactAsync(createDto);
                    if (newContact != null)
                    {
                        Contacts.Add(newContact);
                        await Application.Current.MainPage.DisplayAlert(
                            "Успех", "Контакт создан", "OK");
                    }
                }
                else
                {
                    var updateDto = new UpdateContactDto
                    {
                        FirstName = EditingContact.FirstName,
                        LastName = EditingContact.LastName,
                        Phone = EditingContact.Phone,
                        Email = EditingContact.Email,
                        Address = EditingContact.Address ?? string.Empty
                    };

                    await _apiService.UpdateContactAsync(EditingContact.Id, updateDto);

                    var existingContact = Contacts.FirstOrDefault(c => c.Id == EditingContact.Id);
                    if (existingContact != null)
                    {
                        existingContact.FirstName = EditingContact.FirstName;
                        existingContact.LastName = EditingContact.LastName;
                        existingContact.Phone = EditingContact.Phone;
                        existingContact.Email = EditingContact.Email;
                        existingContact.Address = EditingContact.Address;
                    }

                    await Application.Current.MainPage.DisplayAlert(
                        "Успех", "Контакт обновлен", "OK");
                }

                CloseModal();
                await LoadContacts();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка", $"Ошибка сохранения: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private void CloseModal()
        {
            IsModalVisible = false;
            EditingContact = new ContactDto();
        }
    }
}