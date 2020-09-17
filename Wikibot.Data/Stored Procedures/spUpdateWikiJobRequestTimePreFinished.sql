CREATE PROCEDURE [dbo].[spUpdateWikiJobRequestTimePreFinished]
	@TimePreFinishedUTC Datetime2(7),
	@ID BIGINT
AS
BEGIN
UPDATE WikiJobRequest SET 
[TimePreFinishedUTC] = @TimePreFinishedUTC
WHERE [Id] = @ID
END
