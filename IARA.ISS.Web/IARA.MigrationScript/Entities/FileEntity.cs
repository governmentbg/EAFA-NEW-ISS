using System;

namespace IARA.MigrationScript.Entities
{
    /*
    create table files
    (
        id          bigint unsigned auto_increment
            primary key,
        ctrl_code   varchar(10)                         null,
        file_name   varchar(100)                        null,
        file_size   bigint                              null,
        file_type   varchar(50)                         null,
        content     longblob                            null,
        create_date timestamp default CURRENT_TIMESTAMP not null,
        is_linked   smallint  default 0                 null comment '0 - не е свързан с документ и трябва да се изтрие след ден'
    )
    */

    public class FileEntity
    {
        public long Id { get; set; }
        public string Ctrl_Code { get; set; }
        public string File_Name { get; set; }
        public long File_Size { get; set; }
        public string File_Type { get; set; }
        public byte[] Content { get; set; }
        public DateTime Create_Date { get; set; }
        // is_linked is always 0 or null
    }
}
