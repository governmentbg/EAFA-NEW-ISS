import { BaseGridRequestModel } from './base-grid-request.model';
import { BaseRequestModel } from "./BaseRequestModel";

export class GridRequestModel<T extends BaseRequestModel> extends BaseGridRequestModel {
    public constructor() {
        super();
        this.pageNumber = 1;
        this.pageSize = 50;
        this.sortColumns = new Array<ColumnSorting>();
    }

    public filters: T | undefined;
}

export class ColumnSorting {
    public constructor(propertyName: string, sortOrder: string) {
        this.propertyName = propertyName;
        this.sortOrder = sortOrder;
    }

    public propertyName: string;
    public sortOrder: string;
}