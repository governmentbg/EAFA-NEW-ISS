

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { EgnLncDTO } from './EgnLncDTO';

export class QualifiedFisherBasicDataDTO { 
    public constructor(obj?: Partial<QualifiedFisherBasicDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(EgnLncDTO)
    public identifier?: EgnLncDTO;

    @StrictlyTyped(String)
    public firstName?: string;

    @StrictlyTyped(String)
    public middleName?: string;

    @StrictlyTyped(String)
    public lastName?: string;
}