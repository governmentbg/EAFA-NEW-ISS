

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RecreationalFishingTicketBaseRegixDataDTO } from './RecreationalFishingTicketBaseRegixDataDTO';
import { RecreationalFishingMembershipCardDTO } from './RecreationalFishingMembershipCardDTO';
import { RecreationalFishingTicketDuplicateTableDTO } from './RecreationalFishingTicketDuplicateTableDTO';
import { FileInfoDTO } from './FileInfoDTO';

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
  
    @StrictlyTyped(RecreationalFishingMembershipCardDTO)
    public membershipCard?: RecreationalFishingMembershipCardDTO;

    @StrictlyTyped(String)
    public comment?: string;

    @StrictlyTyped(RecreationalFishingTicketDuplicateTableDTO)
    public ticketDuplicates?: RecreationalFishingTicketDuplicateTableDTO[];

    @StrictlyTyped(Boolean)
    public hasUserConfirmed?: boolean;

    @StrictlyTyped(FileInfoDTO)
    public declarationFile?: FileInfoDTO;

    @StrictlyTyped(RecreationalFishingTicketBaseRegixDataDTO)
    public regiXDataModel?: RecreationalFishingTicketBaseRegixDataDTO;
}