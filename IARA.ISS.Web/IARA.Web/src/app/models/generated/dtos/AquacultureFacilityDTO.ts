

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { LogBookRegisterDTO } from './LogBookRegisterDTO';
import { AquacultureStatusEnum } from '@app/enums/aquaculture-status.enum';

export class AquacultureFacilityDTO { 
    public constructor(obj?: Partial<AquacultureFacilityDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Number)
    public regNum?: number;

    @StrictlyTyped(String)
    public urorNum?: string;

    @StrictlyTyped(Date)
    public registrationDate?: Date;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public owner?: string;

    @StrictlyTyped(String)
    public territoryUnit?: string;

    @StrictlyTyped(Number)
    public status?: AquacultureStatusEnum;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(Number)
    public deliveryId?: number;

    @StrictlyTyped(LogBookRegisterDTO)
    public logBooks?: LogBookRegisterDTO[];

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}