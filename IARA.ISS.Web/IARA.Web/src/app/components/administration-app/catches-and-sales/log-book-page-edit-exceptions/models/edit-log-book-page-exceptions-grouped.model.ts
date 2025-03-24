import { LogBookPageExceptionGroupedDTO } from '@app/models/generated/dtos/LogBookPageExceptionGroupedDTO';

export class EditLogBookPageExceptionsGroupedParameters  {
    public isCopy: boolean = false;
    public viewMode: boolean = false;
    public model: LogBookPageExceptionGroupedDTO | undefined;

    public constructor(obj?: Partial<EditLogBookPageExceptionsGroupedParameters>) {
        Object.assign(this, obj);
    }
}