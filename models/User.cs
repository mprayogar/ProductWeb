using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductWeb.models
{
    public class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}