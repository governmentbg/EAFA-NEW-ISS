

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FileInfoDTO } from './FileInfoDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';

export class LogBookPageFilesDTO { 
    public constructor(obj?: Partial<LogBookPageFilesDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public logBookPageId?: number;

    @StrictlyTyped(Number)
    public logBookType?: LogBookTypesEnum;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}