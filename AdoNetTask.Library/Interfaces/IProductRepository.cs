namespace AdoNetTask.Library;

public interface IProductRepository
{

    void Add(Product product);
    void Update(Product product);
    void Delete(int id);
    Product GetById(int id);
    IEnumerable<Product> GetAll();

}
