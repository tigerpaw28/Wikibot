CREATE PROCEDURE [dbo].[spUpdateWikiJobRequestTimeFinished]
	@TimeFinishedUTC Datetime2(7),
	@ID BIGINT
AS
BEGIN
UPDATE WikiJobRequest SET 
[TimeFinishedUTC] = @TimeFinishedUTC
WHERE [Id] = @ID
END

