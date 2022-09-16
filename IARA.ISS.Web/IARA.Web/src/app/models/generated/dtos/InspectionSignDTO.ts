

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FileInfoDTO } from './FileInfoDTO';

export class InspectionSignDTO { 
    public constructor(obj?: Partial<InspectionSignDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}