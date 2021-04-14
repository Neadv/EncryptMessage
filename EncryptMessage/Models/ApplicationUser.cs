using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace EncryptMessage.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<Message> Messages { get; set; }
    }
}
