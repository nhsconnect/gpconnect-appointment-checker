UPDATE configuration.email SET user_name='***USERNAME***';
UPDATE configuration.email SET password='***PASSWORD***';

ALTER TABLE configuration.email ALTER COLUMN sender_address SET NOT NULL;
ALTER TABLE configuration.email ALTER COLUMN host_name SET NOT NULL;
ALTER TABLE configuration.email ALTER COLUMN port SET NOT NULL;
ALTER TABLE configuration.email ALTER COLUMN encryption SET NOT NULL;
ALTER TABLE configuration.email DROP COLUMN authentication_required;
ALTER TABLE configuration.email ALTER COLUMN user_name SET NOT NULL;
ALTER TABLE configuration.email ALTER COLUMN password SET NOT NULL;
ALTER TABLE configuration.email ALTER COLUMN default_subject SET NOT NULL;

ALTER TABLE configuration.email ADD CONSTRAINT configuration_email_username_ck check (char_length(trim(user_name)) > 0);
ALTER TABLE configuration.email ADD CONSTRAINT configuration_email_password_ck check (char_length(trim(password)) > 0);



