

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class NewsDTO { 
    public constructor(obj?: Partial<NewsDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public title?: string;

    @StrictlyTyped(String)
    public summary?: string;

    @StrictlyTyped(String)
    public mainImage?: string;

    @StrictlyTyped(Date)
    public createdOn?: Date;

    @StrictlyTyped(Date)
    public publishStart?: Date;

    @StrictlyTyped(Date)
    public publishEnd?: Date;

    @StrictlyTyped(Boolean)
    public hasNotificationsSent?: boolean;
}