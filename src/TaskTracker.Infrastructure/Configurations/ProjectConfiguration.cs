using Microsoft.EntityFramework;
using Microsoft.EntityFramework.Metadata.Builders;
using TaskTracker.Domain.Models;

namespace TaskTracker.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
	public void Configure(EnityTypeBuilder<Project> builder)
	{
		builder.ToTable("Projects");

		builder.HasKey(p => p.Id);

		builder.Property(p => p.Name)
			.IsRequired()
			.HasMaxLength(200);

		builder.Propetry(p => p.CreatedAt)
			.IsRequired();

		builder.HasMany(p => p.Tasks)
			.WithOne(p => p.Project)
			.HasForeignKey(p => p.Id)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
