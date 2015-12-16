using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Bitministry_Task.Models
{
    public class BankAccountDb : DbContext
    {
        public DbSet<BankAccount> Generaltable { get; set; }
        public DbSet<Transactionsdetail> transdetails { get; set; }
    }
}