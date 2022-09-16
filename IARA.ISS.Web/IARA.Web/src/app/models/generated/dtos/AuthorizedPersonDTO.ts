

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AuthorizedPersonRegixDataDTO } from './AuthorizedPersonRegixDataDTO';
import { RoleDTO } from './RoleDTO'; 

export class AuthorizedPersonDTO extends AuthorizedPersonRegixDataDTO {
    public constructor(obj?: Partial<AuthorizedPersonDTO>) {
        if (obj != undefined) {
            super(obj as AuthorizedPersonRegixDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(RoleDTO)
    public roles?: RoleDTO[];
}