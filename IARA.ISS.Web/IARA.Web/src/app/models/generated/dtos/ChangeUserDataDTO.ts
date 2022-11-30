

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO'; 

export class ChangeUserDataDTO extends RegixPersonDataDTO {
    public constructor(obj?: Partial<ChangeUserDataDTO>) {
        if (obj != undefined) {
            super(obj as RegixPersonDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public username?: string;

    @StrictlyTyped(Boolean)
    public userMustChangePassword?: boolean;

    @StrictlyTyped(String)
    public password?: string;

    @StrictlyTyped(AddressRegistrationDTO)
    public userAddresses?: AddressRegistrationDTO[];
}