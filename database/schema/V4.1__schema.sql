update application.user set user_account_status_id=1 where user_account_status_id=3;

alter table application.user add constraint application_user_useraccountstatusid_authoriseddate_ck check ((user_account_status_id = 2 and authorised_date is not null) or (user_account_status_id != 2 and authorised_date is null));