import { Component, EventEmitter, Input, Output } from '@angular/core';

import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { ShipLogBookPageRegisterDTO } from '@app/models/generated/dtos/ShipLogBookPageRegisterDTO';
import { AdmissionLogBookPageRegisterDTO } from '@app/models/generated/dtos/AdmissionLogBookPageRegisterDTO';
import { FirstSaleLogBookPageRegisterDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageRegisterDTO';
import { TransportationLogBookPageRegisterDTO } from '@app/models/generated/dtos/TransportationLogBookPageRegisterDTO';
import { ShipPageRecordChanged } from './models/ship-page-record-changed.model';
import { PagesPermissions } from './models/pages-permissions.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';


@Component({
    selector: 'ship-pages-and-declarations-table',
    templateUrl: './ship-pages-and-declarations-table.component.html'
})
export class ShipPagesAndDeclarationsTableComponent {
    @Input()
    public rows: ShipLogBookPageRegisterDTO[] = [];

    @Input()
    public allowEditAfterFinished: boolean = false;

    @Input()
    public isRemote: boolean = false;

    @Input()
    public isSoftDeletable: boolean = true;

    @Input()
    public showInactiveRecords: boolean = true;

    @Input()
    public showAddButton: boolean = false;

    @Input()
    public showActionButtons: boolean = false;

    @Input()
    public showDocumentActionButtons: boolean = false;

    @Input()
    public recordsPerPage: number = 10;

    @Input()
    public canReadInspections: boolean = false;

    @Input()
    public set pagesPermissions(value: PagesPermissions | undefined) {
        if (value !== null && value !== undefined) {
            this._pagesPermissions = value;
        }
        else {
            this._pagesPermissions = new PagesPermissions();
        }
    }

    @Output()
    public onActiveRecordChanged: EventEmitter<ShipPageRecordChanged> = new EventEmitter<ShipPageRecordChanged>();

    @Output()
    public onEditAdmissionLogBookPage: EventEmitter<AdmissionLogBookPageRegisterDTO> = new EventEmitter<AdmissionLogBookPageRegisterDTO>();

    @Output()
    public onViewAdmissionLogBookPage: EventEmitter<AdmissionLogBookPageRegisterDTO> = new EventEmitter<AdmissionLogBookPageRegisterDTO>();

    @Output()
    public onEditTransportationLogBookPage: EventEmitter<TransportationLogBookPageRegisterDTO> = new EventEmitter<TransportationLogBookPageRegisterDTO>();

    @Output()
    public onViewTransportationLogBookPage: EventEmitter<TransportationLogBookPageRegisterDTO> = new EventEmitter<TransportationLogBookPageRegisterDTO>();

    @Output()
    public onEditFirstSaleLogBookPage: EventEmitter<FirstSaleLogBookPageRegisterDTO> = new EventEmitter<FirstSaleLogBookPageRegisterDTO>();

    @Output()
    public onViewFirstSaleLogBookPage: EventEmitter<FirstSaleLogBookPageRegisterDTO> = new EventEmitter<FirstSaleLogBookPageRegisterDTO>();

    @Output()
    public onAnnulShipLogBookPage: EventEmitter<ShipLogBookPageRegisterDTO> = new EventEmitter<ShipLogBookPageRegisterDTO>();

    @Output()
    public onRestoreAnnulledShipLogBookPage: EventEmitter<ShipLogBookPageRegisterDTO> = new EventEmitter<ShipLogBookPageRegisterDTO>();

    @Output()
    public onEditShipLogBookPage: EventEmitter<ShipLogBookPageRegisterDTO> = new EventEmitter<ShipLogBookPageRegisterDTO>();

    @Output()
    public onViewShipLogBookPage: EventEmitter<ShipLogBookPageRegisterDTO> = new EventEmitter<ShipLogBookPageRegisterDTO>();

    @Output()
    public onEditShipLogBookPageNumber: EventEmitter<ShipLogBookPageRegisterDTO> = new EventEmitter<ShipLogBookPageRegisterDTO>();

    @Output()
    public onAddEditShipLogBookPageFiles: EventEmitter<ShipLogBookPageRegisterDTO> = new EventEmitter<ShipLogBookPageRegisterDTO>();

    @Output()
    public onAddAdmissionDeclaration: EventEmitter<ShipLogBookPageRegisterDTO> = new EventEmitter<ShipLogBookPageRegisterDTO>();

    @Output()
    public onAddTransportationDocument: EventEmitter<ShipLogBookPageRegisterDTO> = new EventEmitter<ShipLogBookPageRegisterDTO>();

    @Output()
    public onAddFirstSaleDocument: EventEmitter<ShipLogBookPageRegisterDTO> = new EventEmitter<ShipLogBookPageRegisterDTO>();

    @Output()
    public onAddRelatedDeclaration: EventEmitter<ShipLogBookPageRegisterDTO> = new EventEmitter<ShipLogBookPageRegisterDTO>();

    @Output()
    public onAddAdmissionDocumentFromTransportation: EventEmitter<TransportationLogBookPageRegisterDTO> = new EventEmitter<TransportationLogBookPageRegisterDTO>();

    @Output()
    public onAddFirstSaleDocumentFromTransportation: EventEmitter<TransportationLogBookPageRegisterDTO> = new EventEmitter<TransportationLogBookPageRegisterDTO>();

    @Output()
    public onAddFirstSaleDocumentFromAdmission: EventEmitter<AdmissionLogBookPageRegisterDTO> = new EventEmitter<AdmissionLogBookPageRegisterDTO>();

    public readonly logBookPageStatusesEnum: typeof LogBookPageStatusesEnum = LogBookPageStatusesEnum;
    public readonly logBookTypesEnum: typeof LogBookTypesEnum = LogBookTypesEnum;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    public _pagesPermissions: PagesPermissions = new PagesPermissions();

    public activeRecordChangedEvent(shipPage: ShipLogBookPageRegisterDTO, viewMode: boolean = false): void {
        this.onActiveRecordChanged.emit(
            new ShipPageRecordChanged({
                shipPage: shipPage,
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

    public editTransportationLogBookPage(document: TransportationLogBookPageRegisterDTO, viewMode: boolean): void {
        if (viewMode) {
            this.onViewTransportationLogBookPage.emit(document);
        }
        else {
            this.onEditTransportationLogBookPage.emit(document);
        }
    }

    public editFirstSaleLogBookPage(document: FirstSaleLogBookPageRegisterDTO, viewMode: boolean): void {
        if (viewMode) {
            this.onViewFirstSaleLogBookPage.emit(document);
        }
        else {
            this.onEditFirstSaleLogBookPage.emit(document);
        }
    }

    public onAnnulShipLogBookPageBtnClicked(shipPage: ShipLogBookPageRegisterDTO): void {
        this.onAnnulShipLogBookPage.emit(shipPage);
    }

    public onRestoreAnnulledShipLogBookPageBtnClicked(shipPage: ShipLogBookPageRegisterDTO): void {
        this.onRestoreAnnulledShipLogBookPage.emit(shipPage);
    }

    public editShipLogBookPage(page: ShipLogBookPageRegisterDTO, viewMode: boolean): void {
        if (viewMode) {
            this.onViewShipLogBookPage.emit(page);
        }
        else {
            this.onEditShipLogBookPage.emit(page);
        }
    }

    public onEditShipLogBookPageNumberBtnClicked(shipPage: ShipLogBookPageRegisterDTO): void {
        this.onEditShipLogBookPageNumber.emit(shipPage);
    }

    public onAddEditShipLogBookPageFilesBtnClicked(shipPage: ShipLogBookPageRegisterDTO): void {
        this.onAddEditShipLogBookPageFiles.emit(shipPage);
    }

    public onAddAdmissionDeclarationBtnClicked(shipPage: ShipLogBookPageRegisterDTO): void {
        this.onAddAdmissionDeclaration.emit(shipPage);
    }

    public onAddTransportationDocumentBtnClicked(shipPage: ShipLogBookPageRegisterDTO): void {
        this.onAddTransportationDocument.emit(shipPage);
    }

    public onAddFirstSaleDocumentBtnClicked(shipPage: ShipLogBookPageRegisterDTO): void {
        this.onAddFirstSaleDocument.emit(shipPage);
    }

    public onAddRelatedDeclarationBtnClicked(shipPage: ShipLogBookPageRegisterDTO): void {
        this.onAddRelatedDeclaration.emit(shipPage);
    }

    public onAddAdmissionDocumentFromTransportationDocumentBtnClicked(transportationLogBookPage: TransportationLogBookPageRegisterDTO): void {
        this.onAddAdmissionDocumentFromTransportation.emit(transportationLogBookPage);
    }

    public onAddFirstSaleDocumentFromTransportationDocumentBtnClicked(transportationLogBookPage: TransportationLogBookPageRegisterDTO): void {
        this.onAddFirstSaleDocumentFromTransportation.emit(transportationLogBookPage);
    }

    public onAddFirstSaleDocumentFromAdmissionDeclarationBtnClicked(admissionLogBookPage: AdmissionLogBookPageRegisterDTO): void {
        this.onAddFirstSaleDocumentFromAdmission.emit(admissionLogBookPage);
    }
}