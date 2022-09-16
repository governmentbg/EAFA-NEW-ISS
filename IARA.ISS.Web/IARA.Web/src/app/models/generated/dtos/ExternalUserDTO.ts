

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { UserEditDTO } from './UserEditDTO';
import { UserLegalDTO } from './UserLegalDTO'; 

export class ExternalUserDTO extends UserEditDTO {
    public constructor(obj?: Partial<ExternalUserDTO>) {
        if (obj != undefined) {
            super(obj as UserEditDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(UserLegalDTO)
    public userLegals?: UserLegalDTO[];
}