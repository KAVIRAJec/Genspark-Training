-- EMP
CREATE TABLE TASK_EMP(
	empno INT PRIMARY KEY,
	empname VARCHAR(50),
	salary DECIMAL(10,2),
	bossno INT NULL,
	deptname VARCHAR(50) NULL,
	--FOREIGN KEY (deptname) REFERENCES TASK_DEPARTMENT(deptname) ON DELETE SET NULL,
	FOREIGN KEY (bossno) REFERENCES TASK_EMP(empno)
);
-- DEPARTMENT
CREATE TABLE TASK_DEPARTMENT(
	deptname VARCHAR(50) PRIMARY KEY,
	floor INT,
	phone VARCHAR(12),
	manager_id INT NOT NULL, --managerId
	FOREIGN KEY (manager_id) REFERENCES TASK_EMP(empno)
);
-- Make Manager_id to be null:
ALTER TABLE TASK_DEPARTMENT
ALTER COLUMN manager_id INT NULL;
-- foreign key with EMP:
ALTER TABLE TASK_EMP
ADD CONSTRAINT fk_emp_dept
FOREIGN KEY (deptname) REFERENCES TASK_DEPARTMENT(deptname) ON DELETE SET NULL;

--ITEM
CREATE TABLE TASK_ITEM(
	itemname VARCHAR(50) PRIMARY KEY,
	itemtype VARCHAR(50),
	itemcolor VARCHAR(50)
);
-- SALES
CREATE TABLE TASK_SALES(
	salesno INT PRIMARY KEY,
	salesqty INT,
	itemname VARCHAR(50) NOT NULL,
	deptname VARCHAR(50) NOT NULL,
	FOREIGN KEY (itemname) REFERENCES TASK_ITEM(itemname),
	FOREIGN KEY (deptname) REFERENCES TASK_DEPARTMENT(deptname)
);


-- INSERT INTO DEPARTMENT
INSERT INTO TASK_DEPARTMENT (deptname, floor, phone, manager_id) VALUES ('Management', 5, '34', NULL);
INSERT INTO TASK_DEPARTMENT (deptname, floor, phone, manager_id) VALUES ('Books', 1, '81', NULL);
INSERT INTO TASK_DEPARTMENT (deptname, floor, phone, manager_id) VALUES ('Clothes', 2, '24', 4);
INSERT INTO TASK_DEPARTMENT (deptname, floor, phone, manager_id) VALUES ('Equipment', 3, '57', 3);
INSERT INTO TASK_DEPARTMENT (deptname, floor, phone, manager_id) VALUES ('Furniture', 4, '14', 3);
INSERT INTO TASK_DEPARTMENT (deptname, floor, phone, manager_id) VALUES ('Navigation', 1, '41', NULL);
INSERT INTO TASK_DEPARTMENT (deptname, floor, phone, manager_id) VALUES ('Recreation', 2, '29', 4);
INSERT INTO TASK_DEPARTMENT (deptname, floor, phone, manager_id) VALUES ('Accounting', 5, '35', NULL);
INSERT INTO TASK_DEPARTMENT (deptname, floor, phone, manager_id) VALUES ('Purchasing', 5, '36', NULL);
INSERT INTO TASK_DEPARTMENT (deptname, floor, phone, manager_id) VALUES ('Personnel', 5, '37', NULL);
INSERT INTO TASK_DEPARTMENT (deptname, floor, phone, manager_id) VALUES ('Marketing', 5, '38', NULL);

UPDATE TASK_DEPARTMENT SET manager_id = 1 WHERE deptname = 'Management';
UPDATE TASK_DEPARTMENT SET manager_id = 2 WHERE deptname = 'Marketing';
UPDATE TASK_DEPARTMENT SET manager_id = 5 WHERE deptname = 'Accounting';
UPDATE TASK_DEPARTMENT SET manager_id = 7 WHERE deptname = 'Purchasing';
UPDATE TASK_DEPARTMENT SET manager_id = 9 WHERE deptname = 'Personnel';
UPDATE TASK_DEPARTMENT SET manager_id = 3 WHERE deptname = 'Navigation';
UPDATE TASK_DEPARTMENT SET manager_id = 4 WHERE deptname = 'Books';

SELECT * FROM TASK_DEPARTMENT;

-- INSERT INTO EMP
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (1, 'Alice', 75000, 'Management', NULL);
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (2, 'Ned', 45000, 'Marketing', 1);
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (3, 'Andrew', 25000, 'Marketing', 2);
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (4, 'Clare', 22000, 'Marketing', 2);
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (5, 'Todd', 38000, 'Accounting', 1);
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (6, 'Nancy', 22000, 'Accounting', 5);
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (7, 'Brier', 43000, 'Purchasing', 1);
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (8, 'Sarah', 56000, 'Purchasing', 7);
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (9, 'Sophile', 35000, 'Personnel', 1);
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (10, 'Sanjay', 15000, 'Navigation', 3);
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (11, 'Rita', 15000, 'Books', 4);
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (12, 'Gigi', 16000, 'Clothes', 4);
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (13, 'Maggie', 11000, 'Clothes', 4);
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (14, 'Paul', 15000, 'Equipment', 3);
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (15, 'James', 15000, 'Equipment', 3);
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (16, 'Pat', 15000, 'Furniture', 3);
INSERT INTO TASK_EMP (empno, empname, salary, deptname, bossno) VALUES (17, 'Mark', 15000, 'Recreation', 3);

SELECT * FROM TASK_EMP;

-- Insert into TASK_ITEM
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Pocket Knife-Nile', 'E', 'Brown');
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Pocket Knife-Avon', 'E', 'Brown');
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Compass', 'N', NULL);
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Geo positioning system', 'N', NULL);
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Elephant Polo stick', 'R', 'Bamboo');
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Camel Saddle', 'R', 'Brown');
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Sextant', 'N', NULL);
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Map Measure', 'N', NULL);
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Boots-snake proof', 'C', 'Green');
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Pith Helmet', 'C', 'Khaki');
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Hat-polar Explorer', 'C', 'White');
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Exploring in 10 Easy Lessons', 'B', NULL);
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Hammock', 'F', 'Khaki');
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('How to win Foreign Friends', 'B', NULL);
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Map case', 'E', 'Brown');
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Safari Chair', 'F', 'Khaki');
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Safari cooking kit', 'F', 'Khaki');
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Stetson', 'C', 'Black');
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Tent - 2 person', 'F', 'Khaki');
INSERT INTO TASK_ITEM (itemname, itemtype, itemcolor) VALUES ('Tent -8 person', 'F', 'Khaki');

SELECT * FROM TASK_ITEM;

-- Insert into TASK_SALES
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (101, 2, 'Boots-snake proof', 'Clothes');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (102, 1, 'Pith Helmet', 'Clothes');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (103, 1, 'Sextant', 'Navigation');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (104, 3, 'Hat-polar Explorer', 'Clothes');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (105, 5, 'Pith Helmet', 'Equipment');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (106, 2, 'Pocket Knife-Nile', 'Clothes');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (107, 3, 'Pocket Knife-Nile', 'Recreation');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (108, 1, 'Compass', 'Navigation');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (109, 2, 'Geo positioning system', 'Navigation');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (110, 5, 'Map Measure', 'Navigation');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (111, 1, 'Geo positioning system', 'Books');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (112, 1, 'Sextant', 'Books');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (113, 3, 'Pocket Knife-Nile', 'Books');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (114, 1, 'Pocket Knife-Nile', 'Navigation');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (115, 1, 'Pocket Knife-Nile', 'Equipment');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (116, 1, 'Sextant', 'Clothes');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (117, 1, 'Sextant', 'Equipment');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (118, 1, 'Sextant', 'Recreation');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (119, 1, 'Sextant', 'Furniture');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (120, 1, 'Pocket Knife-Nile', 'Furniture');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (121, 1, 'Exploring in 10 Easy Lessons', 'Books');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (122, 1, 'How to win Foreign Friends', 'Books');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (123, 1, 'Compass', 'Books');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (124, 1, 'Pith Helmet', 'Books');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (125, 1, 'Elephant Polo stick', 'Recreation');
INSERT INTO TASK_SALES (salesno, salesqty, itemname, deptname) VALUES (126, 1, 'Camel Saddle', 'Recreation');

SELECT * FROM TASK_SALES;