using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp4.Model
{
    public class NContact
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Icon { get; set; }

        public NContact(string name, string phone, string email, string icon = null)
        {
            Name = name;
            Phone = phone;
            Email = email;
            Icon = icon;
        }
    }
}