import { Component, Input, ViewChild } from '@angular/core';
import { DataTableColumnDirective } from '@swimlane/ngx-datatable';
import { DataType } from './enums/data-type.enum';

@Component({ template: '' })
export abstract class BaseDataColumn {

    constructor() {
        this.columnName = '';
        this.propertyName = '';
        this.flexRate = 1;
        this.isSortable = true;
        this.isResizable = false;
        this.hidden = false;
        this._dataType = DataType.STRING;
        this.attachedLeft = false;
        this.attachedRight = false;
    }


    @ViewChild(DataTableColumnDirective)
    public ngxDataColumn!: DataTableColumnDirective;

    @Input() public columnName: string;
    @Input() public propertyName: string;
    @Input() public flexRate: number;
    @Input() public isSortable: boolean;
    @Input() public isResizable: boolean;

    public _isEditable: boolean = false;

    public _dataType: DataType;
    @Input() public set dataType(value: DataType) {
        this._dataType = value;
        if (this._dataType == DataType.CALCULATED) {
            this._isEditable = false;
        }
    }

    @Input() public hidden: boolean;
    @Input() public attachedLeft: boolean;
    @Input() public attachedRight: boolean;
}