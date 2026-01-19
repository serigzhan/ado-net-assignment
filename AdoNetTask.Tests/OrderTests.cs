using AdoNetTask.Library;

namespace AdoNetTask.Tests;

public class OrderTests
{

    private IOrderRepository _orderRepository;
    private string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=InventoryDB;Trusted_Connection=True;";

    [SetUp]
    public void Setup()
    {

        _orderRepository = new OrderRepository(_connectionString);
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }

}
