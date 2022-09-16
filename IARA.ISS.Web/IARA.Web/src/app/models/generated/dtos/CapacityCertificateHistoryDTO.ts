

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { CapacityCertificateHistoryApplDTO } from './CapacityCertificateHistoryApplDTO';
import { CapacityCertificateHistoryTransferredToDTO } from './CapacityCertificateHistoryTransferredToDTO';

export class CapacityCertificateHistoryDTO { 
    public constructor(obj?: Partial<CapacityCertificateHistoryDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public certificateId?: number;

    @StrictlyTyped(CapacityCertificateHistoryApplDTO)
    public createdFromApplication?: CapacityCertificateHistoryApplDTO;

    @StrictlyTyped(CapacityCertificateHistoryApplDTO)
    public usedInApplication?: CapacityCertificateHistoryApplDTO;

    @StrictlyTyped(CapacityCertificateHistoryTransferredToDTO)
    public transferredTo?: CapacityCertificateHistoryTransferredToDTO[];

    @StrictlyTyped(CapacityCertificateHistoryTransferredToDTO)
    public remainderTransferredTo?: CapacityCertificateHistoryTransferredToDTO[];
}