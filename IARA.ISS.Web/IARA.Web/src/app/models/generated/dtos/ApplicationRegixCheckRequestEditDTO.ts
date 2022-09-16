

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ApplicationRegixCheckRequestEditDTO { 
    public constructor(obj?: Partial<ApplicationRegixCheckRequestEditDTO>) {
        Object.assign(this, obj);
    }

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
    public requestContent?: string;

    @StrictlyTyped(String)
    public responseStatus?: string;

    @StrictlyTyped(Date)
    public responseDateTime?: Date;

    @StrictlyTyped(String)
    public responseContent?: string;

    @StrictlyTyped(String)
    public expectedResponseContent?: string;

    @StrictlyTyped(String)
    public errorLevel?: string;

    @StrictlyTyped(String)
    public errorDescription?: string;

    @StrictlyTyped(Number)
    public attempts?: number;
}