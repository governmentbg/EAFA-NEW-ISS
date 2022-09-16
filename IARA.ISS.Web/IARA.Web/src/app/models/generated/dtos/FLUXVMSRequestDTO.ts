

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FLUXVMSRequestDTO { 
    public constructor(obj?: Partial<FLUXVMSRequestDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Boolean)
    public isOutgoing?: boolean;

    @StrictlyTyped(String)
    public webServiceName?: string;

    @StrictlyTyped(String)
    public requestUUID?: string;

    @StrictlyTyped(Date)
    public requestDateTime?: Date;

    @StrictlyTyped(String)
    public responseStatus?: string;

    @StrictlyTyped(String)
    public responseUUID?: string;

    @StrictlyTyped(Date)
    public responseDateTime?: Date;

    @StrictlyTyped(String)
    public errorDescription?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}