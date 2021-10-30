CREATE TABLE [dbo].[ReviewComment]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Text] NVARCHAR(MAX) NOT NULL, 
    [TimestampUtc] DATETIME2 NOT NULL, 
    [WikiJobRequestId] BIGINT NULL, 
    CONSTRAINT [FK_ReviewComment_WikiJobRequest] FOREIGN KEY ([WikiJobRequestId]) REFERENCES [WikiJobRequest]([Id])
)
