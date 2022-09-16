select "ID", "Username", "Password"
from "UsrMgmt"."Users"
where "CreatedBy" = 'MigrateScript-IARA-Old-tickets-acc'
    and "Password" is not null
    and length("Password") <> 64;