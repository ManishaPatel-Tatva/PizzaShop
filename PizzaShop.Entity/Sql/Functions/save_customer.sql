-- FUNCTION: public.save_customer(bigint, character varying, character varying, bigint, bigint)

-- DROP FUNCTION IF EXISTS public.save_customer(bigint, character varying, character varying, bigint, bigint);

CREATE OR REPLACE FUNCTION public.save_customer(
	a_id bigint,
	a_name character varying,
	a_email character varying,
	a_phone bigint,
	a_creater_id bigint)
    RETURNS bigint
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
-- variable declaration
    customer_id bigint;
BEGIN
 -- logic
    Select id INTO customer_id
    from customers
    where id = a_id AND NOT is_deleted;

    IF NOT FOUND THEN
        INSERT INTO public.customers(
        name, email, phone, created_by)
        VALUES (a_name, a_email, a_phone, a_creater_id)
        returning id into customer_id;
    ELSE
        UPDATE public.customers
        SET name=a_name, phone=a_phone, updated_by=a_creater_id, updated_at=Now()
        WHERE id= a_id;
    END IF;
    
    RETURN customer_id;
END;
$BODY$;

ALTER FUNCTION public.save_customer(bigint, character varying, character varying, bigint, bigint)
    OWNER TO postgres;
