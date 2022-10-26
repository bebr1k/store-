using Store.Data;
using System.Text.RegularExpressions;

namespace Stor;
public class Book
{

    internal Book(BookDto dto)
    {
        this.dto = dto;
    }

    private readonly BookDto dto;

    public int Id => dto.Id;

    public string Isbn 
    { 
        get => dto.Isbn;
        set
        {
            if (TryFormatIsbn(value, out string formattedString))
                value = formattedString;            
            throw new ArgumentException(nameof(Isbn));
        }
    }

    public string Title 
    { 
        get => dto.Title;
        set
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(Title));

            dto.Title = value;
        }          
    }

    public string Author 
    {
        get => dto.Author;
        set => dto.Author = value;
       
    }

    public string Description 
    {
        get => dto.Description;
        set => dto.Description = value;
    }

    public decimal? Price 
    {
        get => dto.Price;
        set => dto.Price = value; 
    }


    public static bool TryFormatIsbn(string isbn, out string formattedIsbn)
    {
        if (isbn == null)
        {
            formattedIsbn = null;
            return false;
        }

        formattedIsbn = isbn.Replace("-", "")
                            .Replace(" ", "")
                            .ToUpper();
                            
        return Regex.IsMatch(formattedIsbn, "ISBN\\d{10}(\\d{13)?$");
    }   

    public static bool isIsbn(string isbn)
        => TryFormatIsbn(isbn, out _);

    public static class DtoFactory
    {
        public static BookDto Create(string isbn, string author, string title, string description, decimal price)
        {
            if(TryFormatIsbn(isbn, out string formattedString))
                isbn = formattedString;
            else
                throw new ArgumentException(nameof(isbn));
            
            if(string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException(nameof(title));

            return new BookDto
            {
                Isbn = isbn,
                Author = author?.Trim(),
                Title = title.Trim(),
                Description = description?.Trim(),
                Price = price
            };

        }
    }

    public static class Mapper
    {
        public static Book Map(BookDto dto) => new Book(dto);

        public static BookDto Map(Book domain) => domain.dto;
    }
}
