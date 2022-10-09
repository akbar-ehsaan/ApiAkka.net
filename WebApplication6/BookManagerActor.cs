using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication6
{
  
    public delegate IActorRef BooksManagerActorProvider();
  
    public class GetBooks
    {
        private GetBooks() { }
        public static GetBooks Instance { get; } = new GetBooks();
    }
    public class GetBookById
    {
        public GetBookById(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    public class BookNotFound
    {
        private BookNotFound() { }
        public static BookNotFound Instance { get; } = new BookNotFound();
    }

    public class CreateBook
    {
        public CreateBook(string title, string author, decimal cost, int inventoryAmount)
        {
            Title = title;
            Author = author;
            Cost = cost;
            InventoryAmount = inventoryAmount;
        }

        public string Title { get; }
        public string Author { get; }
        public decimal Cost { get; }
        public int InventoryAmount { get; }
    }
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Cost { get; set; }
        public int InventoryAmount { get; set; }
    }
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Cost { get; set; }
        public int InventoryAmount { get; set; }
    }
    public class BooksManagerActor : ReceiveActor
    {
        private readonly Dictionary<Guid, Book> _books = new Dictionary<Guid, Book>();

        public BooksManagerActor()
        {
            Receive<CreateBook>(command =>
            {
                var newBook = new Book
                {
                    Id = Guid.NewGuid(),
                    Title = command.Title,
                    Author = command.Author,
                    Cost = command.Cost,
                    InventoryAmount = command.InventoryAmount,
                };

                _books.Add(newBook.Id, newBook);
            });

            Receive<GetBookById>(query =>
            {
                if (_books.TryGetValue(query.Id, out var book))
                    Sender.Tell(GetBookDto(book));
                else
                    Sender.Tell(BookNotFound.Instance);
            });

            Receive<GetBooks>(query =>
                Sender.Tell(_books.Select(x => GetBookDto(x.Value)).ToList()));
        }

        private static BookDto GetBookDto(Book book) => new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Cost = book.Cost,
            InventoryAmount = book.InventoryAmount
        };

    }
}
