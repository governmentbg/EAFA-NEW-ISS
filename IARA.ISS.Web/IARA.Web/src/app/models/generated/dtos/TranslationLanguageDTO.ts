
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { TranslationGroupDTO } from './TranslationGroupDTO';

export class TranslationLanguageDTO {
    public constructor(obj?: Partial<TranslationLanguageDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public language?: string;

    @StrictlyTyped(TranslationGroupDTO)
    public groups?: TranslationGroupDTO[];
}