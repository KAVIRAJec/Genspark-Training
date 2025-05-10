-- 1) List all films with their length and rental rate, sorted by length descending.
SELECT title, length, rental_rate FROM film
ORDER BY length desc

-- 2) Find the top 5 customers who have rented the most films.
SELECT rental.customer_id, CONCAT(first_name, ' ', last_name) Customer_Name, count(*) as Rental_count 
FROM customer JOIN rental on customer.customer_id = rental.customer_id
GROUP BY rental.customer_id, CONCAT(first_name, ' ', last_name) LIMIT 5

-- 3) Display all films that have never been rented.
SELECT f.title, f.description 
FROM film f LEFT JOIN inventory i on f.film_id = i.film_id
LEFT JOIN rental r on i.inventory_id = r.inventory_id
where r.rental_id is null

-- 4) List all actors who appeared in the film ‘Academy Dinosaur’.
SELECT CONCAT(a.first_name, ' ', a.last_name) Actor_Name
FROM film f 
INNER JOIN film_actor fa on f.film_id = fa.film_id
INNER JOIN actor a on fa.actor_id = a.actor_id
where f.title = 'Academy Dinosaur';

--5) List each customer along with the total number of rentals they made and the total amount paid.
SELECT CONCAT(C.first_name, ' ', C.last_name) Customer_Name, COUNT(*) as Rental_Count, SUM(P.amount) as Total_Amount
FROM customer C JOIN rental R on C.customer_id = R.customer_id
JOIN payment P on P.rental_id = R.rental_id
GROUP BY CONCAT(C.first_name, ' ', C.last_name)
ORDER BY Total_Amount desc

-- 6) Using a CTE, show the top 3 rented movies by number of rentals.
with cteTopRented
as 
	(SELECT F.title, COUNT(R.rental_id) as Rent_Count
	from film F join inventory I on F.film_id = I.film_id
	join rental R on R.inventory_id = I.inventory_id
	GROUP BY F.title
	Order by Rent_Count desc)
select * from cteTopRented limit 3

-- 7) Find customers who have rented more than the average number of films.
WITH cteCustomerRental
as (SELECT customer_id, COUNT(rental_id) as Rental_Count
	from rental GROUP BY customer_id
	),
cteAvgRental as
	(SELECT AVG(Rental_Count) as Avg_Rental from cteCustomerRental)
	
SELECT CONCAT(C.first_name, ' ', C.last_name) CustomerName, CR.Rental_Count
FROM customer C JOIN cteCustomerRental CR on C.customer_id = CR.customer_id
JOIN cteAvgRental AR on CR.Rental_Count > AR.Avg_Rental
Order BY Rental_Count desc

-- 8) Write a function that returns the total number of rentals for a given customer ID.
 CREATE OR REPLACE FUNCTION get_total_rentals(cust_id INT) 
 RETURNS INT AS 
 $$ 
	 DECLARE total_rentals INT;
	 BEGIN 
	 	SELECT COUNT(*) INTO total_rentals 
	 	FROM rental WHERE customer_id = cust_id;
		 RETURN total_rentals;
	END; 
 $$ 
 LANGUAGE plpgsql

 select get_total_rentals(1)

 -- 9) Write a stored procedure that updates the rental rate of a film by film ID and new rate.
 CREATE OR REPLACE PROCEDURE update_rental_rate(IN f_film_id INT,IN new_rate NUMERIC)
 LANGUAGE plpgsql
 AS $$
 BEGIN
 	UPDATE film
	SET rental_rate = new_rate
	WHERE film_id = f_film_id;
 END;
 $$;
-- DROP PROCEDURE update_rental_rate(integer,numeric)
 CALL update_rental_rate(6, 4.99)

-- 10) Write a procedure to list overdue rentals (return date is NULL and rental date older than 7 days).
CREATE OR REPLACE PROCEDURE get_overdue_rentals()
LANGUAGE plpgsql
as $$
begin
	SELECT 
        r.rental_id,
        r.rental_date,
        r.customer_id,
        f.title,
        r.return_date
    FROM rental r
    JOIN inventory i ON r.inventory_id = i.inventory_id
    JOIN film f ON i.film_id = f.film_id
    WHERE r.return_date IS NULL AND r.rental_date < CURRENT_DATE - INTERVAL '7 days';
end
$$

CALL get_overdue_rentals();


-- sample DB fetch
select * from customer
select * from film
select * from film_actor
select * from actor
select * from inventory
select * from rental
select * from payment