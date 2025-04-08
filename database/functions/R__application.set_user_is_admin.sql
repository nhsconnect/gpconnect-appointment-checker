drop function if exists application.set_user_is_admin;

CREATE OR REPLACE FUNCTION application.set_user_is_admin(
    _admin_user_id integer, 
    _user_id integer, 
    _is_admin boolean
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
    org_type_search_enabled boolean
) 
LANGUAGE plpgsql
AS $function$
DECLARE
    _user_account_status_id integer;
        _multi_search_enabled boolean;
        _org_type_search_enabled boolean;
        _user_session_id integer := NULL;  -- Initialize
BEGIN
    -- Update user's admin status
    UPDATE application.user u
    SET is_admin = _is_admin
        WHERE u.user_id = _user_id
      AND u.user_id != _admin_user_id
      AND u.user_account_status_id = 2;

    -- Audit log entry
            PERFORM audit.add_entry(_user_id, 18, (NOT _is_admin)::TEXT, _is_admin::TEXT, NULL, NULL, NULL, _admin_user_id);

    -- Fetch user details
    SELECT
        u.user_account_status_id,
        u.multi_search_enabled,
        u.org_type_search_enabled
    INTO
        _user_account_status_id, 
        _multi_search_enabled, 
        _org_type_search_enabled
    FROM application.user u
    WHERE u.user_id = _user_id;

    -- Return user data
    RETURN QUERY
    SELECT
        u.user_id,
        _user_session_id AS user_session_id,  -- Placeholder, since it's not created here
        u.email_address,
        u.display_name,
        u.organisation_id,
        _user_account_status_id AS user_account_status_id,
        _multi_search_enabled AS multi_search_enabled,
        u.is_admin,
        _org_type_search_enabled AS org_type_search_enabled
    FROM application.user u
    WHERE u.user_id = _user_id;
END;
$function$;
$$ language plpgsql;
