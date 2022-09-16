

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ApplicationRegiXCheckDTO } from './ApplicationRegiXCheckDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class BaseRegixChecksDTO { 
    public constructor(obj?: Partial<BaseRegixChecksDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(String)
    public statusReason?: string;

    @StrictlyTyped(ApplicationRegiXCheckDTO)
    public applicationRegiXChecks?: ApplicationRegiXCheckDTO[];
}