
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class DownloadableFileDTO {
    public constructor(obj?: Partial<DownloadableFileDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public bytes?: number[];

    @StrictlyTyped(String)
    public mimeType?: string;

    @StrictlyTyped(String)
    public fileName?: string;
}