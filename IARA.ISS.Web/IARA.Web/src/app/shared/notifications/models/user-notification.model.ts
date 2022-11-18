import { StrictlyTyped } from "../../decorators/strictly-typed.decorator";

export class UserNotification {

    public constructor(obj?: Partial<UserNotification>) {
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

    @StrictlyTyped(Date)
    public recievedDate?: Date;
}