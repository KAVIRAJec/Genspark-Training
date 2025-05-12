-- 1) In a transaction, if I perform multiple updates and an error happens in the third statement, but I have not used SAVEPOINT, what will happen if I issue a ROLLBACK?
-- Will my first two updates persist?
If without any savepoint, error occured at the third query, calling the 'ROLLBACK' would cause the loss of entire transaction,
This means all previous successful updates (like the 1st and 2nd updates) will also be discarded — nothing will be saved to the database. 
If we want to save the data to the Database, we should use savepoint after each critical update(rollback that particular one) or 
commit each updates immediately wheneven completed.

	BEGIN;
	update1 -> success
	update2 -> success
	update3 -> failed
	rollback -> revert update1, update2, update3

-- 2) Suppose Transaction A updates Alice’s balance but does not commit. Can Transaction B read the new balance if the isolation level is set to READ COMMITTED?
No, Transaction B cannot read the new balance(updated by Transaction A) of Alice unless Transaction A has committed.
In READ COMMITTED isolation level, a transaction only sees data that was committed before each query starts, preventing the dirty data.

tr1: updates -> not committed(not saved to db)
tr2: cannot get updated from tr1

-- 3) What will happen if two concurrent transactions both execute:
-- UPDATE tbl_bank_accounts SET balance = balance - 100 WHERE account_name = 'Alice';
-- at the same time? Will one overwrite the other?
No, the data won't overwrite.
When two transactions try to update the same row concurrently, PostgreSQL uses row-level locking.While one transaction is execting, the other transaction is blocked and waiting for it to complete
After completing(commit) the 1st, other executes and updates the value. 

tr1: update - commit [meanwhile tr2 waits]
tr2: update - commit 

-- 4) If I issue ROLLBACK TO SAVEPOINT after_alice;' will it only undo changes made after the savepoint or everything?
It will only undo the changes made after the after_alice savepoint. So, the savepoint is called as partial Rollback.

	BEGIN
	1st operation
	2nd operation
	-savepoint
	3rd operation
	-rollback (only undo the operation 3)

-- 5) Which isolation level in PostgreSQL prevents phantom reads?
The isolation level in Postgres to prevent the phantom reads is "Serializable Isolation".
It executes bt sequential order so that no row result change after any transaction.
Also, Repeatable Read isolation prevents non-repeatable data(replica).

-- 6) Can Postgres perform a dirty read (reading uncommitted data from another transaction)?
No Postgres do not support Uncommit Read isolation. The dirty read is performed using uncommit read isolation method.
Since, by default, postgres do not support Uncommit read isolation(dirty read).

-- 7) If autocommit is ON (default in Postgres), and I execute an UPDATE, is it safe to assume the change is immediately committed?
Yes, by autocommit the changes is automatically saved to the Database when the operation(update here) finishes.
It does not require user to explicitly define the 'Commit'.

-- 8) If I do this:
-- BEGIN;
-- UPDATE accounts SET balance = balance - 500 WHERE id = 1;
-- (No COMMIT yet)
-- And from another session, I run:

-- SELECT balance FROM accounts WHERE id = 1;
The second session will not see the updated balance from 1st session(since not committed), it will see last committed value.
Postgres, prevents dirty read(reading data from uncommitted operation).



-- Transaction using Exceptional Handling (TASK)
DO $$
BEGIN
	BEGIN
		UPDATE payment SET amount = amount + 10
		WHERE payment_id = 17504;
		
		UPDATE payment SET amount = amount - 10
		WHERE payment_id = 17506;
		--COMMIT;
		RAISE NOTICE 'Transaction Successful!';
	EXCEPTION
		WHEN OTHERS THEN
			--ROLLBACK;
			RAISE NOTICE 'Transaction Failed: %', SQLERRM;
	END;
END;
$$ LANGUAGE plpgsql;

-- If stored Procedure:
CREATE OR REPLACE PROCEDURE sp_amount_transfer(sender_id INT, receiver_id INT, pamount INT)
LANGUAGE plpgsql
AS $$
BEGIN
    BEGIN
        UPDATE payment SET amount = amount - pamount WHERE payment_id = sender_id;
        UPDATE payment SET amount = amount + pamount WHERE payment_id = receiver_id;
        -- COMMIT; -- even not used, in SP, it is called by default
        RAISE NOTICE 'Transaction succeeded.';
    EXCEPTION
        WHEN OTHERS THEN
            -- ROLLBACK; -- even not used, in SP, it is called by default in exception block
            RAISE NOTICE 'Transaction failed: %', SQLERRM;
    END;
END;
$$
		
CALL sp_amount_transfer(17504, 17506, 100);


-- Practice
-- autocommit
UPDATE customer SET first_name = 'Smith' WHERE customer_id = 1;  -- Succeeds
UPDATE customer SET last_name = 'test' WHERE customer_id = -1;  -- Fails

-- Using transaction
begin;
	UPDATE customer SET first_name = 'Smith' WHERE customer_id = 1;  -- Succeeds
	UPDATE customers SET last_name = 'test' WHERE customer_id = -1;  -- Fails
COMMIT;
begin;
	UPDATE customer SET first_name = 'Smith' WHERE customer_id = 1;  -- Succeeds
	UPDATE customers SET last_name = 'test' WHERE customer_id = -1;  -- Fails
ROLLBACK;

-- with savepoint
BEGIN;
	UPDATE customer SET first_name = 'Smith' WHERE customer_id = 1; -- Succeeds
	
	SAVEPOINT update_customer_two;
	UPDATE customer SET last_name = 'Test' WHERE customer_id = -1;
	ROLLBACK TO SAVEPOINT update_customer_two;
	
	INSERT INTO customer (store_id, first_name, last_name, email, address_id, activebool,active)
	VALUES (1, 'Test', 'test', 'test@gmail.com', 10000, true, 1);

COMMIT;

-- Concurrency - read uncommited (Dirty read)
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
BEGIN;
UPDATE payment SET amount = amount - 50 WHERE payment_id = 17504;
BEGIN;
SELECT * FROM payment WHERE payment_id = 17504; 

ROLLBACK

-- read commited(Default)
BEGIN;
UPDATE payment SET amount = amount - 50 WHERE payment_id = 17504;
COMMIT;

BEGIN;
SELECT * FROM payment WHERE payment_id = 17504; 

SELECT * from payment;
SELECT * from customer;

-- repeatable read
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
BEGIN;
SELECT amount FROM payment WHERE payment_id = 17504; -- reads from a replica

BEGIN;
UPDATE payment SET amount = amount + 50 WHERE payment_id = 17504;

SELECT amount FROM payment WHERE payment_id = 17504;
COMMIT;
