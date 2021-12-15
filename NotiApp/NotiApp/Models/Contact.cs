using System;
using System.Collections.Generic;
using System.Text;

namespace NotiApp.Models
{
    public class Contact
    {
        public string Name { get; set; }
        public string Mac { get; set; }

        public Contact(string name, string mac)
        {
            Name = name;
            Mac = mac;
        }
    }
}
