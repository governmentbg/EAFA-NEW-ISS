using System;

namespace IARA.DomainModels.DTOModels.SystemLog
{
    public class SystemLogDTO
    {
        //Уникален номер
        public int Id { get; set; }
        //Дата и час на събитието
        public DateTime LogDate { get; set; }
        //Вид събитие 
        public string ActionType { get; set; }
        //Приложение 
        public string Application { get; set; }
        //Модул 
        public string Module { get; set; }
        //Събитие 
        public string Action { get; set; }
        //Обект
        public string TableName { get; set; }
        //Потребител
        public string Username { get; set; }
        //IP адрес
        public string IPAddress { get; set; }
        //Браузър
        public string BrowserInfo { get; set; }

    }
}
