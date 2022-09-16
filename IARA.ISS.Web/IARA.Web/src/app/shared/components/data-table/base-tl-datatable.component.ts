import { Component, ContentChild, ContentChildren, Input, QueryList, ViewChild } from "@angular/core";
import { ColumnMode, DatatableComponent, SelectionType } from "@swimlane/ngx-datatable";
import { BaseDataColumn } from "./base-data-column";
import { TLDataColumnInlineEditingComponent } from "./data-column-inline-editing/data-column-inline-editing.component";
import { TLDataColumnTemplateComponent } from "./data-column-template/data-column-template.component";
import { TLDataColumnComponent } from "./data-column/data-column.component";
import { DataTableInlineEditingService } from "./datatable-inlineediting.service";
import { TLGroupHeaderComponent } from "./group-header/group-header.component";
import { GridRow } from "./models/row.model";
import { TLRowDetailComponent } from "./row-detail/row-detail.component";

@Component({ template: '' })
export class BaseTLDatatableComponent {


    constructor(inlineEditingService: DataTableInlineEditingService) {
        this._columnMode = ColumnMode.flex;
        this._columns = [];
        this._disableSorting = false;
        this._finalColumns = new QueryList<BaseDataColumn>();
        this._footerHeight = 50;
        this._headerHeight = 50;
        this._isRemote = true;
        this._isSoftDeletable = false;
        this._loading = false;
        this._pageNumber = 0;
        this._recordsPerPage = 20;
        this._remotePaging = true;
        this._remoteSorting = true;
        this._reorderable = false;
        this._hasRowDetail = false;
        this._hasGroupHeader = false;
        this._groupHeaderRowHeight = 'auto';
        this._disableEditing = false;
        this._rows = new Array<GridRow<any>>();
        this._tempRows = new Array<GridRow<any>>();
        this._label = '';
        this._showInactiveRecords = true;
        this._showInactiveRecordsLabel = '';
        this._addButtonLabel = '';
        this._addButtonIcon = 'fa-plus-circle';
        this._showAddButton = true;
        this._totalRecordsCount = 0;
        this.counter = 0;
        this.inlineEditingService = inlineEditingService;
    }

    protected counter: number;
    protected _dataColumns!: QueryList<BaseDataColumn>;
    protected _datatable!: DatatableComponent;
    protected _disableSorting: boolean;
    protected _finalColumns: QueryList<BaseDataColumn>;
    protected _inlineEditing: boolean = false;
    protected inlineEditingService: DataTableInlineEditingService;

    public _columnMode: ColumnMode;
    public _columns: any[];
    public _footerHeight: number;
    public _headerHeight: number;
    public _isRemote: boolean;
    public _isSoftDeletable: boolean;
    public _loading: boolean;
    public _pageNumber: number;
    public _recordsPerPage: number;
    public _remotePaging: boolean;
    public _remoteSorting: boolean
    public _reorderable: boolean;
    public _rows: GridRow<any>[];
    public _tempRows: GridRow<any>[];
    public _selectionType: SelectionType | undefined;
    public _showInactiveRecords: boolean;
    public _label: string;
    public _showInactiveRecordsLabel: string;
    public _addButtonLabel: string;
    public _addButtonIcon: string;
    public _showAddButton: boolean;
    public _totalRecordsCount: number;
    public _rowDetail!: TLRowDetailComponent;
    public _groupHeader!: TLGroupHeaderComponent;
    public _groupHeaderRowHeight: string;
    public _hasRowDetail: boolean;
    public _hasGroupHeader: boolean;
    public _disableEditing: boolean = false;
    public _selectedRows: any[] = [];
    public _groupRowsByFieldName: string = '';
    public _groupsExpandedByDefault: boolean = false;
    public _rowClass: ((row: any) => Record<string, boolean>) | undefined;


    @ViewChild(TLDataColumnTemplateComponent)
    public toggleDetailArrowColumn!: BaseDataColumn;

    @ContentChildren(BaseDataColumn)
    public set dataColumns(columns: QueryList<BaseDataColumn>) {
        this._dataColumns = columns;
    }

    private _inlineGridColumn: TLDataColumnInlineEditingComponent | undefined;

    @ContentChild(TLDataColumnInlineEditingComponent)
    public set inlineGridColumn(value: TLDataColumnInlineEditingComponent | undefined) {
        if (value != undefined) {
            this._inlineGridColumn = value;
            this.inlineEditing = true;
        }
    }

    @ContentChild(TLRowDetailComponent)
    public set rowDetail(value: TLRowDetailComponent | undefined) {
        if (value != undefined) {
            this._rowDetail = value;
            this._hasRowDetail = true;
        }
    }

    @ContentChild(TLGroupHeaderComponent)
    public set groupHeader(value: TLGroupHeaderComponent | undefined) {
        if (value !== undefined && value !== null) {
            this._groupHeader = value;
            this._hasGroupHeader = true;
        }
        else {
            this._hasGroupHeader = false;
        }
    }

    @ViewChild(DatatableComponent)
    public set datatable(val: DatatableComponent) {
        this._datatable = val;
    }


    @Input() public set inlineEditing(value: boolean) {
        this._inlineEditing = value;
    }

    @Input() public set recordsPerPage(value: number) {
        this._recordsPerPage = value;
    }

    public get recordsPerPage(): number {
        return this._recordsPerPage;
    }

    @Input() public set pageNumber(value: number) {
        this._pageNumber = value;
    }

    public get pageNumber(): number {
        return this._pageNumber;
    }

    @Input() public set headerHeight(value: number) {
        this._headerHeight = value;
    }

    @Input() public set footerHeight(value: number) {
        this._footerHeight = value;
    }

    @Input() public set totalRecordsCount(value: number) {
        this._totalRecordsCount = value;
    }

    @Input() public set columnMode(value: ColumnMode) {
        this._columnMode = value;
    }

    @Input() public set isRemote(value: boolean) {
        this._remotePaging = !!value;
        this._remoteSorting = !!value;
        this._isRemote = !!value;
    }

    @Input() public set disableSorting(value: boolean) {
        this._disableSorting = value;
        this.setColumnTemplates();
    }

    @Input() public set reorderable(value: boolean) {
        this._reorderable = value;
    }

    @Input() public set selectionType(value: SelectionType) {
        this._selectionType = value;
    }

    @Input() public set selectedRows(value: any[]) {
        if (value !== null && value !== undefined) {
            this._selectedRows = value;
        }
    }

    @Input() public set label(value: string) {
        this._label = value;
    }

    @Input() public set showInactiveRecordsLabel(value: string) {
        this._showInactiveRecordsLabel = value;
    }

    @Input() public set showInactiveRecords(value: boolean) {
        this._showInactiveRecords = value;
    }

    @Input() public set addButtonLabel(value: string) {
        this._addButtonLabel = value;
    }

    @Input() public set addButtonIcon(value: string) {
        this._addButtonIcon = value;
    }

    @Input() public set showAddButton(value: boolean) {
        this._showAddButton = value;
    }

    @Input() public set isSoftDeletable(value: boolean) {
        this._isSoftDeletable = value;
        this.inlineEditingService.isSoftDeletable = value;
        this.inlineEditingService.rows = this.rows;
    }

    @Input() public set disableEditing(value: boolean) {
        this._disableEditing = value;
    }

    @Input() public set groupRowsByFieldName(value: string) {
        this._groupRowsByFieldName = value;
    }

    @Input() public set groupsExpandedByDefault(value: boolean) {
        this._groupsExpandedByDefault = value;
    }

    @Input() public set groupHeaderRowHeight(value: string) {
        if (value !== null && value !== undefined && value !== '') {
            this._groupHeaderRowHeight = value;
        }
    }

    @Input() public set rowClass(value: (row: any) => Record<string, any>) {
        this._rowClass = value;
    }

    @Input() public set rows(rows: any[]) {
        this.inlineEditingService.rows = rows;
        this.setColumnTemplates();
    }

    public get rows(): any[] {
        return this.inlineEditingService.rows;
    }

    @Input()
    public hasExcelExport: boolean = false;

    @Input()
    public excelBtnTextResource: string = 'common.export-to-excel-btn';

    @Input()
    public excelIcon: string = 'fa-file-export';

    public softDelete(row: GridRow<any>): void {
        this.inlineEditingService.softDelete(row);
    }

    public softUndoDelete(row: GridRow<any>): void {
        this.inlineEditingService.softUndo(row);
    }

    protected setColumnTemplates(): void {
        const columns: BaseDataColumn[] = [];

        if (this._hasRowDetail && this.toggleDetailArrowColumn !== undefined) {
            columns.push(this.toggleDetailArrowColumn);
        }

        if (this._dataColumns != undefined && this._dataColumns.length > 0) {
            for (const item of this._dataColumns) {
                if (!item.hidden) {
                    if (this._disableSorting) {
                        item.ngxDataColumn.sortable = false;
                    }
                    columns.push(item);
                }
            }
            this.inlineEditingService.dataColumns = this._dataColumns.map(x => x as TLDataColumnComponent);
        }

        if (this._inlineGridColumn != undefined) {
            columns.push(this._inlineGridColumn);
        }

        if (columns.length > 0) {
            this._finalColumns.reset(columns);
            this.calculateFlexRates(this._finalColumns);
            this._columns = this._finalColumns.map(x => x.ngxDataColumn);
        }

        this.recalculateGrid();
    }

    protected recalculateGrid(): void {
        if (this._datatable != undefined) {
            this._datatable.recalculateColumns(this._columns);
            this._datatable.recalculate();
        }
    }

    private calculateFlexRates(columnTemplates: QueryList<BaseDataColumn>): void {

        if (columnTemplates != undefined) {
            const columns = columnTemplates.toArray();
            const missingFlexRateColumns = columns.filter(x => x.flexRate == undefined);
            const columnsWithFlexRates = columns.filter(x => x.flexRate != undefined);
            const sumFlexRates = columnsWithFlexRates.map(x => x.flexRate).reduce((prev, current) => prev + current);
            const flexRate = (1 - sumFlexRates) / missingFlexRateColumns.length;
            for (const column of missingFlexRateColumns) {
                column.flexRate = flexRate;
            }
        }
    }
}