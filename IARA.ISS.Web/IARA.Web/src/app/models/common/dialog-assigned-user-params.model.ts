export class DialogAssignedUserParamsModel {
    public id!: number;
    public userId: number | undefined;
    public isReadonly: boolean = false;

    public constructor(params?: Partial<DialogAssignedUserParamsModel>) {
        Object.assign(this, params);
    }
}
