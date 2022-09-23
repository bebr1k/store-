using Stor;

namespace Store
{
    public interface IBookRepository
    {
        public Book[] GetAllByTitleOrAuthor(string titleOrAuthor);
        public Book[] GetAllByIsbn(string isbn);
        Book GetById(int id);
        Book[] GetAllByIds(IEnumerable<int> booksIds);
    }
}
