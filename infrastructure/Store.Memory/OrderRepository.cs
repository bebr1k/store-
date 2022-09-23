
namespace Store.Memory
{
    public class OrderRepository : IOrderRepository
    {
        private readonly List<Order> orders = new List<Order>();
        public Order Create()
        {
            int nextId = orders.Count + 1;
            var order = new Order(nextId,new List<OrderItem>());
            orders.Add(order);
            return order; 
        }

        public Order GetById(int id)
        {
            return orders.Single(order=>order.Id == id);
        }

        public void Update(Order order)
        {
            
        }
    }
}
