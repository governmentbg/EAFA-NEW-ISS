
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class StatusCountReportDTO {
    public constructor(obj?: Partial<StatusCountReportDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(Number)
    public data?: number[];

    @StrictlyTyped(String)
    public color?: string;
}