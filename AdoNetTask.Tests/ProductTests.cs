using AdoNetTask.Library;
using System.Transactions;

namespace AdoNetTask.Tests;

public class ProductTests
{
    private IProductRepository _productRepository;
    private string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=InventoryDB;Trusted_Connection=True;";

    [SetUp]
    public void Setup()
    {

        _productRepository = new ProductRepository(_connectionString);

    }

    [Test]
    public void CreateProduct_ShouldSaveToDatabase()
    {
        using (var scope = new TransactionScope())
        {

            var product = new Product { Name = "Product A", Weight = 1.5m  };

            _productRepository.Add(product);
            var result = _productRepository.GetByName("Product A");

            Assert.That(result, Is.Not.Null);
            Assert.Equals(1.5m, result.Weight);

        }
    }

    [Test]
    public void UpdateProduct_ShouldUpdateToDatabase()
    {

        using (var scope = new TransactionScope())
        {

            var product = CreateTestProduct();

            product.Name = "New Product Name";
            product.Height = 45.5m;
            _productRepository.Update(product);

            var updatedProduct = _productRepository.GetById(product.Id);

            Assert.Equals(updatedProduct.Name, "New Product Name");
            Assert.Equals(updatedProduct.Height, 45.5m);

        }

    }

    [Test]
    public void DeleteProduct_ShouldDeleteFromDatabase()
    {

        using (var scope = new TransactionScope())
        {

            CreateTestProduct();
            var candidate = CreateTestProduct();
            var id = candidate.Id;

            _productRepository.Delete(id);

            var products = _productRepository.GetAll();
            Assert.Equals(products.Count(), 1);

        }

    }

    private Product CreateTestProduct()
    {
        var p = new Product { Name = "Test_" + Guid.NewGuid(), Weight = 1.0m };
        _productRepository.Add(p);
        return _productRepository.GetByName(p.Name);
    }
}
