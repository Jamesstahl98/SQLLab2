using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SQLLab2;

public partial class BookstoreContext : DbContext
{
    public BookstoreContext()
    {
    }

    public BookstoreContext(DbContextOptions<BookstoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrdersPerGenre> OrdersPerGenres { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<StoreSupply> StoreSupplies { get; set; }

    public virtual DbSet<TitlesPerAuthor> TitlesPerAuthors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Initial Catalog=Bookstore;Integrated Security=True;Trust Server Certificate=True;Server SPN=localhost");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__author__3214EC27A34295F7");

            entity.ToTable("author");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BirthDate).HasColumnName("birthDate");
            entity.Property(e => e.DeathDate).HasColumnName("deathDate");
            entity.Property(e => e.FirstName).HasColumnName("firstName");
            entity.Property(e => e.LastName).HasColumnName("lastName");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Isbn).HasName("PK__books__447D36EB413281B1");

            entity.ToTable("books");

            entity.HasIndex(e => e.Isbn, "UQ__books__447D36EA3643F6CF").IsUnique();

            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .HasColumnName("ISBN");
            entity.Property(e => e.Language).HasColumnName("language");
            entity.Property(e => e.Pages).HasColumnName("pages");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.PublishDate).HasColumnName("publishDate");
            entity.Property(e => e.PublisherId).HasColumnName("publisherID");
            entity.Property(e => e.Title).HasColumnName("title");

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("FK__books__publisher__0F824689");

            entity.HasMany(d => d.Authors).WithMany(p => p.BookIsbns)
                .UsingEntity<Dictionary<string, object>>(
                    "BookAuthorJt",
                    r => r.HasOne<Author>().WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__bookAutho__autho__1FB8AE52"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookIsbn")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__bookAutho__bookI__20ACD28B"),
                    j =>
                    {
                        j.HasKey("BookIsbn", "AuthorId").HasName("PK__bookAuth__55489A338DC650A1");
                        j.ToTable("bookAuthorJT");
                        j.IndexerProperty<string>("BookIsbn")
                            .HasMaxLength(13)
                            .HasColumnName("bookISBN");
                        j.IndexerProperty<int>("AuthorId").HasColumnName("authorID");
                    });

            entity.HasMany(d => d.Genres).WithMany(p => p.BookIsbns)
                .UsingEntity<Dictionary<string, object>>(
                    "GenreBookJt",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__genreBook__genre__23893F36"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookIsbn")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__genreBook__bookI__247D636F"),
                    j =>
                    {
                        j.HasKey("BookIsbn", "GenreId").HasName("PK__genreBoo__EE6FAE447194D90A");
                        j.ToTable("genreBookJT");
                        j.IndexerProperty<string>("BookIsbn")
                            .HasMaxLength(13)
                            .HasColumnName("bookISBN");
                        j.IndexerProperty<int>("GenreId").HasColumnName("genreID");
                    });
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__customer__3214EC27E941BBD9");

            entity.ToTable("customers");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Adress).HasColumnName("adress");
            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FirstName).HasColumnName("firstName");
            entity.Property(e => e.LastName).HasColumnName("lastName");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__genre__3214EC27CC068CCA");

            entity.ToTable("genre");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.GenreName).HasColumnName("genreName");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__orders__3214EC271E0E93BF");

            entity.ToTable("orders");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BookIsbn)
                .HasMaxLength(13)
                .HasColumnName("bookISBN");
            entity.Property(e => e.CustomerId).HasColumnName("customerID");

            entity.HasOne(d => d.BookIsbnNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.BookIsbn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__orders__bookISBN__1CDC41A7");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__orders__customer__1BE81D6E");
        });

        modelBuilder.Entity<OrdersPerGenre>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("OrdersPerGenre");

            entity.Property(e => e.GenreName).HasColumnName("genreName");
            entity.Property(e => e.IncomeFromGenre).HasColumnName("incomeFromGenre");
            entity.Property(e => e.Orders).HasColumnName("orders");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__publishe__3214EC2721DBFE90");

            entity.ToTable("publisher");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__stores__3214EC27420BE4C7");

            entity.ToTable("stores");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.StoreName).HasColumnName("storeName");
        });

        modelBuilder.Entity<StoreSupply>(entity =>
        {
            entity.HasKey(e => new { e.StoreId, e.Isbn }).HasName("PK__storeSup__BAE0C55D949B126F");

            entity.ToTable("storeSupply");

            entity.Property(e => e.StoreId).HasColumnName("storeID");
            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .HasColumnName("ISBN");
            entity.Property(e => e.Amount).HasColumnName("amount");

            entity.HasOne(d => d.IsbnNavigation).WithMany(p => p.StoreSupplies)
                .HasForeignKey(d => d.Isbn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__storeSuppl__ISBN__162F4418");

            entity.HasOne(d => d.Store).WithMany(p => p.StoreSupplies)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__storeSupp__store__153B1FDF");
        });

        modelBuilder.Entity<TitlesPerAuthor>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("TitlesPerAuthor");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
