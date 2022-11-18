import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IAuanRegisterService } from '@app/interfaces/administration-app/auan-register.interface';
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
import { EditAuanComponent } from './edit-auan/edit-auan.component';
import { EditAuanDialogParams } from './models/edit-auan-dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { EditAuanInspectionPickerComponent } from './edit-auan-inspection-picker/edit-auan-inspection-picker.component';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';

@Component({
    selector: 'auan-register',
    templateUrl: './auan-register.component.html'
})
export class AuanRegisterComponent implements OnInit, AfterViewInit {
    @Input()
    public inspectionId: number | undefined;

    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public drafters: NomenclatureDTO<number>[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];
    public inspectionTypes: NomenclatureDTO<number>[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public fishingGears: NomenclatureDTO<number>[] = [];
    public appliances: NomenclatureDTO<number>[] = [];
    public deliveries: NomenclatureDTO<boolean>[] = [];

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<AuanRegisterDTO, AuanRegisterFilters>;

    private readonly service: IAuanRegisterService;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly editDialog: TLMatDialog<EditAuanComponent>;
    private readonly inspectionPickerDialog: TLMatDialog<EditAuanInspectionPickerComponent>;

    public constructor(
        service: AuanRegisterService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editDialog: TLMatDialog<EditAuanComponent>,
        inspectionPickerDialog: TLMatDialog<EditAuanInspectionPickerComponent>,
        permissions: PermissionsService
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editDialog = editDialog;
        this.inspectionPickerDialog = inspectionPickerDialog;

        this.canAddRecords = permissions.has(PermissionsEnum.AuanRegisterAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.AuanRegisterEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.AuanRegisterDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.AuanRegisterRestoreRecords);

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
            next: (drafters: NomenclatureDTO<number>[]) => {
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

        const isPerson: boolean | undefined = window.history.state?.isPerson;
        let legalId: number | undefined;
        let personId: number | undefined;
        if (isPerson === true) {
            personId = window.history.state?.id;
        }

        if (isPerson === false) {
            legalId = window.history.state?.id;
        }

        this.grid.advancedFilters = new AuanRegisterFilters({
            inspectionId: this.inspectionId ?? undefined,
            personId: personId ?? undefined,
            legalId: legalId ?? undefined
        });

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
                tableName: 'Auanregister'
            };

            const title: string = viewMode
                ? this.translate.getValue('auan-register.view-auan-dialog-title')
                : this.translate.getValue('auan-register.edit-auan-dialog-title');

            const printBtnTitle: string = viewMode
                ? this.translate.getValue('auan-register.print')
                : this.translate.getValue('auan-register.save-print');

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

    private buildForm(): void {
        this.form = new FormGroup({
            auanNumControl: new FormControl(),
            drafterControl: new FormControl(),
            territoryUnitControl: new FormControl(),
            draftDateRangeControl: new FormControl(),
            inspectionTypeControl: new FormControl(),
            locationDescriptionControl: new FormControl(),
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

    private mapFilters(filters: FilterEventArgs): AuanRegisterFilters {
        const result = new AuanRegisterFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            auanNum: filters.getValue('auanNumControl'),
            territoryUnitId: filters.getValue('territoryUnitControl'),
            draftDateFrom: filters.getValue<DateRangeData>('draftDateRangeControl')?.start,
            draftDateTo: filters.getValue<DateRangeData>('draftDateRangeControl')?.end,
            inspectionTypeId: filters.getValue('inspectionTypeControl'),
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

        const drafter: number | string | undefined = filters.getValue('drafterControl');
        if (drafter !== undefined) {
            if (typeof drafter === 'number') {
                result.drafterId = drafter;
            }
            else {
                result.drafterName = drafter;
            }
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