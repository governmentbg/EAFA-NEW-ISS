import { InspectionCheckDTO } from '@app/models/generated/dtos/InspectionCheckDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspectionLogBookDTO } from '@app/models/generated/dtos/InspectionLogBookDTO';

export class InspectedLogBookTableModel extends InspectionLogBookDTO {
    public isRegistered: boolean = true;
    public description: string | undefined;
    public checkDTO: InspectionCheckDTO | undefined;
    public page: NomenclatureDTO<number> | undefined;
    public pageNum: string | undefined;

    public constructor(params?: Partial<InspectedLogBookTableModel>) {
        super(params);
        Object.assign(this, params);
    }
}