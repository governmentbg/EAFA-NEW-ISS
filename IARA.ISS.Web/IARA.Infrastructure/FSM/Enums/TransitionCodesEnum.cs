namespace IARA.Infrastructure.FSM.Enums
{
    public enum TransitionCodesEnum
    {
        /// <summary> Попълване на заявлние от заявителя </summary>
        ApplFillByApplicant,

        /// <summary> Генериране на PDF (с timestamp) на заявлнието за подпис с КЕП </summary>
        GenSignedPDF,

        /// <summary> Качване на PDF с невалиден подпис или нарушен интегритет </summary>
        UploadApplFailedVer,

        /// <summary> Качване на PDF с валид подпис и ненарушен интегритет </summary>
        UploadApplSuccVer,

        /// <summary> Иницииране на проверки от външни регистри (през опашка към RegiX) </summary>
        InitChecksFromExtRegs,

        /// <summary> Намерени критични грешки след проверките от външни регистри (RegiX) </summary>
        FoundCriticalErr,

        /// <summary> Иницииране на проверка на данните и корекции от служител </summary>
        InitEmpReviewAndCorr,

        /// <summary> Потвърждение за частично премахнати грешки след проверки от RegiX </summary>
        PartialErrElimination,

        /// <summary> Потвърждение за липса (или успешно премахване) на грешки след проверки от RegiX </summary>
        NoErrConfirmByEmp,

        /// <summary> Входиране на услуга, която трябва да бъде платена </summary>
        FileInMustPayService,

        /// <summary> Входирана на вече платена услуга </summary>
        FileInPaidService,

        /// <summary> Входиране на безплатна услуга </summary>
        FileInFreeService,

        /// <summary> Извършване плащане на услуга </summary>
        PerformPayment,

        /// <summary> Неуспешно извършване плащане на услуга </summary>
        PerformUnsuccPayment,

        /// <summary> Изминаване на определен срок за плащане на услуга </summary>
        PayExpOfTerm,

        /// <summary> N-то крайно изминаване на определен срок за плащане на услуга </summary>
        PayExpOfTermNTimes,

        /// <summary> Подновяване на плащане на услуга </summary>
        RenewPaymentRequest,

        /// <summary> Установяване на редовност в данните и основание за издаване на административен акт </summary>
        EstDataIrregularity,

        /// <summary> Установяване на нередовност в данните и необходимост от корекции от заявителя </summary>
        EstDataRegularity,

        /// <summary> Попълване на административен акт </summary>
        FillAdmAct,

        /// <summary> Ръчно анулиране на заявление </summary>
        ManualCancel,

        /// <summary> Подновяване на попълване за корекции от заявител/служител </summary>
        RenewApplFill,

        /// <summary> Добавяне на запис за заявление от деловодството </summary>
        PaperApplRecordAdd,

        /// <summary> Попълване на промени от служител по съдържанието на заявление </summary>
        ApplFillByEmp,

        /// <summary> Потвърждение от служител за коректност в данните на безплатна услуга след проверка от RegiX </summary>
        FreeServiceNoErrConfByEmp,

        /// <summary> Потвърждение от служител за коректност в данните на вече платена услуга след проверка от RegiX </summary>
        PaidServiceNoErrConfByEmp,

        /// <summary> Потвърждение от служител за коректност в данните на услуга, която трябва да бъде платена, след проверка от RegiX </summary>
        MustPayServiceNoErrConfByEmp,

        /// <summary> Процес по въвеждане на платежни данни от служител </summary>
        EnterPaymentData,

        /// <summary> Потвърждение за отказ от плащане </summary>
        PaymentRefusal,

        /// <summary> Установяване, че билетът трябва да се плати </summary>
        MustByPaidTicket,

        /// <summary> Установяване, че няма грешки след проверки от външни регистри (през RegiX) </summary>
        NoErrFromChecks,

        /// <summary> Иницииране на проверки от външни системи (RegiX) след успешно плащане </summary>
        InitExtRegsCheckAfterPaid,

        /// <summary>
        /// При ръчно връщане на заявлението обратно към попълване от заявителя и последващо повторно генериране на PDF
        /// </summary>
        GoBackToFillApplByApplicant,

        /// <summary>
        /// При повторен опит за минаване към Изчакване на проверка за редовност, когато заявлението вече няма нужда от входиране
        /// </summary>
        NoErrConfirmByEmpFiledIn,

        /// <summary>
        /// При направено плащане в онлайн системата, но преди да е минал callback-ът към нашата система (съответно преди да знаем, че
        /// услугата официално е платена) - обработка на плащането
        /// </summary>
        StartPaymentOperation
    }
}
