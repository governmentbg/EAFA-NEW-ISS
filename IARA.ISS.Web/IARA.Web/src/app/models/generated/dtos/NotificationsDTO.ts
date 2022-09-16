

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { GridResultModel } from '../../common/grid-result.model';
import { NotificationDTO } from './NotificationDTO';

export class NotificationsDTO extends GridResultModel<NotificationDTO> {
    public constructor(obj?: Partial<NotificationsDTO>) {
        if (obj != undefined) {
            super();
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(Number)
    public totalUnread!: number;
}