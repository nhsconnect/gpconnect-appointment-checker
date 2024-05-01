alter table reporting.list add column report_id character varying(100)

update reporting.list set report_id='AccessRecordStructured' where report_name='Access Record: Structured';
update reporting.list set report_id='AccessRecordHTML' where report_name='Access Record: HTML';
update reporting.list set report_id='UpdateRecord' where report_name='Update Record';
