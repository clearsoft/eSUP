using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eSUP.Model
{
    public class MainContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Planner> Planners { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Part> Parts { get; set; }

        // Constructors
        public MainContext() : base()
        {
            Database.Migrate();
        }

        public MainContext(DbContextOptions<MainContext> options) : base(options)
        {
            //Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Part>()
                .HasOne<Question>()
                .WithMany(q => q.Parts)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Question>()
                .HasOne<Exercise>()
                .WithMany(e => e.Questions)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Exercise>()
                    .HasOne<Planner>()
                    .WithMany(p => p.Exercises)
                    .OnDelete(DeleteBehavior.Cascade);

            //builder.Entity<PlannerUser>()
            //    .HasKey(pu => new { PlannerId = pu.Planner!.Id, UserId = pu.User!.Id });

            //builder.Entity<PlannerUser>()
            //    .HasOne(pu => pu.Planner)
            //    .WithMany(p => p.PlannerUsers)
            //    .HasForeignKey(pu => pu.Planner!.Id)
            //    .OnDelete(DeleteBehavior.Cascade);

            //builder.Entity<PlannerUser>()
            //    .HasOne(pu => pu.User)
            //    .WithMany(u => u.PlannerUsers)
            //    .HasForeignKey(pu => pu.User!.Id)
            //    .OnDelete(DeleteBehavior.Cascade);


        }
    }

}
