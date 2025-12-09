using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp4.DTO;
using MauiApp4.Services;
using System.Collections.ObjectModel;
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

        [ObservableProperty]
        private bool _isNewContact;

        [ObservableProperty]
        private int _currentContactId; // Добавляем отдельное свойство для ID

        // Конструктор для DI
        public ContactsViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Task.Run(async () => await LoadContacts());
        }

        // Публичный конструктор без параметров для XAML
        public ContactsViewModel() : this(new ApiService())
        {
            // Вызов конструктора с параметром создает ApiService
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
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка",
                    $"Ошибка загрузки данных: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private void AddContact()
        {
            IsNewContact = true;
            ModalTitle = "Добавить новый контакт";
            _currentContactId = 0; // Сбрасываем ID для нового контакта
            EditingContact = new ContactDto();
            IsModalVisible = true;
        }

        [RelayCommand]
        private void EditContact(ContactDto contact)
        {
            if (contact == null) return;

            IsNewContact = false;
            ModalTitle = "Редактировать контакт";
            _currentContactId = contact.Id; // Сохраняем ID отдельно

            // Создаем новую копию контакта для редактирования
            EditingContact = new ContactDto
            {
                // Id здесь НЕ копируем, он хранится в CurrentContactId
                FirstName = contact.FirstName ?? string.Empty,
                LastName = contact.LastName ?? string.Empty,
                Phone = contact.Phone ?? string.Empty,
                Email = contact.Email ?? string.Empty,
                Address = contact.Address ?? string.Empty
            };

            IsModalVisible = true;
        }

        [RelayCommand]
        private async Task SaveContact()
        {
            if (EditingContact == null)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка",
                    "Данные контакта не загружены", "OK");
                return;
            }

            // Проверяем обязательные поля
            if (string.IsNullOrWhiteSpace(EditingContact.FirstName) ||
                string.IsNullOrWhiteSpace(EditingContact.LastName) ||
                string.IsNullOrWhiteSpace(EditingContact.Phone) ||
                string.IsNullOrWhiteSpace(EditingContact.Email))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка",
                    "Заполните обязательные поля: Имя, Фамилия, Телефон и Email", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                if (IsNewContact)
                {
                    var createDto = new CreateContactDto
                    {
                        FirstName = EditingContact.FirstName?.Trim() ?? string.Empty,
                        LastName = EditingContact.LastName?.Trim() ?? string.Empty,
                        Phone = EditingContact.Phone?.Trim() ?? string.Empty,
                        Email = EditingContact.Email?.Trim() ?? string.Empty,
                        Address = EditingContact.Address?.Trim() ?? string.Empty
                    };

                    var newContact = await _apiService.CreateContactAsync(createDto);
                    if (newContact != null)
                    {
                        Contacts.Add(newContact);
                        await Application.Current.MainPage.DisplayAlert("Успех",
                            "Контакт успешно добавлен", "OK");
                    }
                }
                else
                {
                    if (_currentContactId <= 0)
                    {
                        await Application.Current.MainPage.DisplayAlert("Ошибка",
                            "Неверный ID контакта для обновления", "OK");
                        return;
                    }

                    var updateDto = new UpdateContactDto
                    {
                        FirstName = EditingContact.FirstName?.Trim() ?? string.Empty,
                        LastName = EditingContact.LastName?.Trim() ?? string.Empty,
                        Phone = EditingContact.Phone?.Trim() ?? string.Empty,
                        Email = EditingContact.Email?.Trim() ?? string.Empty,
                        Address = EditingContact.Address?.Trim() ?? string.Empty
                    };

                    // Используем CurrentContactId вместо EditingContact.Id
                    await _apiService.UpdateContactAsync(_currentContactId, updateDto);

                    // Обновляем контакт в списке
                    var existingContact = Contacts.FirstOrDefault(c => c.Id == _currentContactId);
                    if (existingContact != null)
                    {
                        existingContact.FirstName = EditingContact.FirstName;
                        existingContact.LastName = EditingContact.LastName;
                        existingContact.Phone = EditingContact.Phone;
                        existingContact.Email = EditingContact.Email;
                        existingContact.Address = EditingContact.Address;

                        // Уведомляем об изменении
                        OnPropertyChanged(nameof(Contacts));
                    }

                    await Application.Current.MainPage.DisplayAlert("Успех",
                        "Контакт успешно обновлен", "OK");
                }

                CloseModal();
                await LoadContacts(); // Перезагружаем для синхронизации
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка",
                    $"Ошибка сохранения контакта: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteContact(ContactDto contact)
        {
            if (contact == null) return;

            bool answer = await Application.Current.MainPage.DisplayAlert(
                "Подтверждение удаления",
                $"Вы уверены, что хотите удалить контакт {contact.FullName}?",
                "Да", "Нет");

            if (answer)
            {
                try
                {
                    IsBusy = true;

                    // Проверяем, что ID валидный
                    if (contact.Id <= 0)
                    {
                        await Application.Current.MainPage.DisplayAlert("Ошибка",
                            "Неверный ID контакта для удаления", "OK");
                        return;
                    }

                    await _apiService.DeleteContactAsync(contact.Id);
                    Contacts.Remove(contact);

                    await Application.Current.MainPage.DisplayAlert("Успех",
                        "Контакт успешно удален", "OK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка",
                        $"Ошибка удаления контакта: {ex.Message}", "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        [RelayCommand]
        private async Task RefreshContacts()
        {
            IsRefreshing = true;
            await LoadContacts();
        }

        [RelayCommand]
        private async Task SearchContact()
        {
            await LoadContacts();
        }

        [RelayCommand]
        private void CloseModal()
        {
            IsModalVisible = false;
            EditingContact = null;
            IsNewContact = false;
            _currentContactId = 0;
            ModalTitle = string.Empty;
        }
    }
}