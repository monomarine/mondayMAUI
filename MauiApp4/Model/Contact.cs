using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp4.Model
{
    class NContact
    {
        public Image Icon { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public NContact(string name, string phone, string email, Image image = null)
        {
            Name = name;
            Phone = phone;
            Email = email;
            if (image == null)
                Icon = new Image { Source = "Resourses/Images/dotnet_bot.png" };
            else Icon = image;
        }
    }
}
