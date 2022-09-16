import { BaseRequestModel } from '@app/models/common/BaseRequestModel';
import { ColumnSorting } from '@app/models/common/grid-request.model';

export type ExcelExporterHeaderNames = { [field: string]: string };

export class ExcelExporterRequestModel<T extends BaseRequestModel> {
    public filename: string;
    public filters: T | undefined;
    public headerNames: ExcelExporterHeaderNames;
    public childHeaderNames: ExcelExporterHeaderNames[];
    public sortColumns: ColumnSorting[];

    public constructor(
        filename: string,
        filters?: T,
        sortColumns?: ColumnSorting[],
        headerNames?: ExcelExporterHeaderNames,
        childHeaderNames?: ExcelExporterHeaderNames[]
    ) {
        this.filename = filename;
        this.filters = filters ?? undefined;
        this.sortColumns = sortColumns ?? [];
        this.headerNames = headerNames ?? {};
        this.childHeaderNames = childHeaderNames ?? [];
    }
}