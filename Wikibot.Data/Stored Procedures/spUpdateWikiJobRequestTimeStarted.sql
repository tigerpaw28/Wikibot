CREATE PROCEDURE [dbo].[spUpdateWikiJobRequestTimeStarted]
	@TimeStartedUTC Datetime2(7),
	@ID BIGINT
AS
BEGIN
UPDATE WikiJobRequest SET 
[TimeStartedUTC] = @TimeStartedUTC
WHERE [Id] = @ID
END
