

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class NewsManagementDTO { 
    public constructor(obj?: Partial<NewsManagementDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public title?: string;

    @StrictlyTyped(Date)
    public publishStart?: Date;

    @StrictlyTyped(Date)
    public publishEnd?: Date;

    @StrictlyTyped(String)
    public createdBy?: string;

    @StrictlyTyped(Boolean)
    public isPublished?: boolean;

    @StrictlyTyped(Boolean)
    public hasNotificationsSent?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}