

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class CapacityCertificateHistoryTransferredToDTO { 
    public constructor(obj?: Partial<CapacityCertificateHistoryTransferredToDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public certificateNum?: string;

    @StrictlyTyped(String)
    public holder?: string;

    @StrictlyTyped(Number)
    public tonnage?: number;

    @StrictlyTyped(Number)
    public power?: number;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(Boolean)
    public invalid?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}