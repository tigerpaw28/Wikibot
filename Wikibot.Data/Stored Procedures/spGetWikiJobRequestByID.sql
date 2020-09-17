CREATE PROCEDURE [dbo].[spGetWikiJobRequestByID]
	@RequestID BIGINT
AS
BEGIN
	SELECT 
	[WJ].[Id],
	[WJ].[Comment], 
	[WJ].[StatusID] AS 'Status', 
	[WJ].[RequestingUsername], 
	[WJ].[Notes], 
	[WJ].[RawRequest], 
	[WJ].[SubmittedDateUTC], 
	[WJ].[TimePreStartedUTC], 
	[WJ].[TimePreFinishedUTC], 
	[WJ].[TimeStartedUTC], 
	[WJ].[TimeFinishedUTC], 
	[WJ].[JobType], 
	[P].[PageId], 
	[P].[Name], 
	[P].[WikiJobRequestID]
	FROM dbo.WikiJobRequest AS WJ
	LEFT JOIN dbo.Page P ON P.WikiJobRequestID = WJ.Id
	WHERE WJ.Id = @RequestID
END
