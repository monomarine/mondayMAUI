
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp4.Model
{
    public class NContact : INotifyPropertyChanged
    {
        private Guid _id;
        private string _icon;
        private string _name;
        private string _phone;
        private string _email;

        public Guid Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string DisplayImage =>
            !string.IsNullOrEmpty(Icon) ? Icon : "user.png";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public NContact()
        {
            Id = Guid.NewGuid();
        }

        public NContact(string name, string phone, string email, string image = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            Phone = phone;
            Email = email;
            Icon = image;
        }
    }
}