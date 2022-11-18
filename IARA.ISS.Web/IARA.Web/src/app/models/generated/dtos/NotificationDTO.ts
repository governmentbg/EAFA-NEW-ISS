

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class NotificationDTO { 
    public constructor(obj?: Partial<NotificationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id!: number;

    @StrictlyTyped(String)
    public icon?: string;

    @StrictlyTyped(String)
    public subtitle?: string;

    @StrictlyTyped(String)
    public text!: string;

    @StrictlyTyped(Boolean)
    public isRead!: boolean;

    @StrictlyTyped(String)
    public url?: string;

    @StrictlyTyped(String)
    public title!: string;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(Number)
    public tableId?: number;
}