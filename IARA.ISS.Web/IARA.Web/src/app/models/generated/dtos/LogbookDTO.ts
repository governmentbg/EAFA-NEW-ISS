
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class LogbookDTO {
    public constructor(obj?: Partial<LogbookDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public buyerId?: number;

    @StrictlyTyped(String)
    public logbookNumber?: string;

    @StrictlyTyped(Date)
    public issueDate?: Date;

    @StrictlyTyped(Date)
    public finishDate?: Date;

    @StrictlyTyped(Number)
    public startPageNum?: number;

    @StrictlyTyped(Number)
    public endPageNum?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(Number)
    public price?: number;

    @StrictlyTyped(String)
    public comment?: string;
}