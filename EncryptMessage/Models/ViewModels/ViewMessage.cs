using System.ComponentModel.DataAnnotations;

namespace EncryptMessage.Models
{
    public class ViewMessage
    {
        public string Message { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(4)]
        public string Key { get; set; }
    }
}
