export class AssociationEditDialogParams {
    public id!: number;
    public adding: boolean = false;
    public readonly: boolean = false;

    public constructor(params?: Partial<AssociationEditDialogParams>) {
        Object.assign(this, params);
    }
}