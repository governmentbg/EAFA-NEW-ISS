import { FuseTranslationLoaderService } from "@/@fuse/services/translation-loader.service";
import { EventEmitter, Injectable, Input } from "@angular/core";
import { FormControl } from "@angular/forms";
import { TLConfirmDialog } from "../confirmation-dialog/tl-confirm-dialog";
import { TLDataColumnComponent } from "./data-column/data-column.component";
import { CommandTypes } from './enums/command-type.enum';
import { DataType } from "./enums/data-type.enum";
import { RecordChangedEventArgs } from './models/record-changed-event.model';
import { GridRow } from "./models/row.model";
import { TLDataTableComponent } from "./tl-data-table.component";

@Injectable()
export class DataTableInlineEditingService {

    private confirmDialog: TLConfirmDialog;
    constructor(confirmDialog: TLConfirmDialog, translate: FuseTranslationLoaderService) {
        this._inActiveRows = [];
        this._activeRows = [];
        this.isSoftDeletable = false;
        this.showInactive = false;
        this.disableEditing = false;
        this.confirmDialog = confirmDialog;
        this.changeRowsContextEvent = new EventEmitter<boolean>();
        this.saveConfirmationMessage = translate.getValue('common.do-you-confirm-the-action-message');
        this.deleteConfirmationMessage = translate.getValue('common.do-you-confirm-the-action-message');
    }

    public isSoftDeletable: boolean;
    public disableEditing: boolean;
    public dataColumns!: TLDataColumnComponent[];
    public saveConfirmationMessage: string = '';
    public deleteConfirmationMessage: string = '';
    public datatable!: TLDataTableComponent;
    public changeRowsContextEvent: EventEmitter<boolean>;
    private currentCommandType: CommandTypes | undefined;

    private _activeRecord: GridRow<any> | undefined;
    private _activeRows: GridRow<any>[];
    private _inActiveRows: GridRow<any>[];
    private showInactive: boolean;


    public activateRecordForEdit(row: GridRow<any>): void {
        if (!this.showInactive) {
            if (this._activeRecord != undefined) {
                if (this.checkFormGroupValiditity()) {
                    if (this._activeRecord != row) {
                        this._activeRecord.editMode = false;
                        this.setActiveRecord(row);
                    }
                }
            } else {
                this.setActiveRecord(row);
            }
        }
    }

    private setFormControlValues(): void {
        for (const column of this.dataColumns) {
            const dataColumn = (column as TLDataColumnComponent);
            this.setFormColumnValue(dataColumn, this._activeRecord);
        }
    }

    private setRecordValues(): void {
        for (const column of this.dataColumns) {
            const dataColumn = (column as TLDataColumnComponent);
            this.setPropertyValue(dataColumn, this._activeRecord);
        }
    }

    public addNewRow(): void {
        if (!this.showInactive && (this._activeRecord === undefined || (!this._activeRecord.editMode && !this._activeRecord.isNewRecord))) {
            const dataRow: any = {};
            this.currentCommandType = CommandTypes.Add;
            for (const column of this.dataColumns) {
                dataRow[column.propertyName] = this.getDefaultValue(column.dataType);
            }

            this._activeRecord = new GridRow<any>(dataRow, true, true);
            this.setFormControlValues();
            //return this.insertFirst(rows, this._activeRecord);
            this.datatable.pageNumber = 0;
            this.datatable._tempRows = this.insertFirst(this.datatable._tempRows, this._activeRecord);
            this.datatable._totalRecordsCount++;
            this.datatable.activeRecordChanged.emit(undefined);
            //this.changeRowsContext(false);
        }
        else if (this._activeRecord?.editMode) {
            this._activeRecord.validationError = true;
        }
    }


    public saveRow(row: GridRow<any>, emitEvent: boolean = true): boolean {

        if (this._activeRecord !== undefined && this.checkFormGroupValiditity()) {

            this.setRecordValues();
            this._activeRecord.editMode = false;
            this.datatable._rows = this.datatable._tempRows.slice();
            this._activeRows = this.datatable._rows;

            if (emitEvent) {
                this.datatable.recordChanged.emit(new RecordChangedEventArgs<any>(row.data, this.currentCommandType as CommandTypes));
            }

            this.currentCommandType = undefined;
            this._activeRecord = undefined;

            return true;
        }

        return false;
    }

    public deleteRow(row: GridRow<any>): Promise<boolean> {
        if (this._activeRecord == undefined) {
            this.currentCommandType = CommandTypes.Delete;
            const mutex: EventEmitter<boolean> = new EventEmitter<boolean>();
            this.confirmDialog.open({
                message: this.deleteConfirmationMessage
            }).subscribe(result => {
                if (result) {
                    if (this.isSoftDeletable) {
                        this.softDelete(row);
                    } else {
                        this.hardDelete(row);
                    }
                    this.datatable.recordChanged.emit(new RecordChangedEventArgs<any>(row.data, this.currentCommandType as CommandTypes));
                }
                this.currentCommandType = undefined;
                mutex.emit(result);
                mutex.complete();
            });

            return mutex.toPromise();
        } else {
            return Promise.resolve(false);
        }
    }

    public undoAddEditRow(row: GridRow<any>): void {
        if (this._activeRecord != undefined) {
            if (this._activeRecord.isNewRecord) {
                this.hardDelete(this._activeRecord);
            } else {
                this._activeRecord.editMode = false;
            }

            this._activeRecord = undefined;
            this.currentCommandType = undefined;
        }
    }

    public undoDeleteRow(row: GridRow<any>): void {

        this.currentCommandType = CommandTypes.UndoDelete;
        this.softUndo(row);
        this.datatable.recordChanged.emit(new RecordChangedEventArgs<any>(row.data, this.currentCommandType as CommandTypes));
        this.currentCommandType = undefined;
    }

    public editRow(row: GridRow<any>): void {
        this.currentCommandType = CommandTypes.Edit;
        if (!this.showInactive && this._activeRecord != undefined) {
            if (this._activeRecord != row) {
                this._activeRecord.validationError = true;
                //this.undoAddEditRow(this._activeRecord);
                //this.setActiveRecord(row);
            } else {
                this.setActiveRecord(row);
            }
        } else {
            this.setActiveRecord(row);
        }
    }

    public get rows(): any[] {
        if (this.isSoftDeletable) {
            const activeRows = this.unwrapGridRows(this._activeRows);
            const inactiveRows = this.unwrapGridRows(this._inActiveRows);
            return activeRows.concat(inactiveRows);
        } else {
            return this.unwrapGridRows(this.datatable._rows);
        }
    }

    public set rows(rows: any[]) {
        if (rows != undefined && rows.length > 0) {
            this.buildGridRows(rows);
        }
        else {
            this._activeRows = [];
            this._inActiveRows = [];
            this.changeRowsContext(this.showInactive);
        }
    }

    private buildGridRows(rows: any[]): void {
        if (this.isSoftDeletable && rows[0]['isActive'] != undefined) {
            this._activeRows = this.wrapGridRows(rows.filter(x => x.isActive));
            this._inActiveRows = this.wrapGridRows(rows.filter(x => !x.isActive));
            this.changeRowsContext(this.showInactive);
        } else {
            this._activeRows = this.wrapGridRows(rows);
            this.changeRowsContext(this.showInactive);
        }
    }

    private wrapGridRows(rows: any[]): GridRow<any>[] {
        const newGridRows: GridRow<any>[] = new Array<GridRow<any>>();
        for (const item of rows) {

            const record = new GridRow(item);
            newGridRows.push(record);
        }

        return newGridRows;
    }

    private unwrapGridRows(rows: GridRow<any>[]): any[] {
        const newGridRows: any[] = [];
        for (const item of rows) {
            newGridRows.push(item.data);
        }
        return newGridRows;
    }

    public changeRowsContext(showInactive: boolean): void {
        this.showInactive = showInactive;
        if (showInactive) {
            this.datatable._tempRows = this._inActiveRows;
            this.datatable._rows = this._inActiveRows.slice();
            this.datatable._totalRecordsCount = this.datatable._tempRows.length;
        } else {
            this.datatable._tempRows = this._activeRows;
            this.datatable._rows = this._activeRows.slice();
            this.datatable._totalRecordsCount = this.datatable._tempRows.length;
        }

        this.changeRowsContextEvent.emit(showInactive);
    }

    private setActiveRecord(row: GridRow<any>): void {
        this._activeRecord = row;
        this._activeRecord.editMode = true;
        this.datatable.activeRecordChanged.emit(this._activeRecord);
        this.setFormControlValues();
    }

    private hardDelete(row: GridRow<any>): void {
        const rowToDelete = this.datatable._tempRows.find(x => x == row);
        this._activeRows = this.deleteRecord(this._activeRows, rowToDelete).slice();
        this.changeRowsContext(false);
    }

    public softDelete(row: GridRow<any>): void {

        if (!row.isNewRecord) {
            this._activeRows = this.deleteRecord(this._activeRows, row).slice();
            this.datatable._tempRows = this._activeRows;
            (row as any).isActive = false;
            (row as any).data.isActive = false;
            this._inActiveRows.push(row);
        } else {
            this.hardDelete(row);
        }
    }

    public softUndo(row: GridRow<any>): void {
        const rowToActivate = this._inActiveRows.find(x => x == row);
        if (rowToActivate != undefined) {
            this._inActiveRows = this.deleteRecord(this._inActiveRows, rowToActivate).slice();
            this.datatable._tempRows = this._inActiveRows;
            (rowToActivate as any).isActive = true;
            (rowToActivate as any).data.isActive = true;
            this._activeRows.push(rowToActivate);
        }
    }

    private checkFormGroupValiditity(): boolean {
        const formGroup = this.dataColumns.find(x => x._formGroup != undefined)?._formGroup;
        if (formGroup != undefined) {
            formGroup.markAllAsTouched();
            formGroup.updateValueAndValidity();

            if (formGroup.valid) {
                formGroup.markAsUntouched();
                return true;
            }
        }
        return false;
    }

    private deleteRecord(array: any[], itemToDelete: any): any[] {
        const newArray: any[] = [];
        for (const item of array) {
            if (item != itemToDelete) {
                newArray.push(item);
            }
        }
        return newArray;
    }

    private insertFirst(array: any[], record: any): any[] {
        const newArray: any[] = [record, ...array];
        return newArray;
    }

    private insertLast(array: any[], record: any): any[] {
        array.push(this._activeRecord);
        return array.slice();
    }

    private getDefaultValue(dataType: DataType): any {
        switch (dataType) {
            case DataType.STRING:
                return '';
            case DataType.BOOLEAN:
                return false;
            case DataType.NUMBER:
                return 0;
            case DataType.DATE:
            case DataType.DATETIME:
                return new Date();
            default:
                return undefined;
        }
    }

    private setFormColumnValue(dataColumn: TLDataColumnComponent, record: any) {
        if (dataColumn._formGroup != undefined) {
            const formControl: FormControl | undefined = dataColumn._formGroup.get(dataColumn._formControlName) as FormControl;
            if (formControl != undefined) {
                if (formControl.disabled) {
                    formControl.setValue(record.data[dataColumn.propertyName]);
                    formControl.disable({ emitEvent: false });
                }
                else {
                    formControl.setValue(record.data[dataColumn.propertyName]);
                }
            }
        }
    }

    private setPropertyValue(dataColumn: TLDataColumnComponent, record: any) {
        if (dataColumn._formGroup != undefined && record != undefined) {
            const value = dataColumn._formGroup.get(dataColumn._formControlName)?.value;
            record.data[dataColumn.propertyName] = value;
            record[dataColumn.propertyName] = value;
        }
    }

}