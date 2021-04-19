using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EncryptMessage.Models
{
    public class EditMessageViewModel
    {
        [Required]
        public string Code { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsDisposable { get; set; }
        public bool LookoutOnFailure { get; set; }
        public List<string> Users { get; set; } = new List<string>();
    }
}
