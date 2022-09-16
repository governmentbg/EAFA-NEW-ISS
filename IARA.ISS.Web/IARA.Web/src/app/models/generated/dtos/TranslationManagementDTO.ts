

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class TranslationManagementDTO { 
    public constructor(obj?: Partial<TranslationManagementDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public code?: string;

    @StrictlyTyped(String)
    public groupCode?: string;

    @StrictlyTyped(String)
    public groupType?: string;

    @StrictlyTyped(String)
    public valueBg?: string;

    @StrictlyTyped(String)
    public valueEn?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}