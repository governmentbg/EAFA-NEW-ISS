using System;

namespace IARA.MigrationScript.TicketEntities
{
    /*
        id          bigint unsigned auto_increment
            primary key,
        ticket_id   bigint                              null,
        file_tip    bigint                              null,
        file_name   varchar(100)                        null,
        file_size   bigint                              null,
        file_type   varchar(50)                         null,
        content     longblob                            null,
        create_date timestamp default CURRENT_TIMESTAMP not null on update CURRENT_TIMESTAMP
    */

    public class FileEntity
    {
        public long Id { get; set; }
        public long Ticket_Id { get; set; }
        public long File_Tip { get; set; }
        public string File_Name { get; set; }
        public long File_Size { get; set; }
        public string File_Type { get; set; }
        public byte[] Content { get; set; }
        public DateTime Create_Date { get; set; }
    }
}
