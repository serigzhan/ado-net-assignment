using System.Data;
using Microsoft.Data.SqlClient;

namespace AdoNetTask.Library;

public class ProductRepository(string connectionString) : IProductRepository
{
    private readonly string _connectionString = connectionString;

    public Product Add(Product product)
    {
        const string sql = @"
            INSERT INTO Product (Name, Description, Weight, Height, Width, Length)
            VALUES (@Name, @Description, @Weight, @Height, @Width, @Length);
            SELECT SCOPE_IDENTITY();";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@Weight", (object?)product.Weight ?? DBNull.Value);
        command.Parameters.AddWithValue("@Height", (object?)product.Height ?? DBNull.Value);
        command.Parameters.AddWithValue("@Width", (object?)product.Width ?? DBNull.Value);
        command.Parameters.AddWithValue("@Length", (object?)product.Length ?? DBNull.Value);

        connection.Open();
        var id = Convert.ToInt32(command.ExecuteScalar());
        product.Id = id;

        return product;
    }

    public Product Update(Product product)
    {
        const string sql = @"
            UPDATE Product
            SET Name = @Name,
                Description = @Description,
                Weight = @Weight,
                Height = @Height,
                Width = @Width,
                Length = @Length
            WHERE Id = @Id";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Id", product.Id);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@Weight", (object?)product.Weight ?? DBNull.Value);
        command.Parameters.AddWithValue("@Height", (object?)product.Height ?? DBNull.Value);
        command.Parameters.AddWithValue("@Width", (object?)product.Width ?? DBNull.Value);
        command.Parameters.AddWithValue("@Length", (object?)product.Length ?? DBNull.Value);

        connection.Open();
        command.ExecuteNonQuery();

        return product;
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM Product WHERE Id = @Id";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Id", id);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public Product? GetById(int id)
    {
        const string sql = "SELECT Id, Name, Description, Weight, Height, Width, Length FROM Product WHERE Id = @Id";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Id", id);

        connection.Open();
        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            return MapToProduct(reader);
        }

        return null;
    }

    public IEnumerable<Product> GetAll()
    {
        const string sql = "SELECT Id, Name, Description, Weight, Height, Width, Length FROM Product";

        using var connection = new SqlConnection(_connectionString);
        using var adapter = new SqlDataAdapter(sql, connection);

        var dataTable = new DataTable();
        adapter.Fill(dataTable);

        return dataTable.AsEnumerable().Select(MapToProduct).ToList();
    }

    private static Product MapToProduct(SqlDataReader reader)
    {
        return new Product
        {
            Id = (int)reader["Id"],
            Name = (string)reader["Name"],
            Description = reader["Description"] as string,
            Weight = reader["Weight"] as decimal?,
            Height = reader["Height"] as decimal?,
            Width = reader["Width"] as decimal?,
            Length = reader["Length"] as decimal?
        };
    }

    private static Product MapToProduct(DataRow row)
    {
        return new Product
        {
            Id = row.Field<int>("Id"),
            Name = row.Field<string>("Name")!,
            Description = row.Field<string?>("Description"),
            Weight = row.Field<decimal?>("Weight"),
            Height = row.Field<decimal?>("Height"),
            Width = row.Field<decimal?>("Width"),
            Length = row.Field<decimal?>("Length")
        };
    }
}
