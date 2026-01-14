namespace AdoNetTask.Library;

public interface IProductRepository
{

    void Add(Product product);
    void Update(Product product);
    void Delete(int id);
    Product GetById(int id);
    Product GetByName(string name);
    IEnumerable<Product> GetAll();

}
