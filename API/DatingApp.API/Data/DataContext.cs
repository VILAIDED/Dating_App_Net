using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Enities;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> AppUsers {get;set;}

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        
    }
}