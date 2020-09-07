using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkSample
{
    public class TestDbContext : DbContext
    {
        public TestDbContext() 
            : this(ConfigurationManager.ConnectionStrings["TestConnectionString"].ConnectionString)
        {
            //Database.SetInitializer<TestDbContext>(new CreateDatabaseIfNotExists<TestDbContext>());
            Database.SetInitializer<TestDbContext>(null);
        }
        public TestDbContext(string connectionString) : base(connectionString)
        { 

        }

        public DbSet<Document> Documents { get; set; }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Document>()
            //    .Property(c => c.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

    }


    public class Document
    {
        public Document()
        {
            Items = new HashSet<DocumentItem>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DocumentDate { get; set; }

        [Required]
        [StringLength(10)]
        public string DocumentNumber { get; set; }

        public double Total { get; set; }
        
        public DateTime CreationDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public ICollection<DocumentItem> Items { get; set; }
    }

    public class DocumentItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        
        public int DocumentId { get; set; }

        //[Required]
        [StringLength(100)]
        public string Name { get; set; }

        public double Quantity { get; set; }

        public double UnitPrice { get; set; }

        public double TotalPrice { get; set; }

    }

    public class Customer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
