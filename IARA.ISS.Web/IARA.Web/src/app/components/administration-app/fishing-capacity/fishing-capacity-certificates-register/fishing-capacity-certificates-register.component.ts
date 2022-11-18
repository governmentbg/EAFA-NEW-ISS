import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Subscription } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IFishingCapacityService } from '@app/interfaces/common-app/fishing-capacity.interface';
import { FishingCapacityCertificateDTO } from '@app/models/generated/dtos/FishingCapacityCertificateDTO';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { RangeInputData } from '@app/shared/components/input-controls/tl-range-input/range-input.component';
import { FishingCapacityAdministrationService } from '@app/services/administration-app/fishing-capacity-administration.service';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditCapacityCertificateComponent } from './edit-capacity-certificate/edit-capacity-certificate.component';
import { FishingCapacityCertificateEditDTO } from '@app/models/generated/dtos/FishingCapacityCertificateEditDTO';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { FishingCapacityCertificatesFilters } from '@app/models/generated/filters/FishingCapacityCertificatesFilters';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { CapacityCertificateHistoryApplDTO } from '@app/models/generated/dtos/CapacityCertificateHistoryApplDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { CapacityCertificateHistoryTransferredToDTO } from '@app/models/generated/dtos/CapacityCertificateHistoryTransferredToDTO';
import { IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IShipsRegisterService } from '@app/interfaces/common-app/ships-register.interface';
import { EditShipComponent } from '@app/components/common-app/ships-register/edit-ship/edit-ship.component';
import { ShipDeregistrationComponent } from '@app/components/common-app/ships-register/ship-deregistration/ship-deregistration.component';
import { IncreaseFishingCapacityComponent } from '@app/components/common-app/fishing-capacity/increase-fishing-capacity/increase-fishing-capacity.component';
import { TransferFishingCapacityComponent } from '@app/components/common-app/fishing-capacity/transfer-fishing-capacity/transfer-fishing-capacity.component';
import { ReduceFishingCapacityComponent } from '@app/components/common-app/fishing-capacity/reduce-fishing-capacity/reduce-fishing-capacity.component';
import { ShipsRegisterAdministrationService } from '@app/services/administration-app/ships-register-administration.service';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { CapacityCertificateDuplicateComponent } from '@app/components/common-app/fishing-capacity/capacity-certificate-duplicate/capacity-certificate-duplicate.component';
import { ApplicationDeliveryDTO } from '@app/models/generated/dtos/ApplicationDeliveryDTO';
import { RegisterDeliveryDialogParams } from '@app/shared/components/register-delivery/models/register-delivery-dialog-params.model';
import { RegisterDeliveryComponent } from '@app/shared/components/register-delivery/register-delivery.component';
import { IDeliveryService } from '@app/interfaces/common-app/delivery.interface';
import { DeliveryAdministrationService } from '@app/services/administration-app/delivery-administration.service';
import { ShipsUtils } from '@app/shared/utils/ships.utils';

type ThreeState = 'yes' | 'no' | 'both';

@Component({
    selector: 'fishing-capacity-certificates-register',
    templateUrl: './fishing-capacity-certificates-register.component.html',
    styleUrls: ['./fishing-capacity-certificates-register.component.scss']
})
export class FishingCapacityCertificatesRegisterComponent implements AfterViewInit {
    public readonly pageCodes: typeof PageCodeEnum = PageCodeEnum;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    public readonly canViewShipApplications: boolean;
    public readonly canViewCapacityApplications: boolean;

    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public isCertificateActiveOptions: NomenclatureDTO<ThreeState>[];

    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<FishingCapacityCertificateDTO, FishingCapacityCertificatesFilters>;

    private ships: ShipNomenclatureDTO[] = [];

    private service: IFishingCapacityService;
    private shipsService: IShipsRegisterService;
    private applicationsService: IApplicationsService;
    private deliveryService: IDeliveryService;

    private nomenclatures: CommonNomenclatures;
    private confirmDialog: TLConfirmDialog;

    private editDialog: TLMatDialog<EditCapacityCertificateComponent>;
    private regVesselDialog: TLMatDialog<EditShipComponent>;
    private deregVesselDialog: TLMatDialog<ShipDeregistrationComponent>;
    private increaseFishCapDialog: TLMatDialog<IncreaseFishingCapacityComponent>;
    private reduceFishCapDialog: TLMatDialog<ReduceFishingCapacityComponent>;
    private transferFishCapDialog: TLMatDialog<TransferFishingCapacityComponent>;
    private certDupDialog: TLMatDialog<CapacityCertificateDuplicateComponent>;
    private deliveryDialog: TLMatDialog<RegisterDeliveryComponent>;

    private navigatingToCertificate: boolean = false;

    private readonly loader: FormControlDataLoader;

    public constructor(
        service: FishingCapacityAdministrationService,
        shipsService: ShipsRegisterAdministrationService,
        applicationsService: ApplicationsAdministrationService,
        deliveryService: DeliveryAdministrationService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        permissions: PermissionsService,
        editDialog: TLMatDialog<EditCapacityCertificateComponent>,
        regVesselDialog: TLMatDialog<EditShipComponent>,
        deregVesselDialog: TLMatDialog<ShipDeregistrationComponent>,
        increaseFishCapDialog: TLMatDialog<IncreaseFishingCapacityComponent>,
        reduceFishCapDialog: TLMatDialog<ReduceFishingCapacityComponent>,
        transferFishCapDialog: TLMatDialog<TransferFishingCapacityComponent>,
        certDupDialog: TLMatDialog<CapacityCertificateDuplicateComponent>,
        deliveryDialog: TLMatDialog<RegisterDeliveryComponent>
    ) {
        this.service = service;
        this.shipsService = shipsService;
        this.applicationsService = applicationsService;
        this.deliveryService = deliveryService;
        this.nomenclatures = nomenclatures;
        this.confirmDialog = confirmDialog;
        this.editDialog = editDialog;
        this.translate = translate;

        this.regVesselDialog = regVesselDialog;
        this.deregVesselDialog = deregVesselDialog;
        this.increaseFishCapDialog = increaseFishCapDialog;
        this.reduceFishCapDialog = reduceFishCapDialog;
        this.transferFishCapDialog = transferFishCapDialog;
        this.certDupDialog = certDupDialog;
        this.deliveryDialog = deliveryDialog;

        this.canEditRecords = permissions.has(PermissionsEnum.FishingCapacityCertificatesEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.FishingCapacityCertificatesDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.FishingCapacityCertificatesRestoreRecords);

        this.canViewShipApplications = permissions.has(PermissionsEnum.ShipsRegisterApplicationsRead);
        this.canViewCapacityApplications = permissions.has(PermissionsEnum.FishingCapacityApplicationsRead);

        this.isCertificateActiveOptions = [
            new NomenclatureDTO<ThreeState>({
                value: 'yes',
                displayName: this.translate.getValue('fishing-capacity.is-active-yes'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'no',
                displayName: this.translate.getValue('fishing-capacity.is-active-no'),
                isActive: true
            }),
            new NomenclatureDTO<ThreeState>({
                value: 'both',
                displayName: this.translate.getValue('fishing-capacity.is-active-all'),
                isActive: true
            })
        ];

        this.buildForm();

        this.loader = new FormControlDataLoader(this.loadShipsNomenclature.bind(this));
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<FishingCapacityCertificateDTO, FishingCapacityCertificatesFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllCapacityCertificates.bind(this.service),
            filtersMapper: this.mapFilters.bind(this),
            excelRequestServiceMethod: this.service.downloadFishingCapacityCertificateExcel.bind(this.service),
            excelFilename: this.translate.getValue('fishing-capacity.certificates-excel-filename')
        });

        const isPerson: boolean | undefined = window.history.state?.isPerson;
        const id: number | undefined = window.history.state?.id;

        if (!CommonUtils.isNullOrEmpty(id)) {
            if (isPerson) {
                this.grid.advancedFilters = new FishingCapacityCertificatesFilters({ personId: id });
            }
            else {
                this.grid.advancedFilters = new FishingCapacityCertificatesFilters({ legalId: id });
            }
        }

        this.grid.onRequestServiceMethodCalled.subscribe({
            next: (records: FishingCapacityCertificateDTO[] | undefined) => {
                if (records && records.length > 0) {
                    this.loader.load(() => {
                        for (const record of records) {
                            const createdFromShipId: number | undefined = record.history?.createdFromApplication?.shipId;
                            if (createdFromShipId !== undefined && createdFromShipId !== null) {
                                record.history!.createdFromApplication!.shipName = ShipsUtils.get(this.ships, createdFromShipId)!.displayName;
                            }

                            const usedInShipId: number | undefined = record.history?.usedInApplication?.shipId;
                            if (usedInShipId !== undefined && usedInShipId !== null) {
                                record.history!.usedInApplication!.shipName = ShipsUtils.get(this.ships, usedInShipId)!.displayName;
                            }
                        }
                    });

                    if (this.navigatingToCertificate) {
                        setTimeout(() => {
                            // simulate a click to body to scroll to top
                            document.querySelector('body')!.click();
                        });

                        this.navigatingToCertificate = false;
                    }
                }
            }
        });

        this.grid.refreshData();
    }

    public editCertificate(certificate: FishingCapacityCertificateDTO | CapacityCertificateHistoryTransferredToDTO, viewMode: boolean = false): void {
        const data: DialogParamsModel = new DialogParamsModel({
            id: certificate.id,
            isReadonly: viewMode
        });

        const auditButton: IHeaderAuditButton = {
            id: certificate.id!,
            getAuditRecordData: this.service.getFishingCapacityCertificateSimpleAudit.bind(this.service),
            tableName: 'CapacityCertificatesRegister'
        };

        const title: string = viewMode
            ? this.translate.getValue('fishing-capacity.view-capacity-certificate-dialog-title')
            : this.translate.getValue('fishing-capacity.edit-capacity-certificate-dialog-title');

        const printBtnTitle: string = viewMode
            ? this.translate.getValue('fishing-capacity.print')
            : this.translate.getValue('fishing-capacity.save-and-print');

        const dialog = this.editDialog.open({
            title: title,
            TCtor: EditCapacityCertificateComponent,
            headerAuditButton: auditButton,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: !viewMode,
            viewMode: viewMode,
            rightSideActionsCollection: [{
                id: 'print',
                color: 'accent',
                translateValue: printBtnTitle,
                isVisibleInViewMode: true
            }],
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translate.getValue('common.save')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translate.getValue('common.cancel'),
            },
        }, '1200px');

        dialog.subscribe({
            next: (entry?: FishingCapacityCertificateEditDTO) => {
                if (entry !== undefined) {
                    this.grid.refreshData();
                }
            }
        });
    }

    public deleteCertificate(certificate: FishingCapacityCertificateDTO): void {
        this.confirmDialog.open({
            title: this.translate.getValue('fishing-capacity.delete-capacity-certificate-dialog-title'),
            message: this.translate.getValue('fishing-capacity.delete-capacity-certificate-confirm-message'),
            okBtnLabel: this.translate.getValue('fishing-capacity.delete-capacity-certificate-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.deleteCapacityCertificate(certificate.id!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    public undoDeleteCertificate(certificate: FishingCapacityCertificateDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.undoDeleteCapacityCertificate(certificate.id!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    public viewCreatedFromApplication(data: CapacityCertificateHistoryApplDTO): void {
        // TODO fix audit buttons
        let title: string;
        let editDialog: TLMatDialog<IDialogComponent>;
        let editDialogTCtor: new (...args: any[]) => IDialogComponent;
        let service: IFishingCapacityService | IShipsRegisterService;
        let auditButton: IHeaderAuditButton;

        if (data.pageCode === PageCodeEnum.RegVessel) {
            title = this.translate.getValue('ships-register.view-ship-application-dialog-title');
            editDialog = this.regVesselDialog;
            editDialogTCtor = EditShipComponent;
            service = this.shipsService;
            auditButton = {
                id: data.applicationId!,
                getAuditRecordData: this.shipsService.getSimpleAudit.bind(this.shipsService),
                tableName: 'ShipsRegister'
            };
        }
        else if (data.pageCode === PageCodeEnum.DeregShip) {
            title = this.translate.getValue('ships-register.view-deregistration-dialog-title');
            editDialog = this.deregVesselDialog;
            editDialogTCtor = ShipDeregistrationComponent;
            service = this.shipsService;
            auditButton = {
                id: data.applicationId!,
                getAuditRecordData: this.shipsService.getSimpleAudit.bind(this.shipsService),
                tableName: 'ShipsRegister'
            };
        }
        else if (data.pageCode === PageCodeEnum.IncreaseFishCap) {
            title = this.translate.getValue('fishing-capacity.view-increase-capacity-application-dialog-title');
            editDialog = this.increaseFishCapDialog;
            editDialogTCtor = IncreaseFishingCapacityComponent;
            service = this.service;
            auditButton = {
                id: data.applicationId!,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'ShipCapacityRegister'
            };
        }
        else if (data.pageCode === PageCodeEnum.ReduceFishCap) {
            title = this.translate.getValue('fishing-capacity.view-reduce-capacity-application-dialog-title');
            editDialog = this.reduceFishCapDialog;
            editDialogTCtor = ReduceFishingCapacityComponent;
            service = this.service;
            auditButton = {
                id: data.applicationId!,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'ShipCapacityRegister'
            };
        }
        else if (data.pageCode === PageCodeEnum.TransferFishCap) {
            title = this.translate.getValue('fishing-capacity.view-transfer-capacity-application-dialog-title');
            editDialog = this.transferFishCapDialog;
            editDialogTCtor = TransferFishingCapacityComponent;
            service = this.service;
            auditButton = {
                id: data.applicationId!,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'ShipCapacityRegister'
            };
        }
        else if (data.pageCode === PageCodeEnum.CapacityCertDup) {
            title = this.translate.getValue('fishing-capacity.view-duplicate-capacity-application-dialog-title');
            editDialog = this.certDupDialog;
            editDialogTCtor = CapacityCertificateDuplicateComponent;
            service = this.service;
            auditButton = {
                id: data.applicationId!,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'ShipCapacityRegister'
            };
        }

        const editDialogData: DialogParamsModel = new DialogParamsModel({
            applicationId: data.applicationId,
            isApplication: true,
            isApplicationHistoryMode: false,
            isReadonly: true,
            viewMode: true,
            service: service!,
            applicationsService: this.applicationsService,
            showOnlyRegiXData: false,
            pageCode: data.pageCode
        });

        const dialog = editDialog!.open({
            title: title!,
            TCtor: editDialogTCtor!,
            headerAuditButton: auditButton!,
            headerCancelButton: {
                cancelBtnClicked: this.closeViewApplicationDialogBtnClicked.bind(this)
            },
            componentData: editDialogData,
            translteService: this.translate,
            disableDialogClose: true,
            viewMode: true
        }, '1400px');

        dialog.subscribe();
    }

    public navigateToCertificate(data: CapacityCertificateHistoryTransferredToDTO | CapacityCertificateHistoryApplDTO): void {
        this.navigatingToCertificate = true;

        if (data instanceof CapacityCertificateHistoryTransferredToDTO) {
            this.grid.advancedFilters = new FishingCapacityCertificatesFilters({ certificateId: data.id });
        }
        else {
            this.grid.advancedFilters = new FishingCapacityCertificatesFilters({ certificateId: data.duplicateCapacityCertificateId });
        }
        this.grid.refreshData();
    }

    public openDeliveryDialog(certificate: FishingCapacityCertificateDTO): void {
        let auditButton: IHeaderAuditButton | undefined;

        if (certificate.deliveryId !== null && certificate.deliveryId !== undefined) {
            auditButton = {
                id: certificate.deliveryId,
                getAuditRecordData: this.deliveryService.getSimpleAudit.bind(this.deliveryService),
                tableName: 'ApplicationDelivery'
            };
        }

        this.deliveryDialog.openWithTwoButtons({
            TCtor: RegisterDeliveryComponent,
            title: this.translate.getValue('fishing-capacity.capacity-certificate-delivery-data-dialog-title'),
            translteService: this.translate,
            componentData: new RegisterDeliveryDialogParams({
                deliveryId: certificate.deliveryId,
                isPublicApp: false,
                service: this.deliveryService,
                pageCode: PageCodeEnum.AquaFarmReg
            }),
            headerCancelButton: {
                cancelBtnClicked: this.closeDeliveryDataDialogBtnClicked.bind(this)
            },
            headerAuditButton: auditButton
        }, '1200px').subscribe({
            next: (model: ApplicationDeliveryDTO | undefined) => {
                if (model !== undefined) {
                    this.grid.refreshData();
                }
            }
        });
    }

    private closeDeliveryDataDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            certificateNumControl: new FormControl(null, TLValidators.number(0)),
            holderNameControl: new FormControl(),
            holderEgnEikControl: new FormControl(),
            validityDateRangeControl: new FormControl(),
            tonnageControl: new FormControl(),
            powerControl: new FormControl(),
            isCertificateActiveControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): FishingCapacityCertificatesFilters {
        const result = new FishingCapacityCertificatesFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            certificateNum: filters.getValue('certificateNumControl'),
            holderNames: filters.getValue('holderNameControl'),
            holderEgnEik: filters.getValue('holderEgnEikControl'),
            validFrom: filters.getValue<DateRangeData>('validityDateRangeControl')?.start,
            validTo: filters.getValue<DateRangeData>('validityDateRangeControl')?.end
        });

        const grossTonnage: RangeInputData | undefined = filters.getValue<RangeInputData>('tonnageControl');
        if (grossTonnage !== undefined && grossTonnage !== null) {
            result.grossTonnageFrom = grossTonnage.start;
            result.grossTonnageTo = grossTonnage.end;
        }

        const power: RangeInputData | undefined = filters.getValue<RangeInputData>('powerControl');
        if (power !== undefined && power !== null) {
            result.powerFrom = power.start;
            result.powerTo = power.end;
        }

        const isCertificateActive: ThreeState | undefined = filters.getValue('isCertificateActiveControl');
        switch (isCertificateActive) {
            case 'yes':
                result.isCertificateActive = true;
                break;
            case 'no':
                result.isCertificateActive = false;
                break;
            default:
            case 'both':
                result.isCertificateActive = undefined;
                break;
        }

        if (result.certificateNum !== null && result.certificateNum !== undefined) {
            result.certificateNum = Number(result.certificateNum);
        }

        return result;
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeViewApplicationDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private loadShipsNomenclature(): Subscription {
        return NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false
        ).subscribe({
            next: (ships: ShipNomenclatureDTO[]) => {
                this.ships = ships;
                this.loader.complete();
            }
        });
    }
}