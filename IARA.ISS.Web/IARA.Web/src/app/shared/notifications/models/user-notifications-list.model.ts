import { GridResultModel } from '@app/models/common/grid-result.model';
import { UserNotification } from "./user-notification.model";

export class UserNotificationsList extends GridResultModel<UserNotification> {

    public constructor(obj?: Partial<UserNotificationsList>) {
        super();
        if (obj != undefined) {
            this.records = obj?.records as UserNotification[];
            this.totalRecordsCount = obj?.totalRecordsCount as number;
            this.totalUnread = obj?.totalUnread as number;
        }
    }

    public totalUnread!: number;
}