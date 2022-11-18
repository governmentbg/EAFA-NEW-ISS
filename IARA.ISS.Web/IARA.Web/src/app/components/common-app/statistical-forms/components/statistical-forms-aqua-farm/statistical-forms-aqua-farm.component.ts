import { Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { forkJoin, Observable, Subject } from 'rxjs';

import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IStatisticalFormsService } from '@app/interfaces/common-app/statistical-forms.interface';
import { StatisticalFormAquaFarmEditDTO } from '@app/models/generated/dtos/StatisticalFormAquaFarmEditDTO';
import { StatisticalFormAquaFarmApplicationEditDTO } from '@app/models/generated/dtos/StatisticalFormAquaFarmApplicationEditDTO';
import { StatisticalFormAquaFarmRegixDataDTO } from '@app/models/generated/dtos/StatisticalFormAquaFarmRegixDataDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { ApplicationSubmittedByRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedByRegixDataDTO';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedForRegixDataDTO';
import { RegixLegalDataDTO } from '@app/models/generated/dtos/RegixLegalDataDTO';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { StatisticalFormGivenMedicineDTO } from '@app/models/generated/dtos/StatisticalFormGivenMedicineDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { StatisticalFormAquaFarmFishOrganismDTO } from '@app/models/generated/dtos/StatisticalFormAquaFarmFishOrganismDTO';
import { StatisticalFormAquaFarmBroodstockDTO } from '@app/models/generated/dtos/StatisticalFormAquaFarmBroodstockDTO';
import { StatisticalFormAquaFarmInstallationSystemNotFullDTO } from '@app/models/generated/dtos/StatisticalFormAquaFarmInstallationSystemNotFullDTO';
import { StatisticalFormAquaFarmInstallationSystemFullDTO } from '@app/models/generated/dtos/StatisticalFormAquaFarmInstallationSystemFullDTO';
import { StatisticalFormNumStatDTO } from '@app/models/generated/dtos/StatisticalFormNumStatDTO';
import { StatisticalFormAquacultureDTO } from '@app/models/generated/dtos/StatisticalFormAquacultureDTO';
import { StatisticalFormEmployeeInfoDTO } from '@app/models/generated/dtos/StatisticalFormEmployeeInfoDTO';
import { AquacultureSystemEnum } from '@app/enums/aquaculture-system.enum';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { CommandTypes } from '@app/shared/components/data-table/enums/command-type.enum';
import { StatisticalFormAquacultureNomenclatureDTO } from '@app/models/generated/dtos/StatisticalFormAquacultureNomenclatureDTO';
import { ApplicationSubmittedForDTO } from '@app/models/generated/dtos/ApplicationSubmittedForDTO';
import { NumericStatTypeGroupsEnum } from '@app/enums/numeric-stat-type-groups.enum';
import { StatisticalFormReworkApplicationEditDTO } from '@app/models/generated/dtos/StatisticalFormReworkApplicationEditDTO';
import { StatisticalFormReworkEditDTO } from '@app/models/generated/dtos/StatisticalFormReworkEditDTO';
import { StatisticalFormEmployeeInfoGroupDTO } from '@app/models/generated/dtos/StatisticalFormEmployeeInfoGroupDTO';
import { StatisticalFormNumStatGroupDTO } from '@app/models/generated/dtos/StatisticalFormNumStatGroupDTO';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { StatisticalFormsPublicService } from '@app/services/public-app/statistical-forms-public.service';
import { StatisticalFormTypesEnum } from '@app/enums/statistical-form-types.enum';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';

type YesNo = 'yes' | 'no';

@Component({
    selector: 'statistical-forms-aqua-farm',
    templateUrl: './statistical-forms-aqua-farm.component.html',
    styleUrls: ['../../statistical-forms-content.component.scss']
})
export class StatisticalFormsAquaFarmComponent implements OnInit, IDialogComponent {
    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;
    public medicineGroup!: FormGroup;
    public producedFishOrganismGroup!: FormGroup;
    public soldFishOrganismGroup!: FormGroup;
    public unrealizedFishOrganismGroup!: FormGroup;
    public broodstockGroup!: FormGroup;
    public installationSystemFullGroup!: FormGroup;
    public installationSystemNotFullGroup!: FormGroup;
    public service!: IStatisticalFormsService;

    public isPublicApp: boolean = false;
    public isOnlineApplication: boolean = false;
    public isRegisterEntry: boolean = false;
    public readOnly: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public showRegiXData: boolean = false;
    public isApplication: boolean = false;
    public loadRegisterFromApplication: boolean = false;
    public viewMode: boolean = false;
    public isEditing: boolean = false;
    public isReadonly: boolean = false;
    public isSystemFull: boolean = false;
    public emptyProducedFishOrganism: boolean = true;
    public emptySoldFishOrganism: boolean = true;
    public emptyUnrealizedFishOrganism: boolean = true;
    public emptyBroodstock: boolean = true;
    public showBasicInfo: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();

    public readonly pageCode: PageCodeEnum = PageCodeEnum.StatFormAquaFarm;
    public readonly today: Date = new Date();

    public notifier: Notifier = new Notifier();
    public expectedResults: StatisticalFormAquaFarmRegixDataDTO;
    public regixChecks: ApplicationRegiXCheckDTO[] = [];
    public medicines: StatisticalFormGivenMedicineDTO[] = [];
    public producedFishOrganisms: StatisticalFormAquaFarmFishOrganismDTO[] = [];
    public soldFishOrganisms: StatisticalFormAquaFarmFishOrganismDTO[] = [];
    public unrealizedFishOrganisms: StatisticalFormAquaFarmFishOrganismDTO[] = [];
    public broodstocks: StatisticalFormAquaFarmBroodstockDTO[] = [];
    public installationsSystemFull: StatisticalFormAquaFarmInstallationSystemFullDTO[] = [];
    public installationsSystemNotFull: StatisticalFormAquaFarmInstallationSystemNotFullDTO[] = [];
    public fishTypes: NomenclatureDTO<number>[] = [];
    public allFishTypes: NomenclatureDTO<number>[] = [];
    public installationTypes: NomenclatureDTO<number>[] = [];
    public allInstallationTypes: NomenclatureDTO<number>[] = [];
    public rawMaterialTypes: StatisticalFormNumStatDTO[] = [];
    public aquacultures: StatisticalFormAquacultureNomenclatureDTO[] = [];
    public ownerEmployeeOptions: NomenclatureDTO<YesNo>[] = [];

    @ViewChild('medicineTable')
    private medicineTable!: TLDataTableComponent;

    @ViewChild('producedFishOrganismTable')
    private producedFishOrganismTable!: TLDataTableComponent;

    @ViewChild('soldFishOrganismTable')
    private soldFishOrganismTable!: TLDataTableComponent;

    @ViewChild('unrealizedFishOrganismTable')
    private unrealizedFishOrganismTable!: TLDataTableComponent;

    @ViewChild('broodstockTable')
    private broodstockTable!: TLDataTableComponent;

    @ViewChild('systemFullTable')
    private systemFullTable!: TLDataTableComponent;

    @ViewChild('systemNotFullTable')
    private systemNotFullTable!: TLDataTableComponent;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private isApplicationHistoryMode: boolean = false;
    private dialogRightSideActions: IActionInfo[] | undefined;
    private applicationsService: IApplicationsService | undefined;
    private formId: number | undefined;
    private applicationId: number | undefined;
    private chosenAquacultureFacilityId: number | undefined;
    private model!: StatisticalFormAquaFarmEditDTO | StatisticalFormAquaFarmApplicationEditDTO | StatisticalFormAquaFarmRegixDataDTO;

    private get employeeInfoFormArray(): FormArray {
        return this.form?.get('employeeInfoArray') as FormArray;
    }

    private get employeeStatsFormArray(): FormArray {
        return this.form?.get('employeeStatsArray') as FormArray;
    }

    private get financialInfoFormArray(): FormArray {
        return this.form?.get('financialInfoArray') as FormArray;
    }

    private employeeStatsGroups: Map<number, number[]> = new Map<number, number[]>();
    private tableMap: Map<number, number[]> = new Map<number, number[]>();

    private employeeStatsLabels: string[] = [];
    private employeeInfoLabels: string[] = [];
    private financialInfoGroupNames: string[] = [];

    private readonly employeeGroupTypes: NumericStatTypeGroupsEnum[] = [NumericStatTypeGroupsEnum.FreeLabor, NumericStatTypeGroupsEnum.NumHours];

    public constructor(translate: FuseTranslationLoaderService) {
        this.translate = translate;

        this.isPublicApp = IS_PUBLIC_APP;

        this.expectedResults = new StatisticalFormAquaFarmRegixDataDTO({
            submittedBy: new ApplicationSubmittedByRegixDataDTO({
                person: new RegixPersonDataDTO(),
                addresses: []
            }),
            submittedFor: new ApplicationSubmittedForRegixDataDTO({
                person: new RegixPersonDataDTO(),
                legal: new RegixLegalDataDTO(),
                addresses: []
            })
        });

        this.ownerEmployeeOptions = [
            new NomenclatureDTO<YesNo>({
                value: 'yes',
                displayName: this.translate.getValue('statistical-forms.yes'),
                isActive: true
            }),
            new NomenclatureDTO<YesNo>({
                value: 'no',
                displayName: this.translate.getValue('statistical-forms.no'),
                isActive: true
            })
        ];

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        if (!this.showOnlyRegiXData) {
            this.aquacultures = await this.service.getAllAquacultureNomenclatures().toPromise();
        }

        // извличане на исторически данни за заявление
        if (this.isApplicationHistoryMode && this.applicationId !== undefined) {
            this.form.disable();

            if (this.applicationsService) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const statisticalForm: StatisticalFormAquaFarmApplicationEditDTO = new StatisticalFormAquaFarmApplicationEditDTO(contentObject);
                        statisticalForm.files = content.files;
                        statisticalForm.applicationId = content.applicationId;

                        this.isOnlineApplication = statisticalForm.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.chosenAquacultureFacilityId = statisticalForm.aquacultureFacilityId;
                        this.model = statisticalForm;
                        this.fillForm();
                    }
                });
            }
        }
        else if (this.applicationId !== undefined && this.formId === undefined && !this.isApplication) {
            // извличане на данни за регистров запис от id на заявление
            if (this.loadRegisterFromApplication === true) {
                this.isEditing = true;

                this.service.getRegisterByApplicationId(this.applicationId, this.pageCode).subscribe({
                    next: (form: unknown) => {
                        this.model = form as StatisticalFormAquaFarmEditDTO;
                        this.isOnlineApplication = (this.model as StatisticalFormAquaFarmEditDTO).isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.chosenAquacultureFacilityId = (this.model as StatisticalFormAquaFarmEditDTO).aquacultureFacilityId;
                        this.fillForm();
                    }
                });
            }
            else {
                // извличане на данни за създаване на регистров запис от заявление
                this.isEditing = false;

                this.service.getApplicationDataForRegister(this.applicationId, this.pageCode).subscribe({
                    next: (statisticalForm: StatisticalFormAquaFarmEditDTO) => {
                        this.chosenAquacultureFacilityId = statisticalForm.aquacultureFacilityId;
                        this.model = statisticalForm;
                        this.isOnlineApplication = statisticalForm.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.fillForm();
                    }
                });
            }
        }
        else {
            if (this.isReadonly || this.viewMode) {
                this.form.disable();
            }

            if (this.isApplication && this.applicationId !== undefined) {
                // извличане на данни за RegiX сверка от служител
                this.isEditing = false;

                if (this.showOnlyRegiXData) {
                    this.service.getRegixData(this.applicationId, this.pageCode).subscribe({
                        next: (regixData: RegixChecksWrapperDTO<StatisticalFormAquaFarmRegixDataDTO>) => {
                            this.model = new StatisticalFormAquaFarmRegixDataDTO(regixData.dialogDataModel);
                            this.expectedResults = new StatisticalFormAquaFarmRegixDataDTO(regixData.regiXDataModel);

                            this.fillForm();
                        }
                    });
                }
                else {
                    // извличане на данни за заявление
                    this.isEditing = false;

                    this.service.getApplication(this.applicationId, this.showRegiXData, this.pageCode).subscribe({
                        next: (statisticalForm: StatisticalFormAquaFarmApplicationEditDTO) => {
                            statisticalForm.applicationId = this.applicationId;

                            this.isOnlineApplication = statisticalForm.isOnlineApplication!;
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new StatisticalFormAquaFarmRegixDataDTO(statisticalForm.regiXDataModel);
                                statisticalForm.regiXDataModel = undefined;
                            }

                            if (this.isPublicApp && this.isOnlineApplication && (statisticalForm.submittedBy === undefined || statisticalForm.submittedBy === null)) {
                                const service = this.service as StatisticalFormsPublicService;
                                service.getCurrentUserAsSubmittedBy(StatisticalFormTypesEnum.AquaFarm).subscribe({
                                    next: (submittedBy: ApplicationSubmittedByDTO) => {
                                        statisticalForm.submittedBy = submittedBy;
                                        this.chosenAquacultureFacilityId = statisticalForm.aquacultureFacilityId;
                                        this.model = statisticalForm;
                                        this.fillForm();
                                    }
                                });
                            }
                            else {
                                this.chosenAquacultureFacilityId = statisticalForm.aquacultureFacilityId;
                                this.model = statisticalForm;
                                this.fillForm();
                            }
                        }
                    });
                }
            }
            else if (this.formId !== undefined) {
                // извличане на данни за регистров запис
                this.isEditing = true;
                this.isRegisterEntry = true;

                this.service.getStatisticalFormAquaFarm(this.formId).subscribe({
                    next: (statisticalForm: StatisticalFormAquaFarmEditDTO) => {
                        this.chosenAquacultureFacilityId = statisticalForm.aquacultureFacilityId;
                        this.model = statisticalForm;
                        this.isOnlineApplication = statisticalForm.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.fillForm();
                    }
                });
            }
        }
    }

    public ngAfterViewInit(): void {
        if (!this.showOnlyRegiXData) {
            this.form.controls.aquacultureFacilityControl.valueChanges.subscribe({
                next: (value: NomenclatureDTO<number> | string | undefined) => {
                    if (value !== undefined && value !== null && typeof value !== 'string') {
                        this.service.getStatisticalFormAquaculture(value.value!).subscribe({
                            next: (aquaculture: StatisticalFormAquacultureDTO) => {
                                if (this.chosenAquacultureFacilityId !== aquaculture.aquacultureId) {
                                    this.producedFishOrganismTable.rows = [];
                                    this.unrealizedFishOrganismTable.rows = [];
                                    this.soldFishOrganismTable.rows = [];
                                    this.broodstockTable.rows = [];

                                    if (this.systemFullTable !== undefined && this.systemFullTable.rows.length === 0) {
                                        this.systemFullTable.rows = [];
                                    }
                                    if (this.systemNotFullTable !== undefined && this.systemNotFullTable.rows.length === 0) {
                                        this.systemNotFullTable.rows = [];
                                    }

                                    this.chosenAquacultureFacilityId = aquaculture.aquacultureId;
                                }

                                this.allInstallationTypes = this.installationTypes = aquaculture.facilityInstalations ?? [];
                                this.allFishTypes = this.fishTypes = aquaculture.fishTypes ?? [];

                                if (aquaculture.systemType === AquacultureSystemEnum.FullSystem) {
                                    this.isSystemFull = true;
                                }
                                else {
                                    this.isSystemFull = false;
                                }

                                this.form.controls.submittedForControl.setValue(new ApplicationSubmittedForDTO({
                                    legal: new RegixLegalDataDTO({
                                        eik: aquaculture.eik,
                                        name: aquaculture.legalName
                                    }),
                                    addresses: this.form.controls.submittedForControl.value?.addresses,
                                    submittedByLetterOfAttorney: this.form.controls.submittedForControl.value?.submittedByLetterOfAttorney,
                                    submittedByRole: this.form.controls.submittedForControl.value?.submittedByRole
                                }));
                            }
                        });
                    }
                    else {
                        this.form.controls.submittedForControl.setValue(new ApplicationSubmittedForDTO({
                            legal: new RegixLegalDataDTO({
                                eik: undefined,
                                name: undefined
                            }),
                            addresses: this.form.controls.submittedForControl.value?.addresses,
                            submittedByLetterOfAttorney: this.form.controls.submittedForControl.value?.submittedByLetterOfAttorney,
                            submittedByRole: this.form.controls.submittedForControl.value?.submittedByRole
                        }));
                    }
                }
            });

            this.form.controls.submittedForControl!.valueChanges.subscribe({
                next: (value: ApplicationSubmittedForDTO | undefined) => {
                    if (value?.submittedByRole !== undefined) {
                        this.showBasicInfo = true;
                    }
                }
            });

            this.installationSystemFullGroup?.get('installationTypeIdControl')?.valueChanges.subscribe({
                next: () => {
                    this.installationTypes = [...this.allInstallationTypes];
                    const currentIds: number[] = this.systemFullTable.rows.map(x => x.installationTypeId);

                    this.installationTypes = this.installationTypes.filter(x => !currentIds.includes(x.value!));
                    this.installationTypes = this.installationTypes.slice();
                }
            });

            this.installationSystemNotFullGroup?.get('installationTypeIdControl')?.valueChanges.subscribe({
                next: () => {
                    this.installationTypes = [...this.allInstallationTypes];
                    const currentIds: number[] = this.systemNotFullTable.rows.map(x => x.installationTypeId);

                    this.installationTypes = this.installationTypes.filter(x => !currentIds.includes(x.value!));
                    this.installationTypes = this.installationTypes.slice();
                }
            });

            this.producedFishOrganismGroup?.get('installationTypeIdControlHidden')?.valueChanges.subscribe({
                next: () => {
                    this.validateRows(this.producedFishOrganismTable);

                    if (this.producedFishOrganismGroup?.get('installationTypeIdControlHidden')!.value !== undefined && this.tableMap.size !== 0) {
                        this.fishTypes = this.fishTypes.filter(x => !this.tableMap.get(this.producedFishOrganismGroup?.get('installationTypeIdControlHidden')!.value.value)?.includes(x.value!));
                    }
                }
            });

            this.producedFishOrganismGroup.get('fishTypeIdControlHidden')?.valueChanges.subscribe({
                next: () => {
                    this.validateRows(this.producedFishOrganismTable);
                }
            });

            this.soldFishOrganismGroup?.get('installationTypeIdControlHidden')?.valueChanges.subscribe({
                next: () => {
                    this.validateRows(this.soldFishOrganismTable);

                    if (this.producedFishOrganismGroup?.get('installationTypeIdControlHidden')!.value !== undefined && this.tableMap.size !== 0) {
                        this.fishTypes = this.fishTypes.filter(x => !this.tableMap.get(this.producedFishOrganismGroup?.get('installationTypeIdControlHidden')!.value.value)?.includes(x.value!));
                    }
                }
            });

            this.soldFishOrganismGroup.get('fishTypeIdControlHidden')?.valueChanges.subscribe({
                next: () => {
                    this.validateRows(this.soldFishOrganismTable);
                }
            });

            this.unrealizedFishOrganismGroup?.get('installationTypeIdControlHidden')?.valueChanges.subscribe({
                next: () => {
                    this.validateRows(this.unrealizedFishOrganismTable);

                    if (this.producedFishOrganismGroup?.get('installationTypeIdControlHidden')!.value !== undefined && this.tableMap.size !== 0) {
                        this.fishTypes = this.fishTypes.filter(x => !this.tableMap.get(this.producedFishOrganismGroup?.get('installationTypeIdControlHidden')!.value.value)?.includes(x.value!));
                    }
                }
            });

            this.unrealizedFishOrganismGroup.get('fishTypeIdControlHidden')?.valueChanges.subscribe({
                next: () => {
                    this.validateRows(this.unrealizedFishOrganismTable);
                }
            });

            this.broodstockGroup?.get('installationTypeIdControlHidden')?.valueChanges.subscribe({
                next: () => {
                    this.validateRows(this.broodstockTable);

                    if (this.producedFishOrganismGroup?.get('installationTypeIdControlHidden')!.value !== undefined && this.tableMap.size !== 0) {
                        this.fishTypes = this.fishTypes.filter(x => !this.tableMap.get(this.producedFishOrganismGroup?.get('installationTypeIdControlHidden')!.value.value)?.includes(x.value!));
                    }
                }
            });

            this.broodstockGroup.get('fishTypeIdControlHidden')?.valueChanges.subscribe({
                next: () => {
                    this.validateRows(this.broodstockTable);
                }
            });

            this.producedFishOrganismTable.recordChanged.subscribe({
                next: (event: RecordChangedEventArgs<StatisticalFormAquaFarmFishOrganismDTO>) => {
                    if (event.Command !== CommandTypes.Edit) {
                        this.emptyProducedFishOrganism = this.producedFishOrganismTable.rows.length === 0;
                    }

                    if (event.Command === CommandTypes.Delete && this.producedFishOrganismTable.rows.length === 1) {
                        this.emptyProducedFishOrganism = true;
                    }
                }
            });

            this.soldFishOrganismTable.recordChanged.subscribe({
                next: (event: RecordChangedEventArgs<StatisticalFormAquaFarmFishOrganismDTO>) => {
                    if (event.Command !== CommandTypes.Edit) {
                        this.emptySoldFishOrganism = this.soldFishOrganismTable.rows.length === 0;
                    }

                    if (event.Command === CommandTypes.Delete && this.soldFishOrganismTable.rows.length === 1) {
                        this.emptySoldFishOrganism = true;
                    }
                }
            });

            this.unrealizedFishOrganismTable.recordChanged.subscribe({
                next: (event: RecordChangedEventArgs<StatisticalFormAquaFarmFishOrganismDTO>) => {
                    if (event.Command !== CommandTypes.Edit) {
                        this.emptyUnrealizedFishOrganism = this.unrealizedFishOrganismTable.rows.length === 0;
                    }

                    if (event.Command === CommandTypes.Delete && this.unrealizedFishOrganismTable.rows.length === 1) {
                        this.emptyUnrealizedFishOrganism = true;
                    }
                }
            });

            this.broodstockTable.recordChanged.subscribe({
                next: (event: RecordChangedEventArgs<StatisticalFormAquaFarmBroodstockDTO>) => {
                    if (event.Command !== CommandTypes.Edit) {
                        this.emptyBroodstock = this.broodstockTable.rows.length === 0;
                    }

                    if (event.Command === CommandTypes.Delete && this.broodstockTable.rows.length === 1) {
                        this.emptyBroodstock = true;
                    }
                }
            });
        }
    }

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        this.formId = data.id;
        this.applicationId = data.applicationId;
        this.applicationsService = data.applicationsService;
        this.isApplication = data.isApplication;
        this.isReadonly = data.isReadonly;
        this.isApplicationHistoryMode = data.isApplicationHistoryMode;
        this.viewMode = data.viewMode;
        this.showOnlyRegiXData = data.showOnlyRegiXData;
        this.showRegiXData = data.showRegiXData;
        this.service = data.service as IStatisticalFormsService;
        this.loadRegisterFromApplication = data.loadRegisterFromApplication;

        this.buildForm();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.saveAquaFarmForm(dialogClose);
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        let applicationAction: boolean = false;

        if (this.model instanceof StatisticalFormAquaFarmApplicationEditDTO || this.model instanceof StatisticalFormAquaFarmRegixDataDTO) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            applicationAction = ApplicationUtils.applicationDialogButtonClicked(new ApplicationDialogData({
                action: actionInfo,
                dialogClose: dialogClose,
                applicationId: this.applicationId,
                model: this.model,
                readOnly: this.isReadonly,
                viewMode: this.viewMode,
                editForm: this.form,
                saveFn: this.saveAquaFarmForm.bind(this),
                onMarkAsTouched: () => {
                    this.validityCheckerGroup.validate();
                }
            }));
        }

        if (!this.isReadonly && !this.viewMode && !applicationAction) {
            if (actionInfo.id === 'save') {
                return this.saveBtnClicked(actionInfo, dialogClose);
            }
        }
    }

    public aquaculturesFilterFn(value: string | StatisticalFormAquacultureNomenclatureDTO): StatisticalFormAquacultureNomenclatureDTO[] {
        return this.aquacultures.filter((aquaculture: StatisticalFormAquacultureNomenclatureDTO) => {
            const noSpacesDisplayName: string = aquaculture.displayName!.replace(/\s+/g, '');
            const noSpacesFilterValue: string = typeof value === 'string' ? value.replace(/\s+/g, '') : value.displayName!.replace(/\s+/g, '');

            const result: boolean = noSpacesDisplayName.toLowerCase().includes(noSpacesFilterValue.toLowerCase());
            return result;
        });
    }

    public fileTypeFilterFn(options: PermittedFileTypeDTO[]): PermittedFileTypeDTO[] {
        const pdfs: FileTypeEnum[] = [FileTypeEnum.SIGNEDAPPL, FileTypeEnum.APPLICATION_PDF];
        const offlineAppl: FileTypeEnum[] = [FileTypeEnum.SCANNED_FORM];

        let result: PermittedFileTypeDTO[] = options;

        if (this.isApplication || !this.isOnlineApplication) {
            result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        if (this.isOnlineApplication) {
            result = result.filter(x => !offlineAppl.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        return result;
    }

    private buildForm(): void {
        if (this.showOnlyRegiXData) {
            this.form = new FormGroup({
                submittedByControl: new FormControl(),
                submittedForControl: new FormControl()
            });

        }
        else if (this.isApplication || this.isApplicationHistoryMode) {
            this.form = new FormGroup({
                submittedByControl: new FormControl(),
                submittedByWorkPositionControl: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
                submittedForControl: new FormControl(),
                aquacultureFacilityControl: new FormControl(null, Validators.required),
                yearControl: new FormControl(null, Validators.required),
                breedingMaterialDeathRateControl: new FormControl(null, TLValidators.number(0, 100)),
                consumationFishDeathRateControl: new FormControl(null, TLValidators.number(0, 100)),
                broodstockDeathRateControl: new FormControl(null, TLValidators.number(0, 100)),
                medicineCommentsControl: new FormControl(),
                isOwnerEmployeeControl: new FormControl(null, Validators.required),
                filesControl: new FormControl(),
                employeeInfoArray: new FormArray([], this.payColumnCountValidator()),
                employeeStatsArray: new FormArray([]),
                financialInfoArray: new FormArray([], this.costsValidator()),
                rawMaterialControl: new FormControl()
            });

            this.initTableFormGroups();
        }
        else {
            this.form = new FormGroup({
                submittedForControl: new FormControl(),
                aquacultureFacilityControl: new FormControl(null, Validators.required),
                yearControl: new FormControl(null, Validators.required),
                formNumControl: new FormControl({ value: null, disabled: true }),
                breedingMaterialDeathRateControl: new FormControl(null, TLValidators.number(0, 100)),
                consumationFishDeathRateControl: new FormControl(null, TLValidators.number(0, 100)),
                broodstockDeathRateControl: new FormControl(null, TLValidators.number(0, 100)),
                medicineCommentsControl: new FormControl(),
                isOwnerEmployeeControl: new FormControl(null, Validators.required),
                filesControl: new FormControl(),
                employeeInfoArray: new FormArray([], this.payColumnCountValidator()),
                employeeStatsArray: new FormArray([]),
                financialInfoArray: new FormArray([], [Validators.required, this.costsValidator()]),
                rawMaterialControl: new FormControl()
            });

            this.initTableFormGroups();
        }

        this.form.get('employeeInfoArray')?.valueChanges.subscribe({
            next: () => {
                this.form.get('financialInfoArray')!.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    private fillForm(): void {
        if (this.model instanceof StatisticalFormAquaFarmRegixDataDTO) {
            this.form.get('submittedByControl')!.setValue(this.model.submittedBy);
            this.form.get('submittedForControl')!.setValue(this.model.submittedFor);

            this.fillFormRegiX(this.model);
        }
        else if (this.model instanceof StatisticalFormAquaFarmApplicationEditDTO) {
            this.fillFormApplication(this.model);
        }
        else if (this.model instanceof StatisticalFormAquaFarmEditDTO) {
            this.fillFormRegister(this.model);

            if (this.showRegiXData) {
                this.fillFormRegiX(this.model);
            }
        }

        if (this.isReadonly || this.viewMode) {
            this.form.disable();
        }
    }

    private fillFormRegiX(model: StatisticalFormAquaFarmRegixDataDTO | StatisticalFormAquaFarmApplicationEditDTO): void {
        if (model.applicationRegiXChecks !== undefined && model.applicationRegiXChecks) {
            const applicationRegiXChecks: ApplicationRegiXCheckDTO[] = model.applicationRegiXChecks;

            setTimeout(() => {
                this.regixChecks = applicationRegiXChecks;
            });
        }

        if (!this.viewMode) {
            this.notifier.start();
            this.notifier.onNotify.subscribe({
                next: () => {
                    this.form.markAllAsTouched();

                    if (this.showOnlyRegiXData) {
                        ApplicationUtils.enableOrDisableRegixCheckButtons(this.form, this.dialogRightSideActions);
                    }

                    this.notifier.stop();
                }
            });
        }
    }

    private fillFormApplication(model: StatisticalFormAquaFarmApplicationEditDTO): void {
        this.form.get('submittedByControl')!.setValue(model.submittedBy);
        this.form.get('submittedByWorkPositionControl')!.setValue(model.submittedByWorkPosition);
        this.form.get('submittedForControl')!.setValue(model.submittedFor);

        if (model.aquacultureFacilityId !== undefined && model.aquacultureFacilityId !== null) {
            this.form.get('aquacultureFacilityControl')!.setValue(this.aquacultures.find(x => x.value === model.aquacultureFacilityId));
        }

        if (model.year !== null && model.year !== undefined) {
            this.form.get('yearControl')!.setValue(new Date(model.year, 0, 1));
        }

        this.form.get('breedingMaterialDeathRateControl')!.setValue(model.breedingMaterialDeathRate);
        this.form.get('consumationFishDeathRateControl')!.setValue(model.consumationFishDeathRate);
        this.form.get('broodstockDeathRateControl')!.setValue(model.broodstockDeathRate);
        this.form.get('medicineCommentsControl')!.setValue(model.medicineComments);

        if (model.isOwnerEmployee !== undefined && model.isOwnerEmployee !== null) {
            if (model.isOwnerEmployee) {
                this.form.get('isOwnerEmployeeControl')!.setValue(this.ownerEmployeeOptions.find(x => x.value === 'yes'));
            }
            else {
                this.form.get('isOwnerEmployeeControl')!.setValue(this.ownerEmployeeOptions.find(x => x.value === 'no'));
            }
        }
        else {
            this.form.get('isOwnerEmployeeControl')!.setValue(undefined);
        }

        this.form.get('filesControl')!.setValue(model.files);

        this.buildEmployeeInfoFormArray(model);
        this.buildEmployeeStatsFormArray(model);
        this.buildFinancialInfoFormArray(model);
        this.buildAndSetRawMaterialControl(model);

        setTimeout(() => {
            this.medicines = model.medicine ?? [];
            this.producedFishOrganisms = model.producedFishOrganism ?? [];
            this.soldFishOrganisms = model.soldFishOrganism ?? [];
            this.unrealizedFishOrganisms = model.unrealizedFishOrganism ?? [];
            this.broodstocks = model.broodstock ?? [];
            this.installationsSystemFull = model.installationSystemFull ?? [];
            this.installationsSystemNotFull = model.installationSystemNotFull ?? [];

            this.emptyProducedFishOrganism = this.producedFishOrganisms.length === 0;
            this.emptySoldFishOrganism = this.soldFishOrganisms.length === 0;
            this.emptyUnrealizedFishOrganism = this.unrealizedFishOrganisms.length === 0;
            this.emptyBroodstock = this.broodstocks.length === 0;
        });
    }

    private fillFormRegister(model: StatisticalFormAquaFarmEditDTO): void {
        this.form.get('submittedForControl')!.setValue(model.submittedFor);
        this.form.get('aquacultureFacilityControl')!.setValue(this.aquacultures.find(x => x.value === model.aquacultureFacilityId));
        this.form.get('yearControl')!.setValue(new Date(model.year!, 0, 1));
        this.form.get('formNumControl')!.setValue(model.formNum);
        this.form.get('breedingMaterialDeathRateControl')!.setValue(model.breedingMaterialDeathRate);
        this.form.get('consumationFishDeathRateControl')!.setValue(model.consumationFishDeathRate);
        this.form.get('broodstockDeathRateControl')!.setValue(model.broodstockDeathRate);
        this.form.get('medicineCommentsControl')!.setValue(model.medicineComments);
        this.form.get('filesControl')!.setValue(model.files);

        if (model.isOwnerEmployee) {
            this.form.get('isOwnerEmployeeControl')!.setValue(this.ownerEmployeeOptions.find(x => x.value === 'yes'));
        }
        else {
            this.form.get('isOwnerEmployeeControl')!.setValue(this.ownerEmployeeOptions.find(x => x.value === 'no'));
        }

        this.buildEmployeeInfoFormArray(model);
        this.buildEmployeeStatsFormArray(model);
        this.buildFinancialInfoFormArray(model);
        this.buildAndSetRawMaterialControl(model);

        setTimeout(() => {
            this.medicines = model.medicine ?? [];
            this.producedFishOrganisms = model.producedFishOrganism ?? [];
            this.soldFishOrganisms = model.soldFishOrganism ?? [];
            this.unrealizedFishOrganisms = model.unrealizedFishOrganism ?? [];
            this.broodstocks = model.broodstock ?? [];
            this.installationsSystemFull = model.installationSystemFull ?? [];
            this.installationsSystemNotFull = model.installationSystemNotFull ?? [];

            this.emptyProducedFishOrganism = this.producedFishOrganisms.length === 0;
            this.emptySoldFishOrganism = this.soldFishOrganisms.length === 0;
            this.emptyUnrealizedFishOrganism = this.unrealizedFishOrganisms.length === 0;
            this.emptyBroodstock = this.broodstocks.length === 0;
        });
    }

    private buildEmployeeInfoFormArray(model: StatisticalFormAquaFarmApplicationEditDTO | StatisticalFormAquaFarmEditDTO): void {
        const groups: StatisticalFormEmployeeInfoGroupDTO[] = model.employeeInfoGroups ?? [];

        for (let i = 0; i < groups.length; ++i) {
            this.employeeInfoLabels.push(groups[i].groupName!);
            this.employeeInfoFormArray.push(new FormControl(groups[i].employeeTypes));
        }
    }

    private buildEmployeeStatsFormArray(model: StatisticalFormAquaFarmApplicationEditDTO | StatisticalFormAquaFarmEditDTO): void {
        let id: number = 0;
        for (const group of (model.numStatGroups ?? []).filter(x => this.employeeGroupTypes.includes(x.groupType!))) {
            const ids: number[] = [];

            for (const statType of group.numericStatTypes ?? []) {
                ids.push(id++);

                this.employeeStatsLabels.push(statType.name!);
                this.employeeStatsFormArray.push(new FormControl(statType.statValue, [Validators.required, TLValidators.number(0)]));
            }

            this.employeeStatsGroups.set(group.id!, ids);
        }
    }

    private buildFinancialInfoFormArray(model: StatisticalFormAquaFarmApplicationEditDTO | StatisticalFormAquaFarmEditDTO): void {
        for (const group of (model.numStatGroups ?? []).filter(x => !this.employeeGroupTypes.includes(x.groupType!) && x.groupType !== NumericStatTypeGroupsEnum.DeathRate && x.groupType !== NumericStatTypeGroupsEnum.RawMatAq)) {
            this.financialInfoGroupNames.push(group.groupName!);
            this.financialInfoFormArray.push(new FormControl(group.numericStatTypes, Validators.required));
        }
    }

    private buildAndSetRawMaterialControl(model: StatisticalFormAquaFarmApplicationEditDTO | StatisticalFormAquaFarmEditDTO): void {
        for (const group of model.numStatGroups!.filter(x => x.groupType === NumericStatTypeGroupsEnum.RawMatAq)) {
            for (const material of group.numericStatTypes!) {
                this.rawMaterialTypes.push(material);
            }
        }
        this.form.get('rawMaterialControl')!.setValue(this.rawMaterialTypes);
    }

    private fillModel(): void {
        if (this.model instanceof StatisticalFormAquaFarmRegixDataDTO) {
            this.fillModelRegix(this.model);
        }
        else if (this.model instanceof StatisticalFormAquaFarmApplicationEditDTO) {
            this.fillModelApplication(this.model);
        }
        else if (this.model instanceof StatisticalFormAquaFarmEditDTO) {
            this.fillModelRegister(this.model);
        }
    }

    private fillModelRegix(model: StatisticalFormAquaFarmRegixDataDTO): void {
        model.submittedBy = this.form.get('submittedByControl')!.value;
        model.submittedFor = this.form.get('submittedForControl')!.value;
    }

    private fillModelApplication(model: StatisticalFormAquaFarmApplicationEditDTO): void {
        model.submittedBy = this.form.get('submittedByControl')!.value;
        model.submittedByWorkPosition = this.form.get('submittedByWorkPositionControl')!.value;
        model.submittedFor = this.form.get('submittedForControl')!.value;
        model.aquacultureFacilityId = this.form.get('aquacultureFacilityControl')!.value?.value;
        model.year = (this.form.get('yearControl')!.value as Date)?.getFullYear();
        model.breedingMaterialDeathRate = this.form.get('breedingMaterialDeathRateControl')!.value;
        model.consumationFishDeathRate = this.form.get('consumationFishDeathRateControl')!.value;
        model.broodstockDeathRate = this.form.get('broodstockDeathRateControl')!.value;

        if (this.form.get('isOwnerEmployeeControl')!.value) {
            model.isOwnerEmployee = this.form.get('isOwnerEmployeeControl')!.value!.value === 'yes';
        }
        else {
            model.isOwnerEmployee = undefined;
        }

        model.employeeInfoGroups = this.getEmployeeInfo(model);
        model.numStatGroups = [...this.getEmployeeStats(model), ...this.getFinancialInfo(model), ...this.getRawMaterial(model)];

        model.medicineComments = this.form.get('medicineCommentsControl')!.value;
        model.files = this.form.get('filesControl')!.value;

        model.medicine = this.getMedicineFromTable();
        model.producedFishOrganism = this.getFishOrganismFromTable(this.producedFishOrganismTable.rows);
        model.soldFishOrganism = this.getFishOrganismFromTable(this.soldFishOrganismTable.rows);
        model.unrealizedFishOrganism = this.getFishOrganismFromTable(this.unrealizedFishOrganismTable.rows);
        model.broodstock = this.getBroodstockFromTable();

        if (this.systemFullTable !== undefined && this.systemFullTable.rows.length !== 0) {
            model.installationSystemFull = this.getSystemFullFromTable();
            model.installationSystemNotFull = undefined;
        }

        if (this.systemNotFullTable !== undefined && this.systemNotFullTable.rows.length !== 0) {
            model.installationSystemNotFull = this.getSystemNotFullFromTable();
            model.installationSystemFull = undefined;
        }
    }

    private fillModelRegister(model: StatisticalFormAquaFarmEditDTO): void {
        model.submittedFor = this.form.get('submittedForControl')!.value;
        model.aquacultureFacilityId = this.form.get('aquacultureFacilityControl')!.value!.value;
        model.year = (this.form.get('yearControl')!.value as Date)!.getFullYear();
        model.formNum = this.form.get('formNumControl')!.value;
        model.breedingMaterialDeathRate = this.form.get('breedingMaterialDeathRateControl')!.value;
        model.consumationFishDeathRate = this.form.get('consumationFishDeathRateControl')!.value;
        model.broodstockDeathRate = this.form.get('broodstockDeathRateControl')!.value;
        model.isOwnerEmployee = this.form.get('isOwnerEmployeeControl')!.value!.value === 'yes';

        model.employeeInfoGroups = this.getEmployeeInfo(model);
        model.numStatGroups = [...this.getEmployeeStats(model), ...this.getFinancialInfo(model), ...this.getRawMaterial(model)];

        model.medicineComments = this.form.get('medicineCommentsControl')!.value;
        model.files = this.form.get('filesControl')!.value;

        model.medicine = this.getMedicineFromTable();
        model.producedFishOrganism = this.getFishOrganismFromTable(this.producedFishOrganismTable.rows);
        model.soldFishOrganism = this.getFishOrganismFromTable(this.soldFishOrganismTable.rows);
        model.unrealizedFishOrganism = this.getFishOrganismFromTable(this.unrealizedFishOrganismTable.rows);
        model.broodstock = this.getBroodstockFromTable();

        if (this.systemFullTable !== undefined && this.systemFullTable.rows.length !== 0) {
            model.installationSystemFull = this.getSystemFullFromTable();
            model.installationSystemNotFull = undefined;
        }

        if (this.systemNotFullTable !== undefined && this.systemNotFullTable.rows.length !== 0) {
            model.installationSystemNotFull = this.getSystemNotFullFromTable();
            model.installationSystemFull = undefined;
        }
    }

    private getEmployeeInfo(model: StatisticalFormReworkApplicationEditDTO | StatisticalFormReworkEditDTO): StatisticalFormEmployeeInfoGroupDTO[] {
        const employeeTypes = this.employeeInfoFormArray.value as StatisticalFormEmployeeInfoDTO[][];

        const result: StatisticalFormEmployeeInfoGroupDTO[] = [];
        for (let i = 0; i < model.employeeInfoGroups!.length; ++i) {
            model.employeeInfoGroups![i].employeeTypes = employeeTypes[i];
            result.push(model.employeeInfoGroups![i]);
        }

        return result;
    }

    private getEmployeeStats(model: StatisticalFormReworkApplicationEditDTO | StatisticalFormReworkEditDTO): StatisticalFormNumStatGroupDTO[] {
        const statTypes: number[] = this.employeeStatsFormArray.value as number[];

        const result: StatisticalFormNumStatGroupDTO[] = [];
        for (const group of model.numStatGroups!) {
            const idx: number[] | undefined = this.employeeStatsGroups.get(group.id!);

            if (idx !== undefined) {
                result.push(group);

                const values: number[] = statTypes.slice(idx[0], idx[0] + idx.length);
                for (let i = 0; i < group.numericStatTypes!.length; ++i) {
                    group.numericStatTypes![i].statValue = values[i];
                }
            }
        }
        return result;
    }

    private getFinancialInfo(model: StatisticalFormReworkApplicationEditDTO | StatisticalFormReworkEditDTO): StatisticalFormNumStatGroupDTO[] {
        const statTypes = this.financialInfoFormArray.value as StatisticalFormNumStatDTO[][];

        const result: StatisticalFormNumStatGroupDTO[] = [];
        for (const statType of statTypes) {
            const group: StatisticalFormNumStatGroupDTO | undefined = model.numStatGroups!.find(x => x.id === statType[0].groupId);
            if (group !== undefined) {
                result.push(group);
                group.numericStatTypes = statType;
            }
        }

        return result;
    }

    private getRawMaterial(model: StatisticalFormReworkApplicationEditDTO | StatisticalFormReworkEditDTO): StatisticalFormNumStatGroupDTO[] {
        const statTypes = this.form.get('rawMaterialControl')!.value as StatisticalFormNumStatDTO[];

        const group: StatisticalFormNumStatGroupDTO = model.numStatGroups!.find(x => x.groupType === NumericStatTypeGroupsEnum.RawMatAq)!;
        group.numericStatTypes = statTypes;

        return [group];
    }

    private markAllAsTouched(): void {
        this.form.markAllAsTouched();
    }

    private saveAquaFarmForm(dialogClose: DialogCloseCallback, saveAsDraft: boolean = false): Observable<boolean> {
        const saveOrEditDone: Subject<boolean> = new Subject<boolean>();

        this.saveOrEdit(saveAsDraft).subscribe({
            next: (id: number | void) => {
                if (typeof id === 'number' && id !== undefined) {
                    this.model.id = id;
                    dialogClose(this.model);
                }
                else {
                    dialogClose(this.model);
                }

                saveOrEditDone.next(true);
                saveOrEditDone.complete();
            }
        });

        return saveOrEditDone;
    }

    private saveOrEdit(saveAsDraft: boolean): Observable<number | void> {
        this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model);

        if (this.model instanceof StatisticalFormAquaFarmEditDTO) {
            if (this.formId !== undefined) {
                return this.service.editStatisticalFormAquaFarm(this.model);
            }
            return this.service.addStatisticalFormAquaFarm(this.model);
        }
        else {
            if (this.model.id !== undefined && this.model.id !== null) {
                return this.service.editApplication(this.model, this.pageCode, saveAsDraft);
            }
            return this.service.addApplication(this.model, this.pageCode);
        }
    }

    private getMedicineFromTable(): StatisticalFormGivenMedicineDTO[] {
        const rows = this.medicineTable.rows as StatisticalFormGivenMedicineDTO[];

        return rows.map(x => new StatisticalFormGivenMedicineDTO({
            id: x.id,
            medicineType: x.medicineType,
            grams: x.grams,
            isActive: x.isActive ?? true
        }));
    }

    private getFishOrganismFromTable(rows: StatisticalFormAquaFarmFishOrganismDTO[]): StatisticalFormAquaFarmFishOrganismDTO[] {
        return rows.map(x => new StatisticalFormAquaFarmFishOrganismDTO({
            id: x.id,
            installationTypeId: x.installationTypeId,
            fishTypeId: x.fishTypeId,
            fishLarvaeCount: x.fishLarvaeCount,
            oneStripBreedingMaterialCount: x.oneStripBreedingMaterialCount,
            oneStripBreedingMaterialWeight: x.oneStripBreedingMaterialWeight,
            oneYearBreedingMaterialCount: x.oneYearBreedingMaterialCount,
            oneYearBreedingMaterialWeight: x.oneYearBreedingMaterialWeight,
            forConsumption: x.forConsumption,
            caviarForConsumption: x.caviarForConsumption,
            isActive: x.isActive ?? true
        }));
    }

    private getBroodstockFromTable(): StatisticalFormAquaFarmBroodstockDTO[] {
        const rows = this.broodstockTable.rows as StatisticalFormAquaFarmBroodstockDTO[];

        return rows.map(x => new StatisticalFormAquaFarmBroodstockDTO({
            id: x.id,
            installationTypeId: x.installationTypeId,
            fishTypeId: x.fishTypeId,
            maleCount: x.maleCount,
            maleWeight: x.maleWeight,
            maleAge: x.maleAge,
            femaleCount: x.femaleCount,
            femaleWeight: x.femaleWeight,
            femaleAge: x.femaleAge,
            isActive: x.isActive ?? true
        }));
    }

    private getSystemFullFromTable(): StatisticalFormAquaFarmInstallationSystemFullDTO[] {
        const rows = this.systemFullTable.rows as StatisticalFormAquaFarmInstallationSystemFullDTO[];

        return rows.map(x => new StatisticalFormAquaFarmInstallationSystemFullDTO({
            id: x.id,
            installationTypeId: x.installationTypeId,
            isInstallationUsed: x.isInstallationUsed ?? false,
            isActive: x.isActive ?? true
        }));
    }

    private getSystemNotFullFromTable(): StatisticalFormAquaFarmInstallationSystemNotFullDTO[] {
        const rows = this.systemNotFullTable.rows as StatisticalFormAquaFarmInstallationSystemNotFullDTO[];

        return rows.map(x => new StatisticalFormAquaFarmInstallationSystemNotFullDTO({
            id: x.id,
            installationTypeId: x.installationTypeId,
            isInstallationUsedBreedingMaterial: x.isInstallationUsedBreedingMaterial ?? false,
            isInstallationUsedConsumationFish: x.isInstallationUsedConsumationFish ?? false,
            isActive: x.isActive ?? true
        }));
    }

    private initTableFormGroups(): void {
        this.medicineGroup = new FormGroup({
            medicineTypeControl: new FormControl(null, Validators.required),
            gramsControl: new FormControl(null, [Validators.required, TLValidators.number(0)])
        });

        this.producedFishOrganismGroup = new FormGroup({
            installationTypeIdControl: new FormControl(null, Validators.required),
            fishTypeIdControl: new FormControl(null, Validators.required),
            fishLarvaeCountControl: new FormControl(null, TLValidators.number(0)),
            oneStripBreedingMaterialCountControl: new FormControl(null, TLValidators.number(0)),
            oneStripBreedingMaterialWeightControl: new FormControl(null, TLValidators.number(0)),
            oneYearBreedingMaterialCountControl: new FormControl(null, TLValidators.number(0)),
            oneYearBreedingMaterialWeightControl: new FormControl(null, TLValidators.number(0)),
            forConsumptionControl: new FormControl(null, TLValidators.number(0)),
            caviarForConsumptionControl: new FormControl(null, TLValidators.number(0))
        });

        this.soldFishOrganismGroup = new FormGroup({
            installationTypeIdControl: new FormControl(null, Validators.required),
            fishTypeIdControl: new FormControl(null, Validators.required),
            fishLarvaeCountControl: new FormControl(null, TLValidators.number(0)),
            oneStripBreedingMaterialCountControl: new FormControl(null, TLValidators.number(0)),
            oneStripBreedingMaterialWeightControl: new FormControl(null, TLValidators.number(0)),
            oneYearBreedingMaterialCountControl: new FormControl(null, TLValidators.number(0)),
            oneYearBreedingMaterialWeightControl: new FormControl(null, TLValidators.number(0)),
            forConsumptionControl: new FormControl(null, TLValidators.number(0)),
            caviarForConsumptionControl: new FormControl(null, TLValidators.number(0))
        });

        this.unrealizedFishOrganismGroup = new FormGroup({
            installationTypeIdControl: new FormControl(null, Validators.required),
            fishTypeIdControl: new FormControl(null, Validators.required),
            oneStripBreedingMaterialCountControl: new FormControl(null, TLValidators.number(0)),
            oneStripBreedingMaterialWeightControl: new FormControl(null, TLValidators.number(0)),
            oneYearBreedingMaterialCountControl: new FormControl(null, TLValidators.number(0)),
            oneYearBreedingMaterialWeightControl: new FormControl(null, TLValidators.number(0)),
            forConsumptionControl: new FormControl(null, TLValidators.number(0)),
            caviarForConsumptionControl: new FormControl(null, TLValidators.number(0))
        });

        this.broodstockGroup = new FormGroup({
            installationTypeIdControl: new FormControl(null, Validators.required),
            fishTypeIdControl: new FormControl(null, Validators.required),
            maleCountControl: new FormControl(null, TLValidators.number(0)),
            maleWeightControl: new FormControl(null, TLValidators.number(0)),
            maleAgeControl: new FormControl(null, TLValidators.number(0)),
            femaleCountControl: new FormControl(null, TLValidators.number(0)),
            femaleWeightControl: new FormControl(null, TLValidators.number(0)),
            femaleAgeControl: new FormControl(null, TLValidators.number(0))
        });

        this.installationSystemFullGroup = new FormGroup({
            installationTypeIdControl: new FormControl({ value: null, disabled: true }, Validators.required),
            isInstallationUsedControl: new FormControl()
        });

        this.installationSystemNotFullGroup = new FormGroup({
            installationTypeIdControl: new FormControl({ value: null, disabled: true }, Validators.required),
            isInstallationUsedBreedingMaterialControl: new FormControl(),
            isInstallationUsedConsumationFishControl: new FormControl()
        });
    }

    private validateRows(table: TLDataTableComponent) {
        this.fishTypes = [...this.allFishTypes];
        this.installationTypes = [...this.allInstallationTypes];
        this.tableMap = new Map<number, number[]>();
        table.rows.map((x: Record<string, number>) => {
            if (this.tableMap.has(x.installationTypeId)) {
                this.tableMap.get(x.installationTypeId)?.push(x.fishTypeId);
            } else {
                this.tableMap.set(x.installationTypeId, [x.fishTypeId]);
            }
        });

        const currentIds: number[] = [];

        this.tableMap.forEach((value: number[], key: number) => {
            if (value.length === this.allFishTypes.length) {
                currentIds.push(key);
                this.tableMap.delete(key);
                if (this.tableMap.size === 0) {
                    this.fishTypes = [];
                }
            } else {
                value = value.filter(x => !value.includes(x));
            }
        });

        this.installationTypes = this.installationTypes.filter(x => !currentIds.includes(x.value!));
        this.installationTypes = this.installationTypes.slice();
    }

    private payColumnCountValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const indices: Record<number, boolean> = {};

            const stats: StatisticalFormEmployeeInfoDTO[][] | undefined = this.employeeInfoFormArray?.value;
            if (stats !== undefined && stats !== null) {
                for (let i = 1; i < stats.length; ++i) {
                    if (!this.areEmployeeInfoColumnsEqual(stats[i - 1], stats[i])) {
                        indices[i] = true;
                    }
                }

                if (Object.keys(indices).length > 0) {
                    return { 'columnCountNotEqual': indices };
                }
            }
            return null;
        };
    }

    private costsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const indices: Record<number, boolean> = {};

            const stats: StatisticalFormEmployeeInfoDTO[][] | undefined = this.employeeInfoFormArray?.value;
            const financials: StatisticalFormNumStatDTO[][] | undefined = this.financialInfoFormArray?.value;

            if (stats !== undefined && stats !== null) {
                for (let l = 0; l < stats.length; ++l) {
                    if (this.notEmpty(stats[l])) {
                        if (financials !== undefined && financials !== null) {
                            for (let i = 0; i < financials.length; ++i) {
                                for (let j = 0; j < financials[i].length; ++j) {
                                    if (financials[i][j].code === 'StaffAq') {
                                        if (Number(financials[i][j].statValue) === Number(0)) {
                                            indices[i] = true;
                                        }
                                    }
                                }
                            }
                        }

                    }
                }

                if (Object.keys(indices).length > 0) {
                    return { 'costsIsEmpty': indices };
                }
            }
            return null;
        };
    }

    private areEmployeeInfoColumnsEqual(lhs: StatisticalFormEmployeeInfoDTO[], rhs: StatisticalFormEmployeeInfoDTO[]): boolean {
        const lhsMenWithPay: number = lhs.map(x => x.menWithPay!).reduce((sum, a) => sum + a, 0);
        const lhsMenWithoutPay: number = lhs.map(x => x.menWithoutPay!).reduce((sum, a) => sum + a, 0);
        const lhsWomenWithPay: number = lhs.map(x => x.womenWithPay!).reduce((sum, a) => sum + a, 0);
        const lhsWomenWithoutPay: number = lhs.map(x => x.womenWithoutPay!).reduce((sum, a) => sum + a, 0);

        const rhsMenWithPay: number = rhs.map(x => x.menWithPay!).reduce((sum, a) => sum + a, 0);
        const rhsMenWithoutPay: number = rhs.map(x => x.menWithoutPay!).reduce((sum, a) => sum + a, 0);
        const rhsWomenWithPay: number = rhs.map(x => x.womenWithPay!).reduce((sum, a) => sum + a, 0);
        const rhsWomenWithoutPay: number = rhs.map(x => x.womenWithoutPay!).reduce((sum, a) => sum + a, 0);

        return lhsMenWithPay === rhsMenWithPay
            && lhsMenWithoutPay === rhsMenWithoutPay
            && lhsWomenWithPay === rhsWomenWithPay
            && lhsWomenWithoutPay === rhsWomenWithoutPay;
    }

    private notEmpty(stat: StatisticalFormEmployeeInfoDTO[]): boolean {
        const menWithPay: number = stat.map(x => x.menWithPay!).reduce((sum, a) => sum + a, 0);
        const womenWithPay: number = stat.map(x => x.womenWithPay!).reduce((sum, a) => sum + a, 0);

        return menWithPay !== 0 || womenWithPay !== 0;
    }
}