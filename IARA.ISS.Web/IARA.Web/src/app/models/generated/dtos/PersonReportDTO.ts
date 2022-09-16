
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ReportHistoryDTO } from './ReportHistoryDTO';

export class PersonReportDTO {
    public constructor(obj?: Partial<PersonReportDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public fullName?: string;

    @StrictlyTyped(String)
    public egn?: string;

    @StrictlyTyped(String)
    public populatedArea?: string;

    @StrictlyTyped(String)
    public street?: string;

    @StrictlyTyped(String)
    public number?: string;

    @StrictlyTyped(ReportHistoryDTO)
    public history?: ReportHistoryDTO[];
}