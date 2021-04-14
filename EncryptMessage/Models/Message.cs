using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptMessage.Models
{
    [Index("Code", IsUnique = true)]
    public class Message
    {
        public Guid MessageId { get; set; }

        [Column(TypeName = "varchar(6)")]
        public string Code { get; set; }

        [Required]
        public byte[] Value { get; set; }
        [Required]
        public byte[] IV { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
