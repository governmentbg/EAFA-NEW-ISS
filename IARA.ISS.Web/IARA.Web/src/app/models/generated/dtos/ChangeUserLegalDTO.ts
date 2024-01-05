

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { UserLegalDTO } from './UserLegalDTO';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO'; 

export class ChangeUserLegalDTO extends UserLegalDTO {
    public constructor(obj?: Partial<ChangeUserLegalDTO>) {
        if (obj != undefined) {
            super(obj as UserLegalDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(RegixLegalDataDTO)
    public legal?: RegixLegalDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public addresses?: AddressRegistrationDTO[];

    @StrictlyTyped(Number)
    public territoryUnitId?: number;

    @StrictlyTyped(Number)
    public associationId?: number;

    @StrictlyTyped(Boolean)
    public hasMissingProperties?: boolean;
}