import { Component, EventEmitter, Input, Output } from '@angular/core';

import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { AdmissionLogBookPageRegisterDTO } from '@app/models/generated/dtos/AdmissionLogBookPageRegisterDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { AdmissionPageRecordChanged } from './models/admission-page-record-change.model';


@Component({
    selector: 'admission-pages-and-declarations-table',
    templateUrl: './admission-pages-and-declarations-table.component.html'
})
export class AdmissionPagesAndDeclarationsTableComponent {
    @Input()
    public rows: AdmissionLogBookPageRegisterDTO[] = [];

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
    public canCancelAdmissionLogBookRecords: boolean = false;

    @Input()
    public canEditAdmissionLogBookRecords: boolean = false;

    @Input()
    public canEditNumberAdmissionLogBookRecords: boolean = false;

    @Input()
    public canAddEditFilesAdmissionLogBookRecords: boolean = false;

    @Input()
    public recordsPerPage: number = 10;

    @Input()
    public canReadInspections: boolean = false;

    @Output()
    public onActiveRecordChanged: EventEmitter<AdmissionPageRecordChanged> = new EventEmitter<AdmissionPageRecordChanged>();

    @Output()
    public onEditAdmissionLogBookPage: EventEmitter<AdmissionLogBookPageRegisterDTO> = new EventEmitter<AdmissionLogBookPageRegisterDTO>();

    @Output()
    public onViewAdmissionLogBookPage: EventEmitter<AdmissionLogBookPageRegisterDTO> = new EventEmitter<AdmissionLogBookPageRegisterDTO>();

    @Output()
    public onAnnulAdmissionLogBookPage: EventEmitter<AdmissionLogBookPageRegisterDTO> = new EventEmitter<AdmissionLogBookPageRegisterDTO>();

    @Output()
    public onRestoreAnnulledAdmissionLogBookPage: EventEmitter<AdmissionLogBookPageRegisterDTO> = new EventEmitter<AdmissionLogBookPageRegisterDTO>();

    @Output()
    public onEditAdmissionLogBookPageNumber: EventEmitter<AdmissionLogBookPageRegisterDTO> = new EventEmitter<AdmissionLogBookPageRegisterDTO>();

    @Output()
    public onAddEditAdmissionLogBookPageFiles: EventEmitter<AdmissionLogBookPageRegisterDTO> = new EventEmitter<AdmissionLogBookPageRegisterDTO>();

    public readonly logBookPageStatusesEnum: typeof LogBookPageStatusesEnum = LogBookPageStatusesEnum;
    public readonly logBookTypesEnum: typeof LogBookTypesEnum = LogBookTypesEnum;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    public activeRecordChangedEvent(page: AdmissionLogBookPageRegisterDTO, viewMode: boolean = false): void {
        this.onActiveRecordChanged.emit(
            new AdmissionPageRecordChanged({
                page: page,
                viewMode: viewMode
            })
        );
    }

    public editAdmissionLogBookPage(document: AdmissionLogBookPageRegisterDTO, viewMode: boolean): void {
        if (viewMode) {
            this.onViewAdmissionLogBookPage.emit(document);
        }
        else {
            this.onEditAdmissionLogBookPage.emit(document);
        }
    }

    public onAnnulAdmissionLogBookPageBtnClicked(document: AdmissionLogBookPageRegisterDTO): void {
        this.onAnnulAdmissionLogBookPage.emit(document);
    }

    public onRestoreAnnulledAdmissionLogBookPageBtnClicked(document: AdmissionLogBookPageRegisterDTO): void {
        this.onRestoreAnnulledAdmissionLogBookPage.emit(document);
    }

    public onEditAdmissionLogBookPageNumberBtnClicked(document: AdmissionLogBookPageRegisterDTO): void {
        this.onEditAdmissionLogBookPageNumber.emit(document);
    }

    public onAddEditAdmissionLogBookPageFilesBtnClicked(document: AdmissionLogBookPageRegisterDTO): void {
        this.onAddEditAdmissionLogBookPageFiles.emit(document);
    }
}