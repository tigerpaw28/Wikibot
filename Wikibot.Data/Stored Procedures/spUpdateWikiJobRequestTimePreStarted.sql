CREATE PROCEDURE [dbo].[spUpdateWikiJobRequestTimePreStarted]
	@TimePreStartedUTC Datetime2(7),
	@ID BIGINT
AS
BEGIN
UPDATE WikiJobRequest SET 
[TimePreStartedUTC] = @TimePreStartedUTC
WHERE [Id] = @ID
END
