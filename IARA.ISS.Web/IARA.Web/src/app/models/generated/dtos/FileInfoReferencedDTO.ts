
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FileInfoDTO } from './FileInfoDTO';

export class FileInfoReferencedDTO extends FileInfoDTO {
    public constructor(obj?: Partial<FileInfoReferencedDTO>) {
        if (obj != undefined) {
            super(obj as FileInfoDTO);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(Number)
    public referenceId?: number;
}