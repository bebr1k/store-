namespace Store.Contractors
{
    public class CashPaymentService : IPaymentService
    {
        public string UniqueCode => "Cash";

        public string Title => "Оплата наличными";

        public Form CreateForm(Order order)
        {
            return new Form(UniqueCode, order.Id, 1, true, new Field[0]);
        }

        public OrderPayment GetPayment(Form form)
        {
            if (!form.IsFinal || UniqueCode != "Cash")
                throw new InvalidOperationException();
            return new OrderPayment(UniqueCode, "Оплата наличными", new Dictionary<string, string>());
        }

        public Form MoveNext(int orderId, int step, IReadOnlyDictionary<string, string> value)
        {
            if (step != 1)
            {
                throw new InvalidOperationException();
            }
            return new Form(UniqueCode, orderId, 2, true, new Field[0]);
        }
    }
}
