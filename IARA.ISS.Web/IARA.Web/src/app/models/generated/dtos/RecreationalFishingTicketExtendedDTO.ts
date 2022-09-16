

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RecreationalFishingTicketDTO } from './RecreationalFishingTicketDTO'; 

export class RecreationalFishingTicketExtendedDTO extends RecreationalFishingTicketDTO {
    public constructor(obj?: Partial<RecreationalFishingTicketExtendedDTO>) {
        if (obj != undefined) {
            super(obj as RecreationalFishingTicketDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Boolean)
    public updateProfileData?: boolean;
}