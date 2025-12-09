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
            Task.Run(async () => await LoadContacts());
        }

        public ContactsViewModel() 
        {
            _apiService = new ApiService();
            Task.Run(async () => await LoadContacts());
        }
        public async Task InitializeAsync()
        {
            await LoadContacts();
        }
        private async Task LoadContacts()
        {
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

            EditingContact = new ContactDto();
            IsModalVisible = true;
        }
        [RelayCommand]
        private void EditContact(ContactDto contact)
        {
            if (contact == null) return;

            EditingContact = new ContactDto
            {
                Id = contact.Id,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Email = contact.Email,
                Address = contact.Address,
                Phone = contact.Phone,
            };
            IsModalVisible = true;
        }
        [RelayCommand]
        private async void DeleteContact(ContactDto contact)
        {
            if (contact == null)
                return;

            try
            {
                
                await _apiService.DeleteContactAsync(contact.Id);

               
                Contacts.Remove(contact);
            }
            catch (Exception ex)
            {
                
                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка удаления",
                    $"Не удалось удалить контакт: {ex.Message}",
                    "OK");
            }
        }

        [RelayCommand]
        private async Task SaveContact()
        {
            try
            {
                if (EditingContact == null)
                    return;

                if (EditingContact.Id == 0)
                {
                    
                    var dto = new CreateContactDto
                    {
                        FirstName = EditingContact.FirstName,
                        LastName = EditingContact.LastName,
                        Phone = EditingContact.Phone,
                        Email = EditingContact.Email,
                        Address = EditingContact.Address
                    };

                    var newContact = await _apiService.CreateContactAsync(dto);
                    Contacts.Add(newContact);
                }
                else
                {
                   
                    var dto = new UpdateContactDto
                    {
                        FirstName = EditingContact.FirstName,
                        LastName = EditingContact.LastName,
                        Phone = EditingContact.Phone,
                        Email = EditingContact.Email,
                        Address = EditingContact.Address
                    };

                    await _apiService.UpdateContactAsync(EditingContact.Id, dto);

                    
                    var old = Contacts.FirstOrDefault(c => c.Id == EditingContact.Id);
                    if (old != null)
                    {
                        old.FirstName = EditingContact.FirstName;
                        old.LastName = EditingContact.LastName;
                        old.Phone = EditingContact.Phone;
                        old.Email = EditingContact.Email;
                        old.Address = EditingContact.Address;
                    }
                }

                IsModalVisible = false;
                EditingContact = null;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Ошибка",
                    $"Не удалось сохранить контакт: {ex.Message}",
                    "OK");
            }
        }
        [RelayCommand]
        private async Task RefreshContacts()
        {
            try
            {
                IsRefreshing = true;

                var contacts = await _apiService.GetContactsAsync(SearchText);

                Contacts.Clear();
                foreach (var c in contacts)
                    Contacts.Add(c);
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
        partial void OnSearchTextChanged(string value)
        {
            RefreshContactsCommand.Execute(null);
        }

        private async void SearchContact()
        {
            await RefreshContacts();
        }

        [RelayCommand]
        private void CloseModal()
        {
            IsModalVisible = false;
            EditingContact = null;
        }

    }
}
