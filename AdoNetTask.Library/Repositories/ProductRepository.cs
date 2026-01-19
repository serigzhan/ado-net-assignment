namespace AdoNetTask.Library;

public class ProductRepository(string connectionString) : IProductRepository
{

    private readonly string _connectionString = connectionString;

    public Product Add(Product product)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Product> GetAll()
    {
        throw new NotImplementedException();
    }

    public Product GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Product Update(Product product)
    {
        throw new NotImplementedException();
    }
}
