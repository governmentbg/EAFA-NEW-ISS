

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PossibleLogBooksForPageDTO { 
    public constructor(obj?: Partial<PossibleLogBooksForPageDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(String)
    public logBookNumber?: string;

    @StrictlyTyped(Number)
    public logBookPermitLicenseId?: number;

    @StrictlyTyped(String)
    public buyerNumber?: string;

    @StrictlyTyped(String)
    public buyerUrorr?: string;

    @StrictlyTyped(String)
    public buyerName?: string;

    @StrictlyTyped(String)
    public personName?: string;

    @StrictlyTyped(String)
    public legalName?: string;

    @StrictlyTyped(String)
    public permitLicenseNumber?: string;

    @StrictlyTyped(String)
    public qualifiedFisherName?: string;

    @StrictlyTyped(String)
    public qualifiedFisherNumber?: string;
}