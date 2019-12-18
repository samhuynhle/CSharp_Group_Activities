using Microsoft.EntityFrameworkCore;

namespace qwerty.Models
{
    // Wrapper model for users, activities, and associations for Controller
    public class MyContext: DbContext
    {
        public MyContext(DbContextOptions options): base(options) { }
        public DbSet<User> Users {get;set;}
        public DbSet<DojoActivity> DojoActivities {get;set;}
        public DbSet<Association> Associations {get;set;}
    }
}