

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FileInfoDTO } from './FileInfoDTO'; 

export class CatchRecordFileDTO extends FileInfoDTO {
    public constructor(obj?: Partial<CatchRecordFileDTO>) {
        if (obj != undefined) {
            super(obj as FileInfoDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public catchRecordId?: number;
}