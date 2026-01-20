CREATE PROCEDURE [dbo].[spOrders_Bulk_Delete]
	@Month INT = NULL,
	@Year INT = NULL,
	@Status NVARCHAR(20) = NULL,
	@ProductId INT = NULL
AS
BEGIN
	DELETE FROM [dbo].[Order]
	WHERE
		(@Month IS NULL OR MONTH([CreatedDate]) = @Month)
		AND (@Year IS NULL OR YEAR([CreatedDate]) = @Year)
		AND (@Status IS NULL OR [Status] = @Status)
		AND (@ProductId IS NULL OR [ProductId] = @ProductId)
END
