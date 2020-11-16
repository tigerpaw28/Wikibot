CREATE PROCEDURE [dbo].[spCreatePages]
	@pages  PageUDT READONLY,
	@jobid BIGINT
AS
BEGIN

	INSERT INTO dbo.[Page] ([Name], [WikiJobRequestID]) 
	SELECT  PageName, @jobid FROM @pages P
	WHERE NOT EXISTS (SELECT PageId FROM dbo.[Page] P2 WHERE P2.[Name] = P.PageName AND P2.WikiJobRequestID = @jobid);

END