
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ApplicationRegiXCheckDTO {
    public constructor(obj?: Partial<ApplicationRegiXCheckDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Number)
    public applicationChangeHistoryId?: number;

    @StrictlyTyped(String)
    public webServiceName?: string;

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

    @StrictlyTyped(Number)
    public attempts?: number;
}