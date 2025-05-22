-- FUNCTION: public.get_kot(bigint, boolean)

-- DROP FUNCTION IF EXISTS public.get_kot(bigint, boolean);

CREATE OR REPLACE FUNCTION public.get_kot(
	cat_id bigint,
	is_ready boolean)
    RETURNS TABLE("OrderId" bigint, "SectionName" character varying, "Tables" character varying[], "Time" timestamp without time zone, "Items" jsonb, "Instruction" character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
	if is_ready then
	   return query 
		Select 
		o.id,
		s.name,
		Array_Agg(distinct t.name),
		o.created_at,
		(
			Select jsonb_agg( jsonb_build_object(
				'Id', oi.id,
				'ItemId', oi.item_id,
				'Name', i.name,
				'Quantity', oi.ready_quantity,
				'Modifierslist',( Select jsonb_agg( jsonb_build_object(
					'Name', m.name,
					'Quantity', oim.quantity
				))
				from order_items_modifier as oim
				join modifier as m on m.id = oim.modifier_id
				where oim.order_item_id = oi.id
			)))
			from order_items as oi
			join items as i on i.id = oi.item_id
			where not oi.is_deleted 
				and oi.order_id = o.id 
				and (cat_id = 0 or i.category_id = cat_id ) 
				and oi.ready_quantity > 0
		),
		o.instructions
		From orders as o
		join "OrderTableMapping" as otm on otm.order_id = o.id
		join tables as t on otm.table_id = t.id
		join sections as s on t.section_id = s.id
		join order_status as os on os.id = o.status_id 
		where not o.is_deleted and os.name != 'Completed' and os.name != 'Cancelled'
		group by o.id, s.name, o.created_at, o.instructions;
	else
		return query 
		Select 
		o.id,
		s.name,
		Array_Agg(distinct t.name),
		o.created_at,
		(
			Select jsonb_agg( jsonb_build_object(
				'Id', oi.id,
				'ItemId', oi.item_id,
				'Name', i.name,
				'Quantity', oi.quantity - oi.ready_quantity,
				'Modifierslist',( Select jsonb_agg( jsonb_build_object(
					'Name', m.name,
					'Quantity', oim.quantity
				))
				from order_items_modifier as oim
				join modifier as m on m.id = oim.modifier_id
				where oim.order_item_id = oi.id
			)))
			from order_items as oi
			join items as i on i.id = oi.item_id
			where not oi.is_deleted 
				and oi.order_id = o.id 
				and (cat_id = 0 or i.category_id = cat_id ) 
				and oi.quantity - oi.ready_quantity > 0
		),
		o.instructions
		From orders as o
		join "OrderTableMapping" as otm on otm.order_id = o.id
		join tables as t on otm.table_id = t.id
		join sections as s on s.id = t.section_id
		join order_status as os on o.status_id = os.id
		where not o.is_deleted and os.name != 'Completed' and os.name != 'Cancelled'
		group by o.id, s.name, o.created_at, o.instructions;
	end if;
END;
$BODY$;

ALTER FUNCTION public.get_kot(bigint, boolean)
    OWNER TO postgres;
