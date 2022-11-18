

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FileInfoDTO } from './FileInfoDTO';
import { RecreationalFishingMembershipCardDTO } from './RecreationalFishingMembershipCardDTO';
import { RecreationalFishingTicketDuplicateTableDTO } from './RecreationalFishingTicketDuplicateTableDTO';
import { RecreationalFishingTicketBaseRegixDataDTO } from './RecreationalFishingTicketBaseRegixDataDTO'; 

export class RecreationalFishingTicketDTO extends RecreationalFishingTicketBaseRegixDataDTO {
    public constructor(obj?: Partial<RecreationalFishingTicketDTO>) {
        if (obj != undefined) {
            super(obj as RecreationalFishingTicketBaseRegixDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public ticketNum?: string;

    @StrictlyTyped(String)
    public duplicateOfTicketNum?: string;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(Number)
    public periodId?: number;

    @StrictlyTyped(Number)
    public price?: number;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(Date)
    public issuedOn?: Date;

    @StrictlyTyped(FileInfoDTO)
    public personPhoto?: FileInfoDTO;

    @StrictlyTyped(RecreationalFishingMembershipCardDTO)
    public membershipCard?: RecreationalFishingMembershipCardDTO;

    @StrictlyTyped(String)
    public comment?: string;

    @StrictlyTyped(RecreationalFishingTicketDuplicateTableDTO)
    public ticketDuplicates?: RecreationalFishingTicketDuplicateTableDTO[];

    @StrictlyTyped(Boolean)
    public hasUserConfirmed?: boolean;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(FileInfoDTO)
    public declarationFile?: FileInfoDTO;

    @StrictlyTyped(RecreationalFishingTicketBaseRegixDataDTO)
    public regiXDataModel?: RecreationalFishingTicketBaseRegixDataDTO;
}