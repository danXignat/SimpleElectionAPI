using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) {
        }

        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // Configure one-to-many relationship
            modelBuilder.Entity<Candidate>()
                .HasMany(c => c.Votes)
                .WithOne(v => v.Candidate)
                .HasForeignKey(v => v.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Additional configurations
            modelBuilder.Entity<Candidate>().Property(c => c.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Candidate>().Property(c => c.Party).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Vote>().Property(v => v.PollingStation).IsRequired().HasMaxLength(100);
        }
    }
}
