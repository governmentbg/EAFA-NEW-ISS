
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { TranslationGroupDTO } from './TranslationGroupDTO';

export class TranslationGroupManagementDTO extends TranslationGroupDTO {
    public constructor(obj?: Partial<TranslationGroupManagementDTO>) {
        if (obj != undefined) {
            super(obj as TranslationGroupDTO);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public languageCode?: string;
}