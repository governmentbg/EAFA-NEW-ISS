
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { LocationDTO } from './LocationDTO';
import { CatchRecordFishEditDTO } from './CatchRecordFishEditDTO';

export class CatchRecordEditDTO {
    public constructor(obj?: Partial<CatchRecordEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public ticketId?: number;

    @StrictlyTyped(Date)
    public catchDate?: Date;

    @StrictlyTyped(LocationDTO)
    public location?: LocationDTO;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(CatchRecordFishEditDTO)
    public fishes?: CatchRecordFishEditDTO[];
}