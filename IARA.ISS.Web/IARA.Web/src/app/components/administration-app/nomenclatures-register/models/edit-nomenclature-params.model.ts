import { ColumnDTO } from '@app/models/generated/dtos/ColumnDTO';

export class EditNomenclatureParams {
    public id: number | undefined;
    public viewMode: boolean = false;
    public columns: ColumnDTO[] = [];

    public constructor(params?: Partial<EditNomenclatureParams>) {
        Object.assign(this, params);
    }
}