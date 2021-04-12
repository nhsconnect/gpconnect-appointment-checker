drop function if exists application.get_email_templates;

create function application.get_email_templates
(
)
returns table
(
	email_template_id smallint, 
	subject character varying (100), 
	body text
)
as $$
begin
	return query
	select
		t.email_template_id,
		t.subject,
		t.body
	from 
		application.email_template t;
end;
$$ language plpgsql;