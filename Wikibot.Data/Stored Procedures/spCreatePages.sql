CREATE PROCEDURE [dbo].[spCreatePages]
	@pages  PageUDT READONLY,
	@jobid BIGINT
AS
BEGIN
	INSERT INTO dbo.[Page] ([Name], [WikiJobRequestID]) 
	SELECT  PageName, @jobid FROM @pages
END