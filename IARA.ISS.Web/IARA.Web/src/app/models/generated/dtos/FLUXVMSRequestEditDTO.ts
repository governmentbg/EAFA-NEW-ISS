

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FLUXVMSRequestEditDTO { 
    public constructor(obj?: Partial<FLUXVMSRequestEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Boolean)
    public isOutgoing?: boolean;

    @StrictlyTyped(String)
    public domainName?: string;

    @StrictlyTyped(String)
    public webServiceName?: string;

    @StrictlyTyped(String)
    public requestUUID?: string;

    @StrictlyTyped(Date)
    public requestDateTime?: Date;

    @StrictlyTyped(String)
    public requestContent?: string;

    @StrictlyTyped(String)
    public responseStatus?: string;

    @StrictlyTyped(String)
    public responseUUID?: string;

    @StrictlyTyped(Date)
    public responseDateTime?: Date;

    @StrictlyTyped(String)
    public responseContent?: string;

    @StrictlyTyped(String)
    public errorDescription?: string;
}