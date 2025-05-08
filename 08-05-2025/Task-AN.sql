-- 1) List all orders with the customer name and the employee who handled the order.
SELECT C.ContactName CustomerName, CONCAT(E.FirstName, E.LastName) EmployeeName
FROM Customers C inner join Orders O on C.CustomerID = O.CustomerID
join Employees E on O.EmployeeID = E.EmployeeID

-- 2) Get a list of products along with their category and supplier name.
SELECT P.ProductName ProductName, C.CategoryName as CategoryName, S.ContactName as SupplierName
FROM Products P join Categories C on P.CategoryID = C.CategoryID
JOIN Suppliers S on P.SupplierID = S.SupplierID

-- 3) Show all orders and the products included in each order with quantity and unit price.
SELECT O.OrderID, OD.Quantity, P.ProductName, P.SupplierID, P.UnitPrice
FROM Orders O join [Order Details] OD on O.OrderID = OD.OrderID
JOIN Products P on OD.ProductID = P.ProductID

-- 4) List employees who report to other employees (manager-subordinate relationship).
SELECT CONCAT(E.FirstName, E.LastName) EmployeeName, CONCAT(R.FirstName, R.LastName) ReportingPerson
FROM Employees E join Employees R on E.ReportsTo = R.EmployeeID

-- 5) Display each customer and their total order count.
SELECT C.ContactName as Customer_Name, COUNT(*) as Order_Count
from Customers C join Orders O on C.CustomerID = O.CustomerID
GROUP BY C.ContactName

-- 6) Find the average unit price of products per category.
SELECT C.CategoryName, AVG(P.UnitPrice) as Avg_UnitPrice
from Categories C join Products P on C.CategoryID = P.CategoryID
GROUP BY C.CategoryName

-- 7) List customers where the contact title starts with 'Owner'.
SELECT * from Customers 
Where ContactTitle like 'Owner%'

-- 8) Show the top 5 most expensive products.
SELECT TOP 5 ProductName, UnitPrice Product_Price from Products
Order by UnitPrice desc

-- 9) Return the total sales amount (quantity × unit price) per order.
SELECT OD.OrderID, SUM(OD.UnitPrice * OD.Quantity) as Total_Price
FROM Orders O join [Order Details] OD on O.OrderID = OD.OrderID
GROUP BY OD.OrderID

-- 10) Create a stored procedure that returns all orders for a given customer ID.
GO
CREATE or ALTER proc proc_ReturnAllOrders(@pcustomerID varchar(15))
as
begin
	SELECT * from Orders 
	where CustomerID = @pcustomerID
end
go
proc_ReturnAllOrders 'ALFKI';

-- 11) Write a stored procedure that inserts a new product.
go
create or alter proc proc_InsertProduct(@pproductName varchar(20),@psupplierID int,@pcategoryID int, @pquantityPerUnit varchar(20), @punitPrice float, @punitsInStock int, @punitsOnOrder int, @preorderLevel int,@pdiscontinued int)
as 
begin
	Insert into Products(ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued)
	Values(@pproductName,@psupplierID,@pcategoryID,@pquantityPerUnit,@punitPrice,@punitsInStock,@punitsOnOrder,@preorderLevel,@pdiscontinued)
end
go
proc_InsertProduct 'Chicken', 4, 6,'1 kg pkg', 40.00, 20, 1, 10, 0  

-- 12) Create a stored procedure that returns total sales per employee.
go
create or alter proc proc_TotalSalesPerEmployee
as 
begin
	SELECT E.EmployeeID, CONCAT(E.FirstName, ' ',E.LastName) as EmployeeName,COUNT(CustomerID) as TotalCustomer,SUM(OD.UnitPrice * OD.Quantity) as TotalSales
	FROM Employees E join Orders O on E.EmployeeID = O.EmployeeID
	join [Order Details] OD on O.OrderID = OD.OrderID
	Group by E.EmployeeID, CONCAT(E.FirstName, ' ',E.LastName)
	Order by EmployeeID
end
go
proc_TotalSalesPerEmployee

-- 13) Use a CTE to rank products by unit price within each category.
GO
with cteRankByPrice 
as 
	(SELECT ProductID, ProductName, QuantityPerUnit, UnitPrice, 
	UnitsInStock, ReorderLevel, Discontinued, CategoryID,
	ROW_NUMBER() OVER(PARTITION BY CategoryID ORDER BY UnitPrice) as PriceRank
	from Products
	)
SELECT * from cteRankByPrice

-- 14) Create a CTE to calculate total revenue per product and filter products with revenue > 10,000.
GO
with cteCalcTotalRevenue
as 
	(SELECT P.ProductName, SUM(OD.UnitPrice*OD.Quantity) as TotalRevenue
	FROM Products P JOIN [Order Details] OD on P.ProductID = OD.ProductID
	GROUP BY P.ProductName)

	SELECT * FROM cteCalcTotalRevenue WHERE TotalRevenue > 10000

-- 15) Use a CTE with recursion to display employee hierarchy.
GO
with cteEmployeeHierarchy 
as
	(Select EmployeeID, CONCAT(FirstName, ' ', LastName) as EmployeeName, ReportsTo, 0 as Level,CAST(FirstName+' '+LastName as varchar(MAX)) as HierarchyPath
	from Employees
	Where ReportsTo IS NULL
	
	UNION ALL
	
	Select E.EmployeeID, CONCAT(E.FirstName, ' ', E.LastName), E.ReportsTo, EH.Level+1, CAST(EH.HierarchyPath+' > '+E.FirstName+' '+E.LastName as varchar(MAX))
	from Employees E join cteEmployeeHierarchy EH ON E.ReportsTo = EH.EmployeeID)

Select * from cteEmployeeHierarchy


-- Tables for visualization
select * from Categories
select * from Products
select * from Suppliers
select * from Customers
select * from Orders
select * from [Order Details]
select * from Employees
