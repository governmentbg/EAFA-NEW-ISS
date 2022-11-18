import { Component, Input } from "@angular/core";
import { BaseDataColumn } from "../base-data-column";
import { DataTableInlineEditingService } from "../datatable-inlineediting.service";
import { GridRow } from "../models/row.model";

@Component({
    selector: 'data-column-inline',
    templateUrl: './data-column-inline-editing.component.html'
})
export class TLDataColumnInlineEditingComponent extends BaseDataColumn {

    private inlineEditingService: DataTableInlineEditingService;
    constructor(inlineEditingService: DataTableInlineEditingService) {
        super();
        this.inlineEditingService = inlineEditingService;
        this.showInactive = false;
        this.inlineEditingService.changeRowsContextEvent.subscribe(this.onChangeContext.bind(this));
    }

    @Input() public saveTooltipText: string = '';

    @Input() public editTooltipText: string = '';

    @Input() public deleteTooltipText: string = '';

    @Input() public undoTooltipText: string = '';

    public showInactive: boolean;

    @Input() public hideEditBtn: boolean = false;
    @Input() public hideRestoreBtn: boolean = false;
    @Input() public hideDeleteBtn: boolean = false;
    @Input() public hideDeleteBtnWhen?: (value: GridRow<any>) => boolean;

    @Input() public set saveConfirmationMessage(value: string) {
        if (value != undefined && value != '') {
            this.inlineEditingService.saveConfirmationMessage = value;
        }
    }

    @Input() public set deleteConfirmationMessage(value: string) {
        if (value != undefined && value != '') {
            this.inlineEditingService.deleteConfirmationMessage = value;
        }
    }

    public onEditRow(row: any): void {
        this.inlineEditingService.editRow(row as GridRow<any>);
    }

    public onDeleteRow(row: any): void {
        this.inlineEditingService.deleteRow(row as GridRow<any>);
    }

    public onSaveRow(row: any): void {

        const gridRow = row as GridRow<any>;
        gridRow.validationError = false;
        this.inlineEditingService.saveRow(gridRow);
    }

    public onUndoAddEditRow(row: any): void {
        const gridRow = row as GridRow<any>;
        this.inlineEditingService.undoAddEditRow(gridRow);
    }

    public onUndoDeleteRow(row: any): void {
        const gridRow = row as GridRow<any>;
        this.inlineEditingService.undoDeleteRow(gridRow);
    }

    private onChangeContext(showInactive: boolean): void {
        this.showInactive = showInactive;
    }
}