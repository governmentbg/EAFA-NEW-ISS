import { AfterViewInit, Component, Input, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { StatisticalFormDTO } from '@app/models/generated/dtos/StatisticalFormDTO';
import { StatisticalFormsFilters } from '@app/models/generated/filters/StatisticalFormsFilters';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { StatisticalFormTypesEnum } from '@app/enums/statistical-form-types.enum';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { IStatisticalFormsService } from '@app/interfaces/common-app/statistical-forms.interface';
import { ChooseApplicationComponent } from '../applications/components/choose-application/choose-application.component';
import { ChooseApplicationDialogParams } from '../applications/components/choose-application/models/choose-application-dialog-params.model';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ApplicationForChoiceDTO } from '@app/models/generated/dtos/ApplicationForChoiceDTO';
import { StatisticalFormsAquaFarmComponent } from './components/statistical-forms-aqua-farm/statistical-forms-aqua-farm.component';
import { StatisticalFormsReworkComponent } from './components/statistical-forms-rework/statistical-forms-rework.component';
import { StatisticalFormsFishVesselComponent } from './components/statistical-forms-fish-vessel/statistical-forms-fish-vessel.component';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { Router } from '@angular/router';

@Component({
    selector: 'statistical-forms-content',
    templateUrl: './statistical-forms-content.component.html'
})
export class StatisticalFormsContentComponent implements AfterViewInit {
    @Input()
    public service!: IStatisticalFormsService;

    @Input()
    public isPublicApp!: boolean;

    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public readonly statisticalFormTypesEnum: typeof StatisticalFormTypesEnum = StatisticalFormTypesEnum;

    public readonly canAddAquaFarmRecords: boolean;
    public readonly canEditAquaFarmRecords: boolean;
    public readonly canDeleteAquaFarmRecords: boolean;
    public readonly canRestoreAquaFarmRecords: boolean;
    public readonly canReadAquaFarmApplications: boolean;

    public readonly canAddReworkRecords: boolean;
    public readonly canEditReworkRecords: boolean;
    public readonly canDeleteReworkRecords: boolean;
    public readonly canRestoreReworkRecords: boolean;
    public readonly canReadReworkApplications: boolean;

    public readonly canAddFishVesselRecords: boolean;
    public readonly canEditFishVesselRecords: boolean;
    public readonly canDeleteFishVesselRecords: boolean;
    public readonly canRestoreFishVesselRecords: boolean;
    public readonly canReadFishVesselApplications: boolean;

    public formTypes: NomenclatureDTO<StatisticalFormTypesEnum>[];
    public processUsers: NomenclatureDTO<number>[];
    public submissionUsers: NomenclatureDTO<number>[];

    private readonly commonNomenclaturesService: CommonNomenclatures;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<StatisticalFormDTO, StatisticalFormsFilters>;

    private readonly confirmDialog: TLConfirmDialog;
    private readonly aquaFarmEditDialog: TLMatDialog<StatisticalFormsAquaFarmComponent>;
    private readonly reworkEditDialog: TLMatDialog<StatisticalFormsReworkComponent>;
    private readonly fishVesselEditDialog: TLMatDialog<StatisticalFormsFishVesselComponent>;
    private readonly chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>;
    private readonly router: Router;

    public constructor(
        commonNomenclaturesService: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        aquaFarmEditDialog: TLMatDialog<StatisticalFormsAquaFarmComponent>,
        reworkEditDialog: TLMatDialog<StatisticalFormsReworkComponent>,
        fishVesselEditDialog: TLMatDialog<StatisticalFormsFishVesselComponent>,
        chooseApplicationDialog: TLMatDialog<ChooseApplicationComponent>,
        permissions: PermissionsService,
        router: Router
    ) {
        this.commonNomenclaturesService = commonNomenclaturesService;
        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.aquaFarmEditDialog = aquaFarmEditDialog;
        this.reworkEditDialog = reworkEditDialog;
        this.fishVesselEditDialog = fishVesselEditDialog;
        this.chooseApplicationDialog = chooseApplicationDialog;
        this.router = router;

        this.canAddAquaFarmRecords = permissions.has(PermissionsEnum.StatisticalFormsAquaFarmAddRecords);
        this.canEditAquaFarmRecords = permissions.has(PermissionsEnum.StatisticalFormsAquaFarmEditRecords);
        this.canDeleteAquaFarmRecords = permissions.has(PermissionsEnum.StatisticalFormsAquaFarmDeleteRecords);
        this.canRestoreAquaFarmRecords = permissions.has(PermissionsEnum.StatisticalFormsAquaFarmRestoreRecords);
        this.canReadAquaFarmApplications = permissions.has(PermissionsEnum.StatisticalFormsAquaFarmApplicationsRead)
            || permissions.has(PermissionsEnum.StatisticalFormsAquaFarmApplicationsReadAll);

        this.canAddReworkRecords = permissions.has(PermissionsEnum.StatisticalFormsReworkAddRecords);
        this.canEditReworkRecords = permissions.has(PermissionsEnum.StatisticalFormsReworkEditRecords);
        this.canDeleteReworkRecords = permissions.has(PermissionsEnum.StatisticalFormsReworkDeleteRecords);
        this.canRestoreReworkRecords = permissions.has(PermissionsEnum.StatisticalFormsReworkRestoreRecords);
        this.canReadReworkApplications = permissions.has(PermissionsEnum.StatisticalFormsReworkApplicationsRead)
            || permissions.has(PermissionsEnum.StatisticalFormsReworkApplicationsReadAll);

        this.canAddFishVesselRecords = permissions.has(PermissionsEnum.StatisticalFormsFishVesselAddRecords);
        this.canEditFishVesselRecords = permissions.has(PermissionsEnum.StatisticalFormsFishVesselEditRecords);
        this.canDeleteFishVesselRecords = permissions.has(PermissionsEnum.StatisticalFormsFishVesselDeleteRecords);
        this.canRestoreFishVesselRecords = permissions.has(PermissionsEnum.StatisticalFormsFishVesselRestoreRecords);
        this.canReadFishVesselApplications = permissions.has(PermissionsEnum.StatisticalFormsFishVesselsApplicationsRead)
            || permissions.has(PermissionsEnum.StatisticalFormsFishVesselsApplicationsReadAll);

        this.formTypes = [
            new NomenclatureDTO<StatisticalFormTypesEnum>({
                value: StatisticalFormTypesEnum.AquaFarm,
                displayName: translate.getValue('statistical-forms.aqua-farm'),
                isActive: true
            }),
            new NomenclatureDTO<StatisticalFormTypesEnum>({
                value: StatisticalFormTypesEnum.FishVessel,
                displayName: translate.getValue('statistical-forms.fish-vessel'),
                isActive: true
            }),
            new NomenclatureDTO<StatisticalFormTypesEnum>({
                value: StatisticalFormTypesEnum.Rework,
                displayName: translate.getValue('statistical-forms.rework'),
                isActive: true
            })
        ];

        this.processUsers = [];
        this.submissionUsers = [];
    }

    public ngOnInit(): void {
        this.buildForm();

        if (!this.isPublicApp) {
            this.commonNomenclaturesService.getUserNames().subscribe({
                next: (usernames: NomenclatureDTO<number>[]) => {
                    this.processUsers = this.submissionUsers = usernames;
                }
            });
        }
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<StatisticalFormDTO, StatisticalFormsFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllStatisticalForms.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        const isPerson: boolean | undefined = window.history.state?.isPerson;
        const id: number | undefined = window.history.state?.id;

        if (!CommonUtils.isNullOrEmpty(id)) {
            if (isPerson) {
                this.grid.advancedFilters = new StatisticalFormsFilters({ personId: id });
            }
            else {
                this.grid.advancedFilters = new StatisticalFormsFilters({ legalId: id });
            }
        }

        this.grid.refreshData();
    }

    public addEditForm(form: StatisticalFormDTO | undefined, type: StatisticalFormTypesEnum | undefined, viewMode: boolean = false): void {
        let data: DialogParamsModel | undefined;
        let auditButton: IHeaderAuditButton | undefined;

        if (form?.id !== undefined && type !== undefined) {
            const readonly: boolean =
                (type === StatisticalFormTypesEnum.AquaFarm && !this.canEditAquaFarmRecords)
                || (type === StatisticalFormTypesEnum.Rework && !this.canEditReworkRecords)
                || (type === StatisticalFormTypesEnum.FishVessel && !this.canEditFishVesselRecords);

            data = new DialogParamsModel({
                id: form.id,
                isApplication: false,
                isReadonly: readonly || viewMode,
                service: this.service
            });

            if (!this.isPublicApp) {
                auditButton = {
                    id: form.id,
                    getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                    tableName: 'RInfStat.StatisticalFormsRegister'
                };
            }

            this.openEditDialog(type, data, auditButton, viewMode);
        }
        else {
            const pageCodes: PageCodeEnum[] = this.getPageCodesFromPermissions();

            this.chooseApplicationDialog.open({
                TCtor: ChooseApplicationComponent,
                title: this.translate.getValue('applications-register.choose-application-for-register-creation'),
                translteService: this.translate,
                componentData: new ChooseApplicationDialogParams({
                    pageCodes: pageCodes
                }),
                disableDialogClose: true,
                headerCancelButton: {
                    cancelBtnClicked: this.closeApplicationChooseDialogBtnClicked.bind(this)
                },
                saveBtn: {
                    id: 'save',
                    color: 'accent',
                    translateValue: this.translate.getValue('applications-register.choose')
                },
                cancelBtn: {
                    id: 'cancel',
                    color: 'primary',
                    translateValue: this.translate.getValue('common.cancel'),
                }
            }).subscribe((dialogData: { selectedApplication: ApplicationForChoiceDTO }) => {
                if (dialogData !== null && dialogData !== undefined) {
                    data = new DialogParamsModel({
                        id: undefined,
                        isApplication: false,
                        isReadonly: false,
                        viewMode: false,
                        service: this.service,
                        applicationId: dialogData.selectedApplication.id
                    });

                    this.openEditDialog(this.pageCodeToType(dialogData.selectedApplication.pageCode!), data, auditButton, viewMode);
                }
            });
        }
    }

    public deleteStatisticalForm(statisticalForm: StatisticalFormDTO): void {
        this.confirmDialog.open({
            title: this.translate.getValue('statistical-forms.delete-statistical-form-dialog-title'),
            message: this.translate.getValue('statistical-forms.confirm-delete-message'),
            okBtnLabel: this.translate.getValue('statistical-forms.delete')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok && statisticalForm?.id) {
                    this.service.deleteStatisticalForm(statisticalForm.id).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    public restoreStatisticalForm(statisticalForm: StatisticalFormDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok && statisticalForm?.id) {
                    this.service.undoDeleteStatisticalForm(statisticalForm.id).subscribe({
                        next: () => {
                            this.grid.refreshData();
                        }
                    });
                }
            }
        });
    }

    public gotToApplication(statisticalForm: StatisticalFormDTO): void {
        switch (statisticalForm.formType) {
            case StatisticalFormTypesEnum.AquaFarm: {
                if (this.canReadAquaFarmApplications) {
                    this.router.navigate(['statistical-forms-applications'], { state: { applicationId: statisticalForm.applicationId } });
                }
            } break;
            case StatisticalFormTypesEnum.Rework: {
                if (this.canReadReworkApplications) {
                    this.router.navigate(['statistical-forms-applications'], { state: { applicationId: statisticalForm.applicationId } });
                }
            } break;
            case StatisticalFormTypesEnum.FishVessel: {
                if (this.canReadFishVesselApplications) {
                    this.router.navigate(['statistical-forms-applications'], { state: { applicationId: statisticalForm.applicationId } });
                }
            } break;
        }
        
    }

    private mapFilters(filters: FilterEventArgs): StatisticalFormsFilters {
        const result = new StatisticalFormsFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            formTypeIds: filters.getValue('formTypesControl'),
            submissionDateFrom: filters.getValue<DateRangeData>('submissionDateControl')?.start,
            submissionDateTo: filters.getValue<DateRangeData>('submissionDateControl')?.end,
            formObject: filters.getValue('formObjectControl')
        });

        if (!this.isPublicApp) {
            result.processUserId = filters.getValue('processUserControl');
            result.submissionUserId = filters.getValue('submissionUserControl');
        }

        return result;
    }

    private buildForm(): void {
        if (this.isPublicApp) {
            this.form = new FormGroup({
                formTypesControl: new FormControl(),
                submissionDateControl: new FormControl(),
                formObjectControl: new FormControl()
            });
        }
        else {
            this.form = new FormGroup({
                processUserControl: new FormControl(),
                formTypesControl: new FormControl(),
                submissionDateControl: new FormControl(),
                submissionUserControl: new FormControl(),
                formObjectControl: new FormControl()
            });
        }
    }

    private getPageCodesFromPermissions(): PageCodeEnum[] {
        const result: PageCodeEnum[] = [];

        if (this.canAddAquaFarmRecords) {
            result.push(PageCodeEnum.StatFormAquaFarm);
        }

        if (this.canAddReworkRecords) {
            result.push(PageCodeEnum.StatFormRework);
        }

        if (this.canAddFishVesselRecords) {
            result.push(PageCodeEnum.StatFormFishVessel);
        }

        return result;
    }

    private pageCodeToType(pageCode: PageCodeEnum): StatisticalFormTypesEnum {
        switch (pageCode) {
            case PageCodeEnum.StatFormAquaFarm:
                return StatisticalFormTypesEnum.AquaFarm;
            case PageCodeEnum.StatFormRework:
                return StatisticalFormTypesEnum.Rework;
            case PageCodeEnum.StatFormFishVessel:
                return StatisticalFormTypesEnum.FishVessel;
        }

        throw new Error('Invalid page code for statistical forms');
    }

    private openEditDialog(
        type: StatisticalFormTypesEnum,
        data: DialogParamsModel,
        auditButton: IHeaderAuditButton | undefined,
        viewMode: boolean
    ) {
        if (type === StatisticalFormTypesEnum.AquaFarm) {
            let title: string;

            if (data.id !== undefined) {
                title = viewMode
                    ? this.translate.getValue('statistical-forms.view-statistical-form-aqua-farm-dialog-title')
                    : this.translate.getValue('statistical-forms.edit-statistical-form-aqua-farm-dialog-title');
            }
            else {
                title = this.translate.getValue('statistical-forms.add-statistical-form-aqua-farm-dialog-title');
            }

            const dialog = this.aquaFarmEditDialog.openWithTwoButtons({
                title: title,
                TCtor: StatisticalFormsAquaFarmComponent,
                headerAuditButton: auditButton,
                headerCancelButton: {
                    cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
                },
                componentData: data,
                translteService: this.translate,
                disableDialogClose: true,
                viewMode: viewMode
            }, '1400px');

            dialog.subscribe((entry?: StatisticalFormDTO) => {
                if (entry !== undefined) {
                    this.grid.refreshData();
                }
            });
        }
        else if (type === StatisticalFormTypesEnum.Rework) {
            let title: string;

            if (data.id !== undefined) {
                title = viewMode
                    ? this.translate.getValue('statistical-forms.view-statistical-form-rework-dialog-title')
                    : this.translate.getValue('statistical-forms.edit-statistical-form-rework-dialog-title');
            }
            else {
                title = this.translate.getValue('statistical-forms.add-statistical-form-rework-dialog-title');
            }

            const dialog = this.reworkEditDialog.openWithTwoButtons({
                title: title,
                TCtor: StatisticalFormsReworkComponent,
                headerAuditButton: auditButton,
                headerCancelButton: {
                    cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
                },
                componentData: data,
                translteService: this.translate,
                disableDialogClose: true,
                viewMode: viewMode
            }, '1400px');

            dialog.subscribe((entry?: StatisticalFormDTO) => {
                if (entry !== undefined) {
                    this.grid.refreshData();
                }
            });
        }
        else if (type === StatisticalFormTypesEnum.FishVessel) {
            let title: string;

            if (data.id !== undefined) {
                title = viewMode
                    ? this.translate.getValue('statistical-forms.view-statistical-form-fish-vessel-dialog-title')
                    : this.translate.getValue('statistical-forms.edit-statistical-form-fish-vessel-dialog-title');
            }
            else {
                title = this.translate.getValue('statistical-forms.add-statistical-form-fish-vessel-dialog-title');
            }

            const dialog = this.fishVesselEditDialog.openWithTwoButtons({
                title: title,
                TCtor: StatisticalFormsFishVesselComponent,
                headerAuditButton: auditButton,
                headerCancelButton: {
                    cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
                },
                componentData: data,
                translteService: this.translate,
                disableDialogClose: true,
                viewMode: viewMode
            }, '1400px');

            dialog.subscribe((entry?: StatisticalFormDTO) => {
                if (entry !== undefined) {
                    this.grid.refreshData();
                }
            });
        }
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeApplicationChooseDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}