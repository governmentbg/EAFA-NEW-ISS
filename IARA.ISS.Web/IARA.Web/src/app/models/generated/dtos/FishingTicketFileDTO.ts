
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FishingTicketFileDTO {
    public constructor(obj?: Partial<FishingTicketFileDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(File)
    public file?: File;

    @StrictlyTyped(Date)
    public uploadedOn?: Date;
}