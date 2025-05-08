-- Out Parameter
SELECT * FROM TEST_Products WHERE
TRY_CAST(json_value(details,'$.spec.cpu') as nvarchar(20)) = 'i7'
GO
create proc proc_FilterProducts(@pcpu varchar(20), @pcount int out)
as 
begin
	SET @pcount = (SELECT COUNT(*) FROM TEST_Products WHERE
	TRY_CAST(json_value(details,'$.spec.cpu') as nvarchar(20)) = 'i7')
end

begin
	declare @count int
	exec proc_FilterProducts 'i7', @count out
	print concat('The number of computers is ',@count)
end
GO
-- Bulk insert using csv
create table TEST_People(
	id int primary key,
	name varchar(20),
	age int
);
GO
create or alter proc proc_BulkInsert(@filepath nvarchar(500))
as
begin
	declare @insertQuery nvarchar(max)
	set @insertQuery = 'BULK INSERT TEST_People from '''+@filepath+'''
	with (
	FIRSTROW=2,
	FIELDTERMINATOR='','',
	ROWTERMINATOR=''\n''
	)'
	exec sp_executesql @insertQuery
end
GO
proc_BulkInsert 'C:\Users\kaviraja\Downloads\Genspark-Training\08-05-2025\Data.csv'
SELECT * FROM TEST_People

-- Built in stored precedure
GO
sp_help authors
GO

-- Storing logs in Table
CREATE TABLE TEST_BulkInsertLog(
	LogId int identity(1,1) PRIMARY KEY,
	FilePath nvarchar(1000),
	status nvarchar(50) constraint chk_status Check(status in('Status','Failed')),
	Message nvarchar(1000),
	InsertedOn DateTime Default GetDate()
);
GO
Create or Alter proc proc_BulkInsert(@filepath nvarchar(500))
as
begin
begin try
	declare @insertQuery nvarchar(max)
	set @insertQuery = 'BULK INSERT TEST_People from '''+@filepath+'''
	with (
	FIRSTROW=2,
	FIELDTERMINATOR='','',
	ROWTERMINATOR=''\n''
	)'
	exec sp_executesql @insertQuery

	insert into TEST_BulkInsertLog(filepath,status,message)
	values(@filepath, 'Success', 'Bulk Insert Completed')
end try
begin catch
	insert into TEST_BulkInsertLog(filepath,status,message)
	values(@filepath, 'Failed', Error_Message())
end catch
end
GO

proc_BulkInsert 'C:\Users\kaviraja\Downloads\Genspark-Training\08-05-2025\Data.csv'

Select * from TEST_BulkInsertLog
truncate table Test_People
GO

-- Common Table Expression(CTE) / With Expression
with cteAuthors
as
(SELECT au_id, concat(au_fname, ' ',au_lname) Author_Name from authors)

SELECT * FROM cteAuthors

-- CTE for pagination
declare @page int=2,@pageSize int=10;
with PaginationBooks as 
(SELECT title_id,title,price,ROW_NUMBER() over (order by price desc) as RowNum 
from titles)

SELECT * from PaginationBooks where RowNum BETWEEN ((@page-1)*@pageSize) and (@page*@pageSize)

-- Create a SP that will take the page number and size as param and print the books
GO
create proc proc_PaginationBooks (@page int =1, @pageSize int=10)
as 
begin 
	with PaginationBooks as 
	(SELECT title_id,title,price,ROW_NUMBER() over (order by price desc) as RowNum 
	from titles)

	SELECT * from PaginationBooks where RowNum BETWEEN ((@page-1)*@pageSize+1) and (@page*@pageSize)
end

EXEC proc_PaginationBooks 2,5

-- Advanced Pagination
 select  title_id,title, price
  from titles
  order by price desc
  offset 10 rows fetch next 10 rows only
  GO

  -- Functions (sp-explicit run, fn-used with select)
  -- Scalar function - a function which returns single value
  Create function fn_CalculateTax(@baseprice float, @tax float)
  returns float
  as
  begin
	return (@baseprice +(@baseprice*@tax/100))
  end
  GO
  SELECT dbo.fn_CalculateTax(1000,10)
  select title,dbo.fn_CalculateTax(price,12) from titles
 GO

 -- Table value function(returns table)
 CREATE function fn_tableSample(@inprice float)
 returns table
 as 
	return select title,price from titles where price>=@inprice

GO
SELECT * from dbo.fn_tableSample(10)
GO

--older and slower but supports more logic
create function fn_tableSampleOld(@minprice float)
  returns @Result table(Book_Name nvarchar(100), price float)
  as
  begin
    insert into @Result select title,price from titles where price>= @minprice
    return 
  end
GO
select * from dbo.fn_tableSampleOld(10)