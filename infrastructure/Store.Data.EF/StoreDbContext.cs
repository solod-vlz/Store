using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Data.EF
{
    public class StoreDbContext : DbContext
    {
        public DbSet<BookDto> Books { get; set; }

        public DbSet<OrderDto> Orders { get; set; }

        public DbSet<OrderItemDto> OerderItems { get; set; }

        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            BuildBooks(modelBuilder);
            BuildOrders(modelBuilder);
            BuildOrderItems(modelBuilder);
        }

        private void BuildOrderItems(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItemDto>(action =>
            {
                action.Property(dto => dto.Price)
                      .HasColumnType("money");

                action.HasOne(dto => dto.Order)
                      .WithMany(orderDto => orderDto.Items)
                      .IsRequired();
            });
        }

        private void BuildOrders(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDto>(action =>
            {
                action.Property(dto => dto.MobilePhone)
                      .HasMaxLength(20);

                action.Property(dto => dto.DeliveryUniqueCode)
                      .HasMaxLength(40);

                action.Property(dto => dto.DeliveryPrice)
                      .HasColumnType("money");

                action.Property(dto => dto.PaymentServiceName)
                      .HasMaxLength(40);

                action.Property(dto => dto.DeliveryParameters)
                      .HasConversion(
                                value => JsonConvert.SerializeObject(value),
                                value => JsonConvert.DeserializeObject<Dictionary<string, string>>(value))
                      .Metadata.SetValueComparer(DictionaryComparer);

                action.Property(dto => dto.PaymentParameters)
                      .HasConversion(
                          value => JsonConvert.SerializeObject(value),
                          value => JsonConvert.DeserializeObject<Dictionary<string, string>>(value))
                      .Metadata.SetValueComparer(DictionaryComparer);
            });
        }

        private readonly ValueComparer DictionaryComparer =
            new ValueComparer<Dictionary<string, string>>(
                (dictionary1, dictionary2) => dictionary1.SequenceEqual(dictionary2),
                 dictionary => dictionary.Aggregate(
                    0,
                    (a, p) => HashCode.Combine(HashCode.Combine(a, p.Key.GetHashCode()), p.Value.GetHashCode())
                 )
        );

        private void BuildBooks(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookDto>(action =>
            {
                action.Property(dto => dto.Isbn)
                      .HasMaxLength(17)
                      .IsRequired();

                action.Property(dto => dto.Title)
                      .IsRequired();

                action.Property(dto => dto.Price)
                      .HasColumnType("money");

                action.HasData(
                     new BookDto
                     {
                         Id = 1,
                         Isbn = "ISBN1231231332",
                         Author = "D. Knuth",
                         Title = "Art Of Programming, Vol. 1",
                         Description = "The bible of all fundamental algorithms and the work that taught many of today's software developers most of what they know about computer programming.",
                         Price = 69.68m,
                     },
                    new BookDto
                    {
                        Id = 2,
                        Isbn = "ISBN1231266666",
                        Author = "M. Fowler",
                        Title = "Refactoring: Improving the Design of Existing Code (2nd Edition)",
                        Description = "A guide to refactoring, the process of changing a software system so that it does not alter" +
                                      "the external behavior of the code yet improves its internal structure, for professional" +
                                      "programmers.Early chapters cover general principles, rationales, examples, and testing." +
                                      "The heart of the book is a catalog of refactorings, organized in chapters on composing" +
                                      "methods, moving features between objects, organizing data, simplifying conditional" +
                                      "expressions, and dealing with generalizations",

                        Price = 40.01m,
                    },
                    new BookDto
                    {
                        Id = 3,
                        Isbn = "ISBN1231255555",
                        Author = "B. Kernighan",
                        Title = "C Programming Language",
                        Description = "C is a general-purpose programming language which features economy of" +
                                      "expression, modern control flow and data structures, and a rich set of operators." +
                                      "C is not a \"very high level\" language, nor a \"big\" one, and is not specialized to" +
                                      "any particular area of application. But its absence of restrictions and its generality " +
                                      "make it more convenient and effective for many tasks than supposedly more powerful languages",
                        Price = 25.34m,
                    });
            });
        }
    }
}
