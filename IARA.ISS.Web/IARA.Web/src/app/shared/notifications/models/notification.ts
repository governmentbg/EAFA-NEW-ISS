import { NotificationTypes } from '../notification-types.enum';
import { BaseNotification } from './base-notification';

export class WebNotification<T> extends BaseNotification {

    public constructor(obj?: Partial<WebNotification<T>>) {
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