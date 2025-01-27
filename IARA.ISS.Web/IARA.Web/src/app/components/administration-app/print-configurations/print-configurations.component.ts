import { FormControl, FormGroup } from '@angular/forms';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PrintConfigurationDTO } from '@app/models/generated/dtos/PrintConfigurationDTO';
import { PrintConfigurationFilters } from '@app/models/generated/filters/PrintConfigurationFilters';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { PrintConfigurationsService } from '@app/services/administration-app/print-configurations.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditPrintConfigurationComponent } from './components/edit-print-configuration/edit-print-configuration.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { PrintConfigurationEditDTO } from '@app/models/generated/dtos/PrintConfigurationEditDTO';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';

@Component({
    selector: 'print-configurations',
    templateUrl: './print-configurations.component.html'
})
export class PrintConfigurationsComponent implements OnInit, AfterViewInit {
    public readonly translationService: FuseTranslationLoaderService;

    public formGroup!: FormGroup;
    public canAddRecords: boolean = false;
    public canEditRecords: boolean = false;
    public canDeleteRecords: boolean = false;
    public canRestoreRecords: boolean = false;

    public applicationTypes: NomenclatureDTO<number>[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    @ViewChild(TLDataTableComponent)
    private datatable!: IRemoteTLDatatableComponent;

    private gridManager!: DataTableManager<PrintConfigurationDTO, PrintConfigurationFilters>;
    private readonly service: PrintConfigurationsService;
    private readonly nomenclaturesService: CommonNomenclatures;
    private readonly editDialog: TLMatDialog<EditPrintConfigurationComponent>;
    private readonly confirmationDialog: TLConfirmDialog;

    public constructor(
        translateService: FuseTranslationLoaderService,
        service: PrintConfigurationsService,
        permissions: PermissionsService,
        nomenclaturesService: CommonNomenclatures,
        editDialog: TLMatDialog<EditPrintConfigurationComponent>,
        confirmationDialog: TLConfirmDialog
    ) {
        this.translationService = translateService;
        this.service = service;
        this.nomenclaturesService = nomenclaturesService;
        this.editDialog = editDialog;
        this.confirmationDialog = confirmationDialog;
        
        this.canAddRecords = permissions.has(PermissionsEnum.PrintConfigurationsAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.PrintConfigurationsEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.PrintConfigurationsDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.PrintConfigurationsRestoreRecords);

        this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.TerritoryUnits, this.nomenclaturesService.getTerritoryUnits.bind(this.nomenclaturesService), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.territoryUnits = result;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.ApplicationTypes, this.service.getApplicationTypes.bind(this.service), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.applicationTypes = result;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<PrintConfigurationDTO, PrintConfigurationFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllPrintConfigurations.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.gridManager.refreshData();
    }

    public createPrintConfiguration(): void {
        const title: string = this.translationService.getValue('print-configurations.add-print-configuration-title');
        const data: DialogParamsModel = new DialogParamsModel();

        this.openEditDialog(title, data, undefined, false);
    }

    public editPrintConfiguration(id: number, viewMode: boolean = false): void {
        let title: string = '';

        if (viewMode) {
            title = this.translationService.getValue('print-configurations.view-print-configuration-title');
        }
        else {
            title = this.translationService.getValue('print-configurations.edit-print-configuration-title');
        }

        const data: DialogParamsModel = new DialogParamsModel({ id: id, viewMode: viewMode });
        const headerAuditBtn: IHeaderAuditButton = {
            getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
            id: id,
            tableName: 'Appl.ApplicationPrintSignUsers'
        };

        this.openEditDialog(title, data, headerAuditBtn, viewMode);
    }

    public deletePrintConfiguration(row: PrintConfigurationDTO): void {
        const message: string = this.translationService.getValue('print-configurations.delete-print-configuration-dialog-message');
        this.confirmationDialog.open({
            title: this.translationService.getValue('print-configurations.delete-print-configuration-title'),
            message: `${message}: ${row.applicationTypeName}`,
            okBtnLabel: this.translationService.getValue('print-configurations.delete-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.deletePrintConfiguration(row.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    public restorePrintConfiguration(row: PrintConfigurationDTO): void {
        this.confirmationDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.undoDeletePrintConfiguration(row.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    private openEditDialog(title: string, componentData: DialogParamsModel, auditButton: IHeaderAuditButton | undefined, viewMode: boolean = false): void {
        const dialogRef = this.editDialog.openWithTwoButtons({
            TCtor: EditPrintConfigurationComponent,
            title: title,
            translteService: this.translationService,
            componentData: componentData,
            headerAuditButton: auditButton,
            headerCancelButton: { cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); } },
            viewMode: viewMode
        }, '1500px');

        dialogRef.subscribe({
            next: (result: PrintConfigurationEditDTO | undefined) => {
                if (result !== null && result !== undefined) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    private buildForm(): void {
        this.formGroup = new FormGroup({
            territoryUnitControl: new FormControl(),
            userEgnLnchControl: new FormControl(),
            userNamesControl: new FormControl(),
            applicationTypeControl: new FormControl(),
            substituteReasonControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): PrintConfigurationFilters {
        return new PrintConfigurationFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            territoryUnitIds: filters.getValue('territoryUnitControl'),
            userEgnLnch: filters.getValue('userEgnLnchControl'),
            userNames: filters.getValue('userNamesControl'),
            applicationTypeIds: filters.getValue('applicationTypeControl'),
            substituteReason: filters.getValue('substituteReasonControl')
        });
    }
}