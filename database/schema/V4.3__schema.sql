alter table application.user add org_type_search_enabled boolean null;
alter table application.user alter column org_type_search_enabled set default false;
update application.user set org_type_search_enabled = false;
alter table application.user alter column org_type_search_enabled set not null;

INSERT INTO audit.entry_type(
	entry_type_id, entry_description, item1_description, item2_description)
	VALUES (17, 'User org type search enabled status changed', 'old org type search enabled status', 'new org type search enabled status');
