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
        [ObservableProperty]
        private bool _isNewContact;


        public ContactsViewModel()
        {
            _apiService = new ApiService();
            LoadContactsCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private async Task LoadContacts()
        {
            try
            {
                IsBusy = true;
                var contacts = await _apiService.GetContactsAsync();

                Contacts.Clear();
                foreach (var contact in contacts)
                {
                    Contacts.Add(contact);
                }
            }
            catch (Exception ex)
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
            try
            {
                IsNewContact = true;
                EditingContact = new ContactDto();
                IsModalVisible = true;
            }
            catch (Exception ex)
            {
                Application.Current?.MainPage?.DisplayAlert("Ошибка",
                    $"Ошибка: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private void EditContact(ContactDto contact)
        {
            try
            {
                if (contact == null) return;

                IsNewContact = false;
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
            catch (Exception ex)
            {
                Application.Current?.MainPage?.DisplayAlert("Ошибка",
                    $"Ошибка: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task SaveContact()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EditingContact?.FirstName) || 
                    string.IsNullOrWhiteSpace(EditingContact?.LastName))
                {
                    await Application.Current?.MainPage?.DisplayAlert("Ошибка",
                        "Заполните имя и фамилию", "OK");
                    return;
                }

                IsBusy = true;

                if (IsNewContact)
                {
                    var newContact = new CreateContactDto
                    {
                        FirstName = EditingContact.FirstName,
                        LastName = EditingContact.LastName,
                        Phone = EditingContact.Phone,
                        Email = EditingContact.Email,
                        Address = EditingContact.Address
                    };

                    var createdContact = await _apiService.CreateContactAsync(newContact);
                    Contacts.Add(createdContact);
                }
                else
                {
                    var updateContact = new UpdateContactDto
                    {
                        FirstName = EditingContact.FirstName,
                        LastName = EditingContact.LastName,
                        Phone = EditingContact.Phone,
                        Email = EditingContact.Email,
                        Address = EditingContact.Address
                    };

                    await _apiService.UpdateContactAsync(EditingContact.Id, updateContact);
                    
                    var existingContact = Contacts.FirstOrDefault(c => c.Id == EditingContact.Id);
                    if (existingContact != null)
                    {
                        var index = Contacts.IndexOf(existingContact);
                        Contacts.RemoveAt(index);
                        Contacts.Insert(index, new ContactDto
                        {
                            Id = EditingContact.Id,
                            FirstName = EditingContact.FirstName,
                            LastName = EditingContact.LastName,
                            Phone = EditingContact.Phone,
                            Email = EditingContact.Email,
                            Address = EditingContact.Address
                        });
                    }
                }

                CloseModal();
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert("Ошибка",
                    $"Ошибка сохранения: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteContact(ContactDto contact)
        {
            try
            {
                if (contact == null) return;

                bool confirm = await Application.Current?.MainPage?.DisplayAlert(
                    "Подтверждение",
                    $"Удалить контакт {contact.FullName}?",
                    "Удалить",
                    "Отмена");

                if (confirm)
                {
                    IsBusy = true;
                    await _apiService.DeleteContactAsync(contact.Id);
                    Contacts.Remove(contact);
                }
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert("Ошибка",
                    $"Ошибка удаления: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task RefreshContacts()
        {
            try
            {
                IsRefreshing = true;
                var contacts = await _apiService.GetContactsAsync();

                Contacts.Clear();
                foreach (var contact in contacts)
                {
                    Contacts.Add(contact);
                }
            }
            catch (Exception ex)
            {
                await Application.Current?.MainPage?.DisplayAlert("Ошибка",
                    $"Ошибка обновления: {ex.Message}", "OK");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private void CloseModal()
        {
            IsModalVisible = false;
            EditingContact = null;
            IsNewContact = false;
        }

    }
}
