-- Cursor Based Question
-- 1) Write a cursor that loops through all films and prints titles longer than 120 minutes.
DO $$
DECLARE 
	film_rec RECORD;
	film_cursor CURSOR FOR
		SELECT title, length from film;

BEGIN
	OPEN film_cursor;

	LOOP
		FETCH film_cursor INTO film_rec;
		EXIT WHEN NOT FOUND;

		If film_rec.length > 120 THEN
			RAISE NOTICE 'Long Film: % (% minutes)', film_rec.title, film_rec.length;
		END If;
	END LOOP;

	CLOSE film_cursor;
END
$$

-- 2) Create a cursor that iterates through all customers and counts how many rentals each made.
DO $$
DECLARE 
	customer_rec RECORD;
	rental_count INT;
	customer_cursor CURSOR FOR
	SELECT customer_id, CONCAT(first_name, ' ', last_name) Customer_Name from customer;
BEGIN
	OPEN customer_cursor;

	LOOP
	FETCH customer_cursor INTO customer_rec;
	EXIT WHEN NOT FOUND;

	SELECT COUNT(customer_id) INTO rental_count from rental
	WHERE customer_id = customer_rec.customer_id;
	
	RAISE NOTICE 'Customer: %, Rentals: %', customer_rec.Customer_Name, rental_count;
	END LOOP;
	CLOSE customer_cursor;
END;
$$;

-- 3) Using a cursor, update rental rates: Increase rental rate by $1 for films with less than 5 rentals.
DO $$
DECLARE 
	film_rec RECORD;
	film_cursor CURSOR FOR
	SELECT F.film_id film_id, F.title title, F.rental_rate rental_rate, COUNT(*) as rental_count
	FROM film F JOIN inventory I on F.film_id = I.film_id
	JOIN rental R on  R.inventory_id = I.inventory_id
	GROUP BY F.film_id, F.title, F.rental_rate;
BEGIN
	OPEN film_cursor;

	LOOP
		FETCH film_cursor INTO film_rec;
		EXIT WHEN NOT FOUND;

		if film_rec.rental_count < 5 THEN
			UPDATE film
			SET rental_rate = film_rec.rental_rate + 1
			WHERE film_id = film_rec.film_id;
			RAISE NOTICE 'Increased rental rate for "%":  % rental counts.',film_rec.title, film_rec.rental_count;
		END if;
	END LOOP;

	CLOSE film_cursor;
END;
$$

-- TO CHECK
--SELECT 
--    f.film_id,
--    f.title,
--    COUNT(r.rental_id) AS rental_count
--FROM film f
--JOIN inventory i ON f.film_id = i.film_id
--JOIN rental r ON i.inventory_id = r.inventory_id
--GROUP BY f.film_id, f.title
--HAVING COUNT(r.rental_id) < 5
--ORDER BY rental_count;

-- 4) Create a function using a cursor that collects titles of all films from a particular category.
CREATE OR REPLACE FUNCTION fnGetFilmByTitle(category_name VARCHAR)
RETURNS TABLE (
	title VARCHAR,
	CategoryName VARCHAR
)
LANGUAGE plpgsql
AS $$
DECLARE 
	film_cursor CURSOR FOR
	SELECT F.title, C.name as CategoryName FROM film F
	JOIN film_category FC ON F.film_id = FC.film_id
	JOIN category C ON FC.category_id = C.category_id
	WHERE C.name = category_name;

	film_rec RECORD;

	BEGIN
		OPEN film_cursor;
		LOOP
			FETCH film_cursor INTO film_rec;
			EXIT WHEN NOT FOUND;

			title:= film_rec.title;
			CategoryName:= film_rec.CategoryName;
			RETURN NEXT;
		END LOOP;
		CLOSE film_cursor;
	END;
$$;

SELECT * FROM fnGetFilmByTitle('Action');

-- 5) Loop through all stores and count how many distinct films are available in each store using a cursor.
DO $$
DECLARE
	store_rec RECORD;
	store_cursor CURSOR FOR
		SELECT DISTINCT S.store_id store_id, COUNT(DISTINCT I.film_id) film_count
		FROM store S JOIN inventory I on S.store_id = I.store_id
		GROUP BY S.store_id;
BEGIN
	OPEN store_cursor;
	LOOP
		FETCH store_cursor INTO store_rec;
		EXIT WHEN NOT FOUND;

		RAISE NOTICE 'Store ID; % Distinct Films: %',store_rec.store_id,store_rec.film_count;
	END LOOP;
	CLOSE store_cursor;
END;
$$;

-- Trigger-Based Questions
-- 1) Write a trigger that logs whenever a new customer is inserted.
CREATE TABLE customer_logs (
	audit_id SERIAL PRIMARY KEY,
	customer_id INT,
	message TEXT,
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE OR REPLACE FUNCTION log_customer_insert()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO customer_logs (customer_id, message)
    VALUES (NEW.customer_id, 'Customer inserted successfully');
    RETURN NEW;
END;
$$;

CREATE OR REPLACE TRIGGER trg_log_new_customer
AFTER INSERT ON customer
FOR EACH ROW
EXECUTE FUNCTION log_customer_insert();

INSERT INTO customer (store_id, first_name, last_name, email, address_id, activebool, create_date, active)
VALUES (1, 'Trigger', 'Test', 'trigger@example.com', 1, true, now(),1);

-- 2) Create a trigger that prevents inserting a payment of amount 0.
CREATE OR REPLACE FUNCTION fn_prevent_zero_payment()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
	IF NEW.amount <= 0 THEN
		RAISE EXCEPTION 'TRG:Payment amount cannot be zero or negative';
	END IF;
	RETURN NEW;
END;
$$

CREATE TRIGGER trg_prevent_zero_payment
BEFORE INSERT ON payment
FOR EACH ROW
EXECUTE FUNCTION fn_prevent_zero_payment();

INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date)
VALUES (1, 1, 1, 0, NOW());

-- 3) Set up a trigger to automatically set last_update on the film table before update.
CREATE OR REPLACE FUNCTION fn_udpate_film_lastModified()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
	NEW.last_update := CURRENT_TIMESTAMP;
	RETURN NEW;
END;
$$;

CREATE TRIGGER trg_udpate_film_lastModified
BEFORE INSERT OR UPDATE ON film
FOR EACH ROW
EXECUTE FUNCTION fn_udpate_film_lastModified()

UPDATE film SET rental_rate = rental_rate + 1 WHERE film_id = 1;
SELECT film_id, rental_rate, last_update FROM film WHERE film_id = 1;

-- 4) Create a trigger to log changes in the inventory table (insert/delete).
CREATE TABLE inventory_logs (
	log_id SERIAL PRIMARY KEY,
	inventory_id INT,
	message TEXT,
	log_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE OR REPLACE FUNCTION fn_log_inventory_changes()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
	IF TG_OP = 'INSERT' THEN
		INSERT INTO inventory_logs (inventory_id,message)
		VALUES (NEW.inventory_id, 'Inserted Successfully');
	ELSEIF TG_OP = 'DELETE' THEN
		INSERT INTO inventory_logs (inventory_id,message)
		VALUES (OLD.inventory_id, 'Deleted Successfully');
	END IF;
	RETURN NEW;
END;
$$

CREATE TRIGGER trg_log_inventory_changes
AFTER INSERT OR DELETE ON inventory
FOR EACH ROW
EXECUTE FUNCTION fn_log_inventory_changes();

INSERT INTO inventory (film_id, store_id) VALUES (1, 1);
DELETE FROM inventory WHERE inventory_id = (SELECT MAX(inventory_id) FROM inventory);

-- 5) Write a trigger that ensures a rental can’t be made for a customer who owes more than $50.
CREATE OR REPLACE FUNCTION fn_check_customer_pending()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
DECLARE
	total_amount NUMERIC;
BEGIN
	SELECT SUM(amount) INTO total_amount FROM payment
	WHERE customer_id = NEW.customer_id;

	if total_amount < 100 THEN
		RAISE EXCEPTION 'Rental not allowed because Customer(%) owes more than 100$', NEW.customer_id;
	END IF;
	RETURN NEW;
END;
$$;

CREATE TRIGGER trg_check_customer_pending
BEFORE INSERT ON rental
FOR EACH ROW
EXECUTE FUNCTION fn_check_customer_pending();

SELECT customer_id, SUM(amount) from payment 
GROUP BY customer_id;

INSERT INTO rental (rental_date, inventory_id, customer_id, return_date, staff_id)
VALUES (NOW(), 1, 184, NULL, 1);

-- Transaction-Based Questions (5)
-- 1) Write a transaction that inserts a customer and an initial rental in one atomic operation.
BEGIN;
WITH new_customer AS (
	INSERT INTO customer(store_id, first_name, last_name, email, address_id, activebool, create_date)
	VALUES (1, 'Transaction', 'Test', 'john.doe@example.com',1, true, NOW())
	RETURNING customer_id
)
INSERT INTO rental (rental_date, inventory_id,customer_id,staff_id)
SELECT NOW(),100,customer_id,1 FROM new_customer;
COMMIT;
ROLLBACK;

SELECT * FROM customer ORDER BY customer_id DESC LIMIT 1;

SELECT * FROM rental WHERE customer_id = (
    SELECT customer_id FROM customer ORDER BY customer_id DESC LIMIT 1
);

-- 2) Simulate a failure in a multi-step transaction (update film + insert into inventory) and roll back.
DO $$
BEGIN
	BEGIN
		UPDATE film set rental_rate=rental_rate+1
		WHERE film_id=1;

		INSERT INTO inventory (film_id, store_id)
		VALUES (-1,-1);
		COMMIT;
	EXCEPTION WHEN OTHERS THEN
		RAISE NOTICE 'Transaction failed: %', SQLERRM;
	END;
END;
$$

-- 3) Create a transaction that transfers an inventory item from one store to another.
DO $$
DECLARE
BEGIN
    BEGIN
        UPDATE inventory
        SET store_id = 2
        WHERE inventory_id = 123 AND store_id = 1;
		RAISE NOTICE 'Transaction Successful';
		
    EXCEPTION WHEN OTHERS THEN
        RAISE NOTICE 'Transfer failed: %', SQLERRM;
    END;
END;
$$;

SELECT * FROM inventory WHERE inventory_id = 123 AND store_id = 1;

-- 4) Demonstrate SAVEPOINT and ROLLBACK TO SAVEPOINT by updating payment amounts, then undoing one.
BEGIN; 
	UPDATE payment
	SET amount = amount + 1 WHERE payment_id = 17503;

	SAVEPOINT before_second_update;

	UPDATE payment
	SET amount = amount + 10 WHERE payment_id = 17504;
	ROLLBACK TO SAVEPOINT before_second_update;

	UPDATE payment
	SET amount = amount + 5	WHERE payment_id = 17505;
	COMMIT;

SELECT payment_id, amount FROM payment WHERE payment_id IN (17503, 17504, 17505);

-- sample DB fetch
select * from customer
select * from film 
select * from film_actor
select * from actor
select * from inventory
select * from rental
select * from payment
select * from store

select * from customer_logs
select * from inventory_logs

