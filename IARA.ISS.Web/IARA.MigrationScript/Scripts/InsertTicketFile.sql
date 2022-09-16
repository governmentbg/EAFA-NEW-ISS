insert into "Files"
("Name", "MimeType", "ReferenceCounter", "ContentHash", "ContentLength", "Content", "UploadedOn", "Comments", "IsActive", "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn")
values (@Name, @MimeType, 1, @ContentHash, @ContentLength, @Content, @UploadedOn, null, true, 'MigrateScript-IARA-Old-tickets-files', current_timestamp, @UpdatedBy, null)
RETURNING "ID"