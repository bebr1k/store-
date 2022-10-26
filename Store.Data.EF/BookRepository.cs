using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Data.EF
{
    public class BookRepository : IBookRepository
    {
        private readonly DbContextFactory dbContextFactory;

        public BookRepository(DbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public Book[] GetAllByIds(IEnumerable<int> booksIds)
        {
            var dbContext = dbContextFactory.Create(typeof(BookRepository));

            var dtos = dbContext.Books.Where(book => booksIds.Contains(book.Id)).AsEnumerable().Select(Book.Mapper.Map).ToArray(); 

            return dtos;
        }

        public Book[] GetAllByIsbn(string isbn)
        {
            var dbContext = dbContextFactory.Create(typeof(BookRepository));

            if(Book.TryFormatIsbn(isbn, out string formattedIsbn))
            {
                return dbContext.Books.Where(book => book.Isbn == formattedIsbn).Select(Book.Mapper.Map).ToArray();
            }

            return new Book[0];
        }

        public Book[] GetAllByTitleOrAuthor(string titleOrAuthor)
        {
            var dbContext = dbContextFactory.Create(typeof(BookRepository));

            titleOrAuthor = titleOrAuthor.ToLower();

            var dtos = dbContext.Books.Where(book => book.Title.ToLower().Contains(titleOrAuthor) || book.Author.ToLower().Contains(titleOrAuthor)).Select(Book.Mapper.Map).ToArray();

            return dtos;
        }

        public Book GetById(int id)
        {
            var dbContext = dbContextFactory.Create(typeof(BookRepository));

            var dto = dbContext.Books.Single(book => book.Id == id);

            return Book.Mapper.Map(dto);
        }
    }
}
