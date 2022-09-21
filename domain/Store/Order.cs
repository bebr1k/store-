using Stor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store
{
    public class Order
    {
        public int Id { get; }

        private List<OrderItem> items;
        public IReadOnlyCollection<OrderItem> Items
        {
            get { return items; }
        }
        public int TotalCount
        {
            get { return items.Sum(item => item.Count); }
        }

        public OrderDelivery Delivery { get; set; }
        public OrderPayment Payment { get; set; }

        public decimal TotalPrice
        {
            get { return items.Sum(item => item.Price * item.Count) + (Delivery?.Amount ?? 0m); }
        }
        public Order(int id, IEnumerable<OrderItem> items)
        {
            if(items == null)
                throw new ArgumentNullException(nameof(items));

            Id = id;

            this.items = new List<OrderItem>(items);

        }

        public string CellPhone { get; set; }
       
        public OrderItem GetItem(int bookId)
        {
            int index = items.FindIndex(item => item.BookId == bookId);

            if (index == -1)
                throw new InvalidOperationException();
            return items[index];
        }


        public void AddOrUpdateItem(Book book,int count)
        {
            if(book == null)
                throw new ArgumentNullException(nameof(book));

            int index = items.FindIndex(item => item.BookId == book.Id);
            if (index == -1)
            {
                items.Add(new OrderItem(book.Id, count, book.Price));
            }
            else
            {
                items[index].Count += count;
            }

            
        }

       
        public void RemoveItem(int bookId)
        {           
            int index = items.FindIndex(item => item.BookId == bookId);

            if(index == -1)            
                throw new InvalidOperationException();          
                          
            items.RemoveAt(index);
        }

    }
}
