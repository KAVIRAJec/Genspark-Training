-- Design the database for a shop which sells products
--	Points for consideration
--  1) One product can be supplied by many suppliers
--  2) One supplier can supply many products
--  3) All customers details have to present
--  4) A customer can buy more than one product in every purchase
--  5) Bill for every purchase has to be stored
--  6) These are just details of one shop

--------------------------------------------------------------------------------------------------

--Categories
--- category_id, name

--Products
--- product_id, name, description, category, price, quantity, image

--Suppliers
--- supplier_id, name, contact, email, address

--Product-Supplier
--- transaction_id, product_id, supplier_id, date_of_supply, quantity

--Customers
--- id, name, contact, age, dob, email, address

--Purchase
--- purchase_id, customer_id, purchase_date, total_amount, purchase_status

--Purchase_details
--- id, purchase_id, product_id, quantity, price
------------------------------------------------------------------------------------------------
--country
--- id,name

--state
--- id,name, country_id

--city
--- id,name, state_id

--area 
--- zipcode, name, city_id

--address
--- id, door_number, address_line1, zipcode
--------------------------------------------------------------------------------------------------

Create TABLE Categories(
	category_id INT PRIMARY KEY,
	name VARCHAR(30) UNIQUE
);
--CREATE TABLE Images(
--	image_id INT PRIMARY KEY,
--	image_url VARCHAR(100)
--);
Create TABLE Products(
	product_id INT PRIMARY KEY,
	name VARCHAR(50),
	description VARCHAR(100),
	category_id INT,
	price DECIMAL(10,2),
	quantity INT,
	image_url VARCHAR(100),
	FOREIGN KEY (category_id) REFERENCES Categories(category_id),
	--FOREIGN KEY (product_id) REFERENCES Images(product_id)
);
CREATE TABLE Country(
	country_id INT PRIMARY KEY,
	name VARCHAR(20)
);
CREATE TABLE State(
	state_id INT PRIMARY KEY,
	name VARCHAR(20),
	country_id INT,
	FOREIGN KEY (country_id) REFERENCES Country(country_id)
);
CREATE TABLE City(
	city_id INT PRIMARY KEY,
	name VARCHAR(20),
	state_id INT,
	FOREIGN KEY (state_id) REFERENCES State(state_id)
);
CREATE TABLE Area(
	zipcode INT PRIMARY KEY,
	area_name VARCHAR(20),
	city_id INT,
	FOREIGN KEY (city_id) REFERENCES City(city_id)
);
CREATE TABLE Address(
	address_id INT PRIMARY KEY,
	door_number VARCHAR(10),
	zipcode INT,
	FOREIGN KEY (zipcode) REFERENCES Area(zipcode)
);
Create TABLE Suppliers(
	supplier_id INT PRIMARY KEY,
	name VARCHAR(50),
	contact VARCHAR(12),
	email VARCHAR(50),
	address_id INT,
	FOREIGN KEY (address_id) REFERENCES Address(address_id)
);
Create TABLE Product_Supplier(
	transaction_id INT PRIMARY KEY,
	product_id INT,
	supplier_id INT,
	date_of_supply DATETIME,
	quantity INT,
	FOREIGN KEY (product_id) REFERENCES Products(product_id),
	FOREIGN KEY (supplier_id) REFERENCES Suppliers(supplier_id)
);
Create TABLE Customers(
	customer_id INT PRIMARY KEY,
	name VARCHAR(50),
	contact VARCHAR(12),
	age INT,
	dob DATE,
	email VARCHAR(50),
	address_id INT,
	FOREIGN KEY (address_id) REFERENCES Address(address_id)
);
Create TABLE Purchase_Status(
	purchase_status_id INT PRIMARY KEY,
	status VARCHAR(50)
);
Create TABLE Purchase(
	purchase_id INT PRIMARY KEY,
	customer_id INT,
	total_amount DECIMAL(10,2),
	purchase_date DATETIME,
	purchase_status_id INT,
	FOREIGN KEY (customer_id) REFERENCES Customers(customer_id),
	FOREIGN KEY (purchase_status_id) REFERENCES Purchase_Status(purchase_status_id)
);
Create TABLE Purchase_details(
	pruchase_details_id INT PRIMARY KEY,
	purchase_id INT,
	product_id INT,
	quantity INT,
	price DECIMAL(10,2),
	FOREIGN KEY (purchase_id) REFERENCES Purchase(purchase_id),
	FOREIGN KEY (product_id) REFERENCES Products(product_id)
);