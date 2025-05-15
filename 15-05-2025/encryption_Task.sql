-- 1) Create a stored procedure to encrypt a given text
-- enable extension
create extension if not exists pgcrypto
-- Stored Procedure
CREATE OR REPLACE PROCEDURE sp_encrypt_text(inp_txt TEXT, enc_secret TEXT, encrypted_text OUT TEXT)
LANGUAGE plpgsql AS $$
BEGIN
	encrypted_text:= pgp_sym_encrypt(inp_txt, enc_secret);
	RAISE NOTICE 'Encrypted text: %', encrypted_text;
END
$$

DO $$
DECLARE result text;
begin
	CALL sp_encrypt_text('My Message', 'Secret',result);
end
$$

-- 2) Create a stored procedure to encrypt a given text
CREATE OR REPLACE PROCEDURE sp_compare_encrypted(encrypted_data BYTEA, enc_secret TEXT, out masked_text TEXT)
LANGUAGE plpgsql AS $$
BEGIN
	BEGIN
		masked_text:= pgp_sym_decrypt(encrypted_data, enc_secret);
		RAISE NOTICE 'Decrypted Message: %', masked_text;
	EXCEPTION
		WHEN OTHERS THEN
			RAISE WARNING 'Decryption failed: %', SQLERRM;
	END;		
END
$$

DO $$
DECLARE result text;
begin
	CALL sp_compare_encrypted('\xc30d040703021bf46214cc4a7f2c6dd23b019b70e64c0209f4cb5e420e0d984c670a5e66a936066626ddcad4aafa6d06630cffe7bd9631bb9f884505068469666b8fd48a4068def7d0a8b5aa', 'Secret',result);
end
$$

-- 3) Create a stored procedure to partially mask a given text
CREATE OR REPLACE PROCEDURE sp_mask_text(inp_text TEXT, out masked_text TEXT)
LANGUAGE plpgsql AS $$
BEGIN
	IF length(inp_text) <= 2 THEN
		masked_text := inp_text;
	ELSIF length(inp_text) <= 4 THEN
		masked_text := substring(inp_text FROM 1 FOR 1)||repeat('*', length(inp_text) -2)||substring(inp_text FROM length(inp_text) FOR 1);
	ELSE
		masked_text := substring(inp_text FROM 1 FOR 2)||repeat('*', length(inp_text) -4)||substring(inp_text FROM length(inp_text)-1 FOR 2);
	END IF;
	RAISE NOTICE 'Masked text: %', masked_text;
END
$$

DO $$
DECLARE result text;
begin
	CALL sp_mask_text('My Message',result);
end
$$


-- 4) Create a procedure to insert into customer with encrypted email and masked name
create table encryption_customer (
	customer_id SERIAL PRIMARY KEY,
	store_id SMALLINT NOT NULL,
    first_name VARCHAR(45) NOT NULL,
	masked_fname VARCHAR (45) NOT NULL,
    last_name VARCHAR(45) NOT NULL,
    email VARCHAR(200),
    address_id SMALLINT NOT NULL,
    activebool BOOLEAN NOT NULL DEFAULT TRUE,
    create_date TIMESTAMP NOT NULL DEFAULT NOW(),
    last_update TIMESTAMP DEFAULT NOW(),
    active INT DEFAULT 1,
	CONSTRAINT fk_customer_address FOREIGN KEY (address_id) REFERENCES address(address_id),
    CONSTRAINT fk_customer_store FOREIGN KEY (store_id) REFERENCES store(store_id)
)

CREATE OR REPLACE PROCEDURE sp_insert_encrypted_in_customer(
p_first_name TEXT,
p_last_name TEXT, 
p_email TEXT,
p_address_id INT
)
LANGUAGE plpgsql AS $$
DECLARE
	encrypted_email TEXT;
	masked_fname TEXT;
BEGIN
	BEGIN
		CALL sp_encrypt_text(p_email,'Secret',encrypted_email);
		CALL sp_mask_text(p_first_name,masked_fname);
		INSERT INTO encryption_customer(store_id,first_name,masked_fname,last_name,email,address_id,activebool,create_date,last_update,active)
		VALUES (1,p_first_name,masked_fname,p_last_name,encrypted_email,p_address_id,true,NOW(),NOW(),1);
		RAISE NOTICE 'Customer data inserted Successfully!';
	EXCEPTION WHEN OTHERS THEN
		RAISE NOTICE 'Error while inserting data: %', SQLERRM;
	END;
END
$$
CALL sp_insert_encrypted_in_customer('TestUser','Test','encryption@gmail.com',1)

-- ALTER TABLE customer
-- ALTER COLUMN email TYPE VARCHAR(200);

SELECT * from encryption_customer
-- DELETE FROM customer where customer_id in (610,611)

-- 5) Create a procedure to fetch and display masked first_name and decrypted email for all customers
CREATE OR REPLACE PROCEDURE sp_read_customer_masked(secret TEXT)
LANGUAGE plpgsql AS $$
DECLARE
	rec record;
	decrypted_email TEXT;
BEGIN
	FOR rec IN
		select customer_id, masked_fname, email from encryption_customer
	loop
		decrypted_email := pgp_sym_decrypt(rec.email::bytea ,secret);
		RAISE NOTICE 'Customer ID: %, First Name: %, Email: %', rec.customer_id, rec.masked_fname,decrypted_email;
	end loop;
END
$$

CALL sp_read_customer_masked('Secret');