drop function if exists application.set_multi_search;

CREATE OR REPLACE FUNCTION application.set_multi_search(
    _admin_user_id integer, 
    _user_id integer, 
    _multi_search_enabled boolean
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
        _is_admin boolean;
        _org_type_search_enabled boolean;
        _user_session_id integer := NULL;  -- Placeholder, as sessions are not managed here
BEGIN
    -- Update multi_search_enabled flag
    UPDATE application.user u
    SET multi_search_enabled = _multi_search_enabled
        WHERE u.user_id = _user_id;

    -- Audit log entry
            PERFORM audit.add_entry(
        _user_id, 
        12, 
        (NOT _multi_search_enabled)::TEXT, 
        _multi_search_enabled::TEXT, 
        NULL, NULL, NULL, 
        _admin_user_id
    );

    -- Fetch user details to match return type
    SELECT
        u.user_account_status_id,
        u.is_admin,
        u.org_type_search_enabled
    INTO
        _user_account_status_id, 
        _is_admin, 
        _org_type_search_enabled
    FROM application.user u
    WHERE u.user_id = _user_id;

    -- Return updated user details
    RETURN QUERY
    SELECT
        u.user_id,
        _user_session_id AS user_session_id,  -- Placeholder
        u.email_address,
        u.display_name,
        u.organisation_id,
        _user_account_status_id AS user_account_status_id,
        u.multi_search_enabled,  -- Use actual column from `u`
        _is_admin AS is_admin,
        _org_type_search_enabled AS org_type_search_enabled
    FROM application.user u
    WHERE u.user_id = _user_id;
END;
$function$;
;