using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptMessage.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DbContext> options) : base(options) { }
    }
}
