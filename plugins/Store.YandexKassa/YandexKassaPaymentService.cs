using Store.Contractors;
using Store.Web.Contractors;

namespace Store.YandexKassa
{
    public class YandexKassaPaymentService : IWebContractorService, IPaymentService
    {
        public string UniqueCode => "YandexKassa";

        public string GetUri => "/YandexKassa/";

        public string Title => "Оплата банковской картой";

        public Form CreateForm(Order order)
        {
            return new Form(UniqueCode, order.Id, 1, false, new Field[0]);
        }

        public OrderPayment GetPayment(Form form)
        {
            return new OrderPayment(UniqueCode, "Оплата картой", new Dictionary<string, string>());
        }

        public Form MoveNext(int orderId, int step, IReadOnlyDictionary<string, string> value)
        {
            return new Form(UniqueCode, orderId, 2, true, new Field[0]);
        }
    }
}
