-- PROCEDURE: public.update_kot_item(boolean, integer, bigint)

-- DROP PROCEDURE IF EXISTS public.update_kot_item(boolean, integer, bigint);

CREATE OR REPLACE PROCEDURE public.update_kot_item(
	IN status boolean,
	IN quantity integer,
	IN orderitemid bigint)
LANGUAGE 'plpgsql'
AS $BODY$
declare
	orderItem record;
BEGIN
	select * into orderItem from order_items where order_items.Id = update_kot_item.orderItemId;
	if orderItem is null then
		raise exception 'Item does not exist';
		return;
	end if;
	if status = true then
		orderItem.ready_quantity := orderItem.ready_quantity - update_kot_item.quantity;
	else
		orderItem.ready_quantity := orderItem.ready_quantity + update_kot_item.quantity;
	end if;
	update order_items
	set
		ready_quantity = orderItem.ready_quantity
		where id = orderItem."id";
END;
$BODY$;
ALTER PROCEDURE public.update_kot_item(boolean, integer, bigint)
    OWNER TO postgres;
