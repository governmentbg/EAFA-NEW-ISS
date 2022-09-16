insert into "LRib"."FishingTicketFiles"
("FileTypeID", "RecordID", "FileID", "IsActive", "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn", "OldDbTicketID")
values(@FileTypeId, @TicketId, @FileId, true, 'MigrateScript-IARA-Old-tickets-files', '2022-01-20', @Id, null, @OldTicketId)