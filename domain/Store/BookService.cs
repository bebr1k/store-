using Stor;

namespace Store
{
    public class BookService
    {   
        private readonly IBookRepository bookRepository;
        public BookService(IBookRepository bookRepository)
        {
            this.bookRepository = bookRepository; 
        }
        public Book[] GetAllByQuery(string query)
        {            
            if (Book.isIsbn(query))
            {
                return bookRepository.GetAllByIsbn(query);
            }            
            return bookRepository.GetAllByTitleOrAuthor(query);                                     
        }
    }
}
