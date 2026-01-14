namespace AdoNetTask.Library;

public interface IOrderRepository
{

    void Add(Order order);
    void Update(Order order);
    void Delete(int id);
    IEnumerable<Order> GetFilteredOrders(int? month, int? year, OrderStatus? status, int? productId);
    void DeleteOrdersBulk(int? month, int? year, OrderStatus? status, int? productId);

}
