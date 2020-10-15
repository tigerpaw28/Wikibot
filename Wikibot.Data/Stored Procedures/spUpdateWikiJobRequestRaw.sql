CREATE PROCEDURE [dbo].[spUpdateWikiJobRequestRaw]
	@ID int,
	@RawText NVARCHAR(MAX)
AS
BEGIN
	UPDATE WikiJobRequest SET
	[RawRequest] = @RawText
	WHERE [Id] = @ID
END
