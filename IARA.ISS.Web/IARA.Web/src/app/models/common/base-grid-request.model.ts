import { ColumnSorting } from './grid-request.model';

export class BaseGridRequestModel {
    public pageNumber!: number;
    public pageSize!: number;
    public sortColumns!: ColumnSorting[];
}