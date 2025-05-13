-- Tasks:
-- 1) Try two concurrent updates to same row → see lock in action
-- Transaction 1
BEGIN;
UPDATE customer SET name = 'Alice A' WHERE id = 1;
-- not committed --> ROW level lock

-- Transaction 2
BEGIN;
UPDATE customer SET name = 'Alice B' WHERE id = 1;
-- Waits until Transaction 1 commits or rollback

-- 2) Write a query using SELECT...FOR UPDATE and check how it locks row.
-- Transaction 1 with for update locking
BEGIN;
SELECT * FROM customer WHERE customer_id = 1 FOR UPDATE;

-- Transaction 2
BEGIN;
SELECT * FROM customer WHERE id = 1; -- it will be block untill 1 finishes

-- 3) Intentionally create a deadlock and observe PostgreSQL cancel one transaction.
-- Transaction 1 with for update locking
BEGIN;
SELECT * FROM customer WHERE id = 1 FOR UPDATE;
-- Transaction 2 with for update locking
SELECT * FROM customer WHERE id = 2 FOR UPDATE;

-- creating dead lock:
SELECT * FROM customer WHERE id = 2 FOR UPDATE;-- Transaction 1
SELECT * FROM customer WHERE id = 1 FOR UPDATE;-- Transaction 2
-- Postgres cancel any one to avoid dead lock

-- 4) Use pg_locks query to monitor active locks.
select * from pg_locks
select * from pg_stat_activity 

-- 5) Explore about Lock Modes.

-- Triggers-----------------------
create table audit_log(
table_name text,
field_name text,
old_value text,
new_value text,
updated_date Timestamp default current_Timestamp
)

create or replace function Update_Audit_log()
returns trigger
as $$
begin
	INSERT INTO audit_log(table_name,field_name,old_value,new_value,updated_date)
	VALUES ('customer','email',OLD.email,NEW.email,current_Timestamp);
	return new;
end;
$$ language plpgsql

CREATE trigger trg_log_customer_email_change
before update on customer
for each row
execute function Update_Audit_log();

update customer set email = 'mary.smiths@sakilacustomer.org1' where customer_id = 1
select * from customer order by customer_id;
select * from audit_log
-- drop trigger trg_log_customer_email_Change on customer
-- drop table audit_log;

-- Trigger using arguments
create or replace function Update_Audit_log()
returns trigger 
as $$
declare 
   col_name text := TG_ARGV[0];
   tab_name text := TG_ARGV[1];
   o_value text;
   n_value text;
begin
	EXECUTE FORMAT('select ($1).%I::TEXT', COL_NAME) INTO O_VALUE USING OLD;
    EXECUTE FORMAT('select ($1).%I::TEXT', COL_NAME) INTO N_VALUE USING NEW;
	if o_value is distinct from n_value then
		Insert into audit_log(table_name,field_name,old_value,new_value,updated_date) 
		values(tab_name,col_name,o_value,n_value,current_Timestamp);
	end if;
	return new;
end;
$$ language plpgsql

create trigger trg_log_customer_email_Change
after update on customer
for each row
execute function Update_Audit_log('last_name','customer');

update customer set last_name = 'Mary1' where customer_id = 1;
select * from audit_log

