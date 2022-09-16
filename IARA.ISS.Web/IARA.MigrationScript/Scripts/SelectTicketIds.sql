select "ID", "CreatedBy"
from "LRib"."FishingTickets"
where "CreatedBy" in ('MigrateScript-IARA-Old-Tickets-lrib', 'MigrateScript-IARA-Old-Tickets-lrib-register')
  and "UpdatedBy" = @TicketId