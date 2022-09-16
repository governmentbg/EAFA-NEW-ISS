

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { EgnLncDTO } from './EgnLncDTO';

export class CustodianOfPropertyDTO { 
    public constructor(obj?: Partial<CustodianOfPropertyDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(EgnLncDTO)
    public egnLnc?: EgnLncDTO;

    @StrictlyTyped(String)
    public firstName?: string;

    @StrictlyTyped(String)
    public middleName?: string;

    @StrictlyTyped(String)
    public lastName?: string;
}