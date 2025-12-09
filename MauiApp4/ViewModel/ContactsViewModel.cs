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
        private ObservableCollection<ContactDto> _contacts = new();

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

        [ObservableProperty]
        private string _modalTitle; 

        public ContactsViewModel(IApiService apiService)
        {
            _apiService = apiService;
            IsModalVisible = false; 
            EditingContact = new ContactDto();
            LoadContactsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadContacts()
        {
            try
            {
                IsBusy = true;
                var contacts = await _apiService.GetContactsAsync();
                Contacts.Clear();
                foreach (var c in contacts)
                    Contacts.Add(c);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "ошибка загрузки данных: " + ex.Message, "OK");
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
            ModalTitle = "Создание контакта";
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
                Phone = contact.Phone,
                Email = contact.Email,
                Address = contact.Address
            };

            ModalTitle = $"Редактирование (ID: {contact.Id})";
            IsModalVisible = true;
        }


        [RelayCommand]
        private async Task DeleteContact(ContactDto contact)
        {
            if (contact == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Удалить?", $"Удалить контакт {contact.FirstName}?", "Да", "Нет");
            if (!confirm) return;

            try
            {
                await _apiService.DeleteContactAsync(contact.Id);
                Contacts.Remove(contact);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task SaveContact()
        {
            if (EditingContact == null) return;

            try
            {
                if (EditingContact.Id == 0)
                {
                    var created = await _apiService.CreateContactAsync(new CreateContactDto
                    {
                        FirstName = EditingContact.FirstName,
                        LastName = EditingContact.LastName,
                        Phone = EditingContact.Phone,
                        Email = EditingContact.Email,
                        Address = EditingContact.Address
                    });

                    Contacts.Add(created);
                }
                else
                {
                    await _apiService.UpdateContactAsync(EditingContact.Id, new UpdateContactDto
                    {
                        FirstName = EditingContact.FirstName,
                        LastName = EditingContact.LastName,
                        Phone = EditingContact.Phone,
                        Email = EditingContact.Email,
                        Address = EditingContact.Address
                    });

                    var item = Contacts.FirstOrDefault(x => x.Id == EditingContact.Id);
                    if (item != null)
                    {
                        item.FirstName = EditingContact.FirstName;
                        item.LastName = EditingContact.LastName;
                        item.Phone = EditingContact.Phone;
                        item.Email = EditingContact.Email;
                        item.Address = EditingContact.Address;
                    }
                }

                IsModalVisible = false;
                EditingContact = null;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
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
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
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

        [RelayCommand]
        private void CloseModal()
        {
            IsModalVisible = false;
            EditingContact = null;
        }
    }
}
