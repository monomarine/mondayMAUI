using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp4.Model;
using System.Collections.ObjectModel;

namespace MauiApp4.ViewModel
{
    public partial class ContactsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<NContact> _contacts = new ObservableCollection<NContact>();

        [ObservableProperty]
        private NContact _selectedContact;

        [ObservableProperty]
        private bool _isEditModalVisible = false; 

        public ContactsViewModel()
        {
            LoadContacts();
        }

        private void LoadContacts()
        {
            Contacts.Clear();
            Contacts.Add(new NContact("Иванов иван иванович", "+7 549 463 85 12", "ivanov@gmail.com"));
            Contacts.Add(new NContact("Петров Сергей иванович", "+7 549 123 12 85", "petrov@gmail.com"));
            Contacts.Add(new NContact("Федоров максим Анатольевич", "+7 549 643 12 75", "fedorov@gmail.com"));
            Contacts.Add(new NContact("Боженов Даниил Петрович", "+7 549 346 12 54", "bojenov@gmail.com"));
        }

        [RelayCommand]
        private void Update()
        {
            LoadContacts();
        }

        
        [RelayCommand]
        private void EditContact(NContact contact)
        {
            if (contact != null)
            {

                SelectedContact = new NContact
                {
                    Id = contact.Id,
                    Name = contact.Name,
                    Phone = contact.Phone,
                    Email = contact.Email,
                    Icon = contact.Icon
                };
                IsEditModalVisible = true;
            }
        }

        [RelayCommand]
        private void SaveContact()
        {
            if (SelectedContact != null)
            {

                var existingContact = Contacts.FirstOrDefault(c => c.Id == SelectedContact.Id);
                if (existingContact != null)
                {
                    existingContact.Name = SelectedContact.Name;
                    existingContact.Phone = SelectedContact.Phone;
                    existingContact.Email = SelectedContact.Email;
                    existingContact.Icon = SelectedContact.Icon;
                }

                IsEditModalVisible = false;
                SelectedContact = null;
            }
        }


        [RelayCommand]
        private void CancelEdit()
        {
            IsEditModalVisible = false;
            SelectedContact = null;
        }
    }
}