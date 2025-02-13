﻿CREATE PROCEDURE [dbo].[spUpdateWikiJobRequest]
	@Comment NVARCHAR(1000),
	@Username NVARCHAR(50),
	@Status INT,
	@RawRequest NVARCHAR(MAX),
	@SubmittedDate DateTime2(7),
	@JobType NVARCHAR(100),
	@ID BIGINT,
	@Message NVARCHAR(MAX)
AS
	UPDATE [dbo].WikiJobRequest SET Comment = @Comment, RequestingUsername = @Username, StatusID = @Status, RawRequest = @RawRequest, SubmittedDateUTC = @SubmittedDate, JobType = @JobType, 
	StatusMessage = @Message
	WHERE Id = @ID
