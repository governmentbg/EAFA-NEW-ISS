

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { UserAuthDTO } from './UserAuthDTO'; 

export class UserRegistrationDTO extends UserAuthDTO {
    public constructor(obj?: Partial<UserRegistrationDTO>) {
        if (obj != undefined) {
            super(obj as UserAuthDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public email?: string;

    @StrictlyTyped(String)
    public password?: string;
}