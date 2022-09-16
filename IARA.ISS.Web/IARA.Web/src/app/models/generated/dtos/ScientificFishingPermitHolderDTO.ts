

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ScientificFishingPermitHolderRegixDataDTO } from './ScientificFishingPermitHolderRegixDataDTO';
import { FileInfoDTO } from './FileInfoDTO'; 

export class ScientificFishingPermitHolderDTO extends ScientificFishingPermitHolderRegixDataDTO {
    public constructor(obj?: Partial<ScientificFishingPermitHolderDTO>) {
        if (obj != undefined) {
            super(obj as ScientificFishingPermitHolderRegixDataDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public requestNumber?: number;

    @StrictlyTyped(Number)
    public permitNumber?: number;

    @StrictlyTyped(String)
    public scientificPosition?: string;

    @StrictlyTyped(FileInfoDTO)
    public photo?: FileInfoDTO;

    @StrictlyTyped(String)
    public photoBase64?: string;
}