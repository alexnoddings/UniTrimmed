using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EduLocate.Core;

namespace EduLocate.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<School> Schools { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set the Schools Id to be its primary key
            modelBuilder.Entity<School>().HasKey(s => s.Id);
            modelBuilder.Entity<School>().Property(s => s.Id).ValueGeneratedNever();

            // Ignore the expression-bodies members
            modelBuilder.Entity<School>().Ignore(s => s.OffersPrimaryEducation);
            modelBuilder.Entity<School>().Ignore(s => s.OffersSecondaryEducation);
            modelBuilder.Entity<School>().Ignore(s => s.OffersCollegeEducation);
            modelBuilder.Entity<School>().Ignore(s => s.IsReligious);
            modelBuilder.Entity<School>().Ignore(s => s.PercentBoys);
        }
    }
}