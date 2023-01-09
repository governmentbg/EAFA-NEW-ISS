

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ApplicationBaseDeliveryDTO } from './ApplicationBaseDeliveryDTO';
import { FileInfoDTO } from './FileInfoDTO'; 

export class ApplicationDeliveryDTO extends ApplicationBaseDeliveryDTO {
    public constructor(obj?: Partial<ApplicationDeliveryDTO>) {
        if (obj != undefined) {
            super(obj as ApplicationBaseDeliveryDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Date)
    public deliveryDate?: Date;

    @StrictlyTyped(Date)
    public sentDate?: Date;

    @StrictlyTyped(String)
    public referenceNumber?: string;

    @StrictlyTyped(Boolean)
    public isDelivered?: boolean;

    @StrictlyTyped(Number)
    public registerId?: number;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}