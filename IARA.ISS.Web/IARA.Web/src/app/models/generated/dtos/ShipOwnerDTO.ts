

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ShipOwnerRegixDataDTO } from './ShipOwnerRegixDataDTO'; 

export class ShipOwnerDTO extends ShipOwnerRegixDataDTO {
    public constructor(obj?: Partial<ShipOwnerDTO>) {
        if (obj != undefined) {
            super(obj as ShipOwnerRegixDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Boolean)
    public isShipHolder?: boolean;

    @StrictlyTyped(Number)
    public ownedShare?: number;
}