export class GridRow<T> implements IGridRow {
    constructor(data: T, editMode: boolean = false, isNewRecord: boolean = false) {
        this.data = data;
        this.copyDataAsProperties();
        this.editMode = editMode;
        this.isNewRecord = isNewRecord;
        this.validationError = false;
    }

    public data: T;
    public editMode: boolean;
    public isNewRecord: boolean;
    public validationError: boolean;

    private copyDataAsProperties(): void {
        const keys = Object.keys(this.data);
        const values = Object.values(this.data);

        for (let i = 0; i < keys.length; i++) {
            (this as any)[keys[i]] = values[i];
        }
    }
}

export interface IGridRow {
    [key: string]: any;
}