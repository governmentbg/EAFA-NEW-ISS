

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class SystemPropertiesDTO { 
    public constructor(obj?: Partial<SystemPropertiesDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public elderTicketFemaleAge?: number;

    @StrictlyTyped(Number)
    public elderTicketMaleAge?: number;

    @StrictlyTyped(Number)
    public maxNumberOfUnder14Tickets?: number;

    @StrictlyTyped(Date)
    public blockAssociationsAddTickets?: Date;

    @StrictlyTyped(Number)
    public addLogBookPagesDaysTolerance?: number;

    @StrictlyTyped(Number)
    public maxNumberFishingGears?: number;

    @StrictlyTyped(String)
    public acdrUserID?: string;

    @StrictlyTyped(String)
    public acdrUserName?: string;

    @StrictlyTyped(String)
    public fishingAssociationRoleCode?: string;

    @StrictlyTyped(String)
    public fishingGearMarkPrefix?: string;

    @StrictlyTyped(Date)
    public fishingGearMarkPrefixApplyDate?: Date;

    @StrictlyTyped(Number)
    public lockShipOver12MLogBookAfterHours?: number;

    @StrictlyTyped(Number)
    public lockShip10M12MLogBookAfterHours?: number;

    @StrictlyTyped(Number)
    public lockShipUnder10MLogBookAfterDays?: number;

    @StrictlyTyped(Number)
    public lockFirstSaleAbove200KLogBookAfterHours?: number;

    @StrictlyTyped(Number)
    public lockFirstSaleBelow200KLogBookAfterHours?: number;

    @StrictlyTyped(Number)
    public lockAdmissionLogBookAfterHours?: number;

    @StrictlyTyped(Number)
    public lockAquacultureLogBookAfterDays?: number;

    @StrictlyTyped(Number)
    public maxNumberOfLogBookPages?: number;

    @StrictlyTyped(Number)
    public addInspectionDaysTollerance?: number;

    @StrictlyTyped(Number)
    public addShipPagesDaysTolerance?: number;

    @StrictlyTyped(Number)
    public addAquaculturePagesDaysTolerance?: number;

    @StrictlyTyped(Number)
    public addAdmissionPagesDaysTolerance?: number;

    @StrictlyTyped(Number)
    public addFirstSalePagesDaysTolerance?: number;

    @StrictlyTyped(Number)
    public addTransportationPagesDaysTolerance?: number;

    @StrictlyTyped(Number)
    public nextScientificPermitOwnersRegistrationNum?: number;
}