using DAW.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static DAW.Models.UserGroup;

namespace DAW.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupPost> GroupPosts { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<UserRelationships> UserRelationships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserGroup>().HasKey(x => new { x.Id, x.UserId, x.GroupId });

            modelBuilder.Entity<UserGroup>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserGroups)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<UserGroup>()
                .HasOne(x => x.Group)
                .WithMany(x => x.UserGroups)
                .HasForeignKey(x => x.GroupId);

            modelBuilder.Entity<FriendRequest>().HasKey(x => new { x.Id, x.UserIdSender, x.UserIdReceiver });

            modelBuilder.Entity<FriendRequest>()
                .HasOne(ur => ur.Reciever)
                .WithMany()
                .HasForeignKey(ur => ur.UserIdReceiver)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(ur => ur.Sender)
                .WithMany()
                .HasForeignKey(ur => ur.UserIdSender)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserRelationships>().HasKey(x => new { x.Id, x.UserId1, x.UserId2 });

            modelBuilder.Entity<UserRelationships>()
                .HasOne(ur => ur.User1)
                .WithMany()
                .HasForeignKey(ur => ur.UserId1)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserRelationships>()
                .HasOne(ur => ur.User2)
                .WithMany()
                .HasForeignKey(ur => ur.UserId2)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
