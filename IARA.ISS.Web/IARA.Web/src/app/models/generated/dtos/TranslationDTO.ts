
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class TranslationDTO {
    public constructor(obj?: Partial<TranslationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public code?: string;

    @StrictlyTyped(String)
    public translation?: string;
}