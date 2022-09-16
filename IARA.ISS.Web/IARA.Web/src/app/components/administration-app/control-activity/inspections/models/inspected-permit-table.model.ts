import { InspectionPermitDTO } from '@app/models/generated/dtos/InspectionPermitDTO';
import { InspectionCheckDTO } from '@app/models/generated/dtos/InspectionCheckDTO';

export class InspectedPermitTableModel extends InspectionPermitDTO {
    public isRegistered: boolean = true;
    public description: string | undefined;
    public checkDTO: InspectionCheckDTO | undefined;

    public constructor(params?: Partial<InspectedPermitTableModel>) {
        super(params);
        Object.assign(this, params);
    }
}