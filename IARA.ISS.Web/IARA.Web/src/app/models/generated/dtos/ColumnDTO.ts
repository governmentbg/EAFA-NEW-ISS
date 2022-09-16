

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ColumnDTO {
    public constructor(obj?: Partial<ColumnDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public displayName!: string;

    @StrictlyTyped(String)
    public propertyName!: string;

    @StrictlyTyped(Boolean)
    public isForeignKey!: boolean;

    @StrictlyTyped(String)
    public dataType!: string;

    @StrictlyTyped(Boolean)
    public isRequired!: boolean;

    @StrictlyTyped(Number)
    public maxLength?: number;
}