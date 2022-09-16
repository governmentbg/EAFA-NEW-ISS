select "ID", "UpdatedBy"
from iss."Files"
where "ContentLength" = @ContentLength
  and "ContentHash" = @ContentHash
limit 1