// ViewModel/ContactsViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp4.Model;
using System.Collections.ObjectModel;

namespace MauiApp4.ViewModel
{
    public partial class ContactsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<NContact> contacts = new ObservableCollection<NContact>();

        [ObservableProperty]
        private NContact selectedContact;

        [ObservableProperty]
        private bool isModalVisible = false;

        [ObservableProperty]
        private NContact editingContact;

        public ContactsViewModel()
        {
            // Инициализация тестовыми данными
            Contacts.Add(new NContact("Иванов Иван Иванович", "+7 549 463 85 12", "ivanov@gmail.com"));
            Contacts.Add(new NContact("Петров Сергей Иванович", "+7 549 123 12 85", "petrov@gmail.com"));
            Contacts.Add(new NContact("Федоров Максим Анатольевич", "+7 549 643 12 75", "fedorov@gmail.com"));
            Contacts.Add(new NContact("Боженов Даниил Петрович", "+7 549 346 12 54", "bojenov@gmail.com"));
        }

        [RelayCommand]
        private void EditContact(NContact contact)
        {
            if (contact != null)
            {
                SelectedContact = contact;
                // Создаем копию для редактирования
                EditingContact = new NContact(
                    contact.Name,
                    contact.Phone,
                    contact.Email,
                    contact.Icon);
                IsModalVisible = true;
            }
        }

        [RelayCommand]
        private void SaveContact()
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(EditingContact?.Name))
            {
                App.Current.MainPage.DisplayAlert("Ошибка", "Введите имя контакта", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(EditingContact?.Phone))
            {
                App.Current.MainPage.DisplayAlert("Ошибка", "Введите телефон", "OK");
                return;
            }

            // Обновляем контакт
            if (SelectedContact != null)
            {
                var index = Contacts.IndexOf(SelectedContact);
                if (index >= 0)
                {
                    Contacts[index] = new NContact(
                        EditingContact.Name,
                        EditingContact.Phone,
                        EditingContact.Email,
                        EditingContact.Icon);
                }
            }

            CloseModal();
        }

        [RelayCommand]
        private void CloseModal()
        {
            IsModalVisible = false;
            EditingContact = null;
            SelectedContact = null;
        }
    }
}