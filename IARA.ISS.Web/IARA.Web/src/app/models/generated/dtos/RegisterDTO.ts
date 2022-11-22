

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PrintConfigurationParameters } from '@app/components/common-app/applications/models/print-configuration-parameters.model';


export class RegisterDTO<T> { 
    public constructor(obj?: Partial<RegisterDTO<T>>) {
        Object.assign(this, obj);
    }

    public dto?: T;

    @StrictlyTyped(PrintConfigurationParameters)
    public printConfiguration?: PrintConfigurationParameters;
}