import { NotificationTypes } from './notification-types.enum';

export abstract class BaseNotification {

    protected constructor(type: NotificationTypes, message: any) {
        this.type = type;
        this._message = message;
    }

    public type: NotificationTypes;
    protected _message: any;
}