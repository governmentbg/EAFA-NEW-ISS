using System;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures
{
    public class LogBookDto
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime From { get; set; }
        public long StartPage { get; set; }
        public long EndPage { get; set; }
    }
}
