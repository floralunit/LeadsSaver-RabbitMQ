using LeadsSaver_RabbitMQ.Consumers;
using LeadsSaverRabbitMQ.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata;

public class AstraContext : DbContext
{
    public DbSet<OuterMessage> OuterMessage { get; set; }
    public DbSet<OuterMessageReader> OuterMessageReader { get; set; }
    public DbSet<EmployeeIdResult> EmployeeIdResults { get; set; }
    public AstraContext(DbContextOptions<AstraContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<OuterMessage>()
           .ToTable("OuterMessage", "stella")
           .ToTable(tb => tb.HasTrigger("[stella].[TR_OuterMessage_AU_101]"))
           .HasKey(b => b.OuterMessage_ID);
        modelBuilder.Entity<OuterMessageReader>()
           .ToTable("OuterMessageReader", "stella")
           .HasKey(b => b.OuterMessageReader_ID);
        modelBuilder.Entity<EmployeeIdResult>()
           .HasNoKey();
    }
}

public class EmployeeIdResult
{
    public Guid Row_ID { get; set; }
}
