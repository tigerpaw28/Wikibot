CREATE PROCEDURE [dbo].[spUpdatePagesForWikiJobRequest]
	@pages  PageUDT READONLY,
	@jobid BIGINT
AS
BEGIN
	DELETE FROM dbo.[Page] WHERE WikiJobRequestID = @jobid

	EXEC spCreatePages @pages, @jobid
END
