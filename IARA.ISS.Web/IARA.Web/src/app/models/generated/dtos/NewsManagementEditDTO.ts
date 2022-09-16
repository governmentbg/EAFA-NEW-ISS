

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FileInfoDTO } from './FileInfoDTO';

export class NewsManagementEditDTO { 
    public constructor(obj?: Partial<NewsManagementEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public title?: string;

    @StrictlyTyped(String)
    public content?: string;

    @StrictlyTyped(String)
    public summary?: string;

    @StrictlyTyped(Date)
    public publishStart?: Date;

    @StrictlyTyped(Date)
    public publishEnd?: Date;

    @StrictlyTyped(Boolean)
    public hasNotificationsSent?: boolean;

    @StrictlyTyped(Boolean)
    public isDistrictLimited?: boolean;

    @StrictlyTyped(Number)
    public districtsIds?: number[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(FileInfoDTO)
    public mainImage?: FileInfoDTO;
}