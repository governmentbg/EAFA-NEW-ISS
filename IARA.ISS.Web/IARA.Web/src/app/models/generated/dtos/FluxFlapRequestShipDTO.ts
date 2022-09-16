

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FluxFlapRequestShipDTO { 
    public constructor(obj?: Partial<FluxFlapRequestShipDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(String)
    public shipIdentifierType?: string;

    @StrictlyTyped(String)
    public shipIdentifier?: string;

    @StrictlyTyped(String)
    public shipName?: string;
}