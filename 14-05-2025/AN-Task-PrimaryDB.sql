CREATE TABLE rental_log (
    log_id SERIAL PRIMARY KEY,
    rental_time TIMESTAMP,
    customer_id INT,
    film_id INT,
    amount NUMERIC,
    logged_on TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

select * from rental_log

-- STORED Procedure:
CREATE OR REPLACE PROCEDURE sp_add_rental_log(
    p_customer_id INT,
    p_film_id INT,
    p_amount NUMERIC
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO rental_log (rental_time, customer_id, film_id, amount)
    VALUES (CURRENT_TIMESTAMP, p_customer_id, p_film_id, p_amount);
EXCEPTION WHEN OTHERS THEN
    RAISE NOTICE 'Error occurred: %', SQLERRM;
END;
$$;

CALL sp_add_rental_log(1, 100, 4.99);
On the standby (port 5433):

-- Add a trigger to log any UPDATE to rental_log
CREATE TABLE rental_log_update_audit (
    audit_id SERIAL PRIMARY KEY,
    log_id INT,
    old_amount NUMERIC,
    new_amount NUMERIC,
    updated_on TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE OR REPLACE FUNCTION fn_log_rental_update()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO rental_log_update_audit (log_id, old_amount, new_amount)
    VALUES (OLD.log_id, OLD.amount, NEW.amount);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_log_rental_update
AFTER UPDATE ON rental_log
FOR EACH ROW
WHEN (OLD.amount IS DISTINCT FROM NEW.amount)
EXECUTE FUNCTION fn_log_rental_update();

UPDATE rental_log SET amount = 5.99 WHERE log_id = 1;

