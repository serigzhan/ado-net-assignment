CREATE PROCEDURE [dbo].[spOrders_Filter]
	@Month INT = NULL,
	@Year INT = NULL,
	@Status NVARCHAR(20) = NULL,
	@ProductId INT = NULL
AS
BEGIN
	SELECT
		o.[Id],
		o.[Status],
		o.[CreatedDate],
		o.[UpdatedDate],
		o.[ProductId]
	FROM [dbo].[Order] o
	WHERE
		(@Month IS NULL OR MONTH(o.[CreatedDate]) = @Month)
		AND (@Year IS NULL OR YEAR(o.[CreatedDate]) = @Year)
		AND (@Status IS NULL OR o.[Status] = @Status)
		AND (@ProductId IS NULL OR o.[ProductId] = @ProductId)
END
