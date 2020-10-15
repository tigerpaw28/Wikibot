CREATE PROCEDURE [dbo].[spUpdateWikiJobRequestStatus]
	@Status INT,
	@ID BIGINT
AS
BEGIN
	UPDATE WikiJobRequest SET 
	[StatusID] = @Status
	WHERE [Id] = @ID
END
