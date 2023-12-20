create temporary table new_users
(
	email_address varchar(200) not null primary key
);

insert into new_users 
(
	email_address
) 
values
('newuser1@nhs.net'),
('newuser2@nhs.net');

insert into application."user" 
(
	email_address, 
	display_name, 
	organisation_id, 
	is_authorised, 
	added_date, 
	authorised_date
) 
select
	nu.email_address, 
	'not-set', 
	-1, 
	true, 
	now(), 
	now()
from new_users nu;

drop table new_users;

