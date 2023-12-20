alter table audit.entry_type add column details_description varchar(100) null;
update audit.entry_type set item3_description = 'date range' where entry_type_id = 4;
update audit.entry_type set details_description = 'slot result count, or error message' where entry_type_id = 4;
