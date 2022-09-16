

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ApplicationSubmittedForDTO } from './ApplicationSubmittedForDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class DuplicatesRegisterEditDTO { 
    public constructor(obj?: Partial<DuplicatesRegisterEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(ApplicationSubmittedForDTO)
    public submittedFor?: ApplicationSubmittedForDTO;

    @StrictlyTyped(String)
    public reason?: string;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(Number)
    public buyerId?: number;

    @StrictlyTyped(Number)
    public permitId?: number;

    @StrictlyTyped(Number)
    public permitLicenceId?: number;

    @StrictlyTyped(Number)
    public qualifiedFisherId?: number;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}