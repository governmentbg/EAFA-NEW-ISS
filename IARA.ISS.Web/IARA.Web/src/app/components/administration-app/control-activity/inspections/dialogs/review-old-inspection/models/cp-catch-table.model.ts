import { InspectedCPCatchDTO } from '@app/models/generated/dtos/InspectedCPCatchDTO';

export class CPCatchTableModel extends InspectedCPCatchDTO {
    public fishName: string | undefined;

    public constructor(params?: Partial<CPCatchTableModel>) {
        super(params);
        Object.assign(this, params);
    }
}