
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class NomenclatureDTO<T> {
    public constructor(obj?: Partial<NomenclatureDTO<T>>) {
        Object.assign(this, obj);
    }

    public value?: T;

    @StrictlyTyped(String)
    public code?: string;

    @StrictlyTyped(String)
    public displayName?: string;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
} 