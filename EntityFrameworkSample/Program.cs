using System;
using System.Linq;

namespace EntityFrameworkSample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var dbContext = new TestDbContext())
            {
                if (!dbContext.Customers.Any(c => c.Code == "1"))
                {
                    dbContext.Customers.Add(new Customer
                    {
                        Code = "1",
                        Name = "Customer 1",
                    });

                    dbContext.SaveChanges();
                }

                AddDocument(dbContext, 1);
            }

            new Program().AddDocument(null, 0);
            Console.ReadLine();
        }

        private void AddDocument(TestDbContext dbContext, int customerId)
        {

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Document document = CreateDocument(customerId);
                    dbContext.Documents.Add(document);
                    dbContext.SaveChanges();

                    AddItems(document);
                    document.Total = document.Items.Sum(c => c.TotalPrice);
                    document.Items.Last().Name = null;

                    dbContext.SaveChanges();
                    transaction.Commit();

                    Console.WriteLine($"Add Document with Id {document.Id}");
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }

        private static void AddItems(Document document)
        {
            for (int i = 1; i <= 3; i++)
            {
                document.Items.Add(new DocumentItem
                {
                    Name = $"Item {i}",
                    Quantity = 10 * i,
                    UnitPrice = 1,
                });
            }

            foreach (var item in document.Items)
            {
                item.TotalPrice = item.Quantity * item.UnitPrice;
            }
        }

        private static Document CreateDocument(int customerId)
        {
            return new Document()
            {
                CreationDate = DateTime.Now,
                CustomerId = customerId,
                DocumentDate = DateTime.Now,
                DocumentNumber = "1",
            };
        }

        private static void AddDocument(TestDbContext dbContext, int customerId)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Document document = new Document()
                    {
                        CreationDate = DateTime.Now,
                        CustomerId = customerId,
                        DocumentDate = DateTime.Now,
                        DocumentNumber = "1",
                    };

                    dbContext.Documents.Add(document);
                    dbContext.SaveChanges();

                    for (int i = 1; i <= 3; i++)
                    {
                        document.Items.Add(new DocumentItem
                        {
                            Name = $"Item {i}",
                            Quantity = 10 * i,
                            UnitPrice = 1,
                        });
                    }

                    foreach (var item in document.Items)
                    {
                        item.TotalPrice = item.Quantity * item.UnitPrice;
                    }

                    document.Total = document.Items.Sum(c => c.TotalPrice);

                   
                    document.Items.Last().Name = null;
                    dbContext.SaveChanges();
                    transaction.Commit();

                    Console.WriteLine($"Add Document with Id {document.Id}");
                }
                catch
                {
                    transaction.Rollback();                   
                }
            }
        }
    }
}
