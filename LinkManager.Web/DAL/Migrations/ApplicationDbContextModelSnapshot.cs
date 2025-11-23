using System;
using LinkManager.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace LinkManager.Web.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("LinkManager.Web.Models.PageLink", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Category")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("ErrorMessage")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int?>("HttpStatusCode")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<DateTime?>("LastCheckedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Notes")
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<long?>("ResponseTimeMs")
                        .HasColumnType("bigint");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("Pending");

                    b.Property<string>("Title")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.HasKey("Id");

                    b.HasIndex("Category")
                        .HasDatabaseName("IX_PageLinks_Category");

                    b.HasIndex("CreatedAt")
                        .HasDatabaseName("IX_PageLinks_CreatedAt");

                    b.HasIndex("Status")
                        .HasDatabaseName("IX_PageLinks_Status");

                    b.HasIndex("Url")
                        .IsUnique()
                        .HasDatabaseName("IX_PageLinks_Url");

                    b.ToTable("PageLinks", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Category = "Search Engine",
                            CreatedAt = new DateTime(2024, 11, 23, 0, 0, 0, 0, DateTimeKind.Utc),
                            Description = "Mecanismo de busca",
                            IsActive = true,
                            Status = "Pending",
                            Title = "Google",
                            Url = "https://www.google.com"
                        },
                        new
                        {
                            Id = 2,
                            Category = "Development",
                            CreatedAt = new DateTime(2024, 11, 23, 0, 0, 0, 0, DateTimeKind.Utc),
                            Description = "Plataforma de desenvolvimento",
                            IsActive = true,
                            Status = "Pending",
                            Title = "GitHub",
                            Url = "https://www.github.com"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
