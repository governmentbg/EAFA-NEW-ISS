export enum CancellationReasonsEnum {
    /** По желание на собственика */
    OwnerRequested,
    /** С решение на Изпълнителния директор на ИАРА */
    IARAHeadDecision,
    /** С решение на Министъра на земеделието и храните */
    MZHHeadDecision,
    /** Служебно отписан */
    ManuallyDeregistered,
    /** Унищожаване */
    ShipDestroy,
    /** Корабокрушение */
    ShipWreck,
    /** Не е дефинирано */
    Unknown,
    /** Друго */
    Other
}