
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { TranslationDTO } from './TranslationDTO';

export class TranslationGroupDTO {
    public constructor(obj?: Partial<TranslationGroupDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public code?: string;

    @StrictlyTyped(TranslationDTO)
    public translations?: TranslationDTO[];
}