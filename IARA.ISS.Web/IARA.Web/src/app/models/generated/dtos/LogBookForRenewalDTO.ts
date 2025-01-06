

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class LogBookForRenewalDTO { 
    public constructor(obj?: Partial<LogBookForRenewalDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public logBookPermitLicenseId?: number;

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(String)
    public number?: string;

    @StrictlyTyped(Boolean)
    public isOnline?: boolean;

    @StrictlyTyped(Number)
    public startPageNumber?: number;

    @StrictlyTyped(Number)
    public endPageNumber?: number;

    @StrictlyTyped(Number)
    public lastUsedPageNumber?: number;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(String)
    public logBookTypeName?: string;

    @StrictlyTyped(String)
    public lastPermitLicenseNumber?: string;

    @StrictlyTyped(Date)
    public issueDate?: Date;

    @StrictlyTyped(Boolean)
    public morePagesThanAllowed?: boolean;

    @StrictlyTyped(Boolean)
    public isChecked?: boolean;
}