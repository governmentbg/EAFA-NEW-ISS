

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ApplicationRegiXCheckDTO } from './ApplicationRegiXCheckDTO';
import { FileInfoDTO } from './FileInfoDTO';

export class ApplicationContentDTO { 
    public constructor(obj?: Partial<ApplicationContentDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(String)
    public draftContent?: string;

    @StrictlyTyped(ApplicationRegiXCheckDTO)
    public latestRegiXChecks?: ApplicationRegiXCheckDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}