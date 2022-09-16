
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class SystemLogViewDTO {
    public constructor(obj?: Partial<SystemLogViewDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public oldValue?: string;

    @StrictlyTyped(String)
    public newValue?: string;
}