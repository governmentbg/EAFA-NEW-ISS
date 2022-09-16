update iss."Files"
set "UpdatedBy" = "UpdatedBy"||','||@Id,
    "Comments" = "Comments"||','||@CtrlCode
where "ID" = @FileId