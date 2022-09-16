update iss."Files"
set "ReferenceCounter" = "ReferenceCounter" + 1,
    "IsActive" = true
where "ID" = @Id