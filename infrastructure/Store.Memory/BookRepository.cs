using System.Linq;
using Stor;

namespace Store.Memory
{
    public class BookRepository : IBookRepository
    {
        private readonly Book[] books = new[]
        {
            new Book(1,"Art of Programming","ISBN 1111111111", "D.Knuth",7.19m,"The Art of Computer Programming (TAOCP) is a comprehensive monograph written by the computer scientist Donald Knuth presenting programming algorithms and their analysis. Volumes 1–5 are intended to represent the central core of computer programming for sequential machines."),
            new Book(2,"Refactoring","ISBN 2222222222", "M.Fowler",12.45m,"The guide to how to transform code with safe and rapid process, vital to keeping it cheap and easy to modify for future needs."),
            new Book(3, "C Programming Language","ISBN 3333333333333", "B.Kernighan",14.98m,"The authors present the complete guide to ANSI standard C language programming. Written by the developers of C, this new version helps readers keep up with the finalized ANSI standard for C while showing how to take advantage of C's rich set of operators, economy of expression, improved control flow, and data structures. The 2/E has been completely rewritten with additional examples and problem sets to clarify the implementation of difficult language constructs.")

        };

        public Book GetById(int id)
        {
            return books.Single(book => book.Id == id);
        }

        public Book[] GetAllByIsbn(string isbn)
        {
            return books.Where(x => x.Isbn==isbn).ToArray();
        }

        public Book[] GetAllByTitleOrAuthor(string query)
        {
                return books.Where(book => book.Title.Contains(query) || book.Author.Contains(query)).ToArray();           
        }

    }
}