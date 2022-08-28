using Stor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public interface IBookRepository
    {
        public Book[] GetAllByTitleOrAuthor(string titleOrAuthor);
        public Book[] GetAllByIsbn(string isbn);
        Book GetById(int id);

    }
}
