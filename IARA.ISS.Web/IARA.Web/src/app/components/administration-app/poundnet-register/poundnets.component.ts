import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PoundNetStatusesEnum } from '@app/enums/pound-net-statuses.enum';
import { IPoundnetRegisterService } from '@app/interfaces/administration-app/poundnet-register.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PoundNetDTO } from '@app/models/generated/dtos/PoundNetDTO';
import { PoundNetRegisterFilters } from '@app/models/generated/filters/PoundNetRegisterFilters';
import { PoundnetRegisterService } from '@app/services/administration-app/poundnet-register.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { EditPoundnetComponent } from './edit-poundnet.component';


@Component({
    selector: 'poundnets',
    templateUrl: './poundnets.component.html'
})
export class PoundnetsComponent implements OnInit, AfterViewInit {
    public form!: FormGroup;
    public translate: FuseTranslationLoaderService;

    public muncipalities: NomenclatureDTO<number>[] = [];
    public seasonalTypes: NomenclatureDTO<number>[] = [];
    public categoryTypes: NomenclatureDTO<number>[] = [];
    public statuses: NomenclatureDTO<number>[] = [];
    public readonly poundnetStatusEnum: typeof PoundNetStatusesEnum = PoundNetStatusesEnum;

    public canAddRecords: boolean = false;
    public canEditRecords: boolean = false;
    public canDeleteRecords: boolean = false;
    public canRestoreRecords: boolean = false;

    @ViewChild(TLDataTableComponent)
    private datatable!: IRemoteTLDatatableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private gridManager!: DataTableManager<PoundNetDTO, PoundNetRegisterFilters>;
    private service: IPoundnetRegisterService;
    private nomenclatures: CommonNomenclatures;
    private confirmDialog: TLConfirmDialog;
    private editDialog: TLMatDialog<EditPoundnetComponent>;

    public constructor(
        service: PoundnetRegisterService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        confirmDialog: TLConfirmDialog,
        editDialog: TLMatDialog<EditPoundnetComponent>,
        permissions: PermissionsService
    ) {
        this.service = service;
        this.translate = translate;
        this.nomenclatures = nomenclatures;
        this.confirmDialog = confirmDialog;
        this.editDialog = editDialog;

        this.canAddRecords = permissions.has(PermissionsEnum.PoundnetsAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.PoundnetsEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.PoundnetsDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.PoundnetsRestoreRecords);

        this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Municipalities, this.nomenclatures.getMunicipalities.bind(this.nomenclatures), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.muncipalities = result;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.PoundnetCategoryTypes, this.service.getCategories.bind(this.service), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.categoryTypes = result;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.PoundnetSeasonalTypes, this.service.getSeasonalTypes.bind(this.service), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.seasonalTypes = result;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.PoundnetStatuses, this.service.getPoundnetStatuses.bind(this.service), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.statuses = result;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<PoundNetDTO, PoundNetRegisterFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAll.bind(this.service),
            filtersMapper: this.mapFilters
        });

        this.gridManager.refreshData();
    }

    public deletePoundnet(poundnet: PoundNetDTO): void {
        this.confirmDialog.open({
            title: this.translate.getValue('poundnet-page.delete-poundnet'),
            message: `${this.translate.getValue('poundnet-page.delete-poundnet-confirmation')} „${poundnet.name}“?`,
            okBtnLabel: this.translate.getValue('poundnet-page.delete')
        }).subscribe({
            next: (result: boolean) => {
                if (result === true) {
                    this.service.delete(poundnet.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    public restorePoundnet(poundnet: PoundNetDTO): void {
        this.confirmDialog.open({
            message: `${this.translate.getValue('common.restore-confirmation')} ${poundnet.name}?`
        }).subscribe({
            next: (result: boolean) => {
                if (result) {
                    this.service.undoDelete(poundnet.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    public createEditPoundnet(poundnet: PoundNetDTO | undefined, viewMode: boolean = false): void {
        let data: DialogParamsModel | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;
        let headerTitle: string = '';

        if (poundnet?.id !== undefined && poundnet?.id !== null) {
            data = new DialogParamsModel({
                id: poundnet.id,
                isReadonly: viewMode
            });

            headerAuditBtn = {
                id: poundnet.id,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'PoundnetRegister'
            };

            headerTitle = this.translate.getValue('poundnet-page.edit-poundnet-dialog-title');
        }
        else {
            headerTitle = this.translate.getValue('poundnet-page.add-poundnet-dialog-title');
        }

        const dialogResult = this.editDialog.openWithTwoButtons({
            title: headerTitle,
            TCtor: EditPoundnetComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            viewMode: viewMode
        });

        dialogResult.subscribe({
            next: (result: PoundNetDTO | undefined) => {
                if (result !== undefined && result !== null) {
                    this.gridManager.refreshData();
                }
            }
        });
    }

    private buildForm(): void {
        this.form = new FormGroup({
            numberControl: new FormControl(),
            nameControl: new FormControl(),
            municipalityControl: new FormControl(),
            seasonalControl: new FormControl(),
            categoryControl: new FormControl(),
            dateRangeControl: new FormControl(),
            statusControl: new FormControl()
        });
    }

    private closeDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private mapFilters(filters: FilterEventArgs): PoundNetRegisterFilters {
        const filtersObj = new PoundNetRegisterFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            number: filters.getValue('numberControl'),
            name: filters.getValue('nameControl'),
            muncipalityId: filters.getValue('municipalityControl'),
            categoryTypeId: filters.getValue('categoryControl'),
            seasonTypeId: filters.getValue('seasonalControl'),
            registeredDateFrom: filters.getValue<DateRangeData>('dateRangeControl')?.start,
            registeredDateTo: filters.getValue<DateRangeData>('dateRangeControl')?.end,
            statusId: filters.getValue('statusControl')
        });

        return filtersObj;
    }
}