

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PaymentTariffDTO { 
    public constructor(obj?: Partial<PaymentTariffDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public tariffId?: number;

    @StrictlyTyped(String)
    public tariffName?: string;

    @StrictlyTyped(String)
    public tariffCode?: string;

    @StrictlyTyped(String)
    public tariffDescription?: string;

    @StrictlyTyped(String)
    public tariffBasedOnPlea?: string;

    @StrictlyTyped(Number)
    public quantity?: number;

    @StrictlyTyped(Number)
    public unitPrice?: number;

    @StrictlyTyped(Number)
    public price?: number;

    @StrictlyTyped(Boolean)
    public isCalculated?: boolean;

    @StrictlyTyped(Boolean)
    public isChecked?: boolean;
}