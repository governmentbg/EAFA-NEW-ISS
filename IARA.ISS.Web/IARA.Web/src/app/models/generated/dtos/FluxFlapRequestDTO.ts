

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FluxFlapRequestDTO { 
    public constructor(obj?: Partial<FluxFlapRequestDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Boolean)
    public isOutgoing?: boolean;

    @StrictlyTyped(String)
    public ship?: string;

    @StrictlyTyped(String)
    public requestUuid?: string;

    @StrictlyTyped(Date)
    public requestDate?: Date;

    @StrictlyTyped(String)
    public responseUuid?: string;

    @StrictlyTyped(Date)
    public responseDate?: Date;
}