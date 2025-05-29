CREATE OR REPLACE FUNCTION fn_transfer_funds(
    from_account TEXT,
    to_account TEXT,
    amount NUMERIC
)
RETURNS TABLE (
    sender_account TEXT,
    receiver_account TEXT,
    transferred_amount NUMERIC,
    transaction_type TEXT
)
AS $$
DECLARE
    from_balance NUMERIC;
    to_balance NUMERIC;
BEGIN
    IF from_account = to_account THEN
        RAISE EXCEPTION 'Cannot transfer to the same account[Use deposit/Withdraw]';
    END IF;

    SELECT acc."Balance" INTO from_balance
    FROM "Accounts" acc
    WHERE acc."AccountNumber" = from_account AND acc."Status" = 'Active'
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Sender Account not found or inactive';
    END IF;

    SELECT acc."Balance" INTO to_balance
    FROM "Accounts" acc
    WHERE acc."AccountNumber" = to_account AND acc."Status" = 'Active'
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Receiver account not found or inactive';
    END IF;

    IF from_balance < amount THEN
        RAISE EXCEPTION 'Insufficient balance in Sender Account';
    END IF;

    UPDATE "Accounts"
    SET "Balance" = "Balance" - amount
    WHERE "AccountNumber" = from_account;

    UPDATE "Accounts"
    SET "Balance" = "Balance" + amount
    WHERE "AccountNumber" = to_account;

    INSERT INTO "Transactions" ("AccountNumber", "TransactionType", "Amount", "TransactionDate", "PayeeAccountNumber")
    VALUES (from_account, 'Transfer', amount, NOW(), to_account);

    INSERT INTO "Transactions" ("AccountNumber", "TransactionType", "Amount", "TransactionDate", "PayeeAccountNumber")
    VALUES (to_account, 'Transfer', amount, NOW(), from_account);

    RETURN QUERY
    SELECT 
        from_account,
        to_account,
        amount,
        'Transfer';
END;
$$ LANGUAGE plpgsql;




SELECT * FROM fn_transfer_funds(
    '01b3906d-55d4-4d3b-888b-40f68d1445dc',
    '7551d98c-7fe7-4118-b16f-b92e76a692c7',
    100
);

DROP FUNCTION fn_transfer_funds(text,text,numeric)