using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptMessage.Models
{
    public class MessageViewModel
    {
        public string Code { get; set; }

        [Required]
        [MinLength(4)]
        public string Message { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(4)]
        public string Key { get; set; }

        public bool IsPrivate { get; set; } = false;
        public bool IsDisposable { get; set; } = false;
        public bool lockoutOnFailure { get; set; } = false;

        public List<string> Users { get; set; } = new List<string>();
    }
}
