

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';

export class QualifiedFisherDuplicateDataDTO { 
    public constructor(obj?: Partial<QualifiedFisherDuplicateDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Boolean)
    public isOnline?: boolean;

    @StrictlyTyped(Number)
    public qualifiedFisherId?: number;

    @StrictlyTyped(RegixPersonDataDTO)
    public qualifiedFisher?: RegixPersonDataDTO;
}