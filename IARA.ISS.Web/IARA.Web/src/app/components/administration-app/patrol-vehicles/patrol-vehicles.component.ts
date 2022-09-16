import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PatrolVehiclesDTO } from '@app/models/generated/dtos/PatrolVehiclesDTO';
import { PatrolVehiclesEditDTO } from '@app/models/generated/dtos/PatrolVehiclesEditDTO';
import { PatrolVehiclesFilters } from '@app/models/generated/filters/PatrolVehiclesFilters';
import { PatrolVehiclesService } from '@app/services/administration-app/patrol-vehicles.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
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
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { EditPatrolVehiclesComponent } from './edit-patrol-vehicles.component';

@Component({
    selector: 'patrol-vehicles',
    templateUrl: './patrol-vehicles.component.html'
})
export class PatrolVehiclesComponent implements AfterViewInit, OnInit {
    public translate: FuseTranslationLoaderService;
    public vehiclesFormGroup!: FormGroup;
    public addVehicleFormGroup!: FormGroup;
    public readOnly: boolean = false;
    public flagCountries: NomenclatureDTO<number>[] = [];
    public patrolVehicleTypes: NomenclatureDTO<number>[] = [];
    public vesselTypes: NomenclatureDTO<number>[] = [];
    public institutions: NomenclatureDTO<number>[] = [];

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;

    private service: PatrolVehiclesService;
    private commonNomenclaturesService!: CommonNomenclatures;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private gridManager!: DataTableManager<PatrolVehiclesDTO, PatrolVehiclesFilters>;
    private confirmDialog: TLConfirmDialog;
    private editDialog: TLMatDialog<EditPatrolVehiclesComponent>;

    public constructor(service: PatrolVehiclesService,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editDialog: TLMatDialog<EditPatrolVehiclesComponent>,
        commonNomenslaturesService: CommonNomenclatures,
        permissions: PermissionsService
    ) {
        this.service = service;
        this.translate = translate;
        this.commonNomenclaturesService = commonNomenslaturesService;
        this.confirmDialog = confirmDialog;
        this.editDialog = editDialog;

        this.canAddRecords = permissions.has(PermissionsEnum.PatrolVehiclesAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.PatrolVehiclesEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.PatrolVehiclesDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.PatrolVehiclesRestoreRecords);

        this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance
            .getNomenclature<number>(NomenclatureTypes.PatrolVehicleTypes, this.commonNomenclaturesService
                .getPatrolVehicleTypes.bind(this.commonNomenclaturesService), false)
            .subscribe((result: NomenclatureDTO<number>[]) => {
                this.patrolVehicleTypes = result;
            });

        NomenclatureStore.instance
            .getNomenclature<number>(NomenclatureTypes.VesselTypes, this.commonNomenclaturesService
                .getVesselTypes.bind(this.commonNomenclaturesService), false)
            .subscribe((result: NomenclatureDTO<number>[]) => {
                this.vesselTypes = result;
            });

        NomenclatureStore.instance
            .getNomenclature<number>(NomenclatureTypes.Countries, this.commonNomenclaturesService
                .getCountries.bind(this.commonNomenclaturesService), false)
            .subscribe((result: NomenclatureDTO<number>[]) => {
                this.flagCountries = result;
            });

        NomenclatureStore.instance
            .getNomenclature<number>(NomenclatureTypes.Institutions, this.commonNomenclaturesService
                .getInstitutions.bind(this.commonNomenclaturesService), false)
            .subscribe((result: NomenclatureDTO<number>[]) => {
                this.institutions = result;
            });
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<PatrolVehiclesDTO, PatrolVehiclesFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAll.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.gridManager.refreshData();
    }

    public deletePatrolVehicle(vehicle: PatrolVehiclesDTO): void {
        this.confirmDialog.open({
            title: this.translate.getValue('patrol-vehicles.delete-vehicle'),
            message: this.translate.getValue('patrol-vehicles.confirm-delete-message'),
            okBtnLabel: this.translate.getValue('patrol-vehicles.delete')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.deletePatrolVehicle(vehicle.id!).subscribe({
                        next: () => {
                            this.gridManager.deleteRecord(vehicle);
                        }
                    })
                }
            }
        });
    }

    public restorePatrolVehicle(vehicle: PatrolVehiclesDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.service.undoDeletePatrolVehicle(vehicle.id!).subscribe({
                        next: () => {
                            this.gridManager.undoDeleteRecord(vehicle);
                        }
                    })
                }
            }
        })
    }

    public createEditPatrolVehicle(vehicle?: PatrolVehiclesDTO, viewMode?: boolean): void {
        let data: DialogParamsModel | undefined = undefined;
        let auditButton: IHeaderAuditButton | undefined = undefined;
        let title: string;

        if (vehicle?.id !== undefined) {

            data = new DialogParamsModel({ id: vehicle.id, isReadonly: viewMode });

            auditButton = {
                id: vehicle.id,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'UnregisteredVessel'
            };

            title = viewMode
                ? this.translate.getValue('patrol-vehicles.view-patrol-vehicles-dialog-title')
                : this.translate.getValue('patrol-vehicles.edit-patrol-vehicles-dialog-title');

        }
        else {
            title = this.translate.getValue('patrol-vehicles.add-patrol-vehicles-dialog-title');
        }

        const dialog = this.editDialog.openWithTwoButtons({
            title: title,
            TCtor: EditPatrolVehiclesComponent,
            headerAuditButton: auditButton,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            viewMode: viewMode
        }, '1400px');

        dialog.subscribe((entry?: PatrolVehiclesEditDTO) => {
            if (entry !== undefined) {
                this.gridManager.refreshData();
            }
        });
    }

    private buildForm(): void {
        this.vehiclesFormGroup = new FormGroup({
            nameControl: new FormControl(),
            flagCountryIdControl: new FormControl(),
            externalMarkControl: new FormControl(),
            patrolVehicleTypeIdControl: new FormControl(),
            cfrControl: new FormControl(),
            uviControl: new FormControl(),
            ircsCallSignControl: new FormControl(),
            mmsiControl: new FormControl(),
            vesselTypeIdControl: new FormControl(),
            institutionIdControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs) {
        return new PatrolVehiclesFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            name: filters.getValue('nameControl'),
            flagCountryId: filters.getValue('flagCountryIdControl'),
            externalMark: filters.getValue('externalMarkControl'),
            patrolVehicleTypeId: filters.getValue('patrolVehicleTypeIdControl'),
            cfr: filters.getValue('cfrControl'),
            uvi: filters.getValue('uviControl'),
            ircsCallSign: filters.getValue('ircsCallSignControl'),
            mmsi: filters.getValue('mmsiControl'),
            vesselTypeId: filters.getValue('vesselTypeIdControl'),
            institutionId: filters.getValue('institutionIdControl')
        });
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}



