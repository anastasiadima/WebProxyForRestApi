using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Proxy.Models;
using Proxy.Service;
using StackExchange.Redis;

namespace Proxy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
         static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

         static readonly IDatabase db = redis.GetDatabase();

         private BookService _bookService;

         public BooksController(BookService bookService)
         {
              _bookService = bookService;
         }

          [HttpGet]
         public ActionResult<List<Book>> Get()
         {
              var books = _bookService.Get();
              UpdateCache();
               return books;
         }

         private void UpdateCache()
         {
              var books = _bookService.Get();
              var bookName = books.Select(book => book.BookName).ToList();
              var lenght = (int)db.ListLength("cachebooks");
              for (int i = 0; i < lenght; i++)
              {
                   db.ListLeftPop("cachebooks");
              }
              bookName.ForEach(name => { db.ListLeftPush("cachebooks", name); });
              db.StringSet("isUpToDate", "yes");
          }

         [HttpGet("list")]
         public ActionResult<List<string>> GetList()
         {
              var books = GetBooksFromCache();
              
               return books;
         }

          [HttpGet("{id:length(24)}")]
         public ActionResult<Book> Get(string id)
         {
              return _bookService.Get(id);
         }

         [HttpPost("create")]
         public ActionResult<Book> Insert(Book book)
         {
              book = _bookService.Create(book);
              db.StringSet("isUpToDate", "no");
              db.ListLeftPush("cachebooks", book.BookName);

              return  book;
          }

          public List<string> GetBooksFromCache()
         {
              var books = new List<string>();
              var isUpToDate = db.StringGet("isUpToDate");

              if (isUpToDate.HasValue && isUpToDate.ToString() == "no")
              {
                   //update la cache
                   UpdateCache();
              }

              var lenght = (int)db.ListLength("cachebooks");
              for (int i = 0; i < lenght; i++)
              {
                   books.Add(db.ListGetByIndex("cachebooks", i).ToString());
              }
               return books;
         }
          
    }
}