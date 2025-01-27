import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { AuanRegisterDTO } from '@app/models/generated/dtos/AuanRegisterDTO';
import { AuanRegisterEditDTO } from '@app/models/generated/dtos/AuanRegisterEditDTO';
import { AuanRegisterFilters } from '@app/models/generated/filters/AuanRegisterFilters';
import { AuanRegisterService } from '@app/services/administration-app/auan-register.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { AuanDeliveryDataDTO } from '@app/models/generated/dtos/AuanDeliveryDataDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { AuanDeliveryComponent } from '@app/components/administration-app/control-activity/auan-register/auan-delivery/auan-delivery.component';
import { EditAuanInspectionPickerComponent } from '@app/components/administration-app/control-activity/auan-register/edit-auan-inspection-picker/edit-auan-inspection-picker.component';
import { EditAuanComponent } from '@app/components/administration-app/control-activity/auan-register/edit-auan/edit-auan.component';
import { InspDeliveryDataDialogParams } from '@app/components/administration-app/control-activity/auan-register/models/insp-delivery-data-dialog-params.model';
import { EditAuanDialogParams } from '@app/components/administration-app/control-activity/auan-register/models/edit-auan-dialog-params.model';
import { AuanStatusEnum } from '@app/enums/auan-status.enum';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { SecurityService } from '@app/services/common-app/security.service';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuanDrafterNomenclatureDTO } from '@app/models/generated/dtos/AuanDrafterNomenclatureDTO';

@Component({
    selector: 'auan-register',
    templateUrl: './auan-register.component.html'
})
export class AuanRegisterComponent implements OnInit, AfterViewInit {
    @Input()
    public inspectionId: number | undefined;

    @Input()
    public recordsPerPage: number = 20;

    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public drafters: AuanDrafterNomenclatureDTO[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];
    public inspectionTypes: NomenclatureDTO<number>[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public fishingGears: NomenclatureDTO<number>[] = [];
    public appliances: NomenclatureDTO<number>[] = [];
    public statuses: NomenclatureDTO<AuanStatusEnum>[] = [];

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;
    public readonly canReadPenalDecreeRecords: boolean;
    public readonly canCancelAuanRecords: boolean;
    public readonly canReturnAuanForCorrections: boolean;

    public auanStatusesEnum: typeof AuanStatusEnum = AuanStatusEnum;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    public currentUser: string;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<AuanRegisterDTO, AuanRegisterFilters>;

    private readonly service: AuanRegisterService;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly editDialog: TLMatDialog<EditAuanComponent>;
    private readonly inspectionPickerDialog: TLMatDialog<EditAuanInspectionPickerComponent>;
    private readonly inspDeliveryDialog: TLMatDialog<AuanDeliveryComponent>;
    private readonly snackbar: MatSnackBar;

    public constructor(
        service: AuanRegisterService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editDialog: TLMatDialog<EditAuanComponent>,
        inspectionPickerDialog: TLMatDialog<EditAuanInspectionPickerComponent>,
        inspDeliveryDialog: TLMatDialog<AuanDeliveryComponent>,
        snackbar: MatSnackBar,
        permissions: PermissionsService,
        authService: SecurityService
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editDialog = editDialog;
        this.inspDeliveryDialog = inspDeliveryDialog;
        this.inspectionPickerDialog = inspectionPickerDialog;
        this.snackbar = snackbar;

        this.canAddRecords = permissions.has(PermissionsEnum.AuanRegisterAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.AuanRegisterEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.AuanRegisterDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.AuanRegisterRestoreRecords);
        this.canReadPenalDecreeRecords = permissions.hasAny(PermissionsEnum.PenalDecreesRead, PermissionsEnum.PenalDecreesReadAll);
        this.canCancelAuanRecords = permissions.has(PermissionsEnum.AuanRegisterCancel);
        this.canReturnAuanForCorrections = permissions.has(PermissionsEnum.AuanRegisterReturnForCorrections);

        this.currentUser = authService.User!.username;

        this.statuses = [
            new NomenclatureDTO<AuanStatusEnum>({
                value: AuanStatusEnum.Draft,
                displayName: this.translate.getValue('auan-register.status-draft'),
                isActive: true
            }),
            new NomenclatureDTO<AuanStatusEnum>({
                value: AuanStatusEnum.Submitted,
                displayName: this.translate.getValue('auan-register.status-submitted'),
                isActive: true
            }),
            new NomenclatureDTO<AuanStatusEnum>({
                value: AuanStatusEnum.Canceled,
                displayName: this.translate.getValue('auan-register.status-canceled'),
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
            NomenclatureTypes.InspectionTypes, this.nomenclatures.getInspectionTypes.bind(this.nomenclatures), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.inspectionTypes = result;
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

        this.service.getAllDrafters().subscribe({
            next: (drafters: AuanDrafterNomenclatureDTO[]) => {
                this.drafters = drafters;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<AuanRegisterDTO, AuanRegisterFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.inspectionId === null || this.inspectionId === undefined ? this.searchpanel : undefined,
            requestServiceMethod: this.service.getAllAuans.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        const filters: AuanRegisterFilters = new AuanRegisterFilters();

        if (window.history.state) {
            const tableId: number | undefined = window.history.state.tableId;
            const isPerson: boolean | undefined = window.history.state.isPerson;
            const id: number | undefined = window.history.state.id;

            if (tableId !== undefined && tableId !== null) {
                filters.id = tableId;
            }

            if (id !== undefined && id !== null) {
                if (isPerson) {
                    filters.personId = id;
                }
                else {
                    filters.legalId = id;
                }
            }

            this.grid.advancedFilters = filters;
        }

        if (this.inspectionId !== undefined && this.inspectionId !== null) {
            this.grid.advancedFilters = new AuanRegisterFilters({
                inspectionId: this.inspectionId
            });
        }

        this.grid.refreshData();
    }

    public addEditAuan(auan: AuanRegisterDTO | undefined, viewMode: boolean): void {
        if (auan !== undefined && auan !== null) {
            const data: EditAuanDialogParams = new EditAuanDialogParams({
                id: auan.id,
                inspectionId: auan.inspectionId,
                isReadonly: viewMode
            });

            const auditBtn: IHeaderAuditButton = {
                id: auan.id!,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'RInsp.AUANRegister'
            };

            const title: string = viewMode
                ? this.translate.getValue('auan-register.view-auan-dialog-title')
                : this.translate.getValue('auan-register.edit-auan-dialog-title');

            const printBtnTitle: string = viewMode
                ? this.translate.getValue('auan-register.print')
                : this.translate.getValue('auan-register.save-print');

            const rightButtons: IActionInfo[] = [];
            const leftButtons: IActionInfo[] = [];

            if (auan.isActive) {
                if (auan.status === AuanStatusEnum.Draft) {
                    rightButtons.push({
                        id: 'save-draft',
                        color: 'primary',
                        translateValue: 'auan-register.save-draft'
                    });
                }

                //ако няма правото за връщане за корекции, може да връща в статус "Чернова" само своите АУАНи, ако не са минали 48 часа от добавянето им
                if (auan.status === AuanStatusEnum.Submitted
                    && (this.canReturnAuanForCorrections || (!auan.lockedForCorrections && auan.createdByUser === this.currentUser))
                ) {
                    rightButtons.push({
                        id: 'more-corrections-needed',
                        color: 'accent',
                        translateValue: 'auan-register.more-corrections-needed',
                        isVisibleInViewMode: true
                    });
                }

                rightButtons.push({
                    id: 'print',
                    color: 'accent',
                    translateValue: printBtnTitle,
                    isVisibleInViewMode: true
                });

                if (this.canCancelAuanRecords) {
                    if (auan.status === AuanStatusEnum.Canceled) {
                        leftButtons.push({
                            id: 'activate-auan',
                            color: 'accent',
                            translateValue: 'auan-register.activate',
                            isVisibleInViewMode: true
                        });
                    }
                    else {
                        leftButtons.push({
                            id: 'cancel-auan',
                            color: 'warn',
                            translateValue: 'auan-register.cancel',
                            isVisibleInViewMode: auan.status === AuanStatusEnum.Submitted
                        });
                    }
                }
            }

            const dialog = this.editDialog.openWithTwoButtons({
                title: title,
                TCtor: EditAuanComponent,
                headerAuditButton: auditBtn,
                headerCancelButton: {
                    cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
                },
                componentData: data,
                translteService: this.translate,
                disableDialogClose: true,
                viewMode: viewMode,
                rightSideActionsCollection: rightButtons,
                leftSideActionsCollection: leftButtons,
                saveBtn: {
                    id: 'save',
                    color: 'accent',
                    translateValue: this.translate.getValue('common.save')
                },
                cancelBtn: {
                    id: 'cancel',
                    color: 'primary',
                    translateValue: this.translate.getValue('common.cancel'),
                }
            }, '1400px');

            dialog.subscribe({
                next: (entry: AuanRegisterEditDTO | undefined) => {
                    if (entry !== undefined && entry !== null) {
                        this.grid.refreshData();
                    }
                }
            });
        }
        else {
            const title: string = this.translate.getValue('auan-register.choose-inspection-report-dialog-title');

            const dialog = this.inspectionPickerDialog.openWithTwoButtons({
                title: title,
                TCtor: EditAuanInspectionPickerComponent,
                headerAuditButton: undefined,
                headerCancelButton: {
                    cancelBtnClicked: this.closeChooseInspectionReportDialogBtnClicked.bind(this)
                },
                componentData: undefined,
                translteService: this.translate,
                disableDialogClose: true,
                viewMode: viewMode
            }, '600px');

            dialog.subscribe({
                next: (entry: AuanRegisterEditDTO | undefined) => {
                    if (entry !== undefined && entry !== null) {
                        this.grid.refreshData();
                    }
                }
            });
        }
    }

    public deleteAuan(auan: AuanRegisterDTO): void {
        this.confirmDialog.open({
            title: this.translate.getValue('auan-register.delete-auan-dialog-title'),
            message: this.translate.getValue('auan-register.delete-auan-dialog-message'),
            okBtnLabel: this.translate.getValue('auan-register.delete-auan-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok === true) {
                    this.service.deleteAuan(auan.id!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        },
                        error: (errorResponse: HttpErrorResponse) => {
                            if ((errorResponse.error as ErrorModel)?.code === ErrorCode.CannotDeleteAuanWithDecrees) {
                                const message: string = this.translate.getValue('auan-register.cannot-delete-auan-with-penal-decrees');
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

    public restoreAuan(auan: AuanRegisterDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok === true) {
                    this.service.undoDeleteAuan(auan.id!).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    public openDeliveryDialog(auan: AuanRegisterDTO): void {
        const data: InspDeliveryDataDialogParams = new InspDeliveryDataDialogParams({
            registerId: auan.id,
            id: auan.deliveryId,
            service: this.service,
            isAuan: true
        });

        let auditBtn: IHeaderAuditButton | undefined;

        if (auan.deliveryId !== undefined && auan.deliveryId !== null) {
            auditBtn = {
                id: auan.deliveryId,
                getAuditRecordData: this.service.getInspDeliverySimpleAudit.bind(this.service),
                tableName: 'RInsp.InspDelivery'
            };
        }

        const dialog = this.inspDeliveryDialog.openWithTwoButtons({
            title: this.translate.getValue('auan-register.delivery-dialog-title'),
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

    private buildForm(): void {
        this.form = new FormGroup({
            auanNumControl: new FormControl(),
            drafterControl: new FormControl(),
            territoryUnitControl: new FormControl(),
            draftDateRangeControl: new FormControl(),
            inspectionTypeControl: new FormControl(),
            locationDescriptionControl: new FormControl(),
            deliveryDateRangeControl: new FormControl(),
            applianceControl: new FormControl(),
            fishingGearControl: new FormControl(),
            fishControl: new FormControl(),
            identifierControl: new FormControl(),
            inspEntityFirstNameControl: new FormControl(),
            inspEntityMiddleNameControl: new FormControl(),
            inspEntityLastNameControl: new FormControl(),
            statusesControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): AuanRegisterFilters {
        const result = new AuanRegisterFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            auanNum: filters.getValue('auanNumControl'),
            territoryUnitId: filters.getValue('territoryUnitControl'),
            draftDateFrom: filters.getValue<DateRangeData>('draftDateRangeControl')?.start,
            draftDateTo: filters.getValue<DateRangeData>('draftDateRangeControl')?.end,
            deliveryDateFrom: filters.getValue<DateRangeData>('deliveryDateRangeControl')?.start,
            deliveryDateTo: filters.getValue<DateRangeData>('deliveryDateRangeControl')?.end,
            inspectionTypeId: filters.getValue('inspectionTypeControl'),
            locationDescription: filters.getValue('locationDescriptionControl'),
            applianceId: filters.getValue('applianceControl'),
            fishingGearId: filters.getValue('fishingGearControl'),
            fishId: filters.getValue('fishControl'),
            identifier: filters.getValue('identifierControl'),
            inspectedEntityFirstName: filters.getValue('inspEntityFirstNameControl'),
            inspectedEntityMiddleName: filters.getValue('inspEntityMiddleNameControl'),
            inspectedEntityLastName: filters.getValue('inspEntityLastNameControl')
        });

        const drafter: number | string | undefined = filters.getValue('drafterControl');
        if (drafter !== undefined) {
            if (typeof drafter === 'number') {
                result.drafterId = drafter;
            }
            else {
                result.drafterName = drafter;
            }
        }

        const statuses: AuanStatusEnum[] | undefined = filters.getValue('statusesControl');
        if (statuses !== undefined && statuses !== null) {
            result.auanStatuses = statuses.map((status: AuanStatusEnum) => {
                return AuanStatusEnum[status];
            });
        }

        return result;
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeChooseInspectionReportDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}