using Backend.Models;
using Backend.Models.Templates;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Session> Sessions { get; set; } = null!;
    public DbSet<Workout> Workouts { get; set; } = null!;
    public DbSet<Exercise> Exercises { get; set; } = null!;
    public DbSet<Set> Sets { get; set; } = null!;

    public DbSet<Set_Template> SetTemplates { get; set; } = null!;
    public DbSet<Exercise_Template> ExerciseTemplates { get; set; } = null!;
    public DbSet<Workout_Template> WorkoutTemplates { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Session>()
            .HasOne(s => s.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Workout_Template>()
            .HasOne<ApplicationUser>()
            .WithMany(u => u.WorkoutTemplates)
            .HasForeignKey(wt => wt.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Session>()
            .HasOne(s => s.ExecutedWorkout)
            .WithMany()
            .HasForeignKey(s => s.ExecutedWorkoutId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Exercise>()
            .HasOne(e => e.Workout)
            .WithMany(w => w.Exercises)
            .HasForeignKey(e => e.WorkoutId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Set>()
            .HasOne(s => s.Exercise)
            .WithMany(e => e.Sets)
            .HasForeignKey(s => s.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Workout>()
            .HasOne(w => w.Template)
            .WithMany()
            .HasForeignKey(w => w.WorkoutTemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Exercise>()
            .HasOne(e => e.Template)
            .WithMany()
            .HasForeignKey(e => e.ExerciseTemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Set>()
            .HasOne(s => s.Template)
            .WithMany()
            .HasForeignKey(s => s.SetTemplateId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
