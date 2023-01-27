import { AfterContentChecked, AfterViewInit, Component, ElementRef, EventEmitter, Input, OnDestroy, Output, ViewChild } from '@angular/core';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';

import { DatatableComponent, TableColumn } from '@swimlane/ngx-datatable';
import { TLTranslatePipe } from '../../pipes/tl-translate.pipe';
import { BaseTLDatatableComponent } from './base-tl-datatable.component';
import { DataTableInlineEditingService } from './datatable-inlineediting.service';
import { IPageEventArgs } from './interfaces/page-event-args.interface';
import { ISortEventArgs } from './interfaces/sort-event-args.interface';
import { ITLDatatableComponent } from './interfaces/tl-datatable.interface';
import { RecordChangedEventArgs } from './models/record-changed-event.model';
import { GridRow } from './models/row.model';

@Component({
    selector: 'tl-data-table',
    templateUrl: './tl-data-table.component.html',
    providers: [DataTableInlineEditingService]
})
export class TLDataTableComponent extends BaseTLDatatableComponent implements ITLDatatableComponent, AfterContentChecked, AfterViewInit, OnDestroy {
    @Input() public disableDblClickEdit: boolean = false;
    @Input() public hasError: boolean = false;

    @Output() public activeRecordChanged: EventEmitter<any> = new EventEmitter<any>();
    @Output() public recordChanged: EventEmitter<RecordChangedEventArgs<any>> = new EventEmitter<RecordChangedEventArgs<any>>();
    @Output() public addButtonClicked: EventEmitter<void> = new EventEmitter<void>();
    @Output() public pageChanged: EventEmitter<IPageEventArgs> = new EventEmitter<IPageEventArgs>();
    @Output() public selectionChanged: EventEmitter<any[]> = new EventEmitter<any[]>();
    @Output() public groupHeaderDetailToggled: EventEmitter<any> = new EventEmitter<any>();
    @Output() public showInactiveChanged: EventEmitter<boolean> = new EventEmitter<boolean>();
    @Output() public sortingChanged: EventEmitter<ISortEventArgs> = new EventEmitter<ISortEventArgs>();
    @Output() public excelExported: EventEmitter<void> = new EventEmitter<void>();

    public get columns(): TableColumn[] {
        return this._datatable.columns;
    }

    protected static readonly SPINNER_DELAY_MS: number = 500;

    @ViewChild('gridWrapperDiv')
    private gridWrapperDiv: ElementRef | undefined;

    @ViewChild(DatatableComponent, { read: ElementRef })
    private datatableElementRef!: ElementRef;

    private wrapperDivWidth: number = 0;
    private wrapperDivHeight: number = 0;

    public constructor(tlTranslatePipe: TLTranslatePipe, inlineEditingService: DataTableInlineEditingService) {
        super(inlineEditingService);
        this.inlineEditingService.datatable = this;
        this._showInactiveRecordsLabel = tlTranslatePipe.transform('common.show-deleted-records', 'cap');
        this._addButtonLabel = tlTranslatePipe.transform('common.add-button-text', 'cap');
    }

    public ngAfterContentChecked(): void {
        if (this.gridWrapperDiv != undefined) {
            if (this.wrapperDivWidth != this.gridWrapperDiv.nativeElement.offsetWidth
                || this.wrapperDivHeight != this.gridWrapperDiv.nativeElement.offsetHeight) {
                this.recalculateGrid();
            }
        }
    }

    public ngAfterViewInit(): void {
        setTimeout(() => {
            this.setColumnTemplates();
        });
    }

    public ngOnDestroy(): void {
        this.rows = [];
    }

    public setRows(value: any[]): void {
        this.rows = value;
    }

    public saveRow(row: GridRow<any>, emitEvent: boolean = true): boolean {
        return this.inlineEditingService.saveRow(row, emitEvent);
    }

    public deleteRow(row: GridRow<any>): Promise<boolean> {
        return this.inlineEditingService.deleteRow(row);
    }

    public undoAddEditRow(row: GridRow<any>): void {
        this.inlineEditingService.undoAddEditRow(row);
    }

    public addButtonClick(): void {
        if (this._inlineEditing) {
            this.inlineEditingService.addNewRow();
        }

        this.addButtonClicked.emit();
    }

    public showLoader(): void {
        this.counter++;

        if (!this._loading) {
            setTimeout(() => {
                if (this.counter > 0) {
                    this._loading = true;

                    this.disableAllButtons();
                }
            }, TLDataTableComponent.SPINNER_DELAY_MS);
        }
    }

    public hideLoader(): void {
        this.counter--;

        if (this._loading) {
            this._loading = false;

            this.enableAllButtons();
        }
    }

    public onActiveRecordChanged(event: any): void {
        if (!this.disableDblClickEdit && event.type == 'dblclick') {
            event.event.preventDefault();
            event.event.stopPropagation();

            if (!this._inlineEditing) {
                this.activeRecordChanged.emit((event.row as GridRow<any>).data);
            } else {
                this.inlineEditingService.activateRecordForEdit((event.row as GridRow<any>));
            }
        }
    }

    public onSelectionChanged(event: any): void {
        this.selectionChanged.emit(event.selected);
    }

    public onGroupHeaderDetailToggle(event: any): void {
        this.groupHeaderDetailToggled.emit(event);
    }

    public onInactiveToggleChanged(event: MatSlideToggleChange): void {
        if (this._isRemote) {
            this.showInactiveChanged.emit(event.checked);

            if (this._inlineEditing) {
                this.inlineEditingService.changeRowsContext(event.checked);
            }
        }
        else {
            this.inlineEditingService.changeRowsContext(event.checked);
        }
    }

    public onSortTable(event: ISortEventArgs): void {
        if (this._isRemote) {
            this.sortingChanged.emit(event);
        }
    }

    public onSetPage(event: IPageEventArgs): void {
        if (this._isRemote) {
            this.pageChanged.emit(event);
        }
    }

    public onToggleExpandRow(row: any): void {
        this._datatable.rowDetail.toggleExpandRow(row);
    }

    public toggleExandGroup(group: { key: any, value: any[] }): void {
        this._datatable.groupHeader.toggleExpandGroup(group);
    }

    public expandAllGroupHeaderRows(): void {
        this._datatable.groupHeader.expandAllGroups();
    }

    public exportExcel(): void {
        this.excelExported.emit();
    }

    private disableAllButtons(): void {
        const table: HTMLElement | undefined = this.datatableElementRef?.nativeElement as HTMLElement;

        if (table) {
            this.disableTableButtons(table);
            this.disablePaginator(table);
            this.disableShowInactiveToggle();
            this.disableExpandRowButtons(table);
        }
    }

    private disableTableButtons(table: HTMLElement): void {
        const buttons: HTMLButtonElement[] = Array.from(table.getElementsByTagName('button'));

        for (const button of buttons) {
            button.disabled = true;
            button.style.opacity = '0.26';
            button.style.cursor = 'wait';
        }
    }

    private disablePaginator(table: HTMLElement): void {
        const paginator: HTMLElement | null = table.getElementsByTagName('datatable-pager').item(0) as HTMLElement;

        if (paginator) {
            const links: HTMLAnchorElement[] = Array.from(paginator.getElementsByTagName('a'));

            for (const link of links) {
                link.style.pointerEvents = 'none';

                const parent: HTMLElement | null = link.parentElement;
                if (parent) {
                    if (parent.className.includes('disabled')) {
                        parent.style.cursor = 'not-allowed';
                    }
                    else {
                        parent.style.cursor = 'wait';
                    }
                }
            }
        }
    }

    private disableShowInactiveToggle(): void {
        const header: HTMLElement | null = document.getElementsByClassName('tl-datatable-header').item(0) as HTMLElement;

        if (header) {
            const toggle: HTMLElement | null = header.getElementsByClassName('mat-slide-toggle').item(0) as HTMLElement;

            if (toggle) {
                toggle.style.opacity = '0.26';
                toggle.style.cursor = 'wait';

                const input: HTMLInputElement | null = header.getElementsByTagName('input').item(0) as HTMLInputElement;
                if (input) {
                    input.disabled = true;

                    const label: HTMLLabelElement | null = input.parentElement?.parentElement as HTMLLabelElement;
                    if (label) {
                        label.style.pointerEvents = 'none';
                        label.style.cursor = 'wait';
                    }
                }
            }
        }
    }

    private disableExpandRowButtons(table: HTMLElement): void {
        const links: HTMLAnchorElement[] = Array.from(table.getElementsByTagName('a'))
            .filter((anchor: HTMLAnchorElement) => anchor.className.includes('datatable-icon'));

        for (const link of links) {
            link.style.pointerEvents = 'none';
            link.style.opacity = '0.26';

            const parent: HTMLElement | null = link.parentElement?.parentElement as HTMLElement;
            if (parent) {
                parent.style.cursor = 'wait';
            }
        }
    }

    private enableAllButtons(): void {
        const table: HTMLElement | undefined = this.datatableElementRef?.nativeElement as HTMLElement;

        if (table) {
            this.enableTableButtons(table);
            this.enablePaginator(table);
            this.enableShowInactiveToggle();
            this.enableExpandRowButtons(table);
        }
    }

    private enableTableButtons(table: HTMLElement): void {
        setTimeout(() => {
            let iconButtons: Element[] = Array.from(table.getElementsByTagName('tl-icon-button'));
            
            iconButtons = iconButtons.filter(x => x.getAttribute('no-enable') === null);

            for (const iconButton of iconButtons) {
                const button = iconButton.getElementsByTagName('button')[0];

                button.disabled = false;
                button.style.opacity = '1';
                button.style.cursor = 'pointer';
            }
        });
}

    private enablePaginator(table: HTMLElement): void {
    const paginator: HTMLElement | null = table.getElementsByTagName('datatable-pager').item(0) as HTMLElement;

if (paginator) {
    const links: HTMLAnchorElement[] = Array.from(paginator.getElementsByTagName('a'));

    for (const link of links) {
        link.style.pointerEvents = 'initial';

        const parent: HTMLElement | null = link.parentElement;
        if (parent) {
            if (parent.className.includes('disabled')) {
                parent.style.cursor = 'not-allowed';
            }
            else {
                parent.style.cursor = 'pointer';
            }
        }
    }
}
    }

    private enableShowInactiveToggle(): void {
    const header: HTMLElement | null = document.getElementsByClassName('tl-datatable-header').item(0) as HTMLElement;

if (header) {
    const toggle: HTMLElement | null = header.getElementsByClassName('mat-slide-toggle').item(0) as HTMLElement;

    if (toggle) {
        toggle.style.opacity = '1';
        toggle.style.cursor = 'pointer';

        const input: HTMLInputElement | null = toggle.getElementsByTagName('input').item(0) as HTMLInputElement;
        if (input) {
            input.disabled = false;

            const label: HTMLLabelElement | null = input.parentElement?.parentElement as HTMLLabelElement;
            if (label) {
                label.style.pointerEvents = 'initial';
                label.style.cursor = 'pointer';
            }
        }
    }
}
    }

    private enableExpandRowButtons(table: HTMLElement): void {
    const links: HTMLAnchorElement[] = Array.from(table.getElementsByTagName('a'))
        .filter((anchor: HTMLAnchorElement) => anchor.className.includes('datatable-icon'));

    for(const link of links) {
        link.style.pointerEvents = 'initial';
        link.style.opacity = '1';

        const parent: HTMLElement | null = link.parentElement?.parentElement as HTMLElement;
        if (parent) {
            parent.style.cursor = 'pointer';
        }
    }
}
}