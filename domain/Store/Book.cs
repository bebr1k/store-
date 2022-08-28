using System.Text.RegularExpressions;

namespace Stor;
public class Book
{
    public Book(int id, string title, string isbn, string author, decimal price, string description)
    {
        Id = id;
        Title = title;
        Isbn = isbn;
        Author = author;
        Price = price;
        Description = description;
    }
    public int Id { get; }
    public string Title { get; }
    public string Isbn { get; }
    public string Author { get; }
    public string Description { get; }
    public decimal Price { get; }

    internal static bool isIsbn(string s)
    {
        if (s == null)
            return false;
        s = s.Replace("-", "").
        Replace(" ", "").
        Replace("-", "").ToUpper();

        return Regex.IsMatch(s, "ISBN\\d{10}(\\d{13)?$");
    }   
}
