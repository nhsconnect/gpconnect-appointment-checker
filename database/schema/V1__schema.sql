/* 
	Schema V1.0: initial schema
*/

create schema audit;

create table audit.entry_type
(
	entry_type_id integer not null,
	description varchar(1000) not null,

	constraint audit_entrytype_entrytypeid_pk primary key (entry_type_id)
);

insert into audit.entry_type
(
	entry_type_id,
	description
)
values
(1, 'User login'),
(2, 'User logout');

create table audit.entry
(
	entry_id serial not null,
	description varchar(1000) not null,
	
	constraint audit_entry_entryid_fk foreign key (entry_id) references audit.entry_type (entry_type_id),
	constraint audit_entry_entryid_pk primary key (entry_id)
);

