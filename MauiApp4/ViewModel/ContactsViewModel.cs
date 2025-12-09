using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp4.DTO;
using MauiApp4.Model;
using MauiApp4.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiApp4.ViewModel
{
    public partial class ContactsViewModel : ObservableObject
    {
        private readonly IApiService _apiService;
        public ObservableCollection<ContactDto> Contacts { get; set; } = new ObservableCollection<ContactDto>();

        public ContactsViewModel()
        {
            _apiService = new ApiService();
            Task.Run(LoadContactsAsync);
        }

        #region Observable Properties (Свойства)
        [ObservableProperty]
        private bool _isBusy;
        [ObservableProperty]
        private ContactDto _editingContact;
        [ObservableProperty]
        private bool _isModalVisible;
        [ObservableProperty]
        private ContactDto _selectedContact;

        partial void OnSelectedContactChanged(ContactDto value)
        {
            if (value != null)
            {
                OpenEditModal(value);
                _selectedContact = null;
            }
        }

        #endregion

        #region Relay Commands (Команды)
        [RelayCommand]
        private async Task LoadContactsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var contactsList = await _apiService.GetContactsAsync();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Contacts.Clear();
                    foreach (var contact in contactsList)
                    {
                        Contacts.Add(contact);
                    }
                });
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (Application.Current?.MainPage != null)
                        await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось загрузить контакты: {ex.Message}", "OK");
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SaveContactAsync()
        {
            if (EditingContact == null) return;

            try
            {
                IsBusy = true;

                var updateDto = new UpdateContactDto
                {
                    FirstName = EditingContact.FirstName,
                    LastName = EditingContact.LastName,
                    Phone = EditingContact.Phone,
                    Email = EditingContact.Email,
                    Address = EditingContact.Address
                };

                await _apiService.UpdateContactAsync(EditingContact.Id, updateDto);

                IsModalVisible = false;
                await LoadContactsAsync();
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (Application.Current?.MainPage != null)
                        await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось сохранить: {ex.Message}", "OK");
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void CloseModal()
        {
            IsModalVisible = false;
            EditingContact = null;
        }
        #endregion

        #region Логика (Methods)
        private void OpenEditModal(ContactDto contactToEdit)
        {
            EditingContact = new ContactDto
            {
                Id = contactToEdit.Id,
                FirstName = contactToEdit.FirstName,
                LastName = contactToEdit.LastName,
                Phone = contactToEdit.Phone,
                Email = contactToEdit.Email,
                Address = contactToEdit.Address
            };
            IsModalVisible = true;
        }
        #endregion
    }
}