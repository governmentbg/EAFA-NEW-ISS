import { Component, EventEmitter, Input, Output } from '@angular/core';

import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { AquacultureLogBookPageRegisterDTO } from '@app/models/generated/dtos/AquacultureLogBookPageRegisterDTO';
import { AquaculturePageRecordChanged } from './models/aquaculture-page-record-changed.model';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';


@Component({
    selector: 'aquaculture-pages-table',
    templateUrl: './aquaculture-pages-table.component.html'
})
export class AquaculturePagesTableComponent {
    @Input()
    public rows: AquacultureLogBookPageRegisterDTO[] = [];

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
    public canCancelAquacultureLogBookRecords: boolean = false;

    @Input()
    public canEditAquacultureLogBookRecords: boolean = false;

    @Input()
    public recordsPerPage: number = 10;

    @Input()
    public canReadInspections: boolean = false;

    @Output()
    public onActiveRecordChanged: EventEmitter<AquaculturePageRecordChanged> = new EventEmitter<AquaculturePageRecordChanged>();

    @Output()
    public onEditAquacultureLogBookPage: EventEmitter<AquacultureLogBookPageRegisterDTO> = new EventEmitter<AquacultureLogBookPageRegisterDTO>();

    @Output()
    public onViewAquacultureLogBookPage: EventEmitter<AquacultureLogBookPageRegisterDTO> = new EventEmitter<AquacultureLogBookPageRegisterDTO>();

    @Output()
    public onAnnulAquacultureLogBookPage: EventEmitter<AquacultureLogBookPageRegisterDTO> = new EventEmitter<AquacultureLogBookPageRegisterDTO>();

    @Output()
    public onRestoreAnnulledAquacultureLogBookPage: EventEmitter<AquacultureLogBookPageRegisterDTO> = new EventEmitter<AquacultureLogBookPageRegisterDTO>();

    public readonly logBookPageStatusesEnum: typeof LogBookPageStatusesEnum = LogBookPageStatusesEnum;
    public readonly logBookTypesEnum: typeof LogBookTypesEnum = LogBookTypesEnum;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    public activeRecordChangedEvent(page: AquacultureLogBookPageRegisterDTO, viewMode: boolean = false): void {
        this.onActiveRecordChanged.emit(
            new AquaculturePageRecordChanged({
                page: page,
                viewMode: viewMode
            })
        );
    }

    public editAquacultureLogBookPage(document: AquacultureLogBookPageRegisterDTO, viewMode: boolean): void {
        if (viewMode) {
            this.onViewAquacultureLogBookPage.emit(document);
        }
        else {
            this.onEditAquacultureLogBookPage.emit(document);
        }
    }

    public onAnnulAquacultureLogBookPageBtnClicked(document: AquacultureLogBookPageRegisterDTO): void {
        this.onAnnulAquacultureLogBookPage.emit(document);
    }

    public onRestoreAnnulledAquacultureLogBookPageBtnClicked(document: AquacultureLogBookPageRegisterDTO): void {
        this.onRestoreAnnulledAquacultureLogBookPage.emit(document);
    }
}