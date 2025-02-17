﻿import { Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IStatisticalFormsService } from '@app/interfaces/common-app/statistical-forms.interface';
import { StatisticalFormFishVesselEditDTO } from '@app/models/generated/dtos/StatisticalFormFishVesselEditDTO';
import { StatisticalFormFishVesselApplicationEditDTO } from '@app/models/generated/dtos/StatisticalFormFishVesselApplicationEditDTO';
import { StatisticalFormFishVesselRegixDataDTO } from '@app/models/generated/dtos/StatisticalFormFishVesselRegixDataDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { forkJoin, Observable, Subject } from 'rxjs';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { ApplicationSubmittedByRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedByRegixDataDTO';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedForRegixDataDTO';
import { RegixLegalDataDTO } from '@app/models/generated/dtos/RegixLegalDataDTO';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { StatisticalFormsSeaDaysDTO } from '@app/models/generated/dtos/StatisticalFormsSeaDaysDTO';
import { StatisticalFormEmployeeInfoDTO } from '@app/models/generated/dtos/StatisticalFormEmployeeInfoDTO';
import { StatisticalFormEmployeeInfoGroupDTO } from '@app/models/generated/dtos/StatisticalFormEmployeeInfoGroupDTO';
import { StatisticalFormNumStatGroupDTO } from '@app/models/generated/dtos/StatisticalFormNumStatGroupDTO';
import { StatisticalFormNumStatDTO } from '@app/models/generated/dtos/StatisticalFormNumStatDTO';
import { StatisticalFormShipDTO } from '@app/models/generated/dtos/StatisticalFormShipDTO';
import { NumericStatTypeGroupsEnum } from '@app/enums/numeric-stat-type-groups.enum';
import { StatisticalFormReworkApplicationEditDTO } from '@app/models/generated/dtos/StatisticalFormReworkApplicationEditDTO';
import { StatisticalFormReworkEditDTO } from '@app/models/generated/dtos/StatisticalFormReworkEditDTO';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { StatisticalFormsPublicService } from '@app/services/public-app/statistical-forms-public.service';
import { StatisticalFormTypesEnum } from '@app/enums/statistical-form-types.enum';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { ShipsUtils } from '@app/shared/utils/ships.utils';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { StatisticalFormShipSeaDaysDTO } from '@app/models/generated/dtos/StatisticalFormShipSeaDaysDTO';
import { SubmittedByRolesEnum } from '@app/enums/submitted-by-roles.enum';

type YesNo = 'yes' | 'no';

@Component({
    selector: 'statistical-forms-fish-vessel',
    templateUrl: './statistical-forms-fish-vessel.component.html',
    styleUrls: ['../../statistical-forms-content.component.scss']
})
export class StatisticalFormsFishVesselComponent implements OnInit, IDialogComponent {
    public readonly pageCode: PageCodeEnum = PageCodeEnum.StatFormFishVessel;
    public readonly currentDate: Date = new Date();

    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;
    public readOnly: boolean = false;
    public lastYear: Date;

    public notifier: Notifier = new Notifier();
    public expectedResults: StatisticalFormFishVesselRegixDataDTO;
    public regixChecks: ApplicationRegiXCheckDTO[] = [];

    public activityPersonTypes: string[] = [];
    public fishingActivityTypes: string[] = [];
    public seaDaysTitle: string | undefined;

    public ships: ShipNomenclatureDTO[] = [];
    public fuelTypes: NomenclatureDTO<number>[] = [];
    public grossTonnageIntervals: NomenclatureDTO<number>[] = [];
    public shipLengthIntervals: NomenclatureDTO<number>[] = [];
    public hasEngine: boolean = false;
    public isShipHolderPartOfCrew: boolean = false;
    public allGearSeaDays: StatisticalFormsSeaDaysDTO[] = [];
    public gearSeaDays: StatisticalFormsSeaDaysDTO[] = [];
    public shipSeaDays: StatisticalFormShipSeaDaysDTO[] = [];
    public numericItemGroups: StatisticalFormNumStatGroupDTO[] = [];
    public employeeInfoGroups: StatisticalFormEmployeeInfoGroupDTO[] = [];
    public employeeInfo: StatisticalFormEmployeeInfoDTO[] = [];
    public numericItems: StatisticalFormNumStatDTO[] = [];

    public fishingMainActivityOptions: NomenclatureDTO<YesNo>[] = [];
    public holderPartOfCrewOptions: NomenclatureDTO<YesNo>[] = [];

    public isPublicApp: boolean = false;
    public isOnlineApplication: boolean = false;
    public isRegisterEntry: boolean = false;
    public isApplication: boolean = false;
    public loadRegisterFromApplication: boolean = false;
    public viewMode: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public showRegiXData: boolean = false;
    public isEditing: boolean = false;
    public isIdReadOnly: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();
    public statFormShip: Map<number, StatisticalFormShipDTO> = new Map<number, StatisticalFormShipDTO>();

    public showShipForm: boolean = false;

    public workHours!: number;
    public freeLabor!: number;

    public employeesWorkDays: StatisticalFormEmployeeInfoDTO[] = [];
    public employeeAge: StatisticalFormEmployeeInfoDTO[] = [];
    public employeesEducation: StatisticalFormEmployeeInfoDTO[] = [];
    public employeesNationality: StatisticalFormEmployeeInfoDTO[] = [];

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private service!: IStatisticalFormsService;

    private model!: StatisticalFormFishVesselEditDTO | StatisticalFormFishVesselApplicationEditDTO | StatisticalFormFishVesselRegixDataDTO;

    private id: number | undefined;
    private applicationId: number | undefined;
    private isApplicationHistoryMode: boolean = false;
    private dialogRightSideActions: IActionInfo[] | undefined;
    private lastSelectedShip: ShipNomenclatureDTO | undefined = undefined;

    private applicationsService: IApplicationsService | undefined;
    private readonly nomenclatures: CommonNomenclatures;

    private get employeeStatsFormArray(): FormArray {
        return this.form?.get('employeeStatsArray') as FormArray;
    }

    private get employeeInfoFormArray(): FormArray {
        return this.form?.get('employeeInfoArray') as FormArray;
    }

    private get financialInfoFormArray(): FormArray {
        return this.form?.get('financialInfoArray') as FormArray;
    }

    private get workHoursArray(): FormArray {
        return this.form?.get('workHoursArray') as FormArray;
    }

    private employeeStatsGroups: Map<number, number[]> = new Map<number, number[]>();
    private workHoursGroups: Map<number, number[]> = new Map<number, number[]>();

    private employeeStatsLabels: string[] = [];
    private employeeInfoLabels: string[] = [];
    private financialInfoGroupNames: string[] = [];

    private readonly employeeGroupTypes: NumericStatTypeGroupsEnum[] = [NumericStatTypeGroupsEnum.FreeLabor, NumericStatTypeGroupsEnum.NumHours];
    private readonly workHoursGroupTypes: NumericStatTypeGroupsEnum[] = [NumericStatTypeGroupsEnum.WorkHours];

    public constructor(translate: FuseTranslationLoaderService, nomenclatures: CommonNomenclatures) {
        this.translate = translate;
        this.nomenclatures = nomenclatures;

        this.isPublicApp = IS_PUBLIC_APP;

        this.fishingMainActivityOptions = [
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

        this.holderPartOfCrewOptions = [
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

        this.lastYear = new Date(this.currentDate.getFullYear() - 1, 0, 1);
       
        this.buildForm();

        this.expectedResults = new StatisticalFormFishVesselRegixDataDTO({
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
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: NomenclatureDTO<number>[][] = await forkJoin([
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.GrossTonnageIntervals, this.service.getGrossTonnageIntervals.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.ShipLengthIntervals, this.service.getVesselLengthIntervals.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.FuelTypes, this.service.getFuelTypes.bind(this.service), false)
        ]).toPromise();

        this.ships = nomenclatures[0];
        this.grossTonnageIntervals = nomenclatures[1];
        this.shipLengthIntervals = nomenclatures[2];
        this.fuelTypes = nomenclatures[3];

        // извличане на исторически данни за заявление
        if (this.isApplicationHistoryMode && this.applicationId !== undefined) {
            this.form.disable();

            if (this.applicationsService) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const fishVesselForm: StatisticalFormFishVesselApplicationEditDTO = new StatisticalFormFishVesselApplicationEditDTO(contentObject);
                        fishVesselForm.files = content.files;
                        fishVesselForm.applicationId = content.applicationId;

                        this.isOnlineApplication = fishVesselForm.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.model = fishVesselForm;
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
        else if (this.applicationId !== undefined && this.id === undefined && !this.isApplication) {
            // извличане на данни за регистров запис от id на заявление
            if (this.loadRegisterFromApplication === true) {
                this.isEditing = true;

                this.service.getRegisterByApplicationId(this.applicationId, this.pageCode).subscribe({
                    next: (fishVessel: unknown) => {
                        this.model = fishVessel as StatisticalFormFishVesselEditDTO;
                        this.isOnlineApplication = (this.model as StatisticalFormFishVesselEditDTO).isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.fillForm();
                    }
                });
            }
            else {
                // извличане на данни за създаване на регистров запис от заявление
                this.isEditing = false;

                this.service.getApplicationDataForRegister(this.applicationId, this.pageCode).subscribe({
                    next: (fishVessel: StatisticalFormFishVesselEditDTO) => {
                        this.model = fishVessel;
                        this.isOnlineApplication = fishVessel.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.fillForm();
                    }
                });
            }
        }
        else {
            if (this.readOnly || this.viewMode) {
                this.form.disable();
            }
            if (this.isApplication && this.applicationId !== undefined) {
                // извличане на данни за RegiX сверка от служител
                this.isEditing = false;

                if (this.showOnlyRegiXData) {
                    this.service.getRegixData(this.applicationId, this.pageCode).subscribe({
                        next: (regixData: RegixChecksWrapperDTO<StatisticalFormFishVesselRegixDataDTO>) => {
                            this.model = new StatisticalFormFishVesselRegixDataDTO(regixData.dialogDataModel);
                            this.expectedResults = new StatisticalFormFishVesselRegixDataDTO(regixData.regiXDataModel);

                            this.fillForm();
                        }
                    });
                }
                else {
                    // извличане на данни за заявление
                    this.isEditing = false;

                    this.service.getApplication(this.applicationId, this.showRegiXData, this.pageCode).subscribe({
                        next: (fishVessel: StatisticalFormFishVesselApplicationEditDTO) => {
                            fishVessel.applicationId = this.applicationId;

                            this.isOnlineApplication = fishVessel.isOnlineApplication!;
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new StatisticalFormFishVesselRegixDataDTO(fishVessel.regiXDataModel);
                                fishVessel.regiXDataModel = undefined;
                            }

                            if (this.isPublicApp && this.isOnlineApplication && (fishVessel.submittedBy === undefined || fishVessel.submittedBy === null)) {
                                const service = this.service as StatisticalFormsPublicService;
                                service.getCurrentUserAsSubmittedBy(StatisticalFormTypesEnum.FishVessel).subscribe({
                                    next: (submittedBy: ApplicationSubmittedByDTO) => {
                                        fishVessel.submittedBy = submittedBy;
                                        this.model = fishVessel;
                                        this.fillForm();
                                    }
                                });
                            }
                            else {
                                this.model = fishVessel;
                                this.fillForm();
                            }
                        }
                    });
                }
            }
            else if (this.id !== undefined) {
                // извличане на данни за регистров запис
                this.isEditing = true;
                this.isRegisterEntry = true;

                this.service.getStatisticalFormFishVessel(this.id).subscribe({
                    next: (fishVessel: StatisticalFormFishVesselEditDTO) => {
                        this.model = fishVessel;
                        this.isOnlineApplication = fishVessel.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.fillForm();
                    }
                });
            }
        }
    }

    public ngAfterViewInit(): void {
        this.form!.get('shipNameControl')!.valueChanges.subscribe({
            next: (value: NomenclatureDTO<number> | string | undefined) => {
                if (value !== undefined && value !== null && typeof value !== 'string') {
                    const year: number | undefined = (this.form.get('yearControl')!.value as Date)?.getFullYear();
                    this.checkIfStatFormAlreadyExists(value!.value, year);
                }
            }
        });

        this.form.get('yearControl')!.valueChanges.subscribe({
            next: (value: Date | undefined) => {
                if (value !== undefined && value !== null) {
                    const shipId: number | undefined = this.form.get('shipNameControl')!.value?.value;
                    this.checkIfStatFormAlreadyExists(shipId, value.getFullYear());
                    this.form.get('shipNameControl')!.updateValueAndValidity({ onlySelf: true });
                }
            }
        });
    }

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        this.id = data.id;
        this.applicationId = data.applicationId;
        this.applicationsService = data.applicationsService;
        this.isApplication = data.isApplication;
        this.readOnly = data.isReadonly;
        this.isApplicationHistoryMode = data.isApplicationHistoryMode;
        this.viewMode = data.viewMode;
        this.showOnlyRegiXData = data.showOnlyRegiXData;
        this.showRegiXData = data.showRegiXData;
        this.service = data.service as IStatisticalFormsService;
        this.dialogRightSideActions = buttons.rightSideActions;
        this.loadRegisterFromApplication = data.loadRegisterFromApplication;

        this.buildForm();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.saveFishVesselForm(dialogClose);
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        let applicationAction: boolean = false;

        if (this.model instanceof StatisticalFormFishVesselApplicationEditDTO || this.model instanceof StatisticalFormFishVesselRegixDataDTO) {
            if (actionInfo.id !== 'no-corrections-needed' || this.form.valid) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);

                applicationAction = ApplicationUtils.applicationDialogButtonClicked(new ApplicationDialogData({
                    action: actionInfo,
                    dialogClose: dialogClose,
                    applicationId: this.applicationId,
                    model: this.model,
                    readOnly: this.readOnly,
                    viewMode: this.viewMode,
                    editForm: this.form,
                    saveFn: this.saveFishVesselForm.bind(this),
                    onMarkAsTouched: () => {
                        this.validityCheckerGroup.validate();
                    }
                }));
            }
        }

        if (!this.readOnly && !this.viewMode && !applicationAction) {
            if (actionInfo.id === 'save') {
                return this.saveBtnClicked(actionInfo, dialogClose);
            }
        }
    }

    public fileTypeFilterFn(options: PermittedFileTypeDTO[]): PermittedFileTypeDTO[] {
        const pdfs: FileTypeEnum[] = [FileTypeEnum.SIGNEDAPPL, FileTypeEnum.APPLICATION_PDF];
        const offlines: FileTypeEnum[] = [FileTypeEnum.PAYEDFEE, FileTypeEnum.SCANNED_FORM];

        let result: PermittedFileTypeDTO[] = options;

        if (!this.isOnlineApplication) {
            result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        if (this.isOnlineApplication) {
            if (this.isApplication && !this.readOnly) {
                result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
            }

            result = result.filter(x => !offlines.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        return result;
    }

    public getControlErrorLabelText(controlName: string, error: unknown, errorCode: string): TLError | undefined {
        if (controlName === 'shipNameControl') {
            if (errorCode === 'shipNoFishingCapacity' && error === true) {
                return new TLError({
                    text: this.translate.getValue('statistical-forms.ship-no-fishing-capacity-error'),
                    type: 'error'
                });
            }
            else if (errorCode === 'statFormExists' && error === true) {
                return new TLError({
                    text: this.translate.getValue('statistical-forms.fish-vessel-stat-form-exist-error'),
                    type: 'error'
                });
            }
        }
        return undefined;
    }

    private saveFishVesselForm(dialogClose: DialogCloseCallback, saveAsDraft: boolean = false): Observable<boolean> {
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
        return saveOrEditDone.asObservable();
    }

    private saveOrEdit(saveAsDraft: boolean): Observable<number | void> {
        this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model);

        if (this.model instanceof StatisticalFormFishVesselEditDTO) {
            if (this.id !== undefined && this.id !== null) {
                return this.service.editStatisticalFormFishVessel(this.model);
            }
            else {
                return this.service.confirmNoErrorsAndFillAdmAct(this.model.applicationId!, this.model, this.pageCode);
            }
        }
        else {
            if (this.model.id !== undefined && this.model.id !== null) {
                return this.service.editApplication(this.model, this.pageCode, saveAsDraft);
            }
            else {
                return this.service.addApplication(this.model, this.pageCode);
            }
        }
    }

    private fillForm(): void {
        if (this.model instanceof StatisticalFormFishVesselRegixDataDTO) {
            this.form.get('submittedByControl')!.setValue(this.model.submittedBy);
            this.form.get('submittedForControl')!.setValue(this.model.submittedFor);

            this.fillFormRegiX(this.model);
        }
        else if (this.model instanceof StatisticalFormFishVesselApplicationEditDTO) {
            this.fillFormApplication(this.model);

            if (this.showRegiXData) {
                this.fillFormRegiX(this.model);
            }
        }
        else if (this.model instanceof StatisticalFormFishVesselEditDTO) {
            this.fillFormRegister(this.model);
        }

        if (this.readOnly || this.viewMode) {
            this.form.disable();
        }
    }

    private fillFormRegiX(model: StatisticalFormFishVesselRegixDataDTO) {
        if (model.applicationRegiXChecks !== undefined && model.applicationRegiXChecks) {
            const applicationRegiXChecks: ApplicationRegiXCheckDTO[] = model.applicationRegiXChecks;

            if (model.shipId !== undefined && model.shipId !== null) {
                this.form.get('shipNameControl')!.setValue(ShipsUtils.get(this.ships, model.shipId));
            }

            if (model.year !== null && model.year !== undefined) {
                this.form.get('yearControl')!.setValue(new Date(model.year, 0, 1));
            }

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

    private fillFormApplication(model: StatisticalFormFishVesselApplicationEditDTO) {
        this.form.get('submittedByControl')!.setValue(model.submittedBy);
        this.form.get('submittedByWorkPositionControl')!.setValue(model.submittedByWorkPosition);
        this.form.get('submittedForControl')!.setValue(model.submittedFor);

        if (model.shipId !== undefined && model.shipId !== null) {
            this.form.get('shipNameControl')!.setValue(ShipsUtils.get(this.ships, model.shipId));
        }

        if (model.year !== null && model.year !== undefined) {
            this.form.get('yearControl')!.setValue(new Date(model.year, 0, 1));
        }
        else {
            this.form.get('yearControl')!.setValue(this.lastYear);
        }

        this.form.get('shipPriceControl')!.setValue(model.shipPrice);
        this.form.get('fuelConsumptionControl')!.setValue(model.fuelConsumption);

        if (model.isFishingMainActivity === true) {
            this.form.get('isFishingMainActivityControl')!.setValue(this.fishingMainActivityOptions.find(x => x.value === 'yes'));
        }
        else if (model.isFishingMainActivity === false) {
            this.form.get('isFishingMainActivityControl')!.setValue(this.fishingMainActivityOptions.find(x => x.value === 'no'));
        }

        if (model.isShipHolderPartOfCrew === true) {
            this.form.get('shipHolderPartOfCrewControl')!.setValue(this.holderPartOfCrewOptions.find(x => x.value === 'yes'));
            this.form.get('shipHolderPositionControl')!.setValue(model.shipHolderPosition);
        }
        else if (model.isShipHolderPartOfCrew === false) {
            this.form.get('shipHolderPartOfCrewControl')!.setValue(this.holderPartOfCrewOptions.find(x => x.value === 'no'));
            this.form.get('shipHolderPositionControl')!.setValue(undefined);
        }

        this.form.get('filesControl')!.setValue(model.files);

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

        id = 0;
        for (const group of (model.numStatGroups ?? []).filter(x => this.workHoursGroupTypes.includes(x.groupType!))) {
            const ids: number[] = [];

            for (const statType of group.numericStatTypes ?? []) {
                ids.push(id++);

                this.workHoursArray.push(new FormControl(statType.statValue, [Validators.required, TLValidators.number(0)]));
            }

            this.workHoursGroups.set(group.id!, ids);
        }

        for (const group of (model.numStatGroups ?? []).filter(x => !this.employeeGroupTypes.includes(x.groupType!) && !this.workHoursGroupTypes.includes(x.groupType!))) {
            this.financialInfoGroupNames.push(group.groupName!);
            this.financialInfoFormArray.push(new FormControl(group.numericStatTypes, Validators.required));
        }

        for (const group of model.employeeInfoGroups ?? []) {
            this.employeeInfoLabels.push(group.groupName!);
            this.employeeInfoFormArray.push(new FormControl(group.employeeTypes));
        }
    }

    private fillFormRegister(model: StatisticalFormFishVesselEditDTO) {
        this.form.get('submittedForControl')!.setValue(model.submittedFor);
        this.form.get('yearControl')!.setValue(new Date(model.year!, 0, 1));
        this.form.get('shipNameControl')!.setValue(ShipsUtils.get(this.ships, model.shipId!));
        this.form.get('shipYearsControl')!.setValue(model.shipYears);
        this.form.get('shipPriceControl')!.setValue(model.shipPrice);
        this.form.get('shipLengthControl')!.setValue(model.shipLengthId);
        this.form.get('shipTonnageControl')!.setValue(model.shipTonnageId);

        this.form.get('hasEngineControl')!.setValue(model.hasEngine ?? false);

        if (this.model.submittedFor!.submittedByRole! & SubmittedByRolesEnum.LegalRole) {
            const eik: string | undefined = this.model.submittedFor!.legal?.eik;
            this.isIdReadOnly = CommonUtils.hasDigitsOnly(eik);
        }
        else if (this.model.submittedFor!.submittedByRole! & SubmittedByRolesEnum.PersonalRole) {
            const egnLnc: string | undefined = this.model.submittedFor!.person?.egnLnc?.egnLnc;
            this.isIdReadOnly = CommonUtils.hasDigitsOnly(egnLnc);
        }

        if ((model.hasEngine ?? false) === true) {
            this.form.get('fuelTypeControl')!.setValue(model.fuelTypeId);
            this.form.get('fuelConsumptionControl')!.setValue(model.fuelConsumption);
        }

        if (model.isFishingMainActivity === true) {
            this.form.get('isFishingMainActivityControl')!.setValue(this.fishingMainActivityOptions.find(x => x.value === 'yes'));
        }
        else if (model.isFishingMainActivity === false) {
            this.form.get('isFishingMainActivityControl')!.setValue(this.fishingMainActivityOptions.find(x => x.value === 'no'));
        }

        if (model.isShipHolderPartOfCrew === true) {
            this.form.get('shipHolderPartOfCrewControl')!.setValue(this.holderPartOfCrewOptions.find(x => x.value === 'yes'));
            this.form.get('shipHolderPositionControl')!.setValue(model.shipHolderPosition);
        }
        else if (model.isShipHolderPartOfCrew === false) {
            this.form.get('shipHolderPartOfCrewControl')!.setValue(this.holderPartOfCrewOptions.find(x => x.value === 'no'));
            this.form.get('shipHolderPositionControl')!.setValue(undefined);
        }

        this.form.get('filesControl')!.setValue(model.files);

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


        id = 0;
        for (const group of (model.numStatGroups ?? []).filter(x => this.workHoursGroupTypes.includes(x.groupType!))) {
            const ids: number[] = [];

            for (const statType of group.numericStatTypes ?? []) {
                ids.push(id++);

                this.workHoursArray.push(new FormControl(statType.statValue, [Validators.required, TLValidators.number(0)]));
            }

            this.workHoursGroups.set(group.id!, ids);
        }

        for (const group of (model.numStatGroups ?? []).filter(x => !this.employeeGroupTypes.includes(x.groupType!) && !this.workHoursGroupTypes.includes(x.groupType!))) {
            this.financialInfoGroupNames.push(group.groupName!);
            this.financialInfoFormArray.push(new FormControl(group.numericStatTypes, Validators.required));
        }

        for (const group of model.employeeInfoGroups ?? []) {
            this.employeeInfoLabels.push(group.groupName!);
            this.employeeInfoFormArray.push(new FormControl(group.employeeTypes));
        }
    }

    private fillShipForm(ship: StatisticalFormShipDTO): void {
        this.form.get('shipYearsControl')!.setValue(ship.shipYears);
        this.form.get('shipLengthControl')!.setValue(this.shipLengthIntervals.find(x => x.value === ship.shipLenghtId!));
        this.form.get('shipTonnageControl')!.setValue(this.grossTonnageIntervals.find(x => x.value === ship.grossTonnageId!));
        this.form.get('hasEngineControl')!.setValue(ship.hasEngine);
        this.form.get('fuelTypeControl')!.setValue(this.fuelTypes.find(x => x.value === ship.fuelTypeId!));

        this.allGearSeaDays = ship.fishingGearSeaDays ?? [];
        this.shipSeaDays = ship.shipSeaDays ?? [];

        this.fillSeaDays();
    }

    private fillModel(): void {
        if (this.model instanceof StatisticalFormFishVesselRegixDataDTO) {
            this.fillModelRegix(this.model);
        }
        else if (this.model instanceof StatisticalFormFishVesselApplicationEditDTO) {
            this.fillModelApplication(this.model);
        }
        else if (this.model instanceof StatisticalFormFishVesselEditDTO) {
            this.fillModelRegister(this.model);
        }
    }

    private fillModelRegix(model: StatisticalFormFishVesselRegixDataDTO) {
        model.submittedBy = this.form.get('submittedByControl')!.value;
        model.submittedFor = this.form.get('submittedForControl')!.value;
    }

    private fillModelApplication(model: StatisticalFormFishVesselApplicationEditDTO) {
        model.submittedBy = this.form.get('submittedByControl')!.value;
        model.submittedByWorkPosition = this.form.get('submittedByWorkPositionControl')!.value;
        model.submittedFor = this.form.get('submittedForControl')!.value;
        model.year = (this.form.get('yearControl')!.value as Date)?.getFullYear();
        model.shipId = this.form.get('shipNameControl')!.value?.value;
        model.shipPrice = this.form.get('shipPriceControl')!.value;
        model.shipYears = this.form.get('shipYearsControl')!.value;
        model.shipLengthId = this.form.get('shipLengthControl')!.value?.value;
        model.shipTonnageId = this.form.get('shipTonnageControl')!.value?.value;
        model.hasEngine = this.hasEngine ?? false;

        if (model.hasEngine === true) {
            model.fuelTypeId = this.form.get('fuelTypeControl')!.value?.value;
            model.fuelConsumption = this.form.get('fuelConsumptionControl')!.value;
        }
        else {
            model.fuelTypeId = undefined;
            model.fuelConsumption = undefined;
        }

        const isFishingMainActivity: NomenclatureDTO<YesNo> | undefined = this.form.get('isFishingMainActivityControl')!.value;
        if (isFishingMainActivity !== null && isFishingMainActivity !== undefined) {
            model.isFishingMainActivity = isFishingMainActivity.value === 'yes';
        }
        else {
            model.isFishingMainActivity = undefined;
        }

        const isShipHolderPartOfCrew: NomenclatureDTO<YesNo> | undefined = this.form.get('shipHolderPartOfCrewControl')!.value;
        if (isShipHolderPartOfCrew !== null && isShipHolderPartOfCrew !== undefined) {
            model.isShipHolderPartOfCrew = isShipHolderPartOfCrew.value === 'yes';
        }
        else {
            model.isShipHolderPartOfCrew = undefined;
        }

        if (model.isShipHolderPartOfCrew === true) {
            model.shipHolderPosition = this.form.get('shipHolderPositionControl')!.value;
        }

        model.employeeInfoGroups = this.getEmployeeInfo(model);
        model.numStatGroups = [...this.getFinancialInfo(model), ...this.getEmployeeStats(model), ...this.getWorkHours(model)];

        model.files = this.form.get('filesControl')!.value;
    }

    private fillModelRegister(model: StatisticalFormFishVesselEditDTO) {
        model.submittedFor = this.form.get('submittedForControl')!.value;
        model.year = (this.form.get('yearControl')!.value as Date)?.getFullYear();
        model.shipId = this.form.get('shipNameControl')!.value?.value;
        model.shipPrice = this.form.get('shipPriceControl')!.value;
        model.shipYears = this.form.get('shipYearsControl')!.value;
        model.shipLengthId = this.form.get('shipLengthControl')!.value?.value;
        model.shipTonnageId = this.form.get('shipTonnageControl')!.value?.value;
        model.hasEngine = this.hasEngine ?? false;

        if (model.hasEngine === true) {
            model.fuelTypeId = this.form.get('fuelTypeControl')!.value?.value;
            model.fuelConsumption = this.form.get('fuelConsumptionControl')!.value;
        }
        else {
            model.fuelTypeId = undefined;
            model.fuelConsumption = undefined;
        }

        model.isFishingMainActivity = this.form.get('isFishingMainActivityControl')!.value.value === 'yes';
        model.isShipHolderPartOfCrew = this.form.get('shipHolderPartOfCrewControl')!.value.value === 'yes';

        if (model.isShipHolderPartOfCrew === true) {
            model.shipHolderPosition = this.form.get('shipHolderPositionControl')!.value;
        }

        model.employeeInfoGroups = this.getEmployeeInfo(model);
        model.numStatGroups = [...this.getFinancialInfo(model), ...this.getEmployeeStats(model), ...this.getWorkHours(model)];

        model.files = this.form.get('filesControl')!.value;
    }

    private buildForm(): void {
        if (this.showOnlyRegiXData) {
            this.form = new FormGroup({
                submittedByControl: new FormControl(),
                submittedForControl: new FormControl(),
                shipNameControl: new FormControl(undefined, [Validators.required, this.shipValidator()]),
                yearControl: new FormControl(undefined, Validators.required)
            });
        }
        else if (this.isApplication || this.isApplicationHistoryMode) {
            this.form = new FormGroup({
                submittedByControl: new FormControl(),
                submittedByWorkPositionControl: new FormControl(undefined, [Validators.required, Validators.maxLength(200)]),
                submittedForControl: new FormControl(),
                filesControl: new FormControl(),
                shipNameControl: new FormControl(undefined, [Validators.required, this.shipValidator()]),
                yearControl: new FormControl(undefined, Validators.required),
                shipYearsControl: new FormControl({ value: undefined, disabled: true }),
                shipPriceControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
                shipLengthControl: new FormControl({ value: undefined, disabled: true }),
                shipTonnageControl: new FormControl({ value: undefined, disabled: true }),
                hasEngineControl: new FormControl({ value: undefined, disabled: true }),
                fuelTypeControl: new FormControl({ value: undefined, disabled: true }),
                fuelConsumptionControl: new FormControl(),
                isFishingMainActivityControl: new FormControl(undefined, Validators.required),
                shipHolderPartOfCrewControl: new FormControl(undefined, Validators.required),
                shipHolderPositionControl: new FormControl(),
                employeeInfoArray: new FormArray([], this.payColumnCountValidator()),
                employeeStatsArray: new FormArray([]),
                financialInfoArray: new FormArray([], this.costsValidator()),
                workHoursArray: new FormArray([])
            });
        }
        else {
            this.form = new FormGroup({
                submittedForControl: new FormControl(),
                filesControl: new FormControl(),
                shipNameControl: new FormControl(undefined, [Validators.required, this.shipValidator()]),
                yearControl: new FormControl(undefined, Validators.required),
                shipYearsControl: new FormControl({ value: undefined, disabled: true }),
                shipPriceControl: new FormControl(undefined, Validators.required),
                shipLengthControl: new FormControl({ value: undefined, disabled: true }),
                shipTonnageControl: new FormControl({ value: undefined, disabled: true }),
                hasEngineControl: new FormControl({ value: undefined, disabled: true }),
                fuelTypeControl: new FormControl({ value: undefined, disabled: true }),
                fuelConsumptionControl: new FormControl(),
                isFishingMainActivityControl: new FormControl(undefined, Validators.required),
                shipHolderPartOfCrewControl: new FormControl(undefined, Validators.required),
                shipHolderPositionControl: new FormControl(),
                employeeInfoArray: new FormArray([], this.payColumnCountValidator()),
                employeeStatsArray: new FormArray([]),
                financialInfoArray: new FormArray([], this.costsValidator()),
                workHoursArray: new FormArray([])
            });
        }

        this.form.get('hasEngineControl')?.valueChanges.subscribe({
            next: (checked: boolean) => {
                this.hasEngine = checked ?? false;

                if (this.hasEngine) { // set field validators
                    this.form.get('fuelTypeControl')!.setValidators(Validators.required);
                    this.form.get('fuelConsumptionControl')!.setValidators([Validators.required, TLValidators.number(0)]);

                    this.form.get('fuelTypeControl')!.markAsPending();
                    this.form.get('fuelConsumptionControl')!.markAsPending();
                }
                else { // clear some validators
                    this.form.get('fuelTypeControl')!.clearValidators();
                    this.form.get('fuelConsumptionControl')!.setValidators(TLValidators.number(0));
                }

                if (this.viewMode || this.readOnly) {
                    this.form.get('fuelTypeControl')!.disable();
                    this.form.get('fuelConsumptionControl')!.disable();
                }
            }
        });

        this.form.get('shipHolderPartOfCrewControl')?.valueChanges.subscribe({
            next: (option: NomenclatureDTO<YesNo>) => {
                this.isShipHolderPartOfCrew = option?.value === 'yes';
            }
        });

        this.form.get('shipNameControl')?.valueChanges.subscribe({
            next: (value: NomenclatureDTO<number> | undefined) => {


                if (value !== undefined && value !== null && value instanceof NomenclatureDTO) {
                    if (this.lastSelectedShip !== null && this.lastSelectedShip !== undefined && this.lastSelectedShip.value !== value.value) {
                        this.form.get('shipPriceControl')!.setValue(undefined);
                        this.form.get('fuelConsumptionControl')!.setValue(undefined);
                    }

                    this.lastSelectedShip = value;
                    const ship: StatisticalFormShipDTO | undefined = this.statFormShip.get(value.value!);

                    if (ship === undefined || ship === null) {
                        this.service.getStatisticalFormShip(value.value!).subscribe({
                            next: (ship: StatisticalFormShipDTO) => {
                                this.statFormShip.set(ship.shipId!, ship);

                                this.fillShipForm(ship);
                                this.showShipForm = true;
                            }
                        });
                    }
                    else {
                        this.fillShipForm(ship);
                        this.showShipForm = true;
                    }
                }
                else {
                    if (this.lastSelectedShip !== null && this.lastSelectedShip !== undefined) {
                        this.form.get('shipPriceControl')!.setValue(undefined);
                        this.form.get('fuelConsumptionControl')!.setValue(undefined);
                    }

                    this.lastSelectedShip = undefined;
                    this.showShipForm = false;
                }
            }
        });

        this.form.get('employeeInfoArray')?.valueChanges.subscribe({
            next: () => {
                this.form.get('financialInfoArray')!.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    private fillSeaDays() {
        const year: number | undefined = this.form.get('yearControl')!.value?.getFullYear();

        if (year !== undefined && year !== null) {
            this.gearSeaDays = this.allGearSeaDays.filter(x => x.year === year);
            const seaDaysTotal: number = this.shipSeaDays.find(x => x.year === year)?.days ?? 0;

            this.seaDaysTitle = `${this.translate.getValue('statistical-forms.sea-days-ship-title')} ${seaDaysTotal} ${this.translate.getValue('statistical-forms.sea-days-ship')}`;
        }
        else {
            this.gearSeaDays = [];
            this.seaDaysTitle = this.translate.getValue('statistical-forms.sea-days-title');
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

                const values: number[] = statTypes.slice(idx[0], idx[0] + idx.length);
                for (let i = 0; i < group.numericStatTypes!.length; ++i) {
                    group.numericStatTypes![i].statValue = values[i];
                }
            }
        }
        return result;
    }

    private getWorkHours(model: StatisticalFormReworkApplicationEditDTO | StatisticalFormReworkEditDTO): StatisticalFormNumStatGroupDTO[] {
        const statTypes: number[] = this.workHoursArray.value as number[];

        const result: StatisticalFormNumStatGroupDTO[] = [];
        for (const group of model.numStatGroups!) {
            const idx: number[] | undefined = this.workHoursGroups.get(group.id!);

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

    private markAllAsTouched(): void {
        this.form.markAllAsTouched();
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
                                    if (financials[i][j].code === 'StaffVe') {
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

    private shipValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const shipId: number | undefined = this.form?.get('shipNameControl')?.value?.value;
            if (shipId !== undefined && shipId !== null) {
                const ship: ShipNomenclatureDTO = ShipsUtils.get(this.ships, shipId);

                if (!ShipsUtils.hasFishingCapacity(ship)) {
                    return { shipNoFishingCapacity: true };
                }
            }

            return null;
        };
    }

    private checkIfStatFormAlreadyExists(shipId: number | undefined, year: number | undefined): void {
        if (!this.isRegisterEntry) {
            if (year !== undefined && year !== null) {
                if (shipId !== undefined && shipId !== null) {
                    this.service.vesselStatFormAlreadyExists(shipId, year, this.id).subscribe({
                        next: (yes: boolean) => {
                            if (yes) {
                                this.form.get('shipNameControl')!.setErrors({ statFormExists: true });
                                this.form.get('shipNameControl')!.markAsTouched();
                            }
                        }
                    });
                }
            }
        }
    }
}