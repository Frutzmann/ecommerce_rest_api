using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using quest_web.Models;

namespace quest_web
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> contextOptions) : base(contextOptions)
        {
             ChangeTracker.StateChanged += UpdateTimestamps;
             ChangeTracker.Tracked += UpdateTimestamps;
        }

        public DbSet<User> User { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Basket> Basket { get; set; }
        public DbSet<Order> Order { get; set; }


        

        private static void UpdateTimestamps(object sender, EntityEntryEventArgs e)
        {
            if(e.Entry.Entity is IHasTimestamps entryWithTimestamps)
            {
                switch(e.Entry.State)
                {
                    case EntityState.Modified:
                        entryWithTimestamps.updatedDate = System.DateTime.Now;
                        break;
                    case EntityState.Added:
                        entryWithTimestamps.creationDate = System.DateTime.Now;
                        break;
                }
            }
        }
        }
}
