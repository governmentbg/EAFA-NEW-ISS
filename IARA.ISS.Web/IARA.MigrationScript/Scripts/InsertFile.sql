insert into iss."Files"
("Name", "MimeType", "ReferenceCounter", "ContentHash", "ContentLength", "Content", "UploadedOn", "Comments", "IsActive", "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn")
values (@Name, @MimeType, 0, @ContentHash, @ContentLength, @Content, @UploadedOn, @Comments, true, 'MigrateScript-IARA-Old-files', current_timestamp, @UpdatedBy, null)