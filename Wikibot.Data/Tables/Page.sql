CREATE TABLE [dbo].[Page]
(
	[PageId] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(1000) NOT NULL, 
    [WikiJobRequestID] BIGINT NOT NULL, 
    CONSTRAINT [FK_Page_WikiJobRequest] FOREIGN KEY ([WikiJobRequestID]) REFERENCES [WikiJobRequest]([Id])
)
