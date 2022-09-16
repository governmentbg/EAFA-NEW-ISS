

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class CommonDocumentDTO { 
    public constructor(obj?: Partial<CommonDocumentDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public num?: string;

    @StrictlyTyped(String)
    public issuer?: string;

    @StrictlyTyped(Date)
    public issueDate?: Date;

    @StrictlyTyped(Boolean)
    public isIndefinite?: boolean;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}