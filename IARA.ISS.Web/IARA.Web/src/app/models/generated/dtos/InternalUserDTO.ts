

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { UserEditDTO } from './UserEditDTO';
import { MobileDeviceDTO } from './MobileDeviceDTO'; 

export class InternalUserDTO extends UserEditDTO {
    public constructor(obj?: Partial<InternalUserDTO>) {
        if (obj != undefined) {
            super(obj as UserEditDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(MobileDeviceDTO)
    public mobileDevices?: MobileDeviceDTO[];
}