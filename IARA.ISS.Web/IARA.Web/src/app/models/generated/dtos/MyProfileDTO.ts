

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { RoleDTO } from './RoleDTO';
import { UserLegalDTO } from './UserLegalDTO';
import { UserNewsDistrictSubscriptionDTO } from './UserNewsDistrictSubscriptionDTO';
import { NewsSubscriptionTypes } from '@app/enums/news-subscription-types.enum'; 

export class MyProfileDTO extends RegixPersonDataDTO {
    public constructor(obj?: Partial<MyProfileDTO>) {
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

    @StrictlyTyped(AddressRegistrationDTO)
    public userAddresses?: AddressRegistrationDTO[];

    @StrictlyTyped(FileInfoDTO)
    public photo?: FileInfoDTO;

    @StrictlyTyped(RoleDTO)
    public roles?: RoleDTO[];

    @StrictlyTyped(UserLegalDTO)
    public legals?: UserLegalDTO[];

    @StrictlyTyped(Number)
    public newsSubscription?: NewsSubscriptionTypes;

    @StrictlyTyped(UserNewsDistrictSubscriptionDTO)
    public newsDistrictSubscriptions?: UserNewsDistrictSubscriptionDTO[];
}