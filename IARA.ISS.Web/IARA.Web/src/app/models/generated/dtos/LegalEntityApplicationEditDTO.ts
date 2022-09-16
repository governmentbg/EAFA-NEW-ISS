

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { LegalEntityBaseRegixDataDTO } from './LegalEntityBaseRegixDataDTO';
import { AuthorizedPersonDTO } from './AuthorizedPersonDTO';
import { FileInfoDTO } from './FileInfoDTO'; 

export class LegalEntityApplicationEditDTO extends LegalEntityBaseRegixDataDTO {
    public constructor(obj?: Partial<LegalEntityApplicationEditDTO>) {
        if (obj != undefined) {
            super(obj as LegalEntityBaseRegixDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(AuthorizedPersonDTO)
    public authorizedPeople?: AuthorizedPersonDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}