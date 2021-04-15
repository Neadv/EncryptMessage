using System;
using System.ComponentModel.DataAnnotations;

namespace EncryptMessage.Models
{
    public class AllowedUserMessages
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid MessageId { get; set; }
        public Message Message { get; set; }
    }
}
