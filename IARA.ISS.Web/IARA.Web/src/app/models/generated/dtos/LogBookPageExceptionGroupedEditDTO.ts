

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FileInfoDTO } from './FileInfoDTO';

export class LogBookPageExceptionGroupedEditDTO { 
    public constructor(obj?: Partial<LogBookPageExceptionGroupedEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public logBookPageExceptionIds?: number[];

    @StrictlyTyped(Number)
    public userIds?: number[];

    @StrictlyTyped(Number)
    public logBookTypeIds?: number[];

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(Date)
    public exceptionActiveFrom?: Date;

    @StrictlyTyped(Date)
    public exceptionActiveTo?: Date;

    @StrictlyTyped(Date)
    public editPageFrom?: Date;

    @StrictlyTyped(Date)
    public editPageTo?: Date;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}