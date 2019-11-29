using System.Collections.Generic;
using MongoDB.Driver;
using Proxy.Models;

namespace Proxy.Service
{
     public class BookService
     {
          private readonly IMongoCollection<Book> _books;

          public BookService(IBookstoreDatabaseSettings settings)
          {
               var client = new MongoClient(settings.ConnectionString);
               var database = client.GetDatabase(settings.DatabaseName);
               _books = database.GetCollection<Book>(settings.BooksCollectionName);
          }

          public List<Book> Get()
          {
               return _books.Find(book => true).ToList();
          }

          public Book Get(string id)
          {
               return _books.Find<Book>(book => book.Id == id).FirstOrDefault();
          }

          public Book Create(Book book)
          {
               _books.InsertOne(book);
               return book;
          }

          public Book Update(Book bookIn, string id)
          {
               _books.ReplaceOne(book => book.Id == id, bookIn);
               return bookIn;
          }
          public void Remove(Book bookIn) =>
               _books.DeleteOne(book => book.Id == bookIn.Id);

          public void Remove(string id) =>
               _books.DeleteOne(book => book.Id == id);
     }
}
