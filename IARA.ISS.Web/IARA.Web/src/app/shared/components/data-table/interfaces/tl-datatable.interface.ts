import { EventEmitter } from "@angular/core";
import { ColumnMode, SelectionType, TableColumn } from "@swimlane/ngx-datatable";

export interface ITLDatatableComponent {
    activeRecordChanged: EventEmitter<any>;
    columnMode: ColumnMode;
    footerHeight: number;
    headerHeight: number;
    pageNumber: number;
    recordsPerPage: number;
    reorderable: boolean;
    selectionChanged: EventEmitter<any[]>;
    selectionType: SelectionType | undefined;
    showInactiveChanged: EventEmitter<boolean>;
    isSoftDeletable: boolean;
    showInactiveRecordsLabel: string;
    isRemote: boolean;
    readonly columns: TableColumn[];
}