
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ReasonDTO {
    public constructor(obj?: Partial<ReasonDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public reason?: string;
}