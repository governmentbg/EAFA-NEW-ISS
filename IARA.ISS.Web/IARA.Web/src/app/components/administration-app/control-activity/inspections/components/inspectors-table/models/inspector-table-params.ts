import { InspectorTableModel } from '../../../models/inspector-table-model';

export class InspectorTableParams {
    public readOnly: boolean = false;
    public isEdit: boolean = false;
    public model: InspectorTableModel | undefined;
    public excludeIds: number[] = [];

    public constructor(params?: Partial<InspectorTableParams>) {
        Object.assign(this, params);
    }
}