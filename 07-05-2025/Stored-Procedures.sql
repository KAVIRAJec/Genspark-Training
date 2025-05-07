-- 1
create procedure proc_FirstProcedure 
as 
begin
	print 'Hello World!'
end

exec proc_FirstProcedure -- exec is not mandatory
GO
-- 2
CREATE TABLE TEST_Products(
	id int identity(1,1) constraint pk_productId primary key,
	name NVARCHAR(100) NOT NULL,
	details NVARCHAR(MAX)
)
select * from TEST_Products
GO
-- Insert in Products
-- ALTER - only change the content inside not parameter
CREATE OR ALTER proc proc_InsertProduct(@pname NVARCHAR(100),@pdetials NVARCHAR(MAX))
as 
begin
	insert into TEST_Products(name,details) values(@pname,@pdetials)
end
GO
proc_InsertProduct 'Laptop','{"brand":"Dell","spec":{"ram":"16GB","cpu":"i5"}}'
GO

-- AD HOC JSON Query
SELECT JSON_QUERY(details, '$.spec') Product_Specification from TEST_Products;
GO
-- Update in Products
CREATE OR ALTER proc proc_UpdateProductSpec(@pid INT,@newvalue VARCHAR(20))
as
begin
   update TEST_Products
   set details = JSON_MODIFY(details, '$.spec.ram',@newvalue) where id = @pid
end
GO
proc_UpdateProductSpec 1, '24GB'

-- Scalar extraction (single value extraction)
SELECT id, name, JSON_VALUE(details, '$.brand') Brand_Name FROM TEST_Products;

-- Bulk insert
CREATE TABLE TEST_Posts(
	id int PRIMARY KEY,
	title NVARCHAR(100),
	user_id INT,
	body NVARCHAR(max)
)

SELECT * FROM TEST_Posts
GO
CREATE proc proc_BulkInsertPosts(@jsondata NVARCHAR(MAX))
as
begin
	INSERT INTO TEST_Posts(user_id,id,title,body)
	SELECT userId,id,title,body from openjson(@jsondata)
	with (userId int, id int, title varchar(100), body varchar(max))
end
GO
delete from TEST_Posts

proc_BulkInsertPosts '
[
  {
    "userId": 1,
    "id": 1,
    "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
    "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
  },
  {
    "userId": 2,
    "id": 2,
    "title": "qui est esse",
    "body": "est rerum tempore vitae\nsequi sint nihil reprehenderit dolor beatae ea dolores neque\nfugiat blanditiis voluptate porro vel nihil molestiae ut reiciendis\nqui aperiam non debitis possimus qui neque nisi nulla"
  }]'

-- Using with where clause
SELECT * FROM TEST_Products WHERE
TRY_CAST(json_value(details,'$.spec.cpu') as nvarchar(20)) = 'i7'
GO
-- Create a procedure that brings post by taking the user_id as parameter
CREATE proc proc_GetPost(@puser_id INT)
as 
begin
	SELECT * FROM TEST_Posts WHERE user_id = @puser_id
end
GO
proc_GetPost 2