import { EventEmitter } from "@angular/core";
import { IPageEventArgs } from "./page-event-args.interface";
import { ISortEventArgs } from "./sort-event-args.interface";
import { ITLDatatableComponent } from "./tl-datatable.interface";

export interface IRemoteTLDatatableComponent extends ITLDatatableComponent {
    showLoader(): void;
    hideLoader(): void;
    sortingChanged: EventEmitter<ISortEventArgs>;
    pageChanged: EventEmitter<IPageEventArgs>;
    addButtonClicked: EventEmitter<void>;
    totalRecordsCount: number;
    setRows(value: any[]): void;
    excelExported: EventEmitter<void>;
}