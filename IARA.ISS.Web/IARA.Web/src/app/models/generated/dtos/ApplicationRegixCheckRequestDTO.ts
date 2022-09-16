

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ApplicationRegixCheckRequestDTO { 
    public constructor(obj?: Partial<ApplicationRegixCheckRequestDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public applicationId?: string;

    @StrictlyTyped(String)
    public applicationType?: string;

    @StrictlyTyped(String)
    public webServiceName?: string;

    @StrictlyTyped(String)
    public remoteAddress?: string;

    @StrictlyTyped(Date)
    public requestDateTime?: Date;

    @StrictlyTyped(String)
    public responseStatus?: string;

    @StrictlyTyped(Date)
    public responseDateTime?: Date;

    @StrictlyTyped(String)
    public errorLevel?: string;

    @StrictlyTyped(String)
    public errorDescription?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}