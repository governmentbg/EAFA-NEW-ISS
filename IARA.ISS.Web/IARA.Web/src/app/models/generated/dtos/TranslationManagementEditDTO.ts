

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class TranslationManagementEditDTO { 
    public constructor(obj?: Partial<TranslationManagementEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public code?: string;

    @StrictlyTyped(String)
    public groupCode?: string;

    @StrictlyTyped(String)
    public valueBg?: string;

    @StrictlyTyped(String)
    public valueEn?: string;
}