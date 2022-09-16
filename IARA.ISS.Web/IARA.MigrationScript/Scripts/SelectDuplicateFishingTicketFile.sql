select "ID"
from "LRib"."FishingTicketFiles"
where "FileID" = @FileId
  and "RecordID" = @TicketId
  and "CreatedBy" = 'MigrateScript-IARA-Old-tickets-files';