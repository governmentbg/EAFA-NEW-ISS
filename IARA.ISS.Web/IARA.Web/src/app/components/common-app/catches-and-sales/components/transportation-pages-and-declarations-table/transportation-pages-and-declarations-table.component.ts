﻿import { Component, EventEmitter, Input, Output } from '@angular/core';

import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TransportationLogBookPageRegisterDTO } from '@app/models/generated/dtos/TransportationLogBookPageRegisterDTO';
import { TransportationPageRecordChanged } from './models/transportation-page-record-changed.model';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';


@Component({
    selector: 'transportation-pages-and-declarations-table',
    templateUrl: './transportation-pages-and-declarations-table.component.html'
})
export class TransportationPagesAndDeclarationsTableComponent {
    @Input()
    public rows: TransportationLogBookPageRegisterDTO[] = [];

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
    public canCancelTransportationLogBookRecords: boolean = false;

    @Input()
    public canEditTransportationLogBookRecords: boolean = false;

    @Input()
    public canEditNumberTransportationLogBookRecords: boolean = false;

    @Input()
    public canAddEditFilesTransportationLogBookRecords: boolean = false;

    @Input()
    public recordsPerPage: number = 10;

    @Input()
    public canReadInspections: boolean = false;

    @Output()
    public onActiveRecordChanged: EventEmitter<TransportationPageRecordChanged> = new EventEmitter<TransportationPageRecordChanged>();

    @Output()
    public onEditTransportationLogBookPage: EventEmitter<TransportationLogBookPageRegisterDTO> = new EventEmitter<TransportationLogBookPageRegisterDTO>();

    @Output()
    public onViewTransportationLogBookPage: EventEmitter<TransportationLogBookPageRegisterDTO> = new EventEmitter<TransportationLogBookPageRegisterDTO>();

    @Output()
    public onAnnulTransportationLogBookPage: EventEmitter<TransportationLogBookPageRegisterDTO> = new EventEmitter<TransportationLogBookPageRegisterDTO>();

    @Output()
    public onRestoreAnnulledTransportationLogBookPage: EventEmitter<TransportationLogBookPageRegisterDTO> = new EventEmitter<TransportationLogBookPageRegisterDTO>();

    @Output()
    public onEditTransportationLogBookPageNumber: EventEmitter<TransportationLogBookPageRegisterDTO> = new EventEmitter<TransportationLogBookPageRegisterDTO>();

    @Output()
    public onAddEditTransportationLogBookPageFiles: EventEmitter<TransportationLogBookPageRegisterDTO> = new EventEmitter<TransportationLogBookPageRegisterDTO>();

    public readonly logBookPageStatusesEnum: typeof LogBookPageStatusesEnum = LogBookPageStatusesEnum;
    public readonly logBookTypesEnum: typeof LogBookTypesEnum = LogBookTypesEnum;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    public activeRecordChangedEvent(page: TransportationLogBookPageRegisterDTO, viewMode: boolean = false): void {
        this.onActiveRecordChanged.emit(
            new TransportationPageRecordChanged({
                page: page,
                viewMode: viewMode
            })
        );
    }

    public editTransportationLogBookPage(document: TransportationLogBookPageRegisterDTO, viewMode: boolean): void {
        if (viewMode) {
            this.onViewTransportationLogBookPage.emit(document);
        }
        else {
            this.onEditTransportationLogBookPage.emit(document);
        }
    }

    public onAnnulTransportationLogBookPageBtnClicked(document: TransportationLogBookPageRegisterDTO): void {
        this.onAnnulTransportationLogBookPage.emit(document);
    }

    public onRestoreAnnulledTransportationLogBookPageBtnClicked(document: TransportationLogBookPageRegisterDTO): void {
        this.onRestoreAnnulledTransportationLogBookPage.emit(document);
    }

    public onEditTransportationLogBookPageNumberBtnClicked(document: TransportationLogBookPageRegisterDTO): void {
        this.onEditTransportationLogBookPageNumber.emit(document);
    }

    public onAddEditTransportationLogBookPageFilesBtnClicked(document: TransportationLogBookPageRegisterDTO): void {
        this.onAddEditTransportationLogBookPageFiles.emit(document);
    }
}