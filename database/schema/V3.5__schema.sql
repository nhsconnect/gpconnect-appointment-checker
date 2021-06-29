ALTER TABLE configuration.email ALTER COLUMN sender_address SET NOT NULL;
ALTER TABLE configuration.email ALTER COLUMN host_name SET NOT NULL;
ALTER TABLE configuration.email ALTER COLUMN port SET NOT NULL;
ALTER TABLE configuration.email ALTER COLUMN encryption SET NOT NULL;
ALTER TABLE configuration.email DROP COLUMN authentication_required;
ALTER TABLE configuration.email ALTER COLUMN user_name SET NOT NULL;
ALTER TABLE configuration.email ALTER COLUMN user_name SET DEFAULT '***USERNAME***';
ALTER TABLE configuration.email ALTER COLUMN password SET NOT NULL;
ALTER TABLE configuration.email ALTER COLUMN password SET DEFAULT '***PASSWORD***';
ALTER TABLE configuration.email ALTER COLUMN default_subject SET NOT NULL;

