import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PenalDecreeDTO } from '@app/models/generated/dtos/PenalDecreeDTO';
import { PenalDecreesFilters } from '@app/models/generated/filters/PenalDecreesFilters';
import { PenalDecreesService } from '@app/services/administration-app/penal-decrees.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { PenalDecreeEditDTO } from '@app/models/generated/dtos/PenalDecreeEditDTO';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { AuanRegisterService } from '@app/services/administration-app/auan-register.service';
import { PenalDecreeTypeEnum } from '@app/enums/penal-decree-type.enum';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { RangeInputData } from '@app/shared/components/input-controls/tl-range-input/range-input.component';
import { PenalDecreeStatusDTO } from '@app/models/generated/dtos/PenalDecreeStatusDTO';
import { AuanDeliveryDataDTO } from '@app/models/generated/dtos/AuanDeliveryDataDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { InspDeliveryTypesNomenclatureDTO } from '@app/models/generated/dtos/InspDeliveryTypesNomenclatureDTO';
import { InspDeliveryTypeGroupsEnum } from '@app/enums/insp-delivery-type-groups.enum';
import { EditPenalDecreeComponent } from '@app/components/administration-app/control-activity/penal-decrees/edit-penal-decree/edit-penal-decree.component';
import { EditDecreeAgreementComponent } from '@app/components/administration-app/control-activity/penal-decrees/edit-decree-agreement/edit-decree-agreement.component';
import { EditDecreeWarningComponent } from '@app/components/administration-app/control-activity/penal-decrees/edit-decree-warning/edit-decree-warning.component';
import { EditDecreeResolutionComponent } from '@app/components/administration-app/control-activity/penal-decrees/edit-decree-resolution/edit-decree-resolution.component';
import { EditPenalDecreeStatusComponent } from '@app/components/administration-app/control-activity/penal-decrees/edit-penal-decree-status/edit-penal-decree-status.component';
import { EditPenalDecreeAuanPickerComponent } from '@app/components/administration-app/control-activity/penal-decrees/edit-penal-decree-auan-picker/edit-penal-decree-auan-picker.component';
import { AuanDeliveryComponent } from '@app/components/administration-app/control-activity/auan-register/auan-delivery/auan-delivery.component';
import { EditPenalDecreeDialogParams } from '@app/components/administration-app/control-activity/penal-decrees/models/edit-penal-decree-params.model';
import { EditPenalDecreeStatusDialogParams } from '@app/components/administration-app/control-activity/penal-decrees/models/edit-penal-decree-status-params.model';
import { InspDeliveryDataDialogParams } from '@app/components/administration-app/control-activity/auan-register/models/insp-delivery-data-dialog-params.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { AuanDrafterNomenclatureDTO } from '@app/models/generated/dtos/AuanDrafterNomenclatureDTO';
import { InspectorUserNomenclatureDTO } from '@app/models/generated/dtos/InspectorUserNomenclatureDTO';
import { AuanStatusEnum } from '@app/enums/auan-status.enum';

@Component({
    selector: 'penal-decrees',
    templateUrl: './penal-decrees.component.html'
})
export class PenalDecreesComponent implements OnInit, AfterViewInit {
    @Input()
    public auanId: number | undefined;

    @Input()
    public recordsPerPage: number = 20;

    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public statusesEnum: typeof AuanStatusEnum = AuanStatusEnum;

    public territoryUnits: NomenclatureDTO<number>[] = [];
    public statuses: NomenclatureDTO<number>[] = [];
    public types: NomenclatureDTO<number>[] = [];
    public drafters: AuanDrafterNomenclatureDTO[] = [];
    public issuers: InspectorUserNomenclatureDTO[] = [];
    public sanctions: NomenclatureDTO<number>[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public fishingGears: NomenclatureDTO<number>[] = [];
    public appliances: NomenclatureDTO<number>[] = [];
    public deliveryTypes: NomenclatureDTO<number>[] = [];
    public deliveryConfirmationTypes: NomenclatureDTO<number>[] = [];
    public penalDecreeStatuses: NomenclatureDTO<AuanStatusEnum>[] = [];

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;

    public readonly canReadStatusRecords: boolean;
    public readonly canAddStatusRecords: boolean;
    public readonly canEditStatusRecords: boolean;
    public readonly canDeleteStatusRecords: boolean;
    public readonly canRestoreStatusRecords: boolean;
    public readonly canReadPointsRecords: boolean;
    public readonly canSubmitRecords: boolean;
    public readonly canCancelRecords: boolean;
    public readonly canReturnForFurtherCorrectionsRecords: boolean;
    public readonly canSaveAfterHours: boolean;

    public readonly penalDecreeTypeEnum: typeof PenalDecreeTypeEnum = PenalDecreeTypeEnum;

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<PenalDecreeDTO, PenalDecreesFilters>;

    private readonly service: PenalDecreesService;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly penalDecreeDialog: TLMatDialog<EditPenalDecreeComponent>;
    private readonly agreementDialog: TLMatDialog<EditDecreeAgreementComponent>;
    private readonly warningDialog: TLMatDialog<EditDecreeWarningComponent>;
    private readonly resolutionDialog: TLMatDialog<EditDecreeResolutionComponent>;
    private readonly editStatusDialog: TLMatDialog<EditPenalDecreeStatusComponent>;
    private readonly auanPickerDialog: TLMatDialog<EditPenalDecreeAuanPickerComponent>;
    private readonly inspDeliveryDialog: TLMatDialog<AuanDeliveryComponent>;
    private readonly snackbar: MatSnackBar;
    private readonly auanService: AuanRegisterService;

    public constructor(
        service: PenalDecreesService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        penalDecreeDialog: TLMatDialog<EditPenalDecreeComponent>,
        agreementDialog: TLMatDialog<EditDecreeAgreementComponent>,
        warningDialog: TLMatDialog<EditDecreeWarningComponent>,
        resolutionDialog: TLMatDialog<EditDecreeResolutionComponent>,
        editStatusDialog: TLMatDialog<EditPenalDecreeStatusComponent>,
        auanPickerDialog: TLMatDialog<EditPenalDecreeAuanPickerComponent>,
        inspDeliveryDialog: TLMatDialog<AuanDeliveryComponent>,
        snackbar: MatSnackBar,
        permissions: PermissionsService,
        auanService: AuanRegisterService
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.penalDecreeDialog = penalDecreeDialog;
        this.agreementDialog = agreementDialog;
        this.warningDialog = warningDialog;
        this.resolutionDialog = resolutionDialog;
        this.editStatusDialog = editStatusDialog;
        this.auanPickerDialog = auanPickerDialog;
        this.inspDeliveryDialog = inspDeliveryDialog;
        this.snackbar = snackbar;
        this.auanService = auanService;

        this.canAddRecords = permissions.has(PermissionsEnum.PenalDecreesAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.PenalDecreesEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.PenalDecreesDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.PenalDecreesRestoreRecords);

        this.canReadStatusRecords = permissions.has(PermissionsEnum.PenalDecreeStatusesRead);
        this.canAddStatusRecords = permissions.has(PermissionsEnum.PenalDecreeStatusesAddRecords);
        this.canEditStatusRecords = permissions.has(PermissionsEnum.PenalDecreeStatusesEditRecords);
        this.canDeleteStatusRecords = permissions.has(PermissionsEnum.PenalDecreeStatusesDeleteRecords);
        this.canRestoreStatusRecords = permissions.has(PermissionsEnum.PenalDecreeStatusesRestoreRecords);
        this.canReadPointsRecords = permissions.hasAny(PermissionsEnum.AwardedPointsRead, PermissionsEnum.AwardedPointsReadAll);
        this.canSubmitRecords = permissions.has(PermissionsEnum.PenalDecreesSubmitRecords);
        this.canCancelRecords = permissions.has(PermissionsEnum.PenalDecreesCancelRecords);
        this.canReturnForFurtherCorrectionsRecords = permissions.has(PermissionsEnum.PenalDecreescanReturnForFurtherCorrectionsRecords);
        this.canSaveAfterHours = permissions.has(PermissionsEnum.PenalDecreesCanSaveAfterHours);

        this.penalDecreeStatuses = [
            new NomenclatureDTO<AuanStatusEnum>({
                value: AuanStatusEnum.Draft,
                displayName: this.translate.getValue('penal-decrees.penal-decree-status-draft'),
                isActive: true
            }),
            new NomenclatureDTO<AuanStatusEnum>({
                value: AuanStatusEnum.Submitted,
                displayName: this.translate.getValue('penal-decrees.penal-decree-status-submitted'),
                isActive: true
            }),
            new NomenclatureDTO<AuanStatusEnum>({
                value: AuanStatusEnum.Canceled,
                displayName: this.translate.getValue('penal-decrees.penal-decree-status-canceled'),
                isActive: true
            })
        ];

        this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.territoryUnits = result;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.PenalDecreeStatuses, this.service.getPenalDecreeStatusTypes.bind(this.service), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.statuses = result;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.PenalDecreeSanctionTypes, this.service.getPenalDecreeSanctionTypes.bind(this.service), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.sanctions = result;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.fishes = result;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.FishingGear, this.nomenclatures.getFishingGear.bind(this.nomenclatures), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.fishingGears = result;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.ConfiscatedAppliances, this.service.getConfiscatedAppliances.bind(this.service), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.appliances = result;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.InspDeliveryTypes, this.service.getAuanDeliveryTypes.bind(this.service), false
        ).subscribe({
            next: (result: InspDeliveryTypesNomenclatureDTO[]) => {
                this.deliveryTypes = result.filter(x => x.group === InspDeliveryTypeGroupsEnum.PD);
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.InspDeliveryConfirmationTypes, this.service.getAuanDeliveryConfirmationTypes.bind(this.service), false
        ).subscribe({
            next: (result: InspDeliveryTypesNomenclatureDTO[]) => {
                this.deliveryConfirmationTypes = (result as InspDeliveryTypesNomenclatureDTO[]).filter(x => x.group === InspDeliveryTypeGroupsEnum.PD);
            }
        });

        this.auanService.getAllDrafters().subscribe({
            next: (drafters: AuanDrafterNomenclatureDTO[]) => {
                this.drafters = drafters;
            }
        });

        this.service.getInspectorUsernames().subscribe({
            next: (issuers: InspectorUserNomenclatureDTO[]) => {
                this.issuers = issuers;
            }
        });

        this.service.getPenalDecreeTypes().subscribe({
            next: (types: NomenclatureDTO<number>[]) => {
                this.types = types;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<PenalDecreeDTO, PenalDecreesFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.auanId === null || this.auanId === undefined ? this.searchpanel : undefined,
            requestServiceMethod: this.service.getAllPenalDecrees.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        const filters: PenalDecreesFilters = new PenalDecreesFilters();

        if (window.history.state) {
            const tableId: number | undefined = window.history.state.tableId;

            if (tableId !== undefined && tableId !== null) {
                filters.id = tableId;
            }

            this.grid.advancedFilters = filters;
        }

        if (this.auanId !== null && this.auanId !== undefined) {
            this.grid.advancedFilters = new PenalDecreesFilters({
                auanId: this.auanId 
            });
        }

        this.grid.refreshData();
    }

    public addEditPenalDecree(decree: PenalDecreeDTO | undefined, viewMode: boolean): void {
        const rightButtons: IActionInfo[] = [];

        if (decree !== undefined && decree !== null) {
            const data: EditPenalDecreeDialogParams = new EditPenalDecreeDialogParams({
                id: decree.id,
                auanId: decree.auanId,
                typeId: decree.typeId,
                status: decree.penalDecreeStatus,
                canSaveAfterHours: this.canSaveAfterHours,
                isReadonly: viewMode
            });

            const auditBtn: IHeaderAuditButton = {
                id: decree.id!,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'RInsp.PenalDecreesRegister'
            };

            const printBtnTitle: string = viewMode
                ? this.translate.getValue('penal-decrees.print')
                : this.translate.getValue('penal-decrees.save-print');

            if (decree.isActive) {
                if (!viewMode
                    && decree.penalDecreeStatus === AuanStatusEnum.Draft
                    && (this.auanId === undefined || this.auanId === null)) {
                    rightButtons.push({
                        id: 'save-draft',
                        color: 'primary',
                        translateValue: this.translate.getValue('penal-decrees.save-draft'),
                        isVisibleInViewMode: true
                    });
                }
                else if (decree.penalDecreeStatus === AuanStatusEnum.Submitted
                    && this.canReturnForFurtherCorrectionsRecords
                    && (this.auanId === undefined || this.auanId === null)) {
                    rightButtons.push({
                        id: 'more-corrections-needed',
                        color: 'accent',
                        translateValue: this.translate.getValue('penal-decrees.more-corrections-needed'),
                        isVisibleInViewMode: true
                    });
                }

                if (this.canSubmitRecords) {
                    rightButtons.push({
                        id: 'print',
                        color: 'accent',
                        translateValue: printBtnTitle,
                        isVisibleInViewMode: true
                    });
                }
            }

            this.openEditDialog(data, decree.decreeType!, viewMode, rightButtons, auditBtn, decree.isActive ?? true);
        }
        else {
            const title: string = this.translate.getValue('penal-decrees.choose-auan-dialog-title');

            const dialog = this.auanPickerDialog.openWithTwoButtons({
                title: title,
                TCtor: EditPenalDecreeAuanPickerComponent,
                headerAuditButton: undefined,
                headerCancelButton: {
                    cancelBtnClicked: this.closeChooseAuanDialogBtnClicked.bind(this)
                },
                componentData: undefined,
                translteService: this.translate,
                disableDialogClose: true,
                viewMode: viewMode
            }, '600px');

            dialog.subscribe({
                next: (entry: PenalDecreeEditDTO | undefined) => {
                    if (entry !== undefined && entry !== null) {
                        this.grid.refreshData();
                    }
                }
            });
        }
    }

    public deletePenalDecree(decree: PenalDecreeDTO): void {
        let title: string = '';
        let message: string = '';
        let okBtnLabel: string = '';

        switch (decree.decreeType) {
            case PenalDecreeTypeEnum.PenalDecree: {
                title = this.translate.getValue('penal-decrees.delete-penal-decree-dialog-title');
                message = this.translate.getValue('penal-decrees.delete-penal-decree-dialog-message');
                okBtnLabel = this.translate.getValue('penal-decrees.delete-penal-decree-dialog-ok-btn-label');
            } break;
            case PenalDecreeTypeEnum.Agreement: {
                title = this.translate.getValue('penal-decrees.delete-agreement-dialog-title');
                message = this.translate.getValue('penal-decrees.delete-agreement-dialog-message');
                okBtnLabel = this.translate.getValue('penal-decrees.delete-agreement-dialog-ok-btn-label');
            } break;
            case PenalDecreeTypeEnum.Warning: {
                title = this.translate.getValue('penal-decrees.delete-warning-dialog-title');
                message = this.translate.getValue('penal-decrees.delete-warning-dialog-message');
                okBtnLabel = this.translate.getValue('penal-decrees.delete-warning-dialog-ok-btn-label');
            } break;
            case PenalDecreeTypeEnum.Resolution: {
                title = this.translate.getValue('penal-decrees.delete-resolution-dialog-title');
                message = this.translate.getValue('penal-decrees.delete-resolution-dialog-message');
                okBtnLabel = this.translate.getValue('penal-decrees.delete-resolution-dialog-ok-btn-label');
            } break;
        }

        this.confirmDialog.open({
            title: title,
            message: message,
            okBtnLabel: okBtnLabel
        }).subscribe({
            next: (ok: boolean) => {
                if (ok === true) {
                    this.service.deletePenalDecree(decree.id!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        },
                        error: (errorResponse: HttpErrorResponse) => {
                            if ((errorResponse.error as ErrorModel)?.code === ErrorCode.CannotDeleteDecreeWithPenalPoints) {
                                //само към наказателните постановления може да има присъдени точки
                                const message: string = this.translate.getValue('penal-decrees.cannot-delete-decree-with-penal-points');
                                this.snackbar.open(message, undefined, {
                                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                                });
                            }
                        }
                    });
                }
            }
        });
    }

    public restorePenalDecree(decree: PenalDecreeDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok === true) {
                    this.service.undoDeletePenalDecree(decree.id!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    public addEditStatus(status: PenalDecreeStatusDTO | undefined, viewMode: boolean = false, decree: PenalDecreeDTO): void {
        const readOnly: boolean = viewMode === true;

        let data: EditPenalDecreeStatusDialogParams | undefined;
        let auditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (status !== undefined) {
            data = new EditPenalDecreeStatusDialogParams(this.service, status, decree.decreeType!, viewMode, decree.id!);

            if (status.id !== undefined) {
                auditBtn = {
                    id: status.id,
                    getAuditRecordData: this.service.getPenalDecreeStatusAudit.bind(this.service),
                    tableName: 'RInsp.PenalDecreeStatuses'
                }
            }

            switch (decree.decreeType) {
                case PenalDecreeTypeEnum.PenalDecree:
                    if (readOnly) {
                        title = this.translate.getValue('penal-decrees.view-penal-decree-status-dialog-title');
                    }
                    else {
                        title = this.translate.getValue('penal-decrees.edit-penal-decree-status-dialog-title');
                    }
                    break;
                case PenalDecreeTypeEnum.Agreement:
                    if (readOnly) {
                        title = this.translate.getValue('penal-decrees.view-agreement-status-dialog-title');
                    }
                    else {
                        title = this.translate.getValue('penal-decrees.edit-agreement-status-dialog-title');
                    }
                    break;
                case PenalDecreeTypeEnum.Warning:
                    if (readOnly) {
                        title = this.translate.getValue('penal-decrees.view-warning-status-dialog-title');
                    }
                    else {
                        title = this.translate.getValue('penal-decrees.edit-warning-status-dialog-title');
                    }
                    break;
                case PenalDecreeTypeEnum.Resolution:
                    if (readOnly) {
                        title = this.translate.getValue('penal-decrees.view-resolution-status-dialog-title');
                    }
                    else {
                        title = this.translate.getValue('penal-decrees.edit-resolution-status-dialog-title');
                    }
                    break;
                default:
                    title = '';
            }
        }
        else {
            data = new EditPenalDecreeStatusDialogParams(this.service, undefined, decree.decreeType!, false, decree.id!);

            switch (decree.decreeType) {
                case PenalDecreeTypeEnum.PenalDecree:
                    title = this.translate.getValue('penal-decrees.add-penal-decree-status-dialog-title');
                    break;
                case PenalDecreeTypeEnum.Agreement:
                    title = this.translate.getValue('penal-decrees.add-agreement-status-dialog-title');
                    break;
                case PenalDecreeTypeEnum.Warning:
                    title = this.translate.getValue('penal-decrees.add-warning-status-dialog-title');
                    break;
                case PenalDecreeTypeEnum.Resolution:
                    title = this.translate.getValue('penal-decrees.add-resolution-status-dialog-title');
                    break;
                default:
                    title = '';
            }
        }

        const dialog = this.editStatusDialog.openWithTwoButtons({
            title: title,
            TCtor: EditPenalDecreeStatusComponent,
            headerAuditButton: auditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditStatusDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            viewMode: viewMode
        }, '1200px');

        dialog.subscribe({
            next: (result: PenalDecreeStatusDTO | undefined) => {
                if (result !== undefined && result !== null) {
                    this.grid.refreshData();
                }
            }
        });
    }

    public deleteStatus(status: PenalDecreeStatusDTO): void {
        this.confirmDialog.open({
            title: this.translate.getValue('penal-decrees.delete-status-dialog-title'),
            message: this.translate.getValue('penal-decrees.delete-status-dialog-message'),
            okBtnLabel: this.translate.getValue('penal-decrees.delete-status-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.deletePenalDecreeStatus(status.id!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        })
    }

    public undoDeleteStatus(status: PenalDecreeStatusDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.undoDeletePenalDecreeStatus(status.id!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        })
    }

    public openDeliveryDialog(decree: PenalDecreeDTO): void {
        const data: InspDeliveryDataDialogParams = new InspDeliveryDataDialogParams({
            registerId: decree.id,
            id: decree.deliveryId,
            service: this.service,
        });

        let auditBtn: IHeaderAuditButton | undefined;

        if (decree.deliveryId !== undefined && decree.deliveryId !== null) {
            auditBtn = {
                id: decree.deliveryId,
                getAuditRecordData: this.service.getInspDeliverySimpleAudit.bind(this.service),
                tableName: 'RInsp.InspDelivery'
            };
        }

        const dialog = this.inspDeliveryDialog.openWithTwoButtons({
            title: this.translate.getValue('penal-decrees.delivery-dialog-title'),
            TCtor: AuanDeliveryComponent,
            headerAuditButton: auditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            viewMode: false,
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
            next: (entry: AuanDeliveryDataDTO | undefined) => {
                if (entry !== undefined && entry !== null) {
                    this.grid.refreshData();
                }
            }
        });
    }

    private closeEditStatusDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            penalDecreeNumControl: new FormControl(),
            auanNumControl: new FormControl(),
            territoryUnitControl: new FormControl(),
            draftDateRangeControl: new FormControl(),
            issueDateRangeControl: new FormControl(),
            effectiveDateRangeControl: new FormControl(),
            drafterControl: new FormControl(),
            issuerControl: new FormControl(),
            sanctionTypeControl: new FormControl(),
            statusTypeControl: new FormControl(),
            locationDescriptionControl: new FormControl(),
            fineAmountControl: new FormControl(),
            deliveryConfirmationTypeControl: new FormControl(),
            deliveryTypeControl: new FormControl(),
            applianceControl: new FormControl(),
            fishingGearControl: new FormControl(),
            fishControl: new FormControl(),
            identifierControl: new FormControl(),
            inspEntityFirstNameControl: new FormControl(),
            inspEntityMiddleNameControl: new FormControl(),
            inspEntityLastNameControl: new FormControl(),
            penalDecreeStatusControl: new FormControl(),
            penalDecreeTypesControl: new FormControl(),
            yearControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): PenalDecreesFilters {
        const result = new PenalDecreesFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            penalDecreeNum: filters.getValue('penalDecreeNumControl'),
            auanNum: filters.getValue('auanNumControl'),
            territoryUnitId: filters.getValue('territoryUnitControl'),
            issueDateFrom: filters.getValue<DateRangeData>('issueDateRangeControl')?.start,
            issueDateTo: filters.getValue<DateRangeData>('issueDateRangeControl')?.end,
            effectiveDateFrom: filters.getValue<DateRangeData>('effectiveDateRangeControl')?.start,
            effectiveDateTo: filters.getValue<DateRangeData>('effectiveDateRangeControl')?.end,
            sanctionTypeIds: filters.getValue('sanctionTypeControl'),
            statusTypeIds: filters.getValue('statusTypeControl'),
            locationDescription: filters.getValue('locationDescriptionControl'),
            applianceId: filters.getValue('applianceControl'),
            fishingGearId: filters.getValue('fishingGearControl'),
            fishId: filters.getValue('fishControl'),
            identifier: filters.getValue('identifierControl'),
            deliveryConfirmationTypeIds: filters.getValue('deliveryConfirmationTypeControl'),
            deliveryTypeIds: filters.getValue('deliveryTypeControl'),
            inspectedEntityFirstName: filters.getValue('inspEntityFirstNameControl'),
            inspectedEntityMiddleName: filters.getValue('inspEntityMiddleNameControl'),
            inspectedEntityLastName: filters.getValue('inspEntityLastNameControl'),
            penalDecreeTypeIds: filters.getValue('penalDecreeTypesControl')
        });

        const fineAmount: RangeInputData | undefined = filters.getValue('fineAmountControl');
        if (fineAmount !== undefined && fineAmount !== null) {
            result.fineAmountFrom = fineAmount.start;
            result.fineAmountTo = fineAmount.end;
        }

        const drafter: number | string | undefined = filters.getValue('drafterControl');
        if (drafter !== undefined) {
            if (typeof drafter === 'number') {
                result.drafterId = drafter;
            }
            else {
                result.drafterName = drafter;
            }
        }

        const issuer: number | string | undefined = filters.getValue('issuerControl');
        if (issuer !== undefined) {
            if (typeof issuer === 'number') {
                result.issuerId = issuer;
            }
            else {
                result.issuerName = issuer;
            }
        }

        const statuses: AuanStatusEnum[] | undefined = filters.getValue('penalDecreeStatusControl');
        if (statuses !== undefined && statuses !== null) {
            result.penalDecreeStatuses = statuses.map((status: AuanStatusEnum) => {
                return AuanStatusEnum[status];
            });
        }

        const yearValue = filters.getValue('yearControl');
        if (yearValue !== null && yearValue !== undefined) {
            result.year = (filters.getValue('yearControl') as Date).getFullYear();
        }

        return result;
    }

    private openEditDialog(
        data: EditPenalDecreeDialogParams,
        type: PenalDecreeTypeEnum,
        viewMode: boolean,
        rightButtons: IActionInfo[],
        auditBtn: IHeaderAuditButton | undefined,
        isActive: boolean = true
    ) {
        const saveBtn: IActionInfo = {
            id: 'save',
            color: 'accent',
            hidden: !this.canSubmitRecords,
            translateValue: this.translate.getValue('common.save')
        };

        const leftButtons: IActionInfo[] = [];
        if (this.canCancelRecords && isActive && (this.auanId === undefined || this.auanId === null)) {
            if (data.status === AuanStatusEnum.Canceled) {
                leftButtons.push({
                    id: 'activate-decree',
                    color: 'accent',
                    translateValue: 'penal-decrees.activate',
                    isVisibleInViewMode: true
                });
            }
            else {
                leftButtons.push({
                    id: 'cancel-decree',
                    color: 'warn',
                    translateValue: 'penal-decrees.cancel',
                    isVisibleInViewMode: data.status === AuanStatusEnum.Submitted
                });
            }
        }

        if (type === PenalDecreeTypeEnum.PenalDecree) {
            let title: string;

            if (data.id !== undefined) {
                title = viewMode
                    ? this.translate.getValue('penal-decrees.view-penal-decree-dialog-title')
                    : this.translate.getValue('penal-decrees.edit-penal-decree-dialog-title');
            }
            else {
                title = this.translate.getValue('penal-decrees.add-penal-decree-dialog-title');
            }

            const dialog = this.penalDecreeDialog.openWithTwoButtons({
                title: title,
                TCtor: EditPenalDecreeComponent,
                headerAuditButton: auditBtn,
                headerCancelButton: {
                    cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
                },
                componentData: data,
                translteService: this.translate,
                disableDialogClose: true,
                viewMode: viewMode,
                saveBtn: saveBtn,
                rightSideActionsCollection: rightButtons,
                leftSideActionsCollection: leftButtons
            }, '1400px');

            dialog.subscribe((entry?: PenalDecreeEditDTO) => {
                if (entry !== undefined && entry !== null) {
                    this.grid.refreshData();
                }
            });
        }
        else if (type === PenalDecreeTypeEnum.Agreement) {
            let title: string;

            if (data.id !== undefined) {
                title = viewMode
                    ? this.translate.getValue('penal-decrees.view-agreement-dialog-title')
                    : this.translate.getValue('penal-decrees.edit-agreement-dialog-title');
            }
            else {
                title = this.translate.getValue('penal-decrees.add-agreement-dialog-title');
            }

            const dialog = this.agreementDialog.openWithTwoButtons({
                title: title,
                TCtor: EditDecreeAgreementComponent,
                headerAuditButton: auditBtn,
                headerCancelButton: {
                    cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
                },
                componentData: data,
                translteService: this.translate,
                disableDialogClose: true,
                viewMode: viewMode,
                saveBtn: saveBtn,
                rightSideActionsCollection: rightButtons,
                leftSideActionsCollection: leftButtons
            }, '1400px');

            dialog.subscribe((entry?: PenalDecreeEditDTO) => {
                if (entry !== undefined && entry !== null) {
                    this.grid.refreshData();
                }
            });
        }
        else if (type === PenalDecreeTypeEnum.Warning) {
            let title: string;

            if (data.id !== undefined) {
                title = viewMode
                    ? this.translate.getValue('penal-decrees.view-warning-dialog-title')
                    : this.translate.getValue('penal-decrees.edit-warning-dialog-title');
            }
            else {
                title = this.translate.getValue('penal-decrees.add-warning-dialog-title');
            }

            const dialog = this.warningDialog.openWithTwoButtons({
                title: title,
                TCtor: EditDecreeWarningComponent,
                headerAuditButton: auditBtn,
                headerCancelButton: {
                    cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
                },
                componentData: data,
                translteService: this.translate,
                disableDialogClose: true,
                viewMode: viewMode,
                saveBtn: saveBtn,
                rightSideActionsCollection: rightButtons,
                leftSideActionsCollection: leftButtons
            }, '1400px');

            dialog.subscribe((entry?: PenalDecreeEditDTO) => {
                if (entry !== undefined && entry !== null) {
                    this.grid.refreshData();
                }
            });
        }
        else if (type === PenalDecreeTypeEnum.Resolution) {
            let title: string;

            if (data.id !== undefined) {
                title = viewMode
                    ? this.translate.getValue('penal-decrees.view-resolution-dialog-title')
                    : this.translate.getValue('penal-decrees.edit-resolution-dialog-title');
            }
            else {
                title = this.translate.getValue('penal-decrees.add-resolution-dialog-title');
            }

            const dialog = this.resolutionDialog.openWithTwoButtons({
                title: title,
                TCtor: EditDecreeResolutionComponent,
                headerAuditButton: auditBtn,
                headerCancelButton: {
                    cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
                },
                componentData: data,
                translteService: this.translate,
                disableDialogClose: true,
                viewMode: viewMode,
                saveBtn: saveBtn,
                rightSideActionsCollection: rightButtons,
                leftSideActionsCollection: leftButtons
            }, '1400px');

            dialog.subscribe((entry?: PenalDecreeEditDTO) => {
                if (entry !== undefined && entry !== null) {
                    this.grid.refreshData();
                }
            });
        }
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeChooseAuanDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}