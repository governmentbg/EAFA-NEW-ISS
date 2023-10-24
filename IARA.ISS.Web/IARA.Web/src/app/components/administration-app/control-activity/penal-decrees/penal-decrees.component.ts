import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IPenalDecreesService } from '@app/interfaces/administration-app/penal-decrees.interface';
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
import { EditPenalDecreeComponent } from './edit-penal-decree/edit-penal-decree.component';
import { EditPenalDecreeAuanPickerComponent } from './edit-penal-decree-auan-picker/edit-penal-decree-auan-picker.component';
import { EditPenalDecreeDialogParams } from './models/edit-penal-decree-params.model';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { IAuanRegisterService } from '@app/interfaces/administration-app/auan-register.interface';
import { AuanRegisterService } from '@app/services/administration-app/auan-register.service';
import { EditDecreeWarningComponent } from './edit-decree-warning/edit-decree-warning.component';
import { EditDecreeAgreementComponent } from './edit-decree-agreement/edit-decree-agreement.component';
import { PenalDecreeTypeEnum } from '@app/enums/penal-decree-type.enum';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { RangeInputData } from '@app/shared/components/input-controls/tl-range-input/range-input.component';
import { EditDecreeResolutionComponent } from './edit-decree-resolution/edit-decree-resolution.component';
import { PenalDecreeStatusDTO } from '@app/models/generated/dtos/PenalDecreeStatusDTO';
import { EditPenalDecreeStatusDialogParams } from './models/edit-penal-decree-status-params.model';
import { EditPenalDecreeStatusComponent } from './edit-penal-decree-status/edit-penal-decree-status.component';

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

    public territoryUnits: NomenclatureDTO<number>[] = [];
    public statuses: NomenclatureDTO<number>[] = [];
    public drafters: NomenclatureDTO<number>[] = [];
    public sanctions: NomenclatureDTO<number>[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public fishingGears: NomenclatureDTO<number>[] = [];
    public appliances: NomenclatureDTO<number>[] = [];
    public deliveries: NomenclatureDTO<boolean>[] = [];

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;

    public readonly canReadStatusRecords: boolean;
    public readonly canAddStatusRecords: boolean;
    public readonly canEditStatusRecords: boolean;
    public readonly canDeleteStatusRecords: boolean;
    public readonly canRestoreStatusRecords: boolean;

    public readonly penalDecreeTypeEnum: typeof PenalDecreeTypeEnum = PenalDecreeTypeEnum;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<PenalDecreeDTO, PenalDecreesFilters>;

    private readonly service: IPenalDecreesService;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly penalDecreeDialog: TLMatDialog<EditPenalDecreeComponent>;
    private readonly agreementDialog: TLMatDialog<EditDecreeAgreementComponent>;
    private readonly warningDialog: TLMatDialog<EditDecreeWarningComponent>;
    private readonly resolutionDialog: TLMatDialog<EditDecreeResolutionComponent>;
    private readonly editStatusDialog: TLMatDialog<EditPenalDecreeStatusComponent>;
    private readonly auanPickerDialog: TLMatDialog<EditPenalDecreeAuanPickerComponent>;
    private readonly auanService: IAuanRegisterService;

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

        this.buildForm();

        this.deliveries = [
            new NomenclatureDTO<boolean>({
                value: true,
                displayName: this.translate.getValue('penal-decrees.delivered'),
                isActive: true
            }),
            new NomenclatureDTO<boolean>({
                value: false,
                displayName: this.translate.getValue('penal-decrees.not-delivered'),
                isActive: true
            })
        ];
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

        this.auanService.getAllDrafters().subscribe({
            next: (drafters: NomenclatureDTO<number>[]) => {
                this.drafters = drafters;
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

        this.grid.advancedFilters = new PenalDecreesFilters({
            auanId: this.auanId ?? undefined
        });

        if (this.auanId !== null || this.auanId !== undefined) {
            this.grid.refreshData();
        }
    }

    public addEditPenalDecree(decree: PenalDecreeDTO | undefined, viewMode: boolean): void {
        const rightButtons: IActionInfo[] = [];

        if (decree !== undefined && decree !== null) {
            const data: EditPenalDecreeDialogParams = new EditPenalDecreeDialogParams({
                id: decree.id,
                auanId: decree.auanId,
                typeId: decree.typeId,
                isReadonly: viewMode
            });

            const auditBtn: IHeaderAuditButton = {
                id: decree.id!,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'PenalDecreesRegister'
            };

            const printBtnTitle: string = viewMode
                ? this.translate.getValue('penal-decrees.print')
                : this.translate.getValue('penal-decrees.save-print');

            rightButtons.push({
                id: 'print',
                color: 'accent',
                translateValue: printBtnTitle,
                isVisibleInViewMode: true
            });

            this.openEditDialog(data, decree.decreeType!, viewMode, rightButtons, auditBtn);
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
                    tableName: 'PenalDecreeStatus'
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

    private closeEditStatusDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            penalDecreeNumControl: new FormControl(),
            territoryUnitControl: new FormControl(),
            draftDateRangeControl: new FormControl(),
            issueDateRangeControl: new FormControl(),
            drafterControl: new FormControl(),
            sanctionTypeControl: new FormControl(),
            statusTypeControl: new FormControl(),
            locationDescriptionControl: new FormControl(),
            fineAmountControl: new FormControl(),
            isDeliveredControl: new FormControl(),
            applianceControl: new FormControl(),
            fishingGearControl: new FormControl(),
            fishControl: new FormControl(),
            identifierControl: new FormControl(),
            inspEntityFirstNameControl: new FormControl(),
            inspEntityMiddleNameControl: new FormControl(),
            inspEntityLastNameControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): PenalDecreesFilters {
        const result = new PenalDecreesFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            penalDecreeNum: filters.getValue('penalDecreeNumControl'),
            territoryUnitId: filters.getValue('territoryUnitControl'),
            issueDateFrom: filters.getValue<DateRangeData>('issueDateRangeControl')?.start,
            issueDateTo: filters.getValue<DateRangeData>('issueDateRangeControl')?.end,
            drafterId: filters.getValue('drafterControl'),
            sanctionTypeIds: filters.getValue('sanctionTypeControl'),
            statusTypeIds: filters.getValue('statusTypeControl'),
            locationDescription: filters.getValue('locationDescriptionControl'),
            isDelivered: filters.getValue('isDeliveredControl'),
            applianceId: filters.getValue('applianceControl'),
            fishingGearId: filters.getValue('fishingGearControl'),
            fishId: filters.getValue('fishControl'),
            identifier: filters.getValue('identifierControl'),
            inspectedEntityFirstName: filters.getValue('inspEntityFirstNameControl'),
            inspectedEntityMiddleName: filters.getValue('inspEntityMiddleNameControl'),
            inspectedEntityLastName: filters.getValue('inspEntityLastNameControl')
        });

        const fineAmount: RangeInputData | undefined = filters.getValue('fineAmountControl');
        if (fineAmount !== undefined && fineAmount !== null) {
            result.fineAmountFrom = fineAmount.start;
            result.fineAmountTo = fineAmount.end;
        }

        return result;
    }

    private openEditDialog(
        data: EditPenalDecreeDialogParams,
        type: PenalDecreeTypeEnum,
        viewMode: boolean,
        rightButtons: IActionInfo[],
        auditBtn: IHeaderAuditButton | undefined) {

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
                rightSideActionsCollection: rightButtons
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
                rightSideActionsCollection: rightButtons
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
                rightSideActionsCollection: rightButtons
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
                rightSideActionsCollection: rightButtons
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