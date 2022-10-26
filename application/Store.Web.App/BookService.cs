using Stor;
using Store.Web.App;

namespace Store
{
    public class BookService
    {   
        private readonly IBookRepository bookRepository;
        public BookService(IBookRepository bookRepository)
        {
            this.bookRepository = bookRepository; 
        }
        public IReadOnlyCollection<BookModel> GetAllByQuery(string query)
        {
            var books = Book.isIsbn(query) 
                ? bookRepository.GetAllByIsbn(query) 
                : bookRepository.GetAllByTitleOrAuthor(query);
            return books.Select(Map)
                        .ToArray();
        }

        public BookModel GetById(int id)
        {
            var book = bookRepository.GetById(id);

            return Map(book);
        }

        private BookModel Map(Book book)
        {

            return new BookModel
            {
                Id  = book.Id,
                Title = book.Title,
                Author = book.Author,
                Isbn = book.Isbn,
                Description = book.Description,
                Price = book.Price,

            };
        }
    }
}
