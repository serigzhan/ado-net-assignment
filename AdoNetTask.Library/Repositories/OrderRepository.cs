using System.Data;
using Microsoft.Data.SqlClient;

namespace AdoNetTask.Library;

public class OrderRepository(string connectionString) : IOrderRepository
{
    private readonly string _connectionString = connectionString;

    public void Add(Order order)
    {
        const string sql = @"
            INSERT INTO [Order] (Status, CreatedDate, UpdatedDate, ProductId)
            VALUES (@Status, @CreatedDate, @UpdatedDate, @ProductId);
            SELECT SCOPE_IDENTITY();";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Status", order.Status.ToString());
        command.Parameters.AddWithValue("@CreatedDate", order.CreatedDate);
        command.Parameters.AddWithValue("@UpdatedDate", order.UpdatedDate);
        command.Parameters.AddWithValue("@ProductId", order.ProductId);

        connection.Open();
        var id = Convert.ToInt32(command.ExecuteScalar());
        order.Id = id;
    }

    public void Update(Order order)
    {
        const string sql = @"
            UPDATE [Order]
            SET Status = @Status,
                CreatedDate = @CreatedDate,
                UpdatedDate = @UpdatedDate,
                ProductId = @ProductId
            WHERE Id = @Id";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Id", order.Id);
        command.Parameters.AddWithValue("@Status", order.Status.ToString());
        command.Parameters.AddWithValue("@CreatedDate", order.CreatedDate);
        command.Parameters.AddWithValue("@UpdatedDate", order.UpdatedDate);
        command.Parameters.AddWithValue("@ProductId", order.ProductId);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM [Order] WHERE Id = @Id";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Id", id);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public IEnumerable<Order> GetFilteredOrders(int? month, int? year, OrderStatus? status, int? productId)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("spOrders_Filter", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);
        command.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
        command.Parameters.AddWithValue("@Status", status.HasValue ? status.Value.ToString() : DBNull.Value);
        command.Parameters.AddWithValue("@ProductId", (object?)productId ?? DBNull.Value);

        using var adapter = new SqlDataAdapter(command);
        var dataTable = new DataTable();
        adapter.Fill(dataTable);

        return dataTable.AsEnumerable().Select(MapToOrder).ToList();
    }

    public void DeleteOrdersBulk(int? month, int? year, OrderStatus? status, int? productId)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("spOrders_Bulk_Delete", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@Month", (object?)month ?? DBNull.Value);
        command.Parameters.AddWithValue("@Year", (object?)year ?? DBNull.Value);
        command.Parameters.AddWithValue("@Status", status.HasValue ? status.Value.ToString() : DBNull.Value);
        command.Parameters.AddWithValue("@ProductId", (object?)productId ?? DBNull.Value);

        connection.Open();
        command.ExecuteNonQuery();
    }

    private static Order MapToOrder(SqlDataReader reader)
    {
        return new Order
        {
            Id = (int)reader["Id"],
            Status = Enum.Parse<OrderStatus>((string)reader["Status"]),
            CreatedDate = (DateTime)reader["CreatedDate"],
            UpdatedDate = (DateTime)reader["UpdatedDate"],
            ProductId = (int)reader["ProductId"]
        };
    }

    private static Order MapToOrder(DataRow row)
    {
        return new Order
        {
            Id = row.Field<int>("Id"),
            Status = Enum.Parse<OrderStatus>(row.Field<string>("Status")!),
            CreatedDate = row.Field<DateTime>("CreatedDate"),
            UpdatedDate = row.Field<DateTime>("UpdatedDate"),
            ProductId = row.Field<int>("ProductId")
        };
    }
}
