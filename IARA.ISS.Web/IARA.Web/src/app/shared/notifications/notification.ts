import { BaseNotification } from './base-notification';
import { NotificationTypes } from './notification-types.enum';

export class Notification<T> extends BaseNotification {

    public constructor(obj?: Partial<Notification<T>>) {
        super(NotificationTypes.User, undefined);
        Object.assign(this, obj);
    }

    public get message(): T {
        return this._message as T;
    }

    public set message(value: T) {
        this._message = value;
    }
}