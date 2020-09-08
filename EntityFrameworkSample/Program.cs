using System;
using System.Data.Entity;
using System.Linq;

namespace EntityFrameworkSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Document newDocument = null;
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

                newDocument = AddDocument(dbContext, 1);
            }

            Console.WriteLine("Press Any Key to Delete ......");
            Console.ReadKey();
            DeleteDocument(newDocument);
            Console.WriteLine($"Item will ID {newDocument.Id}......");

            //Update ==============================================
            Document documentToUpdate = SimulateUIChanges();
            //
            UpdateDocument(documentToUpdate);
            Console.WriteLine("Item Updated");

            Console.ReadLine();
        }

        private static void UpdateDocument(Document documentToUpdate)
        {
            if (documentToUpdate == null)
                return;

            using (TestDbContext dbContext = new TestDbContext())
            {
                var currentDocument = dbContext.Documents.Include(c => c.Items)
                    .FirstOrDefault(c => c.Id == documentToUpdate.Id);
                //If not exist
                if (currentDocument == null)
                    return;

                currentDocument.DocumentNumber = documentToUpdate.DocumentNumber;

                //Remove the deleted items from UI
                foreach (var item in currentDocument.Items.ToList())
                {
                    if (documentToUpdate.Items.Any(c => c.Id == item.Id) == false)
                        dbContext.DocumentItems.Remove(item);
                }

                // Add new && update existing item
                foreach (var item in documentToUpdate.Items)
                {
                    DocumentItem currentDocumentItem = null;
                    if (item.Id > 0)
                        currentDocumentItem = currentDocument.Items
                            .FirstOrDefault(c => c.Id == item.Id);

                    if (currentDocumentItem == null)
                    {
                        currentDocumentItem = new DocumentItem();
                        currentDocument.Items.Add(currentDocumentItem);
                    }


                    currentDocumentItem.Name = item.Name;
                    currentDocumentItem.Quantity = item.Quantity;
                    currentDocumentItem.UnitPrice = item.UnitPrice;
                    currentDocumentItem.TotalPrice = item.Quantity * item.UnitPrice;
                }

                currentDocument.Total = currentDocument.Items.Sum(c => c.TotalPrice);
                dbContext.SaveChanges();
            }
        }

        private static Document SimulateUIChanges()
        {
            var documentToUpdate = (new TestDbContext()).Documents.Include("Items")
                            .FirstOrDefault(c => c.Id == 2);
            documentToUpdate.DocumentNumber = "20";
            //Delete
            var firstItem = documentToUpdate.Items.First();
            documentToUpdate.Items.Remove(firstItem);
            //Update
            var itemToEdit = documentToUpdate.Items.Last();
            itemToEdit.Name = "Update Name";
            itemToEdit.Quantity = 10;
            itemToEdit.UnitPrice = 3;
            //Add
            for (int i = 1; i <= 4; i++)
            {
                documentToUpdate.Items.Add(new DocumentItem
                {
                    Name = $"Item {i}",
                    Quantity = 10 * i,
                    UnitPrice = 1,
                });
            }

            return documentToUpdate;
        }

        private static void DeleteDocument(Document documentToDelete)
        {
            if (documentToDelete == null)
                return;

            using (var dbContext = new TestDbContext())
            {
                var document = dbContext.Documents
                    .Include("Items")
                    .FirstOrDefault(c => c.Id == documentToDelete.Id);
                if (document != null)
                {
                    foreach (var item in document.Items.ToList())
                    {
                        dbContext.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                        //dbContext.DocumentItems.Remove(item);
                        //document.Items.Remove(item);
                    }

                    dbContext.Documents.Remove(document);
                    dbContext.SaveChanges();
                }
            }
        }

        private static Document AddDocument(TestDbContext dbContext, int customerId)
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

                    //document.Items.Last().Name = null;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    Console.WriteLine($"Add Document with Id {document.Id}");
                    return document;
                }
                catch
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }
    }
}
