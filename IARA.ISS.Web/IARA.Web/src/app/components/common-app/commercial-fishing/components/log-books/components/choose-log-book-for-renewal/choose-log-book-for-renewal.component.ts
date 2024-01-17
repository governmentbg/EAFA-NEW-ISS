import { AfterViewInit, Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';

import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { LogBookForRenewalDTO } from '@app/models/generated/dtos/LogBookForRenewalDTO';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { ILogBookService } from '@app/components/common-app/commercial-fishing/components/edit-log-book/interfaces/log-book.interface';
import { ChooseLogBookForRenewalDialogParams } from '../../models/choose-log-book-for-renewal-dialog-params.model';

@Component({
    selector: 'choose-log-book-for-renewal',
    templateUrl: './choose-log-book-for-renewal.component.html'
})
export class ChooseLogBookForRenewalComponent implements IDialogComponent, OnInit, AfterViewInit {
    public readonly filterControl: FormControl;
    public readonly showFinishedControl: FormControl;
    public readonly showOnlySameAquaticTypesControl: FormControl;

    public logBooks: LogBookForRenewalDTO[] = [];
    public logBooksPerPage: number = 8;

    public noLogBooksChosenValidation: boolean = true;
    public touched: boolean = false;
    public numberOfSelectedLogBooks: number = 0;

    private showFinished: boolean = false;
    private showOnlySameAquaticTypes: boolean = false;

    private allLogBooks: LogBookForRenewalDTO[] = [];
    private selectedLogBooks: LogBookForRenewalDTO[] = [];

    private permitLicenseId: number | undefined;
    private service!: ILogBookService;
    private saveToDB: boolean = false;

    public constructor() {
        this.filterControl = new FormControl();
        this.showFinishedControl = new FormControl();
        this.showOnlySameAquaticTypesControl = new FormControl();
    }

    public ngOnInit(): void {
        this.getData(false, false);
    }

    public ngAfterViewInit(): void {
        this.filterControl.valueChanges.subscribe({
            next: (value: string | undefined) => {
                this.filterLogBooks(value);
            }
        });

        this.showFinishedControl.valueChanges.subscribe({
            next: (showFinished: boolean | undefined) => {
                this.showFinished = showFinished ?? false;
                this.getData(showFinished ?? false, this.showOnlySameAquaticTypes);
            }
        });

        this.showOnlySameAquaticTypesControl.valueChanges.subscribe({
            next: (showOnlySameAquaticTypes: boolean | undefined) => {
                this.showOnlySameAquaticTypes = showOnlySameAquaticTypes ?? false;
                this.getData(this.showFinished, showOnlySameAquaticTypes ?? false);
            }
        });
    }

    public setData(data: ChooseLogBookForRenewalDialogParams, wrapperData: DialogWrapperData): void {
        this.permitLicenseId = data.permitLicenseId;
        this.service = data.service!;
        this.saveToDB = data.saveToDB;

        if (data.permitLicenseId === undefined || data.permitLicenseId === null) {
            this.allLogBooks = data.logBooks.map(x => new LogBookForRenewalDTO({
                logBookPermitLicenseId: x.lastLogBookLicenseId,
                logBookId: x.logBookId,
                number: x.logbookNumber,
                isOnline: x.isOnline,
                startPageNumber: x.startPageNumber,
                endPageNumber: x.endPageNumber,
                lastUsedPageNumber: x.lastPageNumber,
                statusName: x.statusName,
                logBookTypeName: x.logBookTypeName,
                isChecked: false
            }));

            this.logBooks = this.allLogBooks.slice();
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.touched = true;

        if (!this.noLogBooksChosenValidation) {
            if (this.saveToDB) {
                // TODO
                const chosenLogBookPermitLicenseIds: number[] = this.selectedLogBooks.map(x => x.logBookPermitLicenseId!);
            }
            else {
                dialogClose(this.selectedLogBooks);
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public checkedRow(row: GridRow<LogBookForRenewalDTO>): void {
        this.touched = true;

        const element: LogBookForRenewalDTO | undefined = this.logBooks?.find(x => x.logBookPermitLicenseId === row.data.logBookPermitLicenseId);

        if (element !== null && element !== undefined) {
            element.isChecked = !element.isChecked;

            if (element.isChecked) {
                this.selectedLogBooks.push(row.data);
            }
            else {
                const indexToDelete: number = this.selectedLogBooks.findIndex(x => x.logBookPermitLicenseId === row.data.logBookPermitLicenseId);
                this.selectedLogBooks.splice(indexToDelete, 1);
            }

            if (this.selectedLogBooks.length > 0) {
                this.noLogBooksChosenValidation = false;
            }
            else {
                this.noLogBooksChosenValidation = true;
            }

            this.numberOfSelectedLogBooks = this.selectedLogBooks.length;
            this.logBooks = this.logBooks!.slice();
        }
    }

    public getRowClass = (row: GridRow<LogBookForRenewalDTO>): Record<string, boolean> => {
        return {
            'row-selected': row.data.isChecked === true,
            'row-not-selected': row.data.isChecked === false
        };
    }

    private filterLogBooks(value: string | undefined): void {
        if (value !== null && value !== undefined && value.length > 0) {
            value = value.toLowerCase();
            const dateValue: Date | undefined = new Date(value);

            this.logBooks = this.allLogBooks.filter((logBook: LogBookForRenewalDTO) => {
                if (logBook.startPageNumber !== null && logBook.startPageNumber !== undefined && logBook.startPageNumber.toString().includes(value!)) {
                    return true;
                }

                if (logBook.endPageNumber !== null && logBook.endPageNumber !== undefined && logBook.endPageNumber.toString().includes(value!)) {
                    return true;
                }

                if (logBook.lastPermitLicenseNumber !== null && logBook.lastPermitLicenseNumber !== undefined && logBook.lastPermitLicenseNumber.toLowerCase().includes(value!)) {
                    return true;
                }

                if (logBook.lastUsedPageNumber !== null && logBook.lastUsedPageNumber !== undefined && logBook.lastUsedPageNumber.toString().includes(value!)) {
                    return true;
                }

                if (logBook.logBookTypeName !== null && logBook.logBookTypeName !== undefined && logBook.logBookTypeName.toLowerCase().includes(value!)) {
                    return true;
                }

                if (logBook.number !== null && logBook.number !== undefined && logBook.number.toLowerCase().includes(value!)) {
                    return true;
                }

                if (logBook.statusName !== null && logBook.statusName !== undefined && logBook.statusName.toLowerCase().includes(value!)) {
                    return true;
                }

                return false;
            });

            const filteredIds: number[] = this.logBooks.map(x => x.logBookPermitLicenseId!);
            const selectedNotInList = this.selectedLogBooks.filter(x => !filteredIds.includes(x.logBookPermitLicenseId!));

            for (const el of selectedNotInList) {
                el.isChecked = false;
            }
        }
        else {
            this.logBooks = this.allLogBooks.slice();
        }
    }

    private getData(showFinished: boolean, showOnlySameAquaticTypes: boolean): void {
        if (this.permitLicenseId !== undefined && this.permitLicenseId !== null) {
            this.service.getLogBooksForRenewal(this.permitLicenseId!, showFinished, showOnlySameAquaticTypes).subscribe({
                next: (results: LogBookForRenewalDTO[]) => {
                    this.allLogBooks = results;

                    setTimeout(() => {
                        this.logBooks = this.allLogBooks.slice();
                    });
                }
            });
        }
    }
}