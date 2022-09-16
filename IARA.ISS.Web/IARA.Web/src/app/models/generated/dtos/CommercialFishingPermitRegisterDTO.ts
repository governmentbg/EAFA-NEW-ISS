

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { CommercialFishingPermitLicenseRegisterDTO } from './CommercialFishingPermitLicenseRegisterDTO';
import { CommercialFishingTypesEnum } from '@app/enums/commercial-fishing-types.enum';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class CommercialFishingPermitRegisterDTO { 
    public constructor(obj?: Partial<CommercialFishingPermitRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public registrationNumber?: string;

    @StrictlyTyped(Number)
    public deliveryId?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(String)
    public typeName?: string;

    @StrictlyTyped(Number)
    public typeCode?: CommercialFishingTypesEnum;

    @StrictlyTyped(String)
    public territoryUnitName?: string;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(String)
    public submittedForName?: string;

    @StrictlyTyped(String)
    public shipName?: string;

    @StrictlyTyped(String)
    public qualifiedFisherName?: string;

    @StrictlyTyped(Date)
    public issueDate?: Date;

    @StrictlyTyped(Boolean)
    public isSuspended?: boolean;

    @StrictlyTyped(String)
    public suspensionsInformation?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(CommercialFishingPermitLicenseRegisterDTO)
    public licenses?: CommercialFishingPermitLicenseRegisterDTO[];
}