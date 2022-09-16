

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RecreationalFishingTicketsDTO } from './RecreationalFishingTicketsDTO'; 

export class RecreationalTicketsExtendedDTO extends RecreationalFishingTicketsDTO {
    public constructor(obj?: Partial<RecreationalTicketsExtendedDTO>) {
        if (obj != undefined) {
            super(obj as RecreationalFishingTicketsDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Boolean)
    public updateProfileData?: boolean;
}