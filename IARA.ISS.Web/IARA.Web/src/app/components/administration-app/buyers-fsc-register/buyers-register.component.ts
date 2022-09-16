import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { BuyerDTO } from '@app/models/generated/dtos/BuyerDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { BuyersFilters } from '@app/models/generated/filters/BuyersFilters';
import { BuyersAdministrationService } from '@app/services/administration-app/buyers-administration.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { EditBuyersComponent } from '@app/components/common-app/buyers/edit-buyers.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { ChooseApplicationComponent } from '@app/components/common-app/applications/components/choose-application/choose-application.component';
import { ChooseApplicationDialogParams } from '@app/components/common-app/applications/components/choose-application/models/choose-application-dialog-params.model';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ApplicationForChoiceDTO } from '@app/models/generated/dtos/ApplicationForChoiceDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { BuyerStatusesEnum } from '@app/enums/buyer-statuses.enum';

@Component({
    selector: 'buyers-register',
    templateUrl: './buyers-register.component.html'
})
export class BuyersComponent implements OnInit, AfterViewInit {
    public translationService: FuseTranslationLoaderService;
    public filterFormGroup: FormGroup;

    public entryTypes: NomenclatureDTO<number>[] = [];
    public populatedAreas: NomenclatureDTO<number>[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];
    public districts: NomenclatureDTO<number>[] = [];
    public buyerStatuses: NomenclatureDTO<number>[] = [];

    public readonly canAddRecords: boolean;
    public readonly canEditRecords: boolean;
    public readonly canDeleteRecords: boolean;
    public readonly canRestoreRecords: boolean;

    public readonly hasReadAllPermission: boolean;

    private editDialog: TLMatDialog<EditBuyersComponent>;
    private confirmDialog: TLConfirmDialog;
    private chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>;
    private service: BuyersAdministrationService;
    private permissions: PermissionsService;
    private commonNomenclatureService: CommonNomenclatures;

    private gridManager!: DataTableManager<BuyerDTO, BuyersFilters>;

    @ViewChild(TLDataTableComponent)
    private datatable!: IRemoteTLDatatableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    public constructor(
        service: BuyersAdministrationService,
        editDialog: TLMatDialog<EditBuyersComponent>,
        confirmDialog: TLConfirmDialog,
        permissions: PermissionsService,
        translationService: FuseTranslationLoaderService,
        commonNomenclatureService: CommonNomenclatures,
        chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>
    ) {
        this.service = service;
        this.editDialog = editDialog;
        this.confirmDialog = confirmDialog;
        this.permissions = permissions;
        this.translationService = translationService;
        this.commonNomenclatureService = commonNomenclatureService;
        this.chooseApplicationDialog = chooseApplicationDialog;

        this.canAddRecords = permissions.has(PermissionsEnum.BuyersAddRecords);
        this.canEditRecords = permissions.has(PermissionsEnum.BuyersEditRecords);
        this.canDeleteRecords = permissions.has(PermissionsEnum.BuyersDeleteRecords);
        this.canRestoreRecords = permissions.has(PermissionsEnum.BuyersRestoreRecords);
        this.hasReadAllPermission = permissions.has(PermissionsEnum.BuyersApplicationsReadAll);

        this.filterFormGroup = new FormGroup({
            entryTypesControl: new FormControl(),
            urorrNumberControl: new FormControl(),
            registrationNumberControl: new FormControl(),
            territoryNodeControl: new FormControl(),
            dateRangeControl: new FormControl(),
            ownerNameControl: new FormControl(),
            ownerEikControl: new FormControl(),
            organizerNameControl: new FormControl(),
            organizerEgnControl: new FormControl(),
            populatedAreaControl: new FormControl(),
            districtControl: new FormControl(),
            logNumberControl: new FormControl(),
            utilityNameControl: new FormControl(),
            territoryUnitControl: new FormControl(),
            buyerSatusesControl: new FormControl()
        });
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.BuyerTypes, this.service.getBuyerTypes.bind(this.service), false
        ).subscribe({
            next: (results: NomenclatureDTO<number>[]) => {
                this.entryTypes = results;
            }
        });

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.PopulatedAreas, this.commonNomenclatureService.getPopulatedAreas.bind(this.commonNomenclatureService), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.populatedAreas = result;
            }
        });

        NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.TerritoryUnits, this.commonNomenclatureService.getTerritoryUnits.bind(this.commonNomenclatureService), false
        ).subscribe({
            next: (results: NomenclatureDTO<number>[]) => {
                this.territoryUnits = results;
            }
        });

        NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.Districts, this.commonNomenclatureService.getDistricts.bind(this.commonNomenclatureService), false
        ).subscribe({
            next: (results: NomenclatureDTO<number>[]) => {
                this.districts = results;
            }
        });

        NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.BuyerStatuses, this.service.getBuyerStatuses.bind(this.service), false
        ).subscribe({
            next: (results: NomenclatureDTO<number>[]) => {
                this.buyerStatuses = results;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.gridManager = new DataTableManager<BuyerDTO, BuyersFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAll.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        const isPerson: boolean | undefined = window.history.state?.isPerson;
        const id: number | undefined = window.history.state?.id;

        if (!CommonUtils.isNullOrEmpty(id)) {
            if (isPerson) {
                this.gridManager.advancedFilters = new BuyersFilters({ personId: id });
            }
            else {
                this.gridManager.advancedFilters = new BuyersFilters({ legalId: id });
            }
        }

        this.gridManager.refreshData();
    }

    public addEntry(): void {
        let data: DialogParamsModel | undefined;
        let headerTitle: string = '';
        const rightButtons: IActionInfo[] = [];

        this.chooseApplicationDialog.open({
            TCtor: ChooseApplicationComponent,
            title: this.translationService.getValue('applications-register.choose-application-for-register-creation'),
            translteService: this.translationService,
            componentData: new ChooseApplicationDialogParams({
                pageCodes: [
                    PageCodeEnum.ChangeFirstSaleBuyer,
                    PageCodeEnum.RegFirstSaleBuyer,
                    PageCodeEnum.TermFirstSaleBuyer,
                    PageCodeEnum.RegFirstSaleCenter,
                    PageCodeEnum.ChangeFirstSaleCenter,
                    PageCodeEnum.TermFirstSaleCenter
                ]
            }),
            disableDialogClose: true,
            headerCancelButton: {
                cancelBtnClicked: this.closeApplicationChooseDialogBtnClicked.bind(this)
            },
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translationService.getValue('applications-register.choose')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translationService.getValue('common.cancel'),
            }
        }).subscribe((dialogData: { selectedApplication: ApplicationForChoiceDTO }) => {
            if (dialogData !== null && dialogData !== undefined) {
                data = new DialogParamsModel({
                    id: undefined,
                    isApplication: false,
                    isReadonly: false,
                    service: this.service,
                    pageCode: dialogData.selectedApplication.pageCode,
                    isThirdCountry: false,
                    isApplicationHistoryMode: false,
                    showOnlyRegiXData: false,
                    viewMode: false,
                    applicationId: dialogData.selectedApplication.id
                });

                switch (dialogData.selectedApplication.pageCode) {
                    case PageCodeEnum.ChangeFirstSaleBuyer:
                        headerTitle = this.translationService.getValue('buyers-and-sales-centers.add-change-buyer-dialog-title');
                        break;
                    case PageCodeEnum.RegFirstSaleBuyer:
                        headerTitle = this.translationService.getValue('buyers-and-sales-centers.add-buyer-dialog-title');
                        break;
                    case PageCodeEnum.TermFirstSaleBuyer:
                        headerTitle = this.translationService.getValue('buyers-and-sales-centers.add-terminate-buyer-dialog-title');
                        break;
                    case PageCodeEnum.RegFirstSaleCenter:
                        headerTitle = this.translationService.getValue('buyers-and-sales-centers.add-first-sale-center-dialog-title');
                        break;
                    case PageCodeEnum.ChangeFirstSaleCenter:
                        headerTitle = this.translationService.getValue('buyers-and-sales-centers.add-change-first-sale-center-dialog-title');
                        break;
                    case PageCodeEnum.TermFirstSaleCenter:
                        headerTitle = this.translationService.getValue('buyers-and-sales-centers.add-terminate-first-sale-center-dialog-title');
                        break;
                }

                rightButtons.push({
                    id: 'print',
                    color: 'accent',
                    translateValue: this.translationService.getValue('buyers-and-sales-centers.save-print')
                });

                this.openEditDialog(data, headerTitle, undefined, rightButtons, undefined);
            }
        });
    }

    public editEntry(entry: BuyerDTO, viewMode: boolean = false): void {
        const data: DialogParamsModel = new DialogParamsModel({
            id: entry.id,
            isApplication: false,
            isReadonly: false,
            service: this.service,
            pageCode: entry.pageCode,
            applicationId: entry.applicationId,
            isThirdCountry: false,
            isApplicationHistoryMode: false,
            showOnlyRegiXData: false,
            viewMode: viewMode || entry.status === BuyerStatusesEnum.Canceled
        });

        const headerAuditBtn: IHeaderAuditButton = {
            id: entry.id!,
            tableName: 'BuyerRegister',
            tooltip: '',
            getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
        };

        let headerTitle: string = '';

        if (viewMode === true) {
            switch (entry.pageCode) {
                case PageCodeEnum.RegFirstSaleBuyer:
                    headerTitle = this.translationService.getValue('buyers-and-sales-centers.view-buyer-dialog-title');
                    break;
                case PageCodeEnum.RegFirstSaleCenter:
                    headerTitle = this.translationService.getValue('buyers-and-sales-centers.view-first-sale-center-dialog-title');
                    break;
            }
        }
        else {
            switch (entry.pageCode) {
                case PageCodeEnum.RegFirstSaleBuyer:
                    headerTitle = this.translationService.getValue('buyers-and-sales-centers.edit-buyer-dialog-title');
                    break;
                case PageCodeEnum.RegFirstSaleCenter:
                    headerTitle = this.translationService.getValue('buyers-and-sales-centers.edit-first-sale-dialog-title');
                    break;

            }
        }

        const rightButtons: IActionInfo[] = [];

        if (!viewMode) {
            if (entry.status === BuyerStatusesEnum.Canceled) {
                rightButtons.push({
                    id: 'activate',
                    color: 'accent',
                    translateValue: 'buyers-and-sales-centers.activate',
                    isVisibleInViewMode: true
                });
            }
            else {
                rightButtons.push({
                    id: 'cancel',
                    color: 'warn',
                    translateValue: 'buyers-and-sales-centers.cancel'
                });
            }
        }

        rightButtons.push({
            id: 'print',
            color: 'accent',
            translateValue: viewMode || entry.status === BuyerStatusesEnum.Canceled
                ? 'buyers-and-sales-centers.print'
                : 'buyers-and-sales-centers.save-print',
            isVisibleInViewMode: true
        });

        this.openEditDialog(data, headerTitle, headerAuditBtn, rightButtons, viewMode || entry.status === BuyerStatusesEnum.Canceled);
    }

    private openEditDialog(data: DialogParamsModel, headerTitle: string, headerAuditBtn: IHeaderAuditButton | undefined, rightButtons: IActionInfo[], viewMode: boolean = false): void {
        this.editDialog.open({
            title: headerTitle,
            TCtor: EditBuyersComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            disableDialogClose: true,
            viewMode: viewMode,
            componentData: data,
            translteService: this.translationService,
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translationService.getValue('common.save')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translationService.getValue('common.cancel'),
            },
            rightSideActionsCollection: rightButtons
        }, '90em').subscribe((result: BuyerDTO | undefined) => {
            if (result !== undefined) {
                this.gridManager.refreshData();
            }
        });
    }

    public deleteEntry(entry: BuyerDTO): void {
        this.confirmDialog.open({
            title: this.translationService.getValue('buyers-and-sales-centers.delete-buyer'),
            message: this.translationService.getValue('buyers-and-sales-centers.confirm-delete-message'),
            okBtnLabel: this.translationService.getValue('buyers-and-sales-centers.delete')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok && entry?.id) {
                    this.service.delete(entry.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    public restoreEntry(entry: BuyerDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok && entry?.id) {
                    this.service.restore(entry.id!).subscribe({
                        next: () => {
                            this.gridManager.refreshData();
                        }
                    });
                }
            }
        });
    }

    private closeDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeApplicationChooseDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private mapFilters(filters: FilterEventArgs): BuyersFilters {
        return new BuyersFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            entryTypeId: filters.getValue('entryTypesControl'),
            urorrNumber: filters.getValue('urorrNumberControl'),
            registrationNumber: filters.getValue('registrationNumberControl'),
            registeredDateFrom: filters.getValue<DateRangeData>('dateRangeControl')?.start,
            registeredDateTo: filters.getValue<DateRangeData>('dateRangeControl')?.end,
            ownerName: filters.getValue('ownerNameControl'),
            ownerEIK: filters.getValue('ownerEikControl'),
            organizerName: filters.getValue('organizerNameControl'),
            organizerEGN: filters.getValue('organizerEgnControl'),
            populatedAreaId: filters.getValue('populatedAreaControl'),
            utilityName: filters.getValue('utilityNameControl'),
            districtId: filters.getValue('districtControl'),
            logBookNumber: filters.getValue('logNumberControl'),
            territoryUnitId: this.hasReadAllPermission ? filters.getValue('territoryUnitControl') : undefined,
            statusIds: filters.getValue('buyerSatusesControl')
        });
    }
}