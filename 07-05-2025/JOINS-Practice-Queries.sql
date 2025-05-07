---------------------JIONS--------------------------------------------------------------
-- Print the publisher details of the publisher who has never published

SELECT * from publishers where pub_id not in (SELECT distinct pub_id from titles)

SELECT title, pub_name 
from titles right join publishers
on titles.pub_id = publishers.pub_id;

-- Select the author_id for all the books. Print the author_id and the book name
SELECT au_id as Author_id, title as Book_name
FROM titleauthor INNER JOIN titles
on titleauthor.title_id = titles.title_id

SELECT au_id, CONCAT(au_fname,' ' ,au_lname) as Author_Name, Book_name
FROM authors INNER JOIN
(SELECT au_id as Author_id, title as Book_name
FROM titleauthor INNER JOIN titles
on titleauthor.title_id = titles.title_id) as books
on authors.au_id = books.Author_id

-- Print the publisher's name, book name and the order date of the books
SELECT pub_name as Publisher_Name, title as Book_Name, ord_date as Order_Date
from sales join titles on sales.title_id = titles.title_id
join publishers on titles.pub_id = publishers.pub_id
ORDER BY Order_Date

-- Print the publisher name and the first book sale date for all the publishers
SELECT pub_name as Publisher_Name, MIN(ord_date) AS First_Sale_Date
from publishers LEFT OUTER JOIN titles t
on publishers.pub_id = t.pub_id
LEFT OUTER JOIN sales on t.title_id = sales.title_id
GROUP BY pub_name
ORDER BY First_Sale_Date DESC

-- Print the bookname and the store address of the sale
SELECT title Book_Name, CONCAT(stor_name, ',',stor_address, ',',city, ',',state, ',',zip) as Store_Address
from titles JOIN sales ON titles.title_id = sales.title_id
JOIN stores ON sales.stor_id = stores.stor_id
