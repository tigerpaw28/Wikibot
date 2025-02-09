CREATE TABLE [dbo].[WikiJobRequest]
(
	[Id] BIGINT NOT NULL  IDENTITY,
	[Comment]            NVARCHAR (MAX)  NOT NULL,
    [StatusID]           INT             NULL,
    [RequestingUsername] NVARCHAR (50)   NULL,
    [RawRequest]         NVARCHAR (MAX)  NULL,
    [SubmittedDateUTC]   DATETIME2 (7)   NULL,
    [TimePreStartedUTC]  DATETIME2 (7)   NULL,
    [TimePreFinishedUTC] DATETIME2 (7)   NULL,
    [TimeStartedUTC]     DATETIME2 (7)   NULL,
    [TimeFinishedUTC]    DATETIME2 (7)   NULL,
    [JobType]            NVARCHAR (50)   NULL,
    [StatusMessage]      NVARCHAR(MAX)   NULL, 
    CONSTRAINT [PK_WikiJobRequest] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WikiJobRequest_Status] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[Status] ([Id])

)
