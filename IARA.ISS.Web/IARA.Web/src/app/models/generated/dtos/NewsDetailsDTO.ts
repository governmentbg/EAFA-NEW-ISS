

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FileInfoDTO } from './FileInfoDTO';

export class NewsDetailsDTO { 
    public constructor(obj?: Partial<NewsDetailsDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public title?: string;

    @StrictlyTyped(String)
    public content?: string;

    @StrictlyTyped(String)
    public image?: string;

    @StrictlyTyped(Date)
    public publishStart?: Date;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}