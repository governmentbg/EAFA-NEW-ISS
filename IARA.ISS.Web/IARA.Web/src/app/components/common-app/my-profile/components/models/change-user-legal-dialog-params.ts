import { ChangeUserLegalDTO } from '@app/models/generated/dtos/ChangeUserLegalDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

export class ChangeUserLegalDialogParams {
    public model!: ChangeUserLegalDTO;
    public viewMode: boolean = false;
    public territoryUnits: NomenclatureDTO<number>[] = [];

    public constructor(params?: Partial<ChangeUserLegalDialogParams>) {
        Object.assign(this, params);
    }
}