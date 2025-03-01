﻿

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class InspectedDeclarationCatchDTO { 
    public constructor(obj?: Partial<InspectedDeclarationCatchDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public inspectionLogBookPageId?: number;

    @StrictlyTyped(Number)
    public catchTypeId?: number;

    @StrictlyTyped(Number)
    public fishTypeId?: number;

    @StrictlyTyped(Number)
    public catchCount?: number;

    @StrictlyTyped(Number)
    public catchQuantity?: number;

    @StrictlyTyped(Number)
    public unloadedQuantity?: number;

    @StrictlyTyped(Number)
    public presentationId?: number;

    @StrictlyTyped(Boolean)
    public undersized?: boolean;

    @StrictlyTyped(Number)
    public catchZoneId?: number;

    @StrictlyTyped(Number)
    public turbotSizeGroupId?: number;

    @StrictlyTyped(String)
    public fishName?: string;
}