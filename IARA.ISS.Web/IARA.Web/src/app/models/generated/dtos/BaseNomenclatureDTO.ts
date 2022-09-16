
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class BaseNomenclatureDTO<T> {
    public constructor(obj?: Partial<BaseNomenclatureDTO<T>>) {
        Object.assign(this, obj);
    }

    public value?: T;

    @StrictlyTyped(String)
    public displayName?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}