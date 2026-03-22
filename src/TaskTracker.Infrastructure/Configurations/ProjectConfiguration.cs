using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskTracker.Domain.Models;

namespace TaskTracker.Infrastructure.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
	public void Configure(EntityTypeBuilder<Project> builder)
	{
		builder.ToTable("Projects");

		builder.HasKey(p => p.Id);

		builder.Property(p => p.Name)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(p => p.CreatedAt)
			.IsRequired();

		builder.HasMany(p => p.Tasks)
			.WithOne(p => p.Project)
			.HasForeignKey(p => p.ProjectId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
