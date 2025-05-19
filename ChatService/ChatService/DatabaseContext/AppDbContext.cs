using ChatService.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ChatService.DatabaseContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Message> Messages => Set<Message>();
    }
}
