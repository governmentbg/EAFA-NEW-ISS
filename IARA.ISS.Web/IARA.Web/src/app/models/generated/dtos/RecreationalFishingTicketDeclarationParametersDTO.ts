

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { TicketTypeEnum } from '@app/enums/ticket-type.enum';
import { TicketPeriodEnum } from '@app/enums/ticket-period.enum';

export class RecreationalFishingTicketDeclarationParametersDTO { 
    public constructor(obj?: Partial<RecreationalFishingTicketDeclarationParametersDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public ticketNum?: string;

    @StrictlyTyped(Number)
    public type?: TicketTypeEnum;

    @StrictlyTyped(Number)
    public period?: TicketPeriodEnum;

    @StrictlyTyped(RegixPersonDataDTO)
    public person?: RegixPersonDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public personAddressRegistrations?: AddressRegistrationDTO[];

    @StrictlyTyped(RegixPersonDataDTO)
    public representativePerson?: RegixPersonDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public representativePersonAddressRegistrations?: AddressRegistrationDTO[];

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Number)
    public associationId?: number;

    @StrictlyTyped(String)
    public territoryUnit?: string;

    @StrictlyTyped(String)
    public address?: string;

    @StrictlyTyped(String)
    public code?: string;
}