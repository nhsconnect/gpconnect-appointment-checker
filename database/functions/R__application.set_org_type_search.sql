drop function if exists application.set_org_type_search;

CREATE OR REPLACE FUNCTION application.set_org_type_search(
    _admin_user_id integer, 
    _user_id integer, 
    _org_type_search_enabled boolean
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
        _is_admin boolean;
        _user_session_id integer := NULL;
BEGIN
    -- Update org_type_search_enabled flag
    UPDATE application.user u
    SET org_type_search_enabled = _org_type_search_enabled
        WHERE u.user_id = _user_id;

    -- Audit log entry
            PERFORM audit.add_entry(_user_id, 17, (NOT _org_type_search_enabled)::TEXT, _org_type_search_enabled::TEXT, NULL, NULL, NULL, _admin_user_id);

    -- Fetch user details
    SELECT
        u.user_account_status_id,
        u.multi_search_enabled,
        u.is_admin
    INTO
        _user_account_status_id, 
        _multi_search_enabled, 
        _is_admin
    FROM application.user u
    WHERE u.user_id = _user_id;

    -- Return user data
    RETURN QUERY
    SELECT
        u.user_id,
        _user_session_id AS user_session_id,
        u.email_address,
        u.display_name,
        u.organisation_id,
        _user_account_status_id AS user_account_status_id,
        _multi_search_enabled AS multi_search_enabled,
        _is_admin AS is_admin,
        u.org_type_search_enabled
    FROM application.user u
    WHERE u.user_id = _user_id;
END;
$function$;