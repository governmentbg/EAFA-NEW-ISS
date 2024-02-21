import { Component, EventEmitter, Input, Output } from '@angular/core';

import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { FirstSaleLogBookPageRegisterDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageRegisterDTO';
import { FirstSalePageRecordChanged } from './models/first-sale-page-record-changed.model';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';


@Component({
    selector: 'first-sale-pages-table',
    templateUrl: './first-sale-pages-table.component.html'
})
export class FirstSalePagesTableComponent {
    @Input()
    public rows: FirstSaleLogBookPageRegisterDTO[] = [];

    @Input()
    public isRemote: boolean = false;

    @Input()
    public allowEditAfterFinished: boolean = false;

    @Input()
    public isSoftDeletable: boolean = false;

    @Input()
    public showInactiveRecords: boolean = true;

    @Input()
    public showAddButton: boolean = false;

    @Input()
    public showActionButtons: boolean = false;

    @Input()
    public canCancelFirstSaleLogBookRecords: boolean = false;

    @Input()
    public canEditFirstSaleLogBookRecords: boolean = false;

    @Input()
    public recordsPerPage: number = 10;

    @Input()
    public canReadInspections: boolean = false;

    @Output()
    public onActiveRecordChanged: EventEmitter<FirstSalePageRecordChanged> = new EventEmitter<FirstSalePageRecordChanged>();

    @Output()
    public onEditFirstSaleLogBookPage: EventEmitter<FirstSaleLogBookPageRegisterDTO> = new EventEmitter<FirstSaleLogBookPageRegisterDTO>();

    @Output()
    public onViewFirstSaleLogBookPage: EventEmitter<FirstSaleLogBookPageRegisterDTO> = new EventEmitter<FirstSaleLogBookPageRegisterDTO>();

    @Output()
    public onAnnulFirstSaleLogBookPage: EventEmitter<FirstSaleLogBookPageRegisterDTO> = new EventEmitter<FirstSaleLogBookPageRegisterDTO>();

    @Output()
    public onRestoreAnnulledFirstSaleLogBookPage: EventEmitter<FirstSaleLogBookPageRegisterDTO> = new EventEmitter<FirstSaleLogBookPageRegisterDTO>();

    public readonly logBookPageStatusesEnum: typeof LogBookPageStatusesEnum = LogBookPageStatusesEnum;
    public readonly logBookTypesEnum: typeof LogBookTypesEnum = LogBookTypesEnum;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    public activeRecordChangedEvent(page: FirstSaleLogBookPageRegisterDTO, viewMode: boolean = false): void {
        this.onActiveRecordChanged.emit(
            new FirstSalePageRecordChanged({
                page: page,
                viewMode: viewMode
            })
        );
    }

    public editFirstSaleLogBookPage(document: FirstSaleLogBookPageRegisterDTO, viewMode: boolean): void {
        if (viewMode) {
            this.onViewFirstSaleLogBookPage.emit(document);
        }
        else {
            this.onEditFirstSaleLogBookPage.emit(document);
        }
    }

    public onAnnulFirstSaleLogBookPageBtnClicked(document: FirstSaleLogBookPageRegisterDTO): void {
        this.onAnnulFirstSaleLogBookPage.emit(document);
    }

    public onRestoreAnnulledFirstSaleLogBookPageBtnClicked(document: FirstSaleLogBookPageRegisterDTO): void {
        this.onRestoreAnnulledFirstSaleLogBookPage.emit(document);
    }
}