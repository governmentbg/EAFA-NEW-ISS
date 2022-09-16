
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ScientificFishingPermitBaseRegixDataDTO } from './ScientificFishingPermitBaseRegixDataDTO';
import { ScientificFishingPermitHolderRegixDataDTO } from './ScientificFishingPermitHolderRegixDataDTO';
import { ApplicationRegiXCheckDTO } from './ApplicationRegiXCheckDTO';

export class ScientificFishingPermitRegixDataDTO extends ScientificFishingPermitBaseRegixDataDTO {
    public constructor(obj?: Partial<ScientificFishingPermitRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as ScientificFishingPermitBaseRegixDataDTO);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(ScientificFishingPermitHolderRegixDataDTO)
    public holders?: ScientificFishingPermitHolderRegixDataDTO[];

    @StrictlyTyped(ApplicationRegiXCheckDTO)
    public applicationRegiXChecks?: ApplicationRegiXCheckDTO[];
}