import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { YearlyQuotaDTO } from '@app/models/generated/dtos/YearlyQuotaDTO';
import { YearlyQuotaEditDTO } from '@app/models/generated/dtos/YearlyQuotaEditDTO';
import { YearlyQuotasFilters } from '@app/models/generated/filters/YearlyQuotasFilters';
import { YearlyQuotasService } from '@app/services/administration-app/yearly-quotas.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { MessageService } from '@app/shared/services/message.service';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { EditYearlyQuotaComponent } from './edit-yearly-quota.component';
import { TransferYearlyQuotaComponent } from './transfer-yearly-quota.component';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { BasePageComponent } from '@app/components/common-app/base-page.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';

@Component({
    selector: 'yearly-quotas-register',
    templateUrl: './yearly-quotas-register.component.html',
})
export class YearlyQuotasComponent extends BasePageComponent implements OnInit, AfterViewInit {
    public translationService: FuseTranslationLoaderService;
    public filterFormGroup: FormGroup;
    public fishes: FishNomenclatureDTO[] = [];

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;
    public readonly canTransferRecords: boolean;

    private commonNomenclatureService: CommonNomenclatures;
    private confirmDialog: TLConfirmDialog;

    private service: YearlyQuotasService;
    private gridManager!: DataTableManager<YearlyQuotaDTO, YearlyQuotasFilters>;
    private editDialog: TLMatDialog<EditYearlyQuotaComponent>;
    private transferDialog: TLMatDialog<TransferYearlyQuotaComponent>;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    public constructor(
        translationService: FuseTranslationLoaderService,
        service: YearlyQuotasService,
        editDialog: TLMatDialog<EditYearlyQuotaComponent>,
        transferDialog: TLMatDialog<TransferYearlyQuotaComponent>,
        commonNomenclatureService: CommonNomenclatures,
        confirmDialog: TLConfirmDialog,
        permissions: PermissionsService,
        messageService: MessageService
    ) {
        super(messageService);

        this.canAddRecords = permissions.has(PermissionsEnum.YearlyQuotasAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.YearlyQuotasEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.YearlyQuotasDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.YearlyQuotasRestoreRecords);
        this.canTransferRecords = permissions.has(PermissionsEnum.YearlyQuotasTransferRecords);

        this.translationService = translationService;
        this.service = service;
        this.commonNomenclatureService = commonNomenclatureService;
        this.editDialog = editDialog;
        this.transferDialog = transferDialog;
        this.confirmDialog = confirmDialog;

        this.filterFormGroup = new FormGroup({
            yearFilterControl: new FormControl(),
            fishesFilterControl: new FormControl()
        });
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.TerritoryUnits, this.commonNomenclatureService.getTerritoryUnits.bind(this.commonNomenclatureService), false
        ).subscribe({
            next: (result: FishNomenclatureDTO[]) => {
                this.fishes = result;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<YearlyQuotaDTO, YearlyQuotasFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAll.bind(this.service),
            filtersMapper: this.mapFilters.bind(this),
            excelRequestServiceMethod: this.service.downloadYearlyQuotaExcel.bind(this.service),
            excelFilename: this.translationService.getValue('catch-quotas.yearly-excel-filename')
        });

        this.gridManager.refreshData();
    }

    public addEditEntry(entry?: YearlyQuotaDTO, readOnly?: boolean): void {
        let data: DialogParamsModel | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;
        let headerTitle: string = '';

        const adding = entry === undefined || entry.id === undefined;

        if (!adding) {
            data = new DialogParamsModel({ id: entry!.id, isReadonly: readOnly });
            headerAuditBtn = {
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                id: entry!.id,
                tableName: 'RQuo.CatchQuotas'
            } as IHeaderAuditButton;
            headerTitle = this.translationService.getValue('catch-quotas.edit-dialog');
        }
        else {
            headerTitle = this.translationService.getValue('catch-quotas.add-dialog');
        }

        const dialogResult = this.editDialog.openWithTwoButtons({
            title: headerTitle,
            TCtor: EditYearlyQuotaComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: data,
            viewMode: readOnly,
            translteService: this.translationService
        }, '85em');

        dialogResult.subscribe({
            next: (entry: YearlyQuotaEditDTO) => {
                if (entry !== undefined && entry !== null) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    public deleteEntry(entry: YearlyQuotaDTO): void {
        if (entry !== undefined) {
            this.confirmDialog.open({
                title: this.translationService.getValue('catch-quotas.delete-entry'),
                message: `${this.translationService.getValue('catch-quotas.delete-entry-sure')} ${entry.fish} ${entry.year} ?`,
                okBtnLabel: this.translationService.getValue('qualified-fishers-page.do-delete')
            }).subscribe({
                next: (result: boolean) => {
                    if (result) {
                        if (entry.id !== undefined) {
                            this.service.delete(entry.id).subscribe({
                                next: () => {
                                    this.gridManager.deleteRecord(entry);
                                }
                            });
                        }
                    }
                }
            });
        }
    }

    public restoreEntry(entry: YearlyQuotaDTO): void {
        if (entry.id !== undefined) {
            this.service.undoDelete(entry.id).subscribe({
                next: () => {
                    this.gridManager.undoDeleteRecord(entry);
                }
            });
        }
    }

    public closeDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }


    public transferQuota(entry?: YearlyQuotaDTO): void {
        let headerTitle: string = '';
        const data: DialogParamsModel = new DialogParamsModel({ id: entry!.id, isReadonly: false });
        headerTitle = this.translationService.getValue('catch-quotas.transfer-quota-dialog');

        const dialogResult = this.transferDialog.openWithTwoButtons({
            title: headerTitle,
            TCtor: TransferYearlyQuotaComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translationService
        }, '85em');

        dialogResult.subscribe({
            next: (entry: YearlyQuotaEditDTO) => {
                if (entry !== undefined && entry !== null) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    private mapFilters(filters: FilterEventArgs): YearlyQuotasFilters {
        const result = new YearlyQuotasFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,
            fishId: filters.getValue('fishesFilterControl')
        });

        const yearValue = filters.getValue('yearFilterControl');
        if (yearValue !== null && yearValue !== undefined) {
            result.year = (filters.getValue('yearFilterControl') as Date).getFullYear();
        }

        return result;
    }
}