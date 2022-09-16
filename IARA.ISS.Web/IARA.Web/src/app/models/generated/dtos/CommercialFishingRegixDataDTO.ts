
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { CommercialFishingBaseRegixDataDTO } from './CommercialFishingBaseRegixDataDTO';
import { ApplicationRegiXCheckDTO } from './ApplicationRegiXCheckDTO';

export class CommercialFishingRegixDataDTO extends CommercialFishingBaseRegixDataDTO {
    public constructor(obj?: Partial<CommercialFishingRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as CommercialFishingBaseRegixDataDTO);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(ApplicationRegiXCheckDTO)
    public applicationRegiXChecks?: ApplicationRegiXCheckDTO[];
}