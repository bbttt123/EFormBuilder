using Microsoft.EntityFrameworkCore;
using EFormBuilder.Domain.Entities;

namespace EFormBuilder.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options)
	{
	}

	public DbSet<User> Users { get; set; }

	public DbSet<Form> Forms { get; set; }

	public DbSet<Field> Fields { get; set; }

	public DbSet<FieldOption> FieldOptions { get; set; }

	public DbSet<Response> Responses { get; set; }

	public DbSet<Answer> Answers { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}