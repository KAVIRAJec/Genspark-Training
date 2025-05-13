--- Cursor
DO $$
declare
	rental_record record;
	rental_cursor cursor for
	select r.rental_id, c.first_name, c.last_name, r.rental_date
	from rental r join customer c on r.customer_id = c.customer_id
	order by r.rental_id;
begin
	open rental_cursor;
	loop
		fetch rental_cursor into rental_record;
		exit when not found;

		raise notice 'rental_id: %, customer: % %, date: %',
			rental_record.rental_id,rental_record.first_name,rental_record.last_name,rental_record.rental_date;
	end loop;
	close rental_cursor;
end;
$$

-----Cursor Application - Rental tax application-------
create table rental_tax_log (
	rental_id int,
	customer_name text,
	rental_date timestamp,
	amount numeric,
	tax numeric
)

SELECT * from rental_tax_log
DO $$
declare
	rec record;
	cur cursor for
		select r.rental_id, c.first_name || ' ' || c.last_name customer_name,r.rental_date,p.amount
		from rental r join payment p on r.rental_id = p.rental_id
		join customer c on r.customer_id = c.customer_id;
begin
	open cur;
	loop
		fetch cur into rec;
		exit when not found;

		insert into rental_tax_log (rental_id, customer_name, rental_date, amount, tax)
			values (rec.rental_id,rec.customer_name,rec.rental_date,rec.amount,rec.amount*0.10);
	end loop;
	close cur;
end;
$$;

--TASK:
-- Cursors 
-- 1) Write a cursor to list all customers and how many rentals each made. Insert these into a summary table.
CREATE TABLE customer_rental_summary (
    customer_id INT PRIMARY KEY,
    customer_name TEXT,
    rental_count INT DEFAULT 0
);
SELECT * FROM customer_rental_summary;

DO $$
declare
	rec record;
	cur cursor for
		SELECT c.customer_id, c.first_name || ' ' || c.last_name customer_name, COUNT(*) rental_count
		from customer c left join rental r on c.customer_id = r.rental_id
		GROUP by c.customer_id
		ORDER by rental_count desc;	
begin
	open cur;
	loop 
		fetch cur into rec;
		exit when not found;

		INSERT INTO customer_rental_summary (customer_id, customer_name, rental_count)
        VALUES (rec.customer_id, rec.customer_name, rec.rental_count);
	end loop;
	close cur;
end;
$$

-- 2) Using a cursor, print the titles of films in the 'Comedy' category rented more than 10 times.
DO $$
DECLARE
	rec record;
	cur cursor for 
		SELECT f.title, COUNT(r.rental_id) as rental_count
		from film f join film_category fc on f.film_id = fc.film_id
		join category c on fc.category_id = c.category_id
		join inventory i on f.film_id = i.film_id
		join rental r on i.inventory_id = r.inventory_id
		where c.name = 'Comedy'
		GROUP BY f.title
		having COUNT(r.rental_id) > 10;
begin
	open cur;
	loop
		fetch cur into rec;
		EXIT WHEN NOT FOUND;
		RAISE NOTICE 'Title: %, Rentals: %', rec.title,rec.rental_count;
	end loop;
	close cur;
end;
$$

-- 3) Create a cursor to go through each store and count the number of distinct films available, and insert results into a report table.
CREATE TABLE store_distinct_film_report (
    store_id INT PRIMARY KEY,
    film_count INT
);
SELECT * from store_distinct_film_report

DO $$
DECLARE
	rec record;
	film_count INT;
	cur cursor for
	SELECT store_id FROM store;
BEGIN
	open cur;
	loop
		fetch cur into rec;
		EXIT WHEN NOT FOUND;
		select COUNT(DISTINCT film_id) into film_count
		from inventory
		where store_id = rec.store_id;

		INSERT INTO store_distinct_film_report(store_id, film_count)
		VALUES (rec.store_id,film_count);
	end loop;
	close cur;
END;
$$

-- 4) Loop through all customers who haven't rented in the last 6 months and insert their details into an inactive_customers table.
CREATE TABLE inactive_customers (
    customer_id INT PRIMARY KEY,
    customer_name TEXT,
    last_rental_date DATE
);
SELECT * FROM inactive_customers;

DO $$
DECLARE
	rec record;
	cur cursor for
	select c.customer_id, c.first_name || ' ' || c.last_name customer_name, MAX(r.rental_date) AS last_rental_date
	from customer c left join rental r on c.customer_id = r.customer_id
	GROUP BY c.customer_id
	HAVING MAX(r.rental_date) is NULL OR MAX(r.rental_date) < current_date - INTERVAL '6 months'; 
BEGIN
	open cur;
	loop
		fetch cur into rec;
		EXIT WHEN NOT FOUND;
		INSERT INTO inactive_customers (customer_id, customer_name, last_rental_date) VALUES (
            rec.customer_id, rec.customer_name, rec.last_rental_date);
	end loop;
	close cur;
END;
$$

-- Transactions
-- 1) Write a transaction that inserts a new customer, adds their rental, and logs the payment – all atomically.
BEGIN;
WITH new_customer AS (
    INSERT INTO customer (store_id, first_name, last_name, email, address_id, active, create_date) 
	VALUES (1, 'John', 'Doe', 'john.doe@example.com', 5, 1, NOW())
    RETURNING customer_id
),
new_rental AS (
	INSERT INTO rental (rental_date, inventory_id, customer_id, staff_id) 
	SELECT NOW(), 1000, customer_id, 1 FROM new_customer
	RETURNING rental_id, customer_id
)

INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date)
SELECT customer_id, 1, rental_id, 4.99, NOW() FROM new_rental;
COMMIT;
SELECT * FROM customer order by customer_id desc;
SELECT * FROM payment order by customer_id desc;
rollback;

-- 2) Simulate a transaction where one update fails (e.g., invalid rental ID), and ensure the entire transaction rolls back.
DO $$
BEGIN
	BEGIN
		UPDATE Customer SET last_name = 'Doe' WHERE customer_id = (SELECT MAX(customer_id) FROM Customer);

		UPDATE rental SET rental_date = NOW() WHERE customer_id = -999;
		COMMIT;
	EXCEPTION WHEN OTHERS THEN
	ROLLBACK;
	RAISE 'Transaction rollbacked because of error';
	END;
END;
$$

-- 3) Use SAVEPOINT to update multiple payment amounts. Roll back only one payment update using ROLLBACK TO SAVEPOINT.
BEGIN;
SAVEPOINT payment1;
UPDATE payment SET amount = amount - 5 WHERE payment_id = 17507;

SAVEPOINT payment2;
UPDATE payment SET amount = amount - 5 WHERE payment_id = -999;
ROLLBACK TO SAVEPOINT payment2;

SAVEPOINT payment3;
UPDATE payment SET amount = amount + 5 WHERE payment_id = 17508;

COMMIT;

select * from payment where payment_id in (17507,17508);

-- 4) Perform a transaction that transfers inventory from one store to another (delete + insert) safely.
BEGIN;
	DELETE FROM payment WHERE rental_id IN (SELECT rental_id FROM rental WHERE inventory_id = 1001);
	DELETE FROM rental where inventory_id = 1001;
WITH deleted_inventory AS (
	DELETE FROM inventory where inventory_id = 1001
	RETURNING film_id)

INSERT INTO inventory (film_id, store_id, last_update)
SELECT film_id, 2, NOW() FROM deleted_inventory;

COMMIT;
-- ROLLBACK

-- 5) Create a transaction that deletes a customer and all associated records (rental, payment), ensuring referential integrity.
BEGIN;
DELETE from payment where customer_id = 605;
DELETE FROM rental where customer_id = 605;
DELETE FROM customer where customer_id = 605;
COMMIT;

-- Triggers
-- 1) Create a trigger to prevent inserting payments of zero or negative amount.
CREATE OR REPLACE FUNCTION fn_payment_amount_validate()
RETURNS TRIGGER AS $$
BEGIN
	if NEW.amount <= 0 THEN
		RAISE EXCEPTION 'Payment amount must be greater than zero. Attempted amount: %', NEW.amount;
	end if;
	return new;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER tgr_payment_amount_validate
BEFORE UPDATE ON payment
FOR EACH ROW
EXECUTE FUNCTION fn_payment_amount_validate();

INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date)
VALUES (1, 1, 1, 0, NOW());

-- 2) Set up a trigger that automatically updates last_update on the film table when the title or rental rate is changed.
CREATE OR REPLACE FUNCTION fn_update_film_lastupdate()
RETURNS TRIGGER AS $$
BEGIN
	if NEW.title is distinct from OLD.title OR NEW.rental_rate is distinct from OLD.rental_rate THEN
		NEW.last_update:= NOW();
	END if;
	RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER tgr_update_film_lastupdate
BEFORE UPDATE ON film
FOR EACH ROW
EXECUTE FUNCTION fn_update_film_lastupdate();

UPDATE film SET title = 'New Title' WHERE film_id = 1;

-- 3) Write a trigger that inserts a log into rental_log whenever a film is rented more than 3 times in a week.
CREATE TABLE rental_log (
	log_id SERIAL PRIMARY KEY,
	film_id INT,
	week_start DATE,
	rental_count INT,
	log_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
drop table rental_log
select * from rental_log

CREATE OR REPLACE FUNCTION fn_insert_rental_log()
RETURNS TRIGGER AS $$
DECLARE
	v_film_id INT;
	v_week_start DATE := date_trunc('week', NEW.rental_date)::DATE;
	v_rental_count INT;
BEGIN
	SELECT film_id into v_film_id from inventory 
	where inventory_id = NEW.inventory_id;
	
	WITH rental_count AS (
	SELECT COUNT(*) as count
	FROM rental r join inventory i on r.inventory_id = i.inventory_id
	WHERE i.film_id = v_film_id
	AND date_trunc('week', r.rental_date) = v_week_start
	)
	SELECT count into v_rental_count FROM rental_count;
	
	if v_rental_count >= 3 THEN
		INSERT INTO rental_log (film_id, week_start, rental_count)
        VALUES (v_film_id, v_week_start, v_rental_count);
	end if;
	RETURN new;
END;
$$ LANGUAGE plpgsql

CREATE TRIGGER tgr_insert_rental_log
AFTER INSERT ON rental
FOR EACH ROW 
EXECUTE FUNCTION fn_insert_rental_log();

INSERT INTO rental (rental_date, inventory_id, customer_id, staff_id)
VALUES (NOW(), 1002, 1, 1);



--TABLE REFERENCE:
select * from film
select * from film_category
select * from category
select * from rental
select * from customer
select * from inventory
select * from store
select * from payment 


