using AdoNetTask.Library;
using System.Transactions;

namespace AdoNetTask.Tests;

public class OrderTests
{
    private IOrderRepository _orderRepository;
    private IProductRepository _productRepository;
    private string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=InventoryDB;Trusted_Connection=True;";

    [SetUp]
    public void Setup()
    {
        _orderRepository = new OrderRepository(_connectionString);
        _productRepository = new ProductRepository(_connectionString);
    }

    [Test]
    public void CreateOrder_ShouldSaveToDatabase()
    {
        using (var scope = new TransactionScope())
        {
            var product = CreateTestProduct();
            var order = new Order
            {
                Status = OrderStatus.NotStarted,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ProductId = product.Id
            };

            _orderRepository.Add(order);

            var orders = _orderRepository.GetFilteredOrders(null, null, null, product.Id);
            Assert.That(orders.Count(), Is.EqualTo(1));
            Assert.That(orders.First().Status, Is.EqualTo(OrderStatus.NotStarted));
            Assert.That(orders.First().ProductId, Is.EqualTo(product.Id));
        }
    }

    [Test]
    public void UpdateOrder_ShouldUpdateToDatabase()
    {
        using (var scope = new TransactionScope())
        {
            var order = CreateTestOrder();
            var originalStatus = order.Status;

            order.Status = OrderStatus.InProgress;
            order.UpdatedDate = DateTime.Now;
            _orderRepository.Update(order);

            var orders = _orderRepository.GetFilteredOrders(null, null, OrderStatus.InProgress, null);
            Assert.That(orders.Any(o => o.Id == order.Id), Is.True);
            Assert.That(orders.First(o => o.Id == order.Id).Status, Is.EqualTo(OrderStatus.InProgress));
        }
    }

    [Test]
    public void DeleteOrder_ShouldDeleteFromDatabase()
    {
        using (var scope = new TransactionScope())
        {
            var order1 = CreateTestOrder();
            var order2 = CreateTestOrder();

            _orderRepository.Delete(order1.Id);

            var orders = _orderRepository.GetFilteredOrders(null, null, null, null);
            Assert.That(orders.Any(o => o.Id == order1.Id), Is.False);
            Assert.That(orders.Any(o => o.Id == order2.Id), Is.True);
        }
    }

    [Test]
    public void GetFilteredOrders_WithNoFilters_ShouldReturnAllOrders()
    {
        using (var scope = new TransactionScope())
        {
            var order1 = CreateTestOrder();
            var order2 = CreateTestOrder();
            var order3 = CreateTestOrder();

            var orders = _orderRepository.GetFilteredOrders(null, null, null, null);

            Assert.That(orders.Count(), Is.GreaterThanOrEqualTo(3));
        }
    }

    [Test]
    public void GetFilteredOrders_ByMonth_ShouldReturnMatchingOrders()
    {
        using (var scope = new TransactionScope())
        {
            var product = CreateTestProduct();
            var januaryOrder = new Order
            {
                Status = OrderStatus.NotStarted,
                CreatedDate = new DateTime(2024, 1, 15),
                UpdatedDate = new DateTime(2024, 1, 15),
                ProductId = product.Id
            };
            var februaryOrder = new Order
            {
                Status = OrderStatus.NotStarted,
                CreatedDate = new DateTime(2024, 2, 15),
                UpdatedDate = new DateTime(2024, 2, 15),
                ProductId = product.Id
            };

            _orderRepository.Add(januaryOrder);
            _orderRepository.Add(februaryOrder);

            var orders = _orderRepository.GetFilteredOrders(1, null, null, null);

            Assert.That(orders.All(o => o.CreatedDate.Month == 1), Is.True);
        }
    }

    [Test]
    public void GetFilteredOrders_ByYear_ShouldReturnMatchingOrders()
    {
        using (var scope = new TransactionScope())
        {
            var product = CreateTestProduct();
            var order2024 = new Order
            {
                Status = OrderStatus.NotStarted,
                CreatedDate = new DateTime(2024, 5, 15),
                UpdatedDate = new DateTime(2024, 5, 15),
                ProductId = product.Id
            };
            var order2025 = new Order
            {
                Status = OrderStatus.NotStarted,
                CreatedDate = new DateTime(2025, 5, 15),
                UpdatedDate = new DateTime(2025, 5, 15),
                ProductId = product.Id
            };

            _orderRepository.Add(order2024);
            _orderRepository.Add(order2025);

            var orders = _orderRepository.GetFilteredOrders(null, 2024, null, null);

            Assert.That(orders.All(o => o.CreatedDate.Year == 2024), Is.True);
        }
    }

    [Test]
    public void GetFilteredOrders_ByStatus_ShouldReturnMatchingOrders()
    {
        using (var scope = new TransactionScope())
        {
            var product = CreateTestProduct();
            var loadingOrder = new Order
            {
                Status = OrderStatus.Loading,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ProductId = product.Id
            };
            var doneOrder = new Order
            {
                Status = OrderStatus.Done,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ProductId = product.Id
            };

            _orderRepository.Add(loadingOrder);
            _orderRepository.Add(doneOrder);

            var orders = _orderRepository.GetFilteredOrders(null, null, OrderStatus.Loading, null);

            Assert.That(orders.All(o => o.Status == OrderStatus.Loading), Is.True);
        }
    }

    [Test]
    public void GetFilteredOrders_ByProductId_ShouldReturnMatchingOrders()
    {
        using (var scope = new TransactionScope())
        {
            var product1 = CreateTestProduct();
            var product2 = CreateTestProduct();

            var orderForProduct1 = new Order
            {
                Status = OrderStatus.NotStarted,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ProductId = product1.Id
            };
            var orderForProduct2 = new Order
            {
                Status = OrderStatus.NotStarted,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ProductId = product2.Id
            };

            _orderRepository.Add(orderForProduct1);
            _orderRepository.Add(orderForProduct2);

            var orders = _orderRepository.GetFilteredOrders(null, null, null, product1.Id);

            Assert.That(orders.All(o => o.ProductId == product1.Id), Is.True);
        }
    }

    [Test]
    public void GetFilteredOrders_WithMultipleFilters_ShouldReturnMatchingOrders()
    {
        using (var scope = new TransactionScope())
        {
            var product = CreateTestProduct();
            var matchingOrder = new Order
            {
                Status = OrderStatus.InProgress,
                CreatedDate = new DateTime(2024, 3, 15),
                UpdatedDate = new DateTime(2024, 3, 15),
                ProductId = product.Id
            };
            var nonMatchingOrder = new Order
            {
                Status = OrderStatus.Done,
                CreatedDate = new DateTime(2024, 3, 15),
                UpdatedDate = new DateTime(2024, 3, 15),
                ProductId = product.Id
            };

            _orderRepository.Add(matchingOrder);
            _orderRepository.Add(nonMatchingOrder);

            var orders = _orderRepository.GetFilteredOrders(3, 2024, OrderStatus.InProgress, product.Id);

            Assert.That(orders.All(o =>
                o.CreatedDate.Month == 3 &&
                o.CreatedDate.Year == 2024 &&
                o.Status == OrderStatus.InProgress &&
                o.ProductId == product.Id), Is.True);
        }
    }

    [Test]
    public void DeleteOrdersBulk_ByMonth_ShouldDeleteMatchingOrders()
    {
        using (var scope = new TransactionScope())
        {
            var product = CreateTestProduct();
            var januaryOrder = new Order
            {
                Status = OrderStatus.NotStarted,
                CreatedDate = new DateTime(2024, 1, 15),
                UpdatedDate = new DateTime(2024, 1, 15),
                ProductId = product.Id
            };
            var februaryOrder = new Order
            {
                Status = OrderStatus.NotStarted,
                CreatedDate = new DateTime(2024, 2, 15),
                UpdatedDate = new DateTime(2024, 2, 15),
                ProductId = product.Id
            };

            _orderRepository.Add(januaryOrder);
            _orderRepository.Add(februaryOrder);

            _orderRepository.DeleteOrdersBulk(1, null, null, null);

            var remainingOrders = _orderRepository.GetFilteredOrders(null, null, null, product.Id);
            Assert.That(remainingOrders.All(o => o.CreatedDate.Month != 1), Is.True);
        }
    }

    [Test]
    public void DeleteOrdersBulk_ByYear_ShouldDeleteMatchingOrders()
    {
        using (var scope = new TransactionScope())
        {
            var product = CreateTestProduct();
            var order2024 = new Order
            {
                Status = OrderStatus.NotStarted,
                CreatedDate = new DateTime(2024, 5, 15),
                UpdatedDate = new DateTime(2024, 5, 15),
                ProductId = product.Id
            };
            var order2025 = new Order
            {
                Status = OrderStatus.NotStarted,
                CreatedDate = new DateTime(2025, 5, 15),
                UpdatedDate = new DateTime(2025, 5, 15),
                ProductId = product.Id
            };

            _orderRepository.Add(order2024);
            _orderRepository.Add(order2025);

            _orderRepository.DeleteOrdersBulk(null, 2024, null, null);

            var remainingOrders = _orderRepository.GetFilteredOrders(null, null, null, product.Id);
            Assert.That(remainingOrders.All(o => o.CreatedDate.Year != 2024), Is.True);
        }
    }

    [Test]
    public void DeleteOrdersBulk_ByStatus_ShouldDeleteMatchingOrders()
    {
        using (var scope = new TransactionScope())
        {
            var product = CreateTestProduct();
            var cancelledOrder = new Order
            {
                Status = OrderStatus.Cancelled,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ProductId = product.Id
            };
            var doneOrder = new Order
            {
                Status = OrderStatus.Done,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ProductId = product.Id
            };

            _orderRepository.Add(cancelledOrder);
            _orderRepository.Add(doneOrder);

            _orderRepository.DeleteOrdersBulk(null, null, OrderStatus.Cancelled, null);

            var remainingOrders = _orderRepository.GetFilteredOrders(null, null, null, product.Id);
            Assert.That(remainingOrders.All(o => o.Status != OrderStatus.Cancelled), Is.True);
        }
    }

    [Test]
    public void DeleteOrdersBulk_ByProductId_ShouldDeleteMatchingOrders()
    {
        using (var scope = new TransactionScope())
        {
            var product1 = CreateTestProduct();
            var product2 = CreateTestProduct();

            var orderForProduct1 = new Order
            {
                Status = OrderStatus.NotStarted,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ProductId = product1.Id
            };
            var orderForProduct2 = new Order
            {
                Status = OrderStatus.NotStarted,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ProductId = product2.Id
            };

            _orderRepository.Add(orderForProduct1);
            _orderRepository.Add(orderForProduct2);

            _orderRepository.DeleteOrdersBulk(null, null, null, product1.Id);

            var remainingOrdersForProduct1 = _orderRepository.GetFilteredOrders(null, null, null, product1.Id);
            var remainingOrdersForProduct2 = _orderRepository.GetFilteredOrders(null, null, null, product2.Id);

            Assert.That(remainingOrdersForProduct1.Count(), Is.EqualTo(0));
            Assert.That(remainingOrdersForProduct2.Count(), Is.EqualTo(1));
        }
    }

    [Test]
    public void DeleteOrdersBulk_WithMultipleFilters_ShouldDeleteMatchingOrders()
    {
        using (var scope = new TransactionScope())
        {
            var product = CreateTestProduct();
            var matchingOrder = new Order
            {
                Status = OrderStatus.Cancelled,
                CreatedDate = new DateTime(2024, 6, 15),
                UpdatedDate = new DateTime(2024, 6, 15),
                ProductId = product.Id
            };
            var nonMatchingOrder = new Order
            {
                Status = OrderStatus.Done,
                CreatedDate = new DateTime(2024, 6, 15),
                UpdatedDate = new DateTime(2024, 6, 15),
                ProductId = product.Id
            };

            _orderRepository.Add(matchingOrder);
            _orderRepository.Add(nonMatchingOrder);

            _orderRepository.DeleteOrdersBulk(6, 2024, OrderStatus.Cancelled, product.Id);

            var remainingOrders = _orderRepository.GetFilteredOrders(null, null, null, product.Id);
            Assert.That(remainingOrders.Count(), Is.EqualTo(1));
            Assert.That(remainingOrders.First().Status, Is.EqualTo(OrderStatus.Done));
        }
    }

    private Product CreateTestProduct()
    {
        var product = new Product { Name = "Test_" + Guid.NewGuid(), Weight = 1.0m };
        return _productRepository.Add(product);
    }

    private Order CreateTestOrder()
    {
        var product = CreateTestProduct();
        var order = new Order
        {
            Status = OrderStatus.NotStarted,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now,
            ProductId = product.Id
        };
        _orderRepository.Add(order);

        var orders = _orderRepository.GetFilteredOrders(null, null, null, product.Id);
        return orders.First();
    }

}
