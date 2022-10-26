
namespace Store.Contractors
{
    public interface IDeliveryService
    {
        public string Name { get; }
        string Title { get; }

        Form FirstForm(Order order);

        Form NextForm(int step, IReadOnlyDictionary<string, string> values);

        OrderDelivery GetDelivery(Form form);
    }
}

