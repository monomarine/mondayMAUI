using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp4.Model;
using System.Collections.ObjectModel;

namespace MauiApp4.ViewModel
{
    partial class ContactsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<NContact> _contacts = new ObservableCollection<NContact>();

        [ObservableProperty]
        private NContact _selectedContact;
        
        [ObservableProperty]
        private bool _isModalVisible;

        [ObservableProperty]
        private NContact _editingContact;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _showError;

        public ContactsViewModel()
        {
            Contacts.Add(new NContact("Иванов иван иванович", "+7 549 463 85 12", "ivanov@gmail.com"));
            Contacts.Add(new NContact("Петров Сергей иванович", "+7 549 123 12 85", "petrov@gmail.com"));
            Contacts.Add(new NContact("Федоров максим Анатольевич", "+7 549 643 12 75", "fedorov@gmail.com"));
            Contacts.Add(new NContact("Боженов Даниил Петрович", "+7 549 346 12 54", "bojenov@gmail.com"));
        }

        [RelayCommand]
        private void Update()
        {
            Contacts.Clear();
            Contacts.Add(new NContact("Иванов иван иванович", "+7 549 463 85 12", "ivanov@gmail.com"));
            Contacts.Add(new NContact("Петров Сергей иванович", "+7 549 123 12 85", "petrov@gmail.com"));
            Contacts.Add(new NContact("Федоров максим Анатольевич", "+7 549 643 12 75", "fedorov@gmail.com"));
            Contacts.Add(new NContact("Боженов Даниил Петрович", "+7 549 346 12 54", "bojenov@gmail.com"));
        }
        
        [RelayCommand]
        private void EditContact(NContact contact)
        {
            if(contact is not null)
            {
                SelectedContact = contact;
                EditingContact = new NContact(
                    contact.Name,
                    contact.Phone,
                    contact.Email,
                    contact.Icon
                );
                IsModalVisible = true;
            }
        }

        [RelayCommand]
        private void SaveContact()
        {
            if (string.IsNullOrWhiteSpace(EditingContact.Name) || 
                string.IsNullOrWhiteSpace(EditingContact.Phone))
            {
                ShowErrorMessage("Имя и телефон обязательны для заполнения");
                return;
            }

            if(SelectedContact != null)
            {
                var index = Contacts.IndexOf(SelectedContact);
                if(index >= 0)
                {
                    Contacts[index] = new NContact(
                        EditingContact.Name,
                        EditingContact.Phone,
                        EditingContact.Email,
                        SelectedContact.Icon
                    );
                    SelectedContact = Contacts[index];
                }
                CloseModal();
            }
            else
            {
                ShowErrorMessage("Не выбран контакт для редактирования");
            }
        }

        private void ShowErrorMessage(string message)
        {
            ErrorMessage = message;
            ShowError = true;
        }

        [RelayCommand]
        private void CloseModal()
        {
            IsModalVisible = false;
            EditingContact = null;
        }
    }
}