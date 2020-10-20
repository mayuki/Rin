using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HelloRin.Data
{
    public class MyDbContext : DbContext
    {
        public DbSet<TodoItem> TodoItems { get; private set; } = default!;

        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }
    }
}
