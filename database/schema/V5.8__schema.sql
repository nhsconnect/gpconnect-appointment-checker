alter table reporting.list add column workflow json;

INSERT INTO reporting.list(report_name, workflow) VALUES ('Update Record', '[ "GPCONNECT_UPDATE_RECORD" ]');