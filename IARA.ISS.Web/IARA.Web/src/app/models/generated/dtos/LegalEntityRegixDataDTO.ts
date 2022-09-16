
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { LegalEntityBaseRegixDataDTO } from './LegalEntityBaseRegixDataDTO';
import { AuthorizedPersonRegixDataDTO } from './AuthorizedPersonRegixDataDTO';
import { ApplicationRegiXCheckDTO } from './ApplicationRegiXCheckDTO';

export class LegalEntityRegixDataDTO extends LegalEntityBaseRegixDataDTO {
    public constructor(obj?: Partial<LegalEntityRegixDataDTO>) {
        if (obj != undefined) {
            super(obj as LegalEntityBaseRegixDataDTO);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(AuthorizedPersonRegixDataDTO)
    public authorizedPeople?: AuthorizedPersonRegixDataDTO[];

    @StrictlyTyped(ApplicationRegiXCheckDTO)
    public applicationRegiXChecks?: ApplicationRegiXCheckDTO[];
}