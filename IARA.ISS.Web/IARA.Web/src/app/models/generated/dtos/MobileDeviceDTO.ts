
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class MobileDeviceDTO {
    public constructor(obj?: Partial<MobileDeviceDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public imei?: string;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(String)
    public accessStatus?: string;

    @StrictlyTyped(String)
    public appVersion?: string;

    @StrictlyTyped(Boolean)
    public userMustReloadAppDatabase?: boolean;

    @StrictlyTyped(Date)
    public requestAccessDate?: Date;
}