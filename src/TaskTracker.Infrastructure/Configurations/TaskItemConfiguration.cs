using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskTracker.Domain.Models;

namespace TaskTracker.Infrastructure.Configurations;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t  => t.Id);

        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(200);
        builder.Property(t => t.Status)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired();

        builder.Property(t=>t.ProjectId)
            .IsRequired();

        builder.HasOne(t=>t.Project)
            .WithMany(p=>p.Tasks)
            .HasForeignKey(t=>t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
