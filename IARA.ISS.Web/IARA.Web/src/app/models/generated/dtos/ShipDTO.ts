
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ShipDTO {
    public constructor(obj?: Partial<ShipDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public associationName?: string;

    @StrictlyTyped(String)
    public shipName?: string;

    @StrictlyTyped(String)
    public cfr?: string;

    @StrictlyTyped(String)
    public extMarking?: string;
}