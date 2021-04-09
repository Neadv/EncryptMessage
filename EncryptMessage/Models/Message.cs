﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptMessage.Models
{
    public class Message
    {
        [Column(TypeName = "varchar(6)")]
        public string Id { get; set; }

        [Required]
        public byte[] Value { get; set; }
        [Required]
        public byte[] IV { get; set; }
    }
}