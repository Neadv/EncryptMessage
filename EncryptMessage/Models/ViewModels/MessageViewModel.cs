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
        [Required]
        public string Message { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Key { get; set; }
    }
}
