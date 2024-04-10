import { Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { forkJoin, Observable, Subject, Subscription } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IStatisticalFormsService } from '@app/interfaces/common-app/statistical-forms.interface';
import { StatisticalFormReworkEditDTO } from '@app/models/generated/dtos/StatisticalFormReworkEditDTO';
import { StatisticalFormReworkApplicationEditDTO } from '@app/models/generated/dtos/StatisticalFormReworkApplicationEditDTO';
import { StatisticalFormReworkRegixDataDTO } from '@app/models/generated/dtos/StatisticalFormReworkRegixDataDTO';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { StatisticalFormReworkRawMaterialDTO } from '@app/models/generated/dtos/StatisticalFormReworkRawMaterialDTO';
import { StatisticalFormReworkProductDTO } from '@app/models/generated/dtos/StatisticalFormReworkProductDTO';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { ApplicationSubmittedByRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedByRegixDataDTO';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedForRegixDataDTO';
import { AddressTypesEnum } from '@app/enums/address-types.enum';
import { EditStatisticalFormReworkDialogParams } from '../../models/edit-statistical-form-rework-dialog-params.model';
import { StatisticalFormRawMaterialOriginEnum } from '@app/enums/statistical-form-raw-material-origin.enum';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { StatisticalFormEmployeeInfoDTO } from '@app/models/generated/dtos/StatisticalFormEmployeeInfoDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { NumericStatTypeGroupsEnum } from '@app/enums/numeric-stat-type-groups.enum';
import { StatisticalFormEmployeeInfoGroupDTO } from '@app/models/generated/dtos/StatisticalFormEmployeeInfoGroupDTO';
import { StatisticalFormNumStatGroupDTO } from '@app/models/generated/dtos/StatisticalFormNumStatGroupDTO';
import { StatisticalFormNumStatDTO } from '@app/models/generated/dtos/StatisticalFormNumStatDTO';
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
    selector: 'statistical-forms-rework',
    templateUrl: './statistical-forms-rework.component.html',
    styleUrls: ['../../statistical-forms-content.component.scss']
})
export class StatisticalFormsReworkComponent implements OnInit, IDialogComponent {
    public readonly pageCode: PageCodeEnum = PageCodeEnum.StatFormRework;
    public readonly companyHeadquartersType: AddressTypesEnum = AddressTypesEnum.COMPANY_HEADQUARTERS;
    public readonly currentDate: Date = new Date();

    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;
    public rawMaterialGroup!: FormGroup;
    public productGroup!: FormGroup;
    public service!: IStatisticalFormsService;

    public isPublicApp: boolean = false;
    public isOnlineApplication: boolean = false;
    public readOnly: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public showRegiXData: boolean = false;
    public isApplication: boolean = false;
    public loadRegisterFromApplication: boolean = false;
    public viewMode: boolean = false;
    public isEditing: boolean = false;
    public isReadonly: boolean = false;
    public isRegisterEntry: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();

    public fishTypes: NomenclatureDTO<number>[] = [];
    public allFishTypes: NomenclatureDTO<number>[] = [];
    public productTypes: NomenclatureDTO<number>[] = [];
    public allProductTypes: NomenclatureDTO<number>[] = [];
    public rawMaterials: StatisticalFormReworkRawMaterialDTO[] = [];
    public rawMaterialOrigin: NomenclatureDTO<StatisticalFormRawMaterialOriginEnum>[] = [];
    public allRawMaterialOrigin: NomenclatureDTO<StatisticalFormRawMaterialOriginEnum>[] = [];
    public products: StatisticalFormReworkProductDTO[] = [];
    public ownerEmployeeOptions: NomenclatureDTO<YesNo>[] = [];

    public notifier: Notifier = new Notifier();
    public regixChecks: ApplicationRegiXCheckDTO[] = [];
    public expectedResults: StatisticalFormReworkRegixDataDTO;

    @ViewChild('rawMaterialTable')
    private rawMaterialTable!: TLDataTableComponent;

    @ViewChild('productTable')
    private productTable!: TLDataTableComponent;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private isApplicationHistoryMode: boolean = false;
    private dialogRightSideActions: IActionInfo[] | undefined;
    private applicationsService: IApplicationsService | undefined;
    private formId: number | undefined;
    private applicationId: number | undefined;
    private model!: StatisticalFormReworkEditDTO | StatisticalFormReworkApplicationEditDTO | StatisticalFormReworkRegixDataDTO;
    private rawMaterialsMap = new Map<number, { origin: StatisticalFormRawMaterialOriginEnum, isActive: boolean }[]>();

    private get employeeStatsFormArray(): FormArray {
        return this.form?.get('employeeStatsArray') as FormArray;
    }

    private get employeeInfoFormArray(): FormArray {
        return this.form?.get('employeeInfoArray') as FormArray;
    }

    private get financialInfoFormArray(): FormArray {
        return this.form?.get('financialInfoArray') as FormArray;
    }

    private employeeStatsGroups: Map<number, number[]> = new Map<number, number[]>();

    private employeeStatsLabels: string[] = [];
    private employeeInfoLabels: string[] = [];
    private financialInfoGroupNames: string[] = [];

    private readonly employeeGroupTypes: NumericStatTypeGroupsEnum[] = [NumericStatTypeGroupsEnum.FreeLabor, NumericStatTypeGroupsEnum.NumHours];

    private readonly nomenclatures: CommonNomenclatures;
    private readonly loader: FormControlDataLoader;

    public constructor(translate: FuseTranslationLoaderService, nomenclatures: CommonNomenclatures) {
        this.translate = translate;
        this.nomenclatures = nomenclatures;

        this.isPublicApp = IS_PUBLIC_APP;

        this.expectedResults = new StatisticalFormReworkRegixDataDTO({
            submittedBy: new ApplicationSubmittedByRegixDataDTO({
                person: new RegixPersonDataDTO(),
                addresses: []
            }),
            submittedFor: new ApplicationSubmittedForRegixDataDTO({
                person: new RegixPersonDataDTO(),
                addresses: []
            })
        });

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
        this.productTypes = [];

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

    public ngOnInit(): void {
        this.loader.load();

        // извличане на исторически данни за заявление
        if (this.isApplicationHistoryMode && this.applicationId !== undefined) {
            this.form.disable();

            if (this.applicationsService) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const statisticalForm: StatisticalFormReworkApplicationEditDTO = new StatisticalFormReworkApplicationEditDTO(contentObject);
                        statisticalForm.files = content.files;
                        statisticalForm.applicationId = content.applicationId;

                        this.isOnlineApplication = statisticalForm.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.model = statisticalForm;
                        this.fillForm();

                        if (!this.isPublicApp && content.latestRegiXChecks !== undefined && content.latestRegiXChecks !== null && content.latestRegiXChecks.length > 0) {
                            this.showRegiXData = true;

                            setTimeout(() => {
                                this.regixChecks = content.latestRegiXChecks!;
                            }, 100);
                        }
                    }
                });
            }
        }
        else if (this.applicationId !== undefined && this.formId === undefined && !this.isApplication) {
            // извличане на данни за регистров запис от id на заявление
            if (this.loadRegisterFromApplication === true) {
                this.isEditing = true;

                this.service.getRegisterByApplicationId(this.applicationId, this.pageCode).subscribe({
                    next: (statisticalForm: unknown) => {
                        this.model = statisticalForm as StatisticalFormReworkEditDTO;
                        this.isOnlineApplication = (this.model as StatisticalFormReworkEditDTO).isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.fillForm();
                    }
                });
            }
            else {
                // извличане на данни за създаване на регистров запис от заявление
                this.isEditing = false;

                this.service.getApplicationDataForRegister(this.applicationId, this.pageCode).subscribe({
                    next: (statisticalForm: StatisticalFormReworkEditDTO) => {
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
                this.isEditing = false;

                // извличане на данни за RegiX сверка от служител
                if (this.showOnlyRegiXData) {
                    this.service.getRegixData(this.applicationId, this.pageCode).subscribe({
                        next: (regixData: RegixChecksWrapperDTO<StatisticalFormReworkRegixDataDTO>) => {
                            this.model = new StatisticalFormReworkRegixDataDTO(regixData.dialogDataModel);
                            this.expectedResults = new StatisticalFormReworkRegixDataDTO(regixData.regiXDataModel);

                            this.fillForm();
                        }
                    });
                }
                else {
                    // извличане на данни за заявление
                    this.isEditing = false;

                    this.service.getApplication(this.applicationId, this.showRegiXData, this.pageCode).subscribe({
                        next: (statisticalForm: StatisticalFormReworkApplicationEditDTO) => {
                            statisticalForm.applicationId = this.applicationId;

                            this.isOnlineApplication = statisticalForm.isOnlineApplication!;
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new StatisticalFormReworkRegixDataDTO(statisticalForm.regiXDataModel);
                                statisticalForm.regiXDataModel = undefined;
                            }

                            if (this.isPublicApp && this.isOnlineApplication && (statisticalForm.submittedBy === undefined || statisticalForm.submittedBy === null)) {
                                const service = this.service as StatisticalFormsPublicService;
                                service.getCurrentUserAsSubmittedBy(StatisticalFormTypesEnum.Rework).subscribe({
                                    next: (submittedBy: ApplicationSubmittedByDTO) => {
                                        statisticalForm.submittedBy = submittedBy;
                                        this.model = statisticalForm;
                                        this.fillForm();
                                    }
                                });
                            }
                            else {
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

                this.service.getStatisticalFormRework(this.formId).subscribe({
                    next: (statisticalForm: StatisticalFormReworkEditDTO) => {
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
            this.productGroup?.get('productTypeIdControl')?.valueChanges.subscribe({
                next: () => {
                    this.productTypes = [...this.allProductTypes];
                    const currentIds: number[] = this.productTable.rows.filter(x => x.isActive !== false).map(x => x.productTypeId);

                    this.productTypes = this.productTypes.filter(x => !currentIds.includes(x.value!));
                    this.productTypes = this.productTypes.slice();
                }
            });

            this.rawMaterialGroup?.get('fishTypeIdControlHidden')?.valueChanges.subscribe({
                next: () => {
                    this.validateRows();

                    if (this.rawMaterialGroup?.get('fishTypeIdControlHidden')!.value !== undefined && this.rawMaterialsMap.size !== 0) {
                        const origins: StatisticalFormRawMaterialOriginEnum[] | undefined = this.rawMaterialsMap
                            .get(this.rawMaterialGroup?.get('fishTypeIdControlHidden')!.value.value)
                            ?.filter(x => x.isActive)
                            ?.map(x => x.origin);

                        this.rawMaterialOrigin = this.rawMaterialOrigin.filter(x => !origins?.includes(x.value!));
                    }
                }
            });

            this.rawMaterialGroup.get('originControlHidden')?.valueChanges.subscribe({
                next: () => {
                    this.validateRows();
                }
            });
        }
    }

    public setData(data: EditStatisticalFormReworkDialogParams, buttons: DialogWrapperData): void {
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

        if (data.model !== undefined && data.model !== null) {
            this.model = data.model;
        }
        this.buildForm();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.saveReworkForm(dialogClose);
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        let applicationAction: boolean = false;

        if (this.model instanceof StatisticalFormReworkApplicationEditDTO || this.model instanceof StatisticalFormReworkRegixDataDTO) {
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
                saveFn: this.saveReworkForm.bind(this),
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

    public fileTypeFilterFn(options: PermittedFileTypeDTO[]): PermittedFileTypeDTO[] {
        const pdfs: FileTypeEnum[] = [FileTypeEnum.SIGNEDAPPL, FileTypeEnum.APPLICATION_PDF];
        const offlines: FileTypeEnum[] = [FileTypeEnum.PAYEDFEE, FileTypeEnum.SCANNED_FORM];

        let result: PermittedFileTypeDTO[] = options;

        if (this.isApplication || !this.isOnlineApplication) {
            result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        if (this.isOnlineApplication) {
            result = result.filter(x => !offlines.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        return result;
    }

    public rawMaterialsActiveRecordChanged(): void {
        if (this.fishTypes.length === 1 && !this.rawMaterialGroup.get('fishTypeIdControlHidden')!.value) {
            this.rawMaterialGroup.get('fishTypeIdControlHidden')!.setValue(this.fishTypes[0]);
        }
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
                yearControl: new FormControl(null, Validators.required),
                vetRegistrationNumControl: new FormControl(null),
                licenceNumControl: new FormControl(null),
                licenceDateControl: new FormControl(null),
                totalRawMaterialTonsControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
                totalReworkedProductTonsControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
                totalYearTurnoverControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
                isOwnerEmployeeControl: new FormControl(null, Validators.required),
                employeeInfoArray: new FormArray([], this.payColumnCountValidator()),
                employeeStatsArray: new FormArray([]),
                financialInfoArray: new FormArray([], this.costsValidator()),
                filesControl: new FormControl()
            });

            this.rawMaterialGroup = new FormGroup({
                fishTypeIdControl: new FormControl(null, Validators.required),
                tonsControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
                originControl: new FormControl(null, Validators.required),
                countryZoneControl: new FormControl(null, Validators.maxLength(100))
            });

            this.productGroup = new FormGroup({
                productTypeIdControl: new FormControl(null, Validators.required),
                tonsControl: new FormControl(null, [Validators.required, TLValidators.number(0)])
            });
        }
        else {
            this.form = new FormGroup({
                submittedForControl: new FormControl(),
                yearControl: new FormControl(),
                vetRegistrationNumControl: new FormControl(null),
                licenceNumControl: new FormControl(null),
                licenceDateControl: new FormControl(null),
                totalRawMaterialTonsControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
                totalReworkedProductTonsControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
                totalYearTurnoverControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
                isOwnerEmployeeControl: new FormControl(null, Validators.required),
                employeeInfoArray: new FormArray([], this.payColumnCountValidator()),
                employeeStatsArray: new FormArray([]),
                financialInfoArray: new FormArray([], this.costsValidator()),
                filesControl: new FormControl()
            });

            this.rawMaterialGroup = new FormGroup({
                fishTypeIdControl: new FormControl(null, Validators.required),
                tonsControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
                originControl: new FormControl(null, Validators.required),
                countryZoneControl: new FormControl(null, Validators.maxLength(100))
            });

            this.productGroup = new FormGroup({
                productTypeIdControl: new FormControl(null, Validators.required),
                tonsControl: new FormControl(null, [Validators.required, TLValidators.number(0)])
            });
        }

        this.form.get('employeeInfoArray')?.valueChanges.subscribe({
            next: () => {
                this.form.get('financialInfoArray')!.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    private fillForm(): void {
        if (this.model instanceof StatisticalFormReworkRegixDataDTO) {
            this.form.get('submittedByControl')!.setValue(this.model.submittedBy);
            this.form.get('submittedForControl')!.setValue(this.model.submittedFor);

            this.fillFormRegiX(this.model);
        }
        else if (this.model instanceof StatisticalFormReworkApplicationEditDTO) {
            this.fillFormApplication(this.model);

            if (this.showRegiXData) {
                this.fillFormRegiX(this.model);
            }
        }
        else if (this.model instanceof StatisticalFormReworkEditDTO) {
            this.fillFormRegister(this.model);
        }

        if (this.isReadonly || this.viewMode) {
            this.form.disable();
        }
    }

    private fillFormRegiX(model: StatisticalFormReworkRegixDataDTO): void {
        if (model.applicationRegiXChecks !== undefined && model.applicationRegiXChecks) {
            const applicationRegiXChecks: ApplicationRegiXCheckDTO[] = model.applicationRegiXChecks;

            setTimeout(() => {
                this.regixChecks = applicationRegiXChecks;
            });

            model.applicationRegiXChecks = undefined;
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

    private fillFormApplication(model: StatisticalFormReworkApplicationEditDTO): void {
        this.form.get('submittedByControl')!.setValue(model.submittedBy);
        this.form.get('submittedByWorkPositionControl')!.setValue(model.submittedByWorkPosition);
        this.form.get('submittedForControl')!.setValue(model.submittedFor);

        if (model.year !== null && model.year !== undefined) {
            this.form.get('yearControl')!.setValue(new Date(model.year, 0, 1));
        }

        this.form.get('vetRegistrationNumControl')!.setValue(model.vetRegistrationNum);
        this.form.get('licenceNumControl')!.setValue(model.licenceNum);
        this.form.get('licenceDateControl')!.setValue(model.licenceDate);
        this.form.get('totalRawMaterialTonsControl')!.setValue(model.totalRawMaterialTons);
        this.form.get('totalReworkedProductTonsControl')!.setValue(model.totalReworkedProductTons);
        this.form.get('totalYearTurnoverControl')!.setValue(model.totalYearTurnover);
        this.form.get('filesControl')!.setValue(model.files);

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

        const statTypes: StatisticalFormNumStatDTO[] = [];

        for (const group of (model.numStatGroups ?? []).filter(x => this.employeeGroupTypes.includes(x.groupType!))) {
            for (const statType of group.numericStatTypes ?? []) {
                statTypes.push(statType);
            }
        }

        let idx: number = 0;
        for (const statType of statTypes.sort((lhs, rhs) => lhs.orderNum! - rhs.orderNum!)) {
            this.employeeStatsLabels.push(statType.name!);
            this.employeeStatsFormArray.push(new FormControl(statType.statValue, [Validators.required, TLValidators.number(0)]));

            const group: number[] | undefined = this.employeeStatsGroups.get(statType.groupId!);
            if (group === undefined) {
                this.employeeStatsGroups.set(statType.groupId!, [idx]);
            }
            else {
                this.employeeStatsGroups.set(statType.groupId!, [...group, idx]);
            }
            ++idx;
        }

        for (const group of (model.numStatGroups ?? []).filter(x => !this.employeeGroupTypes.includes(x.groupType!))) {
            this.financialInfoGroupNames.push(group.groupName!);
            this.financialInfoFormArray.push(new FormControl(group.numericStatTypes, Validators.required));
        }

        for (const group of model.employeeInfoGroups ?? []) {
            this.employeeInfoLabels.push(group.groupName!);
            this.employeeInfoFormArray.push(new FormControl(group.employeeTypes));
        }

        setTimeout(() => {
            this.rawMaterials = model.rawMaterial ?? [];
            this.products = model.products ?? [];

            this.loader.load(() => {
                for (const product of this.products) {
                    if (product.isNewProductType === true) {
                        if (this.productTypes.findIndex(x => x.value === product.productTypeId) === -1) {
                            this.productTypes.push(new NomenclatureDTO<number>({
                                value: product.productTypeId,
                                displayName: product.productTypeName,
                                isActive: true
                            }));
                        }
                    }
                }
            });
        });
    }

    private fillFormRegister(model: StatisticalFormReworkEditDTO): void {
        this.form.get('submittedForControl')!.setValue(model.submittedFor);
        this.form.get('yearControl')!.setValue(new Date(model.year!, 0, 1));
        this.form.get('vetRegistrationNumControl')!.setValue(model.vetRegistrationNum);
        this.form.get('licenceNumControl')!.setValue(model.licenceNum);
        this.form.get('licenceDateControl')!.setValue(model.licenceDate);
        this.form.get('totalRawMaterialTonsControl')!.setValue(model.totalRawMaterialTons);
        this.form.get('totalReworkedProductTonsControl')!.setValue(model.totalReworkedProductTons);
        this.form.get('totalYearTurnoverControl')!.setValue(model.totalYearTurnover);

        this.form.get('filesControl')!.setValue(model.files);

        if (model.isOwnerEmployee) {
            this.form.get('isOwnerEmployeeControl')!.setValue(this.ownerEmployeeOptions.find(x => x.value === 'yes'));
        }
        else {
            this.form.get('isOwnerEmployeeControl')!.setValue(this.ownerEmployeeOptions.find(x => x.value === 'no'));
        }

        const statTypes: StatisticalFormNumStatDTO[] = [];

        for (const group of (model.numStatGroups ?? []).filter(x => this.employeeGroupTypes.includes(x.groupType!))) {
            for (const statType of group.numericStatTypes ?? []) {
                statTypes.push(statType);
            }
        }

        let idx: number = 0;
        for (const statType of statTypes.sort((lhs, rhs) => lhs.orderNum! - rhs.orderNum!)) {
            this.employeeStatsLabels.push(statType.name!);
            this.employeeStatsFormArray.push(new FormControl(statType.statValue, [Validators.required, TLValidators.number(0)]));

            const group: number[] | undefined = this.employeeStatsGroups.get(statType.groupId!);
            if (group === undefined) {
                this.employeeStatsGroups.set(statType.groupId!, [idx]);
            }
            else {
                this.employeeStatsGroups.set(statType.groupId!, [...group, idx]);
            }
            ++idx;
        }

        for (const group of (model.numStatGroups ?? []).filter(x => !this.employeeGroupTypes.includes(x.groupType!))) {
            this.financialInfoGroupNames.push(group.groupName!);
            this.financialInfoFormArray.push(new FormControl(group.numericStatTypes, Validators.required));
        }

        for (const group of model.employeeInfoGroups ?? []) {
            this.employeeInfoLabels.push(group.groupName!);
            this.employeeInfoFormArray.push(new FormControl(group.employeeTypes, Validators.required));
        }

        setTimeout(() => {
            this.rawMaterials = model.rawMaterial ?? [];
            this.products = model.products ?? [];

            this.loader.load(() => {
                for (const product of this.products) {
                    if (product.isNewProductType === true) {
                        if (this.productTypes.findIndex(x => x.value === product.productTypeId) === -1) {
                            this.productTypes.push(new NomenclatureDTO<number>({
                                value: product.productTypeId,
                                displayName: product.productTypeName,
                                isActive: true
                            }));
                        }
                    }
                }
            });
        });
    }

    private fillModel(): void {
        if (this.model instanceof StatisticalFormReworkRegixDataDTO) {
            this.fillModelRegix(this.model);
        }
        else if (this.model instanceof StatisticalFormReworkApplicationEditDTO) {
            this.fillModelApplication(this.model);
        }
        else if (this.model instanceof StatisticalFormReworkEditDTO) {
            this.fillModelRegister(this.model);
        }
    }

    private fillModelRegix(model: StatisticalFormReworkRegixDataDTO): void {
        model.submittedBy = this.form.get('submittedByControl')!.value;
        model.submittedFor = this.form.get('submittedForControl')!.value;
    }

    private fillModelApplication(model: StatisticalFormReworkApplicationEditDTO): void {
        model.submittedBy = this.form.get('submittedByControl')!.value;
        model.submittedByWorkPosition = this.form.get('submittedByWorkPositionControl')!.value;
        model.submittedFor = this.form.get('submittedForControl')!.value;
        model.year = (this.form.get('yearControl')!.value as Date)?.getFullYear();
        model.vetRegistrationNum = this.form.get('vetRegistrationNumControl')!.value;
        model.licenceNum = this.form.get('licenceNumControl')!.value;
        model.licenceDate = this.form.get('licenceDateControl')!.value;
        model.totalRawMaterialTons = this.form.get('totalRawMaterialTonsControl')!.value;
        model.totalReworkedProductTons = this.form.get('totalReworkedProductTonsControl')!.value;
        model.totalYearTurnover = this.form.get('totalYearTurnoverControl')!.value;
        model.files = this.form.get('filesControl')!.value;

        if (this.form.get('isOwnerEmployeeControl')!.value) {
            model.isOwnerEmployee = this.form.get('isOwnerEmployeeControl')!.value!.value === 'yes';
        }
        else {
            model.isOwnerEmployee = undefined;
        }

        model.rawMaterial = this.getRawMaterialFromTable();
        model.products = this.getProductFromTable();

        model.employeeInfoGroups = this.getEmployeeInfo(model);
        model.numStatGroups = [...this.getFinancialInfo(model), ...this.getEmployeeStats(model)];
    }

    private fillModelRegister(model: StatisticalFormReworkEditDTO): void {
        model.submittedFor = this.form.get('submittedForControl')!.value;
        model.year = (this.form.get('yearControl')!.value as Date)!.getFullYear();
        model.vetRegistrationNum = this.form.get('vetRegistrationNumControl')!.value;
        model.licenceNum = this.form.get('licenceNumControl')!.value;
        model.licenceDate = this.form.get('licenceDateControl')!.value;
        model.totalRawMaterialTons = this.form.get('totalRawMaterialTonsControl')!.value;
        model.totalReworkedProductTons = this.form.get('totalReworkedProductTonsControl')!.value;
        model.totalYearTurnover = this.form.get('totalYearTurnoverControl')!.value;
        model.files = this.form.get('filesControl')!.value;
        model.isOwnerEmployee = this.form.get('isOwnerEmployeeControl')!.value!.value === 'yes';

        model.rawMaterial = this.getRawMaterialFromTable();
        model.products = this.getProductFromTable();

        model.employeeInfoGroups = this.getEmployeeInfo(model);
        model.numStatGroups = [...this.getFinancialInfo(model), ...this.getEmployeeStats(model)];
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

    private getEmployeeStats(model: StatisticalFormReworkApplicationEditDTO | StatisticalFormReworkEditDTO): StatisticalFormNumStatGroupDTO[] {
        const statTypes: number[] = this.employeeStatsFormArray.value as number[];

        const result: StatisticalFormNumStatGroupDTO[] = [];
        for (const group of model.numStatGroups!) {
            const idx: number[] | undefined = this.employeeStatsGroups.get(group.id!);

            if (idx !== undefined) {
                result.push(group);

                let values: number[] = [];
                for (const i of idx) {
                    values.push(statTypes[i]);
                }

                for (let i = 0; i < group.numericStatTypes!.length; ++i) {
                    group.numericStatTypes![i].statValue = values[i];
                }
            }
        }
        return result;
    }

    private markAllAsTouched(): void {
        this.form.markAllAsTouched();
    }

    private saveReworkForm(dialogClose: DialogCloseCallback, saveAsDraft: boolean = false): Observable<boolean> {
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

                if (!(this.model instanceof StatisticalFormReworkRegixDataDTO)) {
                    if (this.model.products?.some(x => x.isNewProductType)) {
                        NomenclatureStore.instance.refreshNomenclature(
                            NomenclatureTypes.ReworkProductTypes,
                            this.service.getReworkProductTypes.bind(this.service)
                        );
                    }
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

        if (this.model instanceof StatisticalFormReworkEditDTO) {
            if (this.formId !== undefined) {
                return this.service.editStatisticalFormRework(this.model);
            }
            return this.service.confirmNoErrorsAndFillAdmAct(this.model.applicationId!, this.model, this.pageCode);
        }
        else {
            if (this.model.id !== undefined && this.model.id !== null) {
                return this.service.editApplication(this.model, this.pageCode, saveAsDraft);
            }
            return this.service.addApplication(this.model, this.pageCode);
        }
    }

    private getRawMaterialFromTable(): StatisticalFormReworkRawMaterialDTO[] {
        const rows = this.rawMaterialTable.rows as StatisticalFormReworkRawMaterialDTO[];

        const materials: StatisticalFormReworkRawMaterialDTO[] = rows.map(x => new StatisticalFormReworkRawMaterialDTO({
            id: x.id,
            fishTypeId: x.fishTypeId,
            origin: x.origin,
            tons: x.tons,
            countryZone: x.countryZone,
            isActive: x.isActive ?? true
        }));

        const result: StatisticalFormReworkRawMaterialDTO[] = [];

        for (const material of materials) {
            if (result.findIndex(x => x.fishTypeId === material.fishTypeId && x.origin === material.origin) === -1) {
                const original = materials.filter(x => x.fishTypeId === material.fishTypeId && x.origin === material.origin);

                if (original.length === 1) {
                    result.push(material);
                }
                else {
                    result.push(original.find(x => x.isActive === true)!);
                }
            }
        }

        return result;
    }

    private getProductFromTable(): StatisticalFormReworkProductDTO[] {
        const result: StatisticalFormReworkProductDTO[] = [];

        const rows = this.productTable.rows as StatisticalFormReworkProductDTO[];
        const startIndex: number = -100;

        const newProductTypeIds: number[] = rows
            .filter(x => typeof x.productTypeId !== 'string' && x.productTypeId! <= startIndex)
            .map(x => x.productTypeId!);

        let newProductIndex: number = newProductTypeIds.length > 0 ? Math.min(...newProductTypeIds) - 1 : startIndex;

        for (const product of rows) {
            const entry = new StatisticalFormReworkProductDTO({
                id: product.id,
                tons: product.tons,
                isActive: product.isActive ?? true
            });

            if (typeof product.productTypeId === 'string') {
                entry.productTypeId = newProductIndex--;
                entry.productTypeName = product.productTypeId;
                entry.isNewProductType = true;
            }
            else {
                entry.productTypeId = product.productTypeId;

                if (entry.productTypeId! <= startIndex) {
                    const nomenclature: NomenclatureDTO<number> | undefined = this.productTypes.find(x => x.value === product.productTypeId);
                    if (nomenclature !== undefined) {
                        entry.productTypeName = nomenclature.displayName;
                    }
                    else {
                        entry.productTypeName = product.productTypeName;
                    }

                    entry.isNewProductType = true;
                }
                else {
                    entry.isNewProductType = false;
                }
            }

            if (result.findIndex(x => x.productTypeId === product.productTypeId) === -1) {
                if (rows.length > 0) {
                    const original = rows.filter(x => x.productTypeId === product.productTypeId);

                    if (original.length === 1) {
                        result.push(entry);
                    }
                    else {
                        if (entry.isActive === true) {
                            result.push(entry);
                        }
                    }
                }
                else {
                    result.push(entry);
                }
            }
        }

        return result;
    }

    private getNomenclatures(): Subscription {
        const subscription: Subscription = forkJoin(
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.ReworkProductTypes, this.service.getReworkProductTypes.bind(this.service), false
            )
        ).subscribe({
            next: (data: (NomenclatureDTO<number> | FishNomenclatureDTO)[][]) => {
                this.allFishTypes = this.fishTypes = data[0];
                this.allProductTypes = this.productTypes = [...data[1]];

                this.loader.complete();
            }
        });

        this.allRawMaterialOrigin = this.rawMaterialOrigin = [
            new NomenclatureDTO<StatisticalFormRawMaterialOriginEnum>({
                value: StatisticalFormRawMaterialOriginEnum.Aqua,
                displayName: this.translate.getValue('statistical-forms.rework-aquaculture'),
                isActive: true
            }),
            new NomenclatureDTO<StatisticalFormRawMaterialOriginEnum>({
                value: StatisticalFormRawMaterialOriginEnum.Catch,
                displayName: this.translate.getValue('statistical-forms.rework-fishing'),
                isActive: true
            })
        ];

        return subscription;
    }

    private validateRows() {
        this.fishTypes = [...this.allFishTypes];
        this.rawMaterialOrigin = [...this.allRawMaterialOrigin];

        this.rawMaterialsMap = new Map<number, { origin: StatisticalFormRawMaterialOriginEnum, isActive: boolean }[]>();

        for (const x of this.rawMaterialTable.rows) {
            if (this.rawMaterialsMap.has(x.fishTypeId)) {
                const values: { origin: StatisticalFormRawMaterialOriginEnum, isActive: boolean }[] = this.rawMaterialsMap.get(x.fishTypeId)!;
                values.push({
                    origin: x.origin!,
                    isActive: x.isActive ?? true
                });

                this.rawMaterialsMap.set(x.fishTypeId, values);
            }
            else {
                this.rawMaterialsMap.set(x.fishTypeId!, [{
                    origin: x.origin!,
                    isActive: x.isActive ?? true
                }]);
            }
        }

        const excemptFishTypeIds: number[] = [];

        for (const [key, value] of this.rawMaterialsMap) {
            const count: number = value.filter(x => x.isActive).length;

            if (count === this.allRawMaterialOrigin.length) {
                excemptFishTypeIds.push(key);
            }
        }

        this.fishTypes = this.fishTypes.filter(x => !excemptFishTypeIds.includes(x.value!));
        this.fishTypes = this.fishTypes.slice();
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
                                    if (financials[i][j].code === 'StaffRe') {
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