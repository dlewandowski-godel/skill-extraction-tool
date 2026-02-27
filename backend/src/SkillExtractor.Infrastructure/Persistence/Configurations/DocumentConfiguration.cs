using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillExtractor.Domain.Entities;

namespace SkillExtractor.Infrastructure.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
  public void Configure(EntityTypeBuilder<Document> builder)
  {
    builder.HasKey(d => d.Id);

    builder.Property(d => d.FileName)
        .IsRequired()
        .HasMaxLength(512);

    builder.Property(d => d.FilePath)
        .IsRequired()
        .HasMaxLength(1024);

    builder.Property(d => d.DocumentType)
        .IsRequired();

    builder.Property(d => d.Status)
        .IsRequired();

    builder.Property(d => d.UploadedAt)
        .IsRequired();

    builder.Property(d => d.IsActive)
        .IsRequired();

    builder.Property(d => d.ErrorMessage)
        .HasMaxLength(2048);

    // Index for common queries: get documents by user
    builder.HasIndex(d => d.UserId);

    // Index for getting active document per user+type
    builder.HasIndex(d => new { d.UserId, d.DocumentType, d.IsActive });
  }
}
