

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { UnregisteredPersonDTO } from './UnregisteredPersonDTO';

export class InspectorDTO extends UnregisteredPersonDTO {
    public constructor(obj?: Partial<InspectorDTO>) {
        if (obj != undefined) {
            super(obj as UnregisteredPersonDTO);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(String)
    public cardNum?: string;

    @StrictlyTyped(String)
    public territoryCode?: string;

    @StrictlyTyped(Number)
    public inspectorId?: number;

    @StrictlyTyped(Number)
    public userId?: number;

    @StrictlyTyped(Number)
    public unregisteredPersonId?: number;

    @StrictlyTyped(Boolean)
    public isNotRegistered?: boolean;

    @StrictlyTyped(Number)
    public institutionId?: number;

    @StrictlyTyped(String)
    public institution?: string;
}