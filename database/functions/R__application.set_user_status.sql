drop function if exists application.set_user_status;

CREATE OR REPLACE FUNCTION application.set_user_status(
    _user_id integer, 
    _admin_user_id integer, 
    _user_account_status_id integer
)
RETURNS TABLE(
    user_id integer, 
    user_session_id integer, 
    email_address character varying, 
    display_name character varying, 
    organisation_id integer, 
    user_account_status_id integer, 
    multi_search_enabled boolean, 
    is_admin boolean, 
    status_changed boolean
)
LANGUAGE plpgsql
AS $function$
DECLARE
    _old_user_status character varying(100);
    _new_user_status character varying(100);
    _status_changed boolean;
    _user_session_id integer := NULL;
BEGIN
    SELECT us.user_session_id
    INTO _user_session_id
    FROM application.user_session us
    WHERE us.user_id = _admin_user_id AND us.end_time IS NULL
    ORDER BY us.start_time DESC
    LIMIT 1;

    -- Compare current vs new status
    SELECT
        uas1.description,
        uas2.description,
        u.user_account_status_id != _user_account_status_id
    INTO
        _old_user_status,
        _new_user_status,
        _status_changed
    FROM
        application.user u
    JOIN application.user_account_status uas1 ON uas1.user_account_status_id = u.user_account_status_id
    JOIN application.user_account_status uas2 ON uas2.user_account_status_id = _user_account_status_id
    WHERE u.user_id = _user_id;

    -- Only update if status has changed
    IF _status_changed THEN
        UPDATE application.user
        SET
            user_account_status_id = _user_account_status_id,
            authorised_date = CASE WHEN _user_account_status_id = 2 THEN now() ELSE NULL END
        WHERE user_id = _user_id;

        PERFORM audit.add_entry(
            _user_id, 
            10, 
            _old_user_status, 
            _new_user_status, 
            NULL, NULL, NULL, 
            _admin_user_id
        );
    END IF;

    -- Return updated user
    RETURN QUERY
    SELECT
        u.user_id,
        _user_session_id AS user_session_id,
        u.email_address,
        u.display_name,
        u.organisation_id,
        u.user_account_status_id,
        u.multi_search_enabled,
        u.is_admin,
        _status_changed AS status_changed
    FROM application.user u
    WHERE u.user_id = _user_id;
END;
$function$;
