import { FLUXVMSRequestDTO } from '@app/models/generated/dtos/FLUXVMSRequestDTO';

export class FluxVmsRequestsDialogParams {
    public request!: FLUXVMSRequestDTO;

    constructor(obj?: Partial<FluxVmsRequestsDialogParams>) {
        Object.assign(this, obj);
    }
}