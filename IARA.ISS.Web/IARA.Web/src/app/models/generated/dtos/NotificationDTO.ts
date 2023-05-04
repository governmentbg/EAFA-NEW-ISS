

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { UserNotification } from '@app/shared/notifications/models/user-notification.model';

export class NotificationDTO extends UserNotification {
    public constructor(obj?: Partial<NotificationDTO>) {
        if (obj != undefined) {
            super(obj as UserNotification);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(Number)
    public tableId?: number;
}