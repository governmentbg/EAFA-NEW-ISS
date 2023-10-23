namespace IARA.Common.Enums
{
    public enum FluxAcdrReportStatusEnum
    {
        ///<summary>Автоматично генериран на период</summary>
        GENERATED,

        ///<summary>Ръчно генериран от служител</summary>
        MANUAL,

        ///<summary>Изтеглен за преглед</summary>
        DOWNLOADED,

        ///<summary>Качен коригиран</summary>
        UPLOADED,

        ///<summary>Изпратен към FLUX</summary>
        SENT
    }
}
