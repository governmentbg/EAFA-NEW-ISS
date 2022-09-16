
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FishingTicketPersonDTO } from './FishingTicketPersonDTO';
import { FishingTicketFileDTO } from './FishingTicketFileDTO';

export class Under14FishingTicketDTO {
    public constructor(obj?: Partial<Under14FishingTicketDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(FishingTicketPersonDTO)
    public person?: FishingTicketPersonDTO;

    @StrictlyTyped(FishingTicketFileDTO)
    public photo?: FishingTicketFileDTO;

    @StrictlyTyped(FishingTicketFileDTO)
    public birthCertificate?: FishingTicketFileDTO;
}