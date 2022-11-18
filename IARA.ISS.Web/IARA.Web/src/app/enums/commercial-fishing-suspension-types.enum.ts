export enum CommercialFishingSuspensionTypesEnum {
    /**
     * Временно прекратяване на разрешително
     * */
    TemporaryPermit,

    /*
     * Временно отнемане на разрешително
     * */
    TemporaryWithdrawalPermit,

    /**
     * Окончателно отнемане на разрешително
     * */
    PermenentWithdrawalPermit,

    /**
     * Временно прекратяване на удостоверение
     * */
    TemporaryLicense,

    /**
     * Окончателно прекратяване на удостоверение
     * */
    PermanentLicense,

    /**
     * Окончателно по заявка на собственика - за разрешително
     * */
    OwnerRequest,

    /**
     * Прекратено удостоверение, поради прекратено разрешително
     * */
    PermitSuspendedLicense
}