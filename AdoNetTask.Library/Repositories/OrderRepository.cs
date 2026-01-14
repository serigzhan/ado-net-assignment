namespace AdoNetTask.Library;

public class OrderRepository(string connectionString) : IOrderRepository
{

    public string ConnectionString { get; set; } = connectionString;

    public void Add(Order order)
    {
        throw new NotImplementedException();
    }

    public void Update(Order order)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteOrdersBulk(int? month, int? year, OrderStatus? status, int? productId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Order> GetFilteredOrders(int? month, int? year, OrderStatus? status, int? productId)
    {
        throw new NotImplementedException();
    }
}
