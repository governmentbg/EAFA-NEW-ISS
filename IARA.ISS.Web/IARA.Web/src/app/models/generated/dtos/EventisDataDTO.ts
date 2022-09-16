
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class EventisDataDTO {
    public constructor(obj?: Partial<EventisDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public eventisNumber?: string;
}