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

            var product = _productRepository.Add(new Product { Name = "Product A", Weight = 1.5m });

            Assert.That(product, Is.Not.Null);
            Assert.That(product.Weight, Is.EqualTo(1.5m));

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
            var updatedProduct = _productRepository.Update(product);

            Assert.That(updatedProduct.Name, Is.EqualTo("New Product Name"));
            Assert.That(updatedProduct.Height, Is.EqualTo(45.5m));

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
            Assert.That(products.Count(), Is.EqualTo(1));

        }

    }

    [Test]
    public void GetById_ShouldReturnProduct()
    {
        using (var scope = new TransactionScope())
        {
            var product = CreateTestProduct();

            var result = _productRepository.GetById(product.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(product.Id));
            Assert.That(result.Name, Is.EqualTo(product.Name));
            Assert.That(result.Weight, Is.EqualTo(product.Weight));
        }
    }

    [Test]
    public void GetAll_ShouldReturnAllProducts()
    {
        using (var scope = new TransactionScope())
        {
            var product1 = CreateTestProduct();
            var product2 = CreateTestProduct();
            var product3 = CreateTestProduct();

            var products = _productRepository.GetAll();

            Assert.That(products.Count(), Is.EqualTo(3));
            Assert.That(products.Any(p => p.Id == product1.Id), Is.True);
            Assert.That(products.Any(p => p.Id == product2.Id), Is.True);
            Assert.That(products.Any(p => p.Id == product3.Id), Is.True);
        }
    }

    private Product CreateTestProduct()
    {
        var p = new Product { Name = "Test_" + Guid.NewGuid(), Weight = 1.0m };
        return _productRepository.Add(p);
    }
}
