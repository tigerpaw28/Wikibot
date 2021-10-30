CREATE PROCEDURE [dbo].[spGetReviewCommentForRequest]
	@RequestId bigint
AS
	SELECT [Id], [Text], [WikiJobRequestId], [TimestampUtc]
	FROM [ReviewComment] 
	WHERE [WikiJobRequestId] = @RequestId
