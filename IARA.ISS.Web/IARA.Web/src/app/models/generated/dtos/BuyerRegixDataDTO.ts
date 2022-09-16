

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ApplicationSubmittedByRegixDataDTO } from './ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from './ApplicationSubmittedForRegixDataDTO';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { UsageDocumentRegixDataDTO } from './UsageDocumentRegixDataDTO';
import { ApplicationRegiXCheckDTO } from './ApplicationRegiXCheckDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class BuyerRegixDataDTO { 
    public constructor(obj?: Partial<BuyerRegixDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(ApplicationSubmittedByRegixDataDTO)
    public submittedBy?: ApplicationSubmittedByRegixDataDTO;

    @StrictlyTyped(ApplicationSubmittedForRegixDataDTO)
    public submittedFor?: ApplicationSubmittedForRegixDataDTO;

    @StrictlyTyped(RegixPersonDataDTO)
    public organizer?: RegixPersonDataDTO;

    @StrictlyTyped(RegixPersonDataDTO)
    public agent?: RegixPersonDataDTO;

    @StrictlyTyped(UsageDocumentRegixDataDTO)
    public premiseUsageDocument?: UsageDocumentRegixDataDTO;

    @StrictlyTyped(ApplicationRegiXCheckDTO)
    public applicationRegiXChecks?: ApplicationRegiXCheckDTO[];
}