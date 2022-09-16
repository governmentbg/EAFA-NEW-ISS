
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class RegixChecksWrapperDTO<T> {
    public constructor(obj?: Partial<RegixChecksWrapperDTO<T>>) {
        Object.assign(this, obj);
    }

    public dialogDataModel?: T;

    public regiXDataModel?: T;
}