import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ShipQuotaDTO } from '@app/models/generated/dtos/ShipQuotaDTO';
import { ShipQuotaEditDTO } from '@app/models/generated/dtos/ShipQuotaEditDTO';
import { ShipQuotasFilters } from '@app/models/generated/filters/ShipQuotasFilters';
import { ShipQuotasService } from '@app/services/administration-app/ship-quotas.service';
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
import { BasePageComponent } from '@app/components/common-app/base-page.component';
import { EditShipQuotaComponent } from './edit-ship-quota.component';
import { TransferShipQuotaComponent } from './transfer-ship-quota.component';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { CommonUtils } from '@app/shared/utils/common.utils';

@Component({
    selector: 'ship-quotas-register',
    templateUrl: './ship-quotas-register.component.html',
})
export class ShipQuotasComponent extends BasePageComponent implements OnInit, AfterViewInit {
    public translationService: FuseTranslationLoaderService;
    public filterFormGroup!: FormGroup;
    public fishes: NomenclatureDTO<number>[] = [];
    public ships: ShipNomenclatureDTO[] = [];

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;
    public readonly canTransferRecords: boolean;

    private commonNomenclatureService: CommonNomenclatures;
    private confirmDialog: TLConfirmDialog;

    private service: ShipQuotasService;
    private gridManager!: DataTableManager<ShipQuotaDTO, ShipQuotasFilters>;
    private editDialog: TLMatDialog<EditShipQuotaComponent>;
    private transferDialog: TLMatDialog<TransferShipQuotaComponent>;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    public constructor(
        translationService: FuseTranslationLoaderService,
        service: ShipQuotasService,
        editDialog: TLMatDialog<EditShipQuotaComponent>,
        transferDialog: TLMatDialog<TransferShipQuotaComponent>,
        commonNomenclatureService: CommonNomenclatures,
        confirmDialog: TLConfirmDialog,
        permissions: PermissionsService,
        messageService: MessageService
    ) {
        super(messageService);

        this.canAddRecords = permissions.has(PermissionsEnum.ShipQuotasAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.ShipQuotasEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.ShipQuotasDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.ShipQuotasRestoreRecords);
        this.canTransferRecords = permissions.has(PermissionsEnum.ShipQuotasTransferRecords);

        this.translationService = translationService;
        this.service = service;
        this.commonNomenclatureService = commonNomenclatureService;
        this.editDialog = editDialog;
        this.transferDialog = transferDialog;
        this.confirmDialog = confirmDialog;

        this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Ships, this.commonNomenclatureService.getShips.bind(this.commonNomenclatureService), false
        ).subscribe({
            next: (ships: ShipNomenclatureDTO[]) => {
                this.ships = ships;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Fishes, this.commonNomenclatureService.getFishTypes.bind(this.commonNomenclatureService), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.fishes = result;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<ShipQuotaDTO, ShipQuotasFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAll.bind(this.service),
            filtersMapper: this.mapFilters.bind(this),
            excelRequestServiceMethod: this.service.downloadShipQuotaExcel.bind(this.service),
            excelFilename: this.translationService.getValue('catch-quotas.ship-excel-filename')
        });

        const tableId: number | undefined = window.history.state?.tableId;

        if (!CommonUtils.isNullOrEmpty(tableId)) {
            this.gridManager.advancedFilters = new ShipQuotasFilters({ shipQuotaId: tableId });
        }

        this.gridManager.refreshData();
    }

    public addEditEntry(entry?: ShipQuotaDTO, readOnly?: boolean): void {
        let data: DialogParamsModel | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;
        let headerTitle: string = '';

        const adding = entry == undefined || entry.id == undefined;

        if (!adding) {
            data = new DialogParamsModel({ id: entry!.id, isReadonly: readOnly });
            headerAuditBtn = {
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                id: entry!.id,
                tableName: 'ShipCatchQuota'
            } as IHeaderAuditButton;
            headerTitle = this.translationService.getValue('catch-quotas.edit-dialog');
        }
        else {
            headerTitle = this.translationService.getValue('catch-quotas.add-dialog');
        }

        const dialogResult = this.editDialog.openWithTwoButtons({
            title: headerTitle,
            TCtor: EditShipQuotaComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: data,
            viewMode: readOnly,
            translteService: this.translationService
        }, '85em');

        dialogResult.subscribe({
            next: (entry: ShipQuotaEditDTO) => {
                if (entry !== undefined && entry !== null) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    public transferQuota(entry?: ShipQuotaDTO): void {
        let headerTitle: string = '';
        const data: DialogParamsModel = new DialogParamsModel({ id: entry!.id, isReadonly: false });
        headerTitle = this.translationService.getValue('catch-quotas.transfer-quota-dialog');

        const dialogResult = this.transferDialog.openWithTwoButtons({
            title: headerTitle,
            TCtor: TransferShipQuotaComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translationService
        }, '85em');

        dialogResult.subscribe({
            next: (entry: ShipQuotaEditDTO[]) => {
                if (entry !== undefined && entry !== null) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    public deleteEntry(entry: ShipQuotaDTO): void {
        if (entry != undefined) {
            this.confirmDialog.open({
                title: this.translationService.getValue('catch-quotas.delete-entry'),
                message: `${this.translationService.getValue('catch-quotas.delete-entry-sure')} ${entry.shipName} ${entry.fish} ?`,
                okBtnLabel: this.translationService.getValue('qualified-fishers-page.do-delete')
            }).subscribe({
                next: (result: boolean) => {
                    if (result) {
                        if (entry.id != undefined) {
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

    public restoreEntry(entry: ShipQuotaDTO): void {
        if (entry.id != undefined) {
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

    private buildForm(): void {
        this.filterFormGroup = new FormGroup({
            shipFilterControl: new FormControl(),
            yearFilterControl: new FormControl(),
            fishesFilterControl: new FormControl(),
            assocFilterControl: new FormControl(),
            cfrFilterControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): ShipQuotasFilters {
        const result = new ShipQuotasFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,
            shipId: filters.getValue('shipFilterControl'),
            fishId: filters.getValue('fishesFilterControl'),
            association: filters.getValue('assocFilterControl'),
            cfr: filters.getValue('cfrFilterControl')
        });

        const yearValue = filters.getValue('yearFilterControl');
        if (yearValue !== null && yearValue !== undefined) {
            result.year = (filters.getValue('yearFilterControl') as Date).getFullYear();
        }

        return result;
    }
}