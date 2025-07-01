CREATE PROCEDURE [dbo].[spGetWikiJobRequestsForApproval]
	@PageNumber int,
	@PageSize int,
	@SortColumn nvarchar(50),
	@SortDirection nvarchar(3)
AS
BEGIN

 	DECLARE @ParameterDef					NVARCHAR(500)
 
    SET @ParameterDef =    '@PageNumber			INT,
							@PageSize			INT,
							@SortColumn			NVARCHAR(50),
							@SortDirection		NVARCHAR(3)'

  DECLARE @query NVARCHAR(1000) = ' ;WITH pg AS
  (
    SELECT ID
      FROM dbo.WikiJobRequest WJ
      ORDER BY ID
      OFFSET @PageSize * (@PageNumber - 1) ROWS
      FETCH NEXT @PageSize ROWS ONLY
  )
  SELECT
	[WJ].[Id],
	[WJ].[Comment], 
	[WJ].[StatusID] AS ''Status'', 
	[WJ].[RequestingUsername], 
	[WJ].[StatusMessage], 
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
  LEFT JOIN dbo.Page P ON P.WikiJobRequestID = WJ.ID
  INNER JOIN dbo.Status S ON S.Id = WJ.StatusID AND (StatusName = ''PendingPreApproval'' OR StatusName = ''PendingApproval'')'
DECLARE @where NVARCHAR(MAX) = ' WHERE EXISTS (SELECT 1 FROM pg WHERE pg.ID = WJ.ID) '
DECLARE @and NVARCHAR(MAX) = ''
DECLARE @col NVARCHAR(MAX) = 'ID'
DECLARE @direction NVARCHAR(MAX) = 'ASC'
DECLARE @paging NVARCHAR(MAX) = ''

  -- ignore any invalid sort directions:
  IF UPPER(@SortDirection) NOT IN ('ASC','DESC')
  BEGIN
    SET @SortDirection = 'ASC'
  END 
 
  -- reject any unexpected column names:
  IF LOWER(@SortColumn) NOT IN (N'id', N'statusid', N'submitteddateutc', N'timeprestartedutc', N'timeprefinishedutc', N'timestartedutc', N'timefinished', N'jobtype')
  BEGIN
    SET @SortColumn = 'ID'
  END 

--IF(@Offset IS NOT NULL AND @Fetch IS NOT NULL)
	--SET @paging = ' OFFSET ' + CAST(@Offset AS NVARCHAR(10)) + ' ROWS FETCH NEXT ' + CAST(@Fetch AS NVARCHAR(10)) + ' ROWS ONLY;'

--IF(@SortColumn IS NOT NULL AND @SortColumn <> '')
	--SET @col = @SortColumn

--IF(@SortDirection IS NOT NULL AND @SortDirection <> '')
--	IF(@SortDirection = 'ASC' OR @SortDirection = 'DESC')
--		SET @direction = @SortDirection

--IF(@Title IS NOT NULL AND @Title <> '')
--BEGIN
--	SET @where = @where + @and + 'Title LIKE ''%'' + @Title + ''%'''
--	SET @and = ' AND '
--END

--IF(@Studio IS NOT NULL AND @Studio <> '')
--BEGIN
--	SET @where = @where + @and + 'Studio IN (''+ @Studio + '')'
--	SET @and = ' AND '
--END

--IF(@Media IS NOT NULL AND @Media <> '')
--BEGIN
--	SET @where = @where + @and + 'Media = ''+ @Media + '''
--	SET @and = ' AND '
--END

--IF(@Aspect IS NOT NULL AND @Aspect <> '')
--BEGIN
--	SET @where = @where + @and + 'Aspect = ''+ @Aspect + '''
--	SET @and = ' AND '
--END

--IF(@Distribution IS NOT NULL AND @Distribution <> '')
--BEGIN
--	SET @where = @where + @and + 'Distribution = ''+ @Distribution + '''
--END

IF(@where <> ' WHERE ')
	SET @query = @query + @where
	
SET @query = @query + 'ORDER BY '+ @SortColumn + ' ' + @SortDirection + ' OPTION (RECOMPILE);'

PRINT @query

EXEC sp_executesql @query, @ParameterDef, @PageNumber=@PageNumber, @PageSize=@PageSize, @SortDirection=@SortDirection, @SortColumn=@SortColumn 
END