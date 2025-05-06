--1. Print all the titles names
SELECT title as Book_Name from titles;

--2. Print all the titles that have been published by 1389
SELECT title from titles where pub_id = 1389;

--3. Print the books that have price in range of 10 to 15
SELECT * from titles where price between 10 and 15;

--4. Print those books that have no price
SELECT * from titles where price is null;

--5. Print the book names that starts with 'The'
SELECT title from titles where title like 'The%';

--6. Print the book names that do not have 'v' in their name
SELECT title from titles where title not like '%v%';

--7. print the books sorted by the royalty
SELECT * from titles Order by royalty;

--8. print the books sorted by publisher in descending then by types in ascending then by price in descending
SELECT * from titles
Order by pub_id DESC, type ASC, price DESC;

--9. Print the average price of books in *every* type
SELECT type, AVG(price) as Avg_Price from titles
GROUP BY type;

--10. print all the types in unique
SELECT DISTINCT type from titles;

-- 11. Print the first 2 costliest books
SELECT top 2 title, price from titles Order by price DESC;

--WITH RankedTitles AS (
--    SELECT title, price, RANK() OVER (ORDER BY price DESC) AS rank
--    FROM titles
--)
--SELECT title, price
--FROM RankedTitles
--WHERE rank <= 2;

-- 12. Print books that are of type business and have price less than 20 which also have advance greater than 7000
SELECT title from titles where type = 'business' AND price < 20 AND advance > 7000

-- 13. Select those publisher id and number of books which have price between 15 to 25 and have 'It' in its name. Print only those which have count greater than 2. Also sort the result in ascending order of count
SELECT pub_id, COUNT(*) AS book_count from titles
WHERE price BETWEEN 15 AND 25 AND title LIKE '%It%'
GROUP BY pub_id
HAVING COUNT(*) > 2
ORDER BY book_count;

-- 14. Print the Authors who are from 'CA'
SELECT au_fname, au_lname from authors WHERE state='CA';

-- SELECT CONCAT(au_fname, ' ', au_lname) Customer_Name from authors WHERE state = 'CA';

-- 15. Print the count of authors from every state
SELECT state, COUNT(*) AS Count_Of_Author
from authors GROUP BY state;