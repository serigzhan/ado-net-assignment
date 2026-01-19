namespace AdoNetTask.Library;

public interface IProductRepository
{

    Product Add(Product product);
    Product Update(Product product);
    void Delete(int id);
    Product GetById(int id);
    IEnumerable<Product> GetAll();

}
