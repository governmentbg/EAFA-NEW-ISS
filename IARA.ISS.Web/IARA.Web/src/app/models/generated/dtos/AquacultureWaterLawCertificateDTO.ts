

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class AquacultureWaterLawCertificateDTO { 
    public constructor(obj?: Partial<AquacultureWaterLawCertificateDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public certificateTypeId?: number;

    @StrictlyTyped(String)
    public certificateNum?: string;

    @StrictlyTyped(String)
    public certificateIssuer?: string;

    @StrictlyTyped(Boolean)
    public isCertificateIndefinite?: boolean;

    @StrictlyTyped(Date)
    public certificateValidFrom?: Date;

    @StrictlyTyped(Date)
    public certificateValidTo?: Date;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}