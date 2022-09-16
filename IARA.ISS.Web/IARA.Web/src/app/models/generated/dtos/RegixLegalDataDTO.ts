

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { CustodianOfPropertyDTO } from './CustodianOfPropertyDTO';

export class RegixLegalDataDTO { 
    public constructor(obj?: Partial<RegixLegalDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public eik?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public phone?: string;

    @StrictlyTyped(String)
    public email?: string;

    @StrictlyTyped(Boolean)
    public isCustodianOfPropertySameAsApplicant?: boolean;

    @StrictlyTyped(CustodianOfPropertyDTO)
    public custodianOfProperty?: CustodianOfPropertyDTO;
}