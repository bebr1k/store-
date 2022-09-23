
namespace Store.Contractors
{
    public interface IDeliveryService
    {
        public string UniqueCode { get; }
        string Title { get; }

        Form CreateForm(Order order);
        Form MoveNext(int orderId, int step, IReadOnlyDictionary<string, string> value);

        OrderDelivery GetDelivery(Form form);
    }
}

