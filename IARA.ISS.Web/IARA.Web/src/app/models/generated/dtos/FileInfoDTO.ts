

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FileInfoDTO { 
    public constructor(obj?: Partial<FileInfoDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(File)
    public file?: File;

    @StrictlyTyped(Number)
    public fileTypeId?: number;

    @StrictlyTyped(Number)
    public size?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public contentType?: string;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(Date)
    public uploadedOn?: Date;

    @StrictlyTyped(Boolean)
    public deleted?: boolean;
}