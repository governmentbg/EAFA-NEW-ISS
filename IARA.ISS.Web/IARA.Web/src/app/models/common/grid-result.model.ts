export class GridResultModel<T> {
    public constructor() {
        this.records = new Array<T>();
        this.totalRecordsCount = 0;
    }

    public records: T[];
    public totalRecordsCount: number;
}