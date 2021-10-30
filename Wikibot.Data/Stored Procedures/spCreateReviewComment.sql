CREATE PROCEDURE [dbo].[spCreateReviewComment]
	@requestId bigint,
	@comment nvarchar(max),
	@timestamp datetime2
AS
	INSERT INTO ReviewComment ([TimestampUtc], [Text], [WikiJobRequestId]) VALUES (@timestamp, @comment, @requestId)

