

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { CapacityCertificateHistoryDTO } from './CapacityCertificateHistoryDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class FishingCapacityCertificateDTO { 
    public constructor(obj?: Partial<FishingCapacityCertificateDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Number)
    public certificateNum?: number;

    @StrictlyTyped(Date)
    public certificateValidFrom?: Date;

    @StrictlyTyped(Date)
    public certificateValidTo?: Date;

    @StrictlyTyped(String)
    public holderNames?: string;

    @StrictlyTyped(Number)
    public grossTonnage?: number;

    @StrictlyTyped(Number)
    public power?: number;

    @StrictlyTyped(Boolean)
    public invalid?: boolean;

    @StrictlyTyped(Number)
    public deliveryId?: number;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(CapacityCertificateHistoryDTO)
    public history?: CapacityCertificateHistoryDTO;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}