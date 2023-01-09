

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { CommercialFishingLogbookRegisterDTO } from './CommercialFishingLogbookRegisterDTO';
import { CommercialFishingTypesEnum } from '@app/enums/commercial-fishing-types.enum';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class CommercialFishingPermitLicenseRegisterDTO { 
    public constructor(obj?: Partial<CommercialFishingPermitLicenseRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public registrationNumber?: string;

    @StrictlyTyped(Number)
    public permitId?: number;

    @StrictlyTyped(Number)
    public deliveryId?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(Number)
    public typeCode?: CommercialFishingTypesEnum;

    @StrictlyTyped(String)
    public typeName?: string;

    @StrictlyTyped(String)
    public territoryUnitName?: string;

    @StrictlyTyped(String)
    public submittedForName?: string;

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

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(Boolean)
    public isSubmittedForPerson?: boolean;

    @StrictlyTyped(Boolean)
    public isForOnlineLogBooks?: boolean;

    @StrictlyTyped(CommercialFishingLogbookRegisterDTO)
    public logbooks?: CommercialFishingLogbookRegisterDTO[];
}