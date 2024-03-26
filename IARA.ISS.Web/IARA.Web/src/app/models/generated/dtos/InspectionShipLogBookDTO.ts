

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectionLogBookDTO } from './InspectionLogBookDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum'; 

export class InspectionShipLogBookDTO extends InspectionLogBookDTO {
    public constructor(obj?: Partial<InspectionShipLogBookDTO>) {
        if (obj != undefined) {
            super(obj as InspectionLogBookDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Date)
    public issuedOn?: Date;

    @StrictlyTyped(Number)
    public logBookType?: LogBookTypesEnum;
}