import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { PermitLicenseFishingGearsRegixDataDTO } from '@app/models/generated/dtos/PermitLicenseFishingGearsRegixDataDTO';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { ApplicationSubmittedByRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedForRegixDataDTO';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PermitLicenseFishingGearsApplicationDTO } from '@app/models/generated/dtos/PermitLicenseFishingGearsApplicationDTO';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { Observable, Subject, Subscription } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { CommercialFishingPublicService } from '@app/services/public-app/commercial-fishing-public.service';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { SystemParametersService } from '@app/services/common-app/system-parameters.service';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { PermitLicenseTariffCalculationParameters } from '../../models/permit-license-tariff-calculation-parameters.model';
import { ApplicationPaymentInformationDTO } from '@app/models/generated/dtos/ApplicationPaymentInformationDTO';
import { PaymentTariffDTO } from '@app/models/generated/dtos/PaymentTariffDTO';
import { PaymentSummaryDTO } from '@app/models/generated/dtos/PaymentSummaryDTO';
import { CommercialFishingTypesEnum } from '@app/enums/commercial-fishing-types.enum';
import { FishingGearUtils } from '../../utils/fishing-gear.utils';
import { FishingGearMarkStatusesEnum } from '@app/enums/fishing-gear-mark-statuses.enum';
import { FishingGearPingerStatusesEnum } from '@app/enums/fishing-gear-pinger-statuses.enum';
import { PermitLicensesNomenclatureDTO } from '@app/models/generated/dtos/PermitLicensesNomenclatureDTO';
import { WaterTypesEnum } from '@app/enums/water-types.enum';
import { CommercialFishingValidationErrorsEnum } from '@app/enums/commercial-fishing-validation-errors.enum';
import { ShipsUtils } from '@app/shared/utils/ships.utils';

@Component({
    selector: 'edit-permit-license-fishing-gears',
    templateUrl: './edit-permit-license-fishing-gears.component.html',
})
export class EditPermitLicenseFishingGearsComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;

    public readonly pageCode: PageCodeEnum = PageCodeEnum.FishingGearsCommFish;

    public permitLicensePageCode: PageCodeEnum | undefined;
    public isDunabe: boolean = false;

    public permitLicenses: PermitLicensesNomenclatureDTO[] = [];
    public ships: ShipNomenclatureDTO[] = [];
    public permitLicenseAppliedTariffs: string[] = [];

    public notifier: Notifier = new Notifier();
    public expectedResults: PermitLicenseFishingGearsRegixDataDTO;
    public regixChecks: ApplicationRegiXCheckDTO[] = [];

    public isPublicApp: boolean = false;
    public isOnlineApplication: boolean = false;
    public isEditing: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public showRegiXData: boolean = false;
    public isReadonly: boolean = false;
    public viewMode: boolean = false;
    public isPaid: boolean = false;
    public isDraft: boolean = false;
    public hasDelivery: boolean = false;
    public hasNoEDeliveryRegistrationError: boolean = false;
    public hideBasicPaymentInfo: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();
    public service!: ICommercialFishingService;

    public shipId: number | undefined;
    public noShipSelected: boolean = true;
    public hasInvalidPermitLicenseRegistrationNumber: boolean = false;
    public maxNumberOfFishingGears: number = 0;

    private permitLicenseId: number | undefined;
    private duplicatedMarkNumbers: string[] = [];
    private duplicatedPingerNumbers: string[] = [];

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private applicationId: number | undefined;
    private isApplicationHistoryMode: boolean = false;
    private applicationsService: IApplicationsService | undefined;
    private dialogRightSideActions: IActionInfo[] | undefined;
    private systemParametersService: SystemParametersService;
    private nomenclatures: CommonNomenclatures;

    private model!: PermitLicenseFishingGearsApplicationDTO | PermitLicenseFishingGearsRegixDataDTO;

    private readonly loader!: FormControlDataLoader;

    public constructor(
        nomenclatures: CommonNomenclatures,
        systemParametersService: SystemParametersService
    ) {
        this.nomenclatures = nomenclatures;
        this.systemParametersService = systemParametersService

        this.isPublicApp = IS_PUBLIC_APP;

        this.expectedResults = new PermitLicenseFishingGearsRegixDataDTO({
            submittedBy: new ApplicationSubmittedByRegixDataDTO(),
            submittedFor: new ApplicationSubmittedForRegixDataDTO()
        });

        this.loader = new FormControlDataLoader(this.loadShipsNomenclature.bind(this));
    }

    public ngOnInit(): void {
        this.loader.load(() => {
            this.loadData();
        });
    }

    public ngAfterViewInit(): void {
        this.form.get('deliveryDataControl')?.valueChanges.subscribe({
            next: () => {
                this.hasNoEDeliveryRegistrationError = false;
            }
        });
    }

    public setData(data: DialogParamsModel, wrapperData: DialogWrapperData): void {
        this.applicationId = data.applicationId;
        this.isReadonly = data.isReadonly;
        this.isApplicationHistoryMode = data.isApplicationHistoryMode;
        this.viewMode = data.viewMode;
        this.showOnlyRegiXData = data.showOnlyRegiXData;
        this.showRegiXData = data.showRegiXData;
        this.applicationsService = data.applicationsService;
        this.service = data.service as ICommercialFishingService;
        this.dialogRightSideActions = wrapperData.rightSideActions;

        this.buildForm();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.saveApplication(dialogClose);
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        let applicationAction: boolean = false;

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
            saveFn: this.saveApplication.bind(this),
            onMarkAsTouched: () => {
                this.validityCheckerGroup.validate();
            }
        }));

        if (!this.isReadonly && !this.viewMode && !applicationAction) {
            this.form.markAllAsTouched();
            this.validityCheckerGroup.validate();

            if (this.form.valid) {
                if (actionInfo.id === 'save') {
                    return this.saveBtnClicked(actionInfo, dialogClose);
                }
            }
        }
        else {
            dialogClose();
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public fileTypeFilterFn(options: PermittedFileTypeDTO[]): PermittedFileTypeDTO[] {
        let result: PermittedFileTypeDTO[] = options;

        if (this.isOnlineApplication) {
            const offlines: FileTypeEnum[] = [FileTypeEnum.PAYEDFEE, FileTypeEnum.SCANNED_FORM];
            result = result.filter(x => !offlines.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }
        else {
            const pdfs: FileTypeEnum[] = [FileTypeEnum.SIGNEDAPPL, FileTypeEnum.APPLICATION_PDF];
            result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        return result;
    }

    public loadFishingGearsFromPermitLicense(): void {
        const permitNumber: string | undefined = this.form.get('permitLicenseNumberControl')!.value;
        const shipId: number | undefined = this.form.get('shipControl')!.value?.value;

        if (permitNumber !== undefined && permitNumber !== null && shipId !== undefined && shipId !== null) {
            this.service.getFishingGearsByPermitLicenseRegistrationNumber(permitNumber, shipId).subscribe({
                next: (fishingGears: FishingGearDTO[] | undefined) => {
                    this.form.get('fishingGearsGroup.fishingGearsControl')!.setValue(fishingGears);
                    this.form.get('fishingGearsGroup.fishingGearsControl')!.updateValueAndValidity();
                },
                error: (response: HttpErrorResponse) => {
                    if (response.error !== null && response.error !== undefined) {
                        const error = response.error as ErrorModel;

                        if (error?.code === ErrorCode.InvalidPermitLicenseNumber) {
                            this.hasInvalidPermitLicenseRegistrationNumber = true;
                        }
                    }
                }
            });
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            submittedByControl: new FormControl(null),
            submittedForControl: new FormControl(null),
            shipControl: new FormControl(null, Validators.required),
            deliveryDataControl: new FormControl(),
            applicationPaymentInformationControl: new FormControl(),
            filesControl: new FormControl(null),
            fishingGearsGroup: new FormGroup({
                fishingGearsControl: new FormControl(undefined)
            })
        });

        if (this.isPublicApp) {
            this.form.addControl('permitLicenseNumberControl', new FormControl(undefined, Validators.required));
        }
        else {
            this.form.addControl('permitLicenseControl', new FormControl(undefined, Validators.required));
        }

        this.form.get('fishingGearsGroup')!.setValidators([
            FishingGearUtils.atLeastOneFishingGear(),
            FishingGearUtils.permitLicenseDuplicateMarkNumbersValidator(),
            FishingGearUtils.permitLicenseDuplicatePingerNumbersValidator()
        ]);
    }

    private fillForm(): void {
        this.form.get('submittedByControl')!.setValue(this.model.submittedBy);
        this.form.get('submittedForControl')!.setValue(this.model.submittedFor);

        if (this.model instanceof PermitLicenseFishingGearsRegixDataDTO) {
            this.form.get('permitLicenseControl')!.disable();
            this.fillFormRegiX();
        }
        else {
            this.form.get('filesControl')!.setValue(this.model.files);
            this.form.get('applicationPaymentInformationControl')!.setValue(this.model.paymentInformation);
            this.hideBasicPaymentInfo = this.shouldHideBasicPaymentInfo();

            const shipId: number | undefined = this.model.shipId;
            if (shipId !== undefined && shipId !== null) {
                const selectedShip: ShipNomenclatureDTO = ShipsUtils.get(this.ships, shipId);
                this.shipId = shipId;
                this.form.get('shipControl')!.setValue(selectedShip);

                if (this.isPublicApp) {
                    this.form.get('permitLicenseNumberControl')!.setValue(this.model.permitLicenseNumber);
                }
                else {
                    const permitLicenseId: number | undefined = this.model.permitLicenseId;
                    this.permitLicenseId = permitLicenseId;
                    this.form.get('permitLicenseControl')!.setValue(this.permitLicenses.find(x => x.value === permitLicenseId));
                }
            }

            this.form.get('fishingGearsGroup.fishingGearsControl')!.setValue(this.model.fishingGears);

            if (this.isPaid
                && !this.isReadonly
                && !this.viewMode
            ) {
                this.updateAppliedTariffs();
            }

            if (this.hasDelivery === true) {
                this.form.get('deliveryDataControl')!.setValue(this.model.deliveryData);
            }

            if (this.showRegiXData === true) {
                this.fillFormRegiX();
            }
        }
    }

    private fillFormRegiX(): void {
        if (this.model.applicationRegiXChecks !== undefined && this.model.applicationRegiXChecks !== null) {
            const checks: ApplicationRegiXCheckDTO[] = this.model.applicationRegiXChecks;

            setTimeout(() => {
                this.regixChecks = checks;
            });

            this.model.applicationRegiXChecks = undefined;
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

    private fillModel(): void {
        this.model.submittedBy = this.form.get('submittedByControl')!.value;
        this.model.submittedFor = this.form.get('submittedForControl')!.value;

        if (this.model instanceof PermitLicenseFishingGearsApplicationDTO) {
            this.model.shipId = this.form.get('shipControl')!.value?.value;

            if (this.isPublicApp) {
                this.model.permitLicenseNumber = this.form.get('permitLicenseNumberControl')!.value;
            }
            else {
                this.model.permitLicenseId = this.form.get('permitLicenseControl')!.value?.value;
            }

            this.model.fishingGears = this.form.get('fishingGearsGroup.fishingGearsControl')!.value;
            this.model.files = this.form.get('filesControl')!.value;

            if (this.hasDelivery === true) {
                this.model.deliveryData = this.form.get('deliveryDataControl')!.value;
            }

            if (this.isPaid === true) {
                this.model.paymentInformation = this.form.get('applicationPaymentInformationControl')!.value;
            }
        }
    }

    private saveApplication(dialogClose: DialogCloseCallback, saveAsDraft: boolean = false): Observable<boolean> {
        const saveOrEditDone: Subject<boolean> = new Subject<boolean>();

        this.saveOrEdit(saveAsDraft).subscribe({
            next: (id: number | void) => {
                this.hasNoEDeliveryRegistrationError = false;
                this.hasInvalidPermitLicenseRegistrationNumber = false;

                if (typeof id === 'number' && id !== undefined) {
                    this.model.applicationId = id;
                    dialogClose(this.model);
                }
                else {
                    dialogClose(this.model);
                }

                saveOrEditDone.next(true);
                saveOrEditDone.complete();
            },
            error: (response: HttpErrorResponse) => {
                this.handleAddEditApplicationErrorResponse(response);
            }
        });

        return saveOrEditDone.asObservable();
    }

    private saveOrEdit(fromSaveAsDraft: boolean): Observable<number | void> {
        this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model);

        if (this.model instanceof PermitLicenseFishingGearsApplicationDTO && !this.model.isDraft) {
            return this.service.editApplication(this.model, this.pageCode, fromSaveAsDraft);
        }
        else {
            return this.service.addApplication(this.model, this.pageCode);
        }
    }

    private async loadData(): Promise<void> {
        if (!this.showOnlyRegiXData) {
            this.maxNumberOfFishingGears = (await this.systemParametersService.systemParameters()).maxNumberFishingGears!;

            this.form.get('shipControl')!.valueChanges.subscribe({
                next: (ship: ShipNomenclatureDTO | undefined | string) => {
                    this.shipId = undefined;

                    if (!this.isPublicApp && !this.showOnlyRegiXData) {
                        const shipId: number | undefined = (ship as ShipNomenclatureDTO)?.value;

                        if (shipId !== undefined && shipId !== null) {
                            this.noShipSelected = false;
                            this.shipId = shipId;
                            this.getShipPermitLicenses(shipId);
                        }
                        else {
                            this.permitLicenses = [];
                            this.form.get('permitLicenseControl')!.reset();
                            this.form.get('permitLicenseControl')!.updateValueAndValidity();
                            this.noShipSelected = true;
                        }
                    }

                    if ((ship instanceof NomenclatureDTO || ship === null || ship === undefined)
                        && this.isPaid
                        && !this.isReadonly
                        && !this.viewMode
                    ) { // update applied tariffs based on selected ship and permit license
                        this.updateAppliedTariffs();
                    }
                }
            });

            if (this.isPublicApp) {
                this.permitLicensePageCode = PageCodeEnum.FishingGearsCommFish;

                this.form.get('permitLicenseNumberControl')!.valueChanges.subscribe({
                    next: (permitLicense: string | undefined) => {
                        this.hasInvalidPermitLicenseRegistrationNumber = false;
                    }
                });
            }
            else {
                this.form.get('permitLicenseControl')!.valueChanges.subscribe({
                    next: (permitLicense: PermitLicensesNomenclatureDTO | undefined) => {
                        if (permitLicense !== undefined && permitLicense !== null) {
                            this.permitLicensePageCode = this.getPermitLicensePageCodeByType(permitLicense.type!); //filter fishing gears nomenclature by permit license page code
                            this.isDunabe = permitLicense.waterTypeCode === WaterTypesEnum[WaterTypesEnum.DANUBE]; //filter fishing gears nomenclature by permit license applied tariffs and water type
                            this.permitLicenseAppliedTariffs = permitLicense.tariffs ?? [];

                            if (this.permitLicenseId !== permitLicense.value) {
                                this.permitLicenseId = permitLicense.value;

                                if (permitLicense.value !== undefined && permitLicense.value !== null) {
                                    this.service.getPermitLicenseFishingGears(permitLicense.value!).subscribe({
                                        next: (fishingGears: FishingGearDTO[] | undefined) => {
                                            this.form.get('fishingGearsGroup.fishingGearsControl')!.setValue(fishingGears);
                                            this.form.get('fishingGearsGroup.fishingGearsControl')!.updateValueAndValidity();
                                        }
                                    });
                                }
                            }
                        }
                        else {
                            this.permitLicenseAppliedTariffs = [];
                        }

                        if ((permitLicense instanceof NomenclatureDTO || permitLicense === null || permitLicense === undefined)
                            && this.isPaid
                            && !this.isReadonly
                            && !this.viewMode
                        ) { // update applied tariffs based on selected ship and permit license
                            this.updateAppliedTariffs();
                        }
                    }
                });
            }

            this.form.get('fishingGearsGroup.fishingGearsControl')!.valueChanges.subscribe({
                next: (value: FishingGearDTO[] | undefined) => {
                    this.updateDuplicatedMarksAndPingers(value);

                    if (this.isPaid
                        && !this.isReadonly
                        && !this.viewMode
                    ) { // update applied tariffs based on selected marked fishing gears
                        this.updateAppliedTariffs();
                    }
                }
            });
        }

        // извличане на исторически данни за заявление
        if (this.isApplicationHistoryMode && this.applicationId !== undefined) {
            this.form.disable();

            if (this.applicationsService) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const application: PermitLicenseFishingGearsApplicationDTO = new PermitLicenseFishingGearsApplicationDTO(contentObject);
                        application.files = content.files;
                        application.applicationId = content.applicationId;

                        this.isPaid = application.isPaid!;
                        this.hasDelivery = application.hasDelivery!;
                        this.hideBasicPaymentInfo = this.shouldHideBasicPaymentInfo();
                        this.isOnlineApplication = application.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.model = application;
                        this.fillForm();
                    }
                });
            }
        }
        else {
            if (this.isReadonly || this.viewMode) {
                this.form.disable();
            }

            if (this.applicationId !== undefined) {
                // извличане на данни за RegiX справка от служител
                this.isEditing = false;

                if (this.showOnlyRegiXData) {
                    this.service.getRegixData(this.applicationId, this.pageCode).subscribe({
                        next: (regixData: RegixChecksWrapperDTO<PermitLicenseFishingGearsRegixDataDTO>) => {
                            this.model = new PermitLicenseFishingGearsRegixDataDTO(regixData.dialogDataModel);
                            this.expectedResults = new PermitLicenseFishingGearsRegixDataDTO(regixData.regiXDataModel);

                            this.fillForm();
                        }
                    });
                }
                else {
                    // извличане на данни за заявление
                    this.isEditing = false;

                    this.service.getApplication(this.applicationId, this.showRegiXData, this.pageCode).subscribe({
                        next: (application: PermitLicenseFishingGearsApplicationDTO) => {
                            application.applicationId = this.applicationId;
                            application.isDraft = application.isDraft ?? true;

                            this.isOnlineApplication = application.isOnlineApplication!;
                            this.isDraft = application.isDraft!;
                            this.isPaid = application.isPaid!;
                            this.hasDelivery = application.hasDelivery!;
                            this.hideBasicPaymentInfo = this.shouldHideBasicPaymentInfo();
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new PermitLicenseFishingGearsRegixDataDTO(application.regiXDataModel);
                                application.regiXDataModel = undefined;
                            }

                            if (this.isPublicApp && this.isOnlineApplication && (application.submittedBy === undefined || application.submittedBy === null)) {
                                const service = this.service as CommercialFishingPublicService;
                                service.getCurrentUserAsSubmittedBy().subscribe({
                                    next: (submittedBy: ApplicationSubmittedByDTO) => {
                                        application.submittedBy = submittedBy;
                                        this.model = application;
                                        this.fillForm();
                                    }
                                });
                            }
                            else {
                                this.model = application;
                                this.fillForm();
                            }
                        }
                    });
                }
            }
        }
    }

    private loadShipsNomenclature(): Subscription {
        return NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false
        ).subscribe({
            next: (ships: ShipNomenclatureDTO[]) => {
                this.ships = ships;
                this.loader.complete();
            }
        });
    }

    private getShipPermitLicenses(shipId: number): void {
        this.service.getPermitLicensesNomenclatures(shipId).subscribe({
            next: (values: PermitLicensesNomenclatureDTO[]) => {
                this.permitLicenses = values;

                if (this.model instanceof PermitLicenseFishingGearsApplicationDTO) {
                    const permitLicenseId: number | undefined = this.model.permitLicenseId;
                    this.form.get('permitLicenseControl')!.setValue(undefined);

                    if (permitLicenseId !== undefined && permitLicenseId !== null) {
                        this.form.get('permitLicenseControl')!.setValue(this.permitLicenses.find(x => x.value === permitLicenseId));
                    }
                    else {
                        if (this.permitLicenses.filter(x => x.isActive).length === 1) {
                            this.form.get('permitLicenseControl')!.setValue(this.permitLicenses.filter(x => x.isActive)[0]);
                        }
                    }
                }
            }
        });
    }

    private updateAppliedTariffs(): void {
        (this.model as PermitLicenseFishingGearsApplicationDTO).paymentInformation = this.form.get('applicationPaymentInformationControl')!.value;

        const parameters: PermitLicenseTariffCalculationParameters = this.getTariffCalculationParameters();

        this.service.calculatePermitLicenseAppliedTariffs(parameters).subscribe({
            next: (tariffs: PaymentTariffDTO[]) => {
                if ((this.model as PermitLicenseFishingGearsApplicationDTO).paymentInformation!.paymentSummary !== null
                    && (this.model as PermitLicenseFishingGearsApplicationDTO).paymentInformation!.paymentSummary !== undefined
                ) {
                    (this.model as PermitLicenseFishingGearsApplicationDTO).paymentInformation!.paymentSummary!.tariffs = tariffs;
                    (this.model as PermitLicenseFishingGearsApplicationDTO).paymentInformation!.paymentSummary!.totalPrice = this.calculateAppliedTariffsTotalPrice(tariffs);
                }
                else {
                    (this.model as PermitLicenseFishingGearsApplicationDTO).paymentInformation!.paymentSummary = new PaymentSummaryDTO({
                        tariffs: tariffs,
                        totalPrice: this.calculateAppliedTariffsTotalPrice(tariffs)
                    });
                }

                this.form.get('applicationPaymentInformationControl')!.setValue((this.model as PermitLicenseFishingGearsApplicationDTO).paymentInformation);
                this.hideBasicPaymentInfo = this.shouldHideBasicPaymentInfo();
            }
        });
    }

    private calculateAppliedTariffsTotalPrice(appliedTariffs: PaymentTariffDTO[]): number {
        return appliedTariffs.map(x => x.quantity! * x.unitPrice!).reduce((a, b) => a + b, 0);
    }

    private getTariffCalculationParameters(): PermitLicenseTariffCalculationParameters {
        const paymentIntormation: ApplicationPaymentInformationDTO = this.form.get('applicationPaymentInformationControl')!.value;

        let excludedTariffsIds: number[] = [];

        if (paymentIntormation.paymentSummary !== null
            && paymentIntormation.paymentSummary !== undefined
            && paymentIntormation.paymentSummary.tariffs !== null
            && paymentIntormation.paymentSummary.tariffs !== undefined
        ) {
            excludedTariffsIds = paymentIntormation.paymentSummary.tariffs.filter(x => !x.isChecked).map(x => x.tariffId!);
        }

        const parameters: PermitLicenseTariffCalculationParameters = new PermitLicenseTariffCalculationParameters({
            applicationId: this.applicationId,
            pageCode: this.pageCode,
            shipId: undefined,
            waterTypeCode: this.isPublicApp ? undefined : this.form.get('permitLicenseControl')!.value?.waterTypeCode,
            aquaticOrganismTypeIds: undefined,
            fishingGears: this.form.get('fishingGearsGroup.fishingGearsControl')!.value,
            poundNetId: undefined,
            excludedTariffsIds: excludedTariffsIds
        });

        return parameters;
    }

    private updateDuplicatedMarksAndPingers(values: FishingGearDTO[] | null | undefined): void {
        if (values !== null && values !== undefined) {
            const marksMarkNumbers = values.map(x => x.marks?.filter(x => x.isActive && x.selectedStatus === FishingGearMarkStatusesEnum.NEW).map(x => `${x.fullNumber?.prefix ?? ''}${x.fullNumber?.inputValue ?? ''}`));
            if (marksMarkNumbers !== null && marksMarkNumbers !== undefined && marksMarkNumbers.length > 0) {
                const markNumbers = marksMarkNumbers.reduce(x => x);
                const dupMarksCopy = this.duplicatedMarkNumbers.slice();

                if (markNumbers !== null && markNumbers !== undefined) {
                    for (const mark of dupMarksCopy) {
                        if (!markNumbers.includes(mark)) {
                            const indexToDelete = this.duplicatedMarkNumbers.findIndex(x => x === mark);
                            this.duplicatedMarkNumbers.splice(indexToDelete, 1);
                        }
                    }
                }
            }

            const pingersPingerNumbers = values.map(x => x.pingers?.filter(x => x.isActive && x.selectedStatus === FishingGearPingerStatusesEnum.NEW)?.map(x => x.number!));
            if (pingersPingerNumbers !== null && pingersPingerNumbers !== undefined && pingersPingerNumbers.length > 0) {
                const pingerNumbers = pingersPingerNumbers.reduce(x => x);
                const dupPingersCopy = this.duplicatedPingerNumbers.slice();

                for (const pinger of dupPingersCopy) {
                    if (!pingerNumbers?.includes(pinger)) {
                        const indexToDelete = this.duplicatedPingerNumbers.findIndex(x => x === pinger);
                        this.duplicatedPingerNumbers.splice(indexToDelete, 1);
                    }
                }
            }

            this.duplicatedMarkNumbers = this.duplicatedMarkNumbers.slice();
            this.duplicatedPingerNumbers = this.duplicatedPingerNumbers.slice();
        }
        else {
            this.duplicatedMarkNumbers = [];
            this.duplicatedPingerNumbers = [];
        }

        this.form.get('fishingGearsGroup.fishingGearsControl')!.updateValueAndValidity({ emitEvent: false });
    }

    private handleAddEditApplicationErrorResponse(errorResponse: HttpErrorResponse): void {
        if (errorResponse.error !== null
            && errorResponse.error !== undefined
            && errorResponse.error.messages !== null
            && errorResponse.error.messages !== undefined
        ) {
            const messages: string[] = errorResponse.error.messages;

            if (Array.isArray(messages) === true) {
                for (const message of messages) {
                    const validationError = CommercialFishingValidationErrorsEnum[message as keyof typeof CommercialFishingValidationErrorsEnum];

                    if (validationError === CommercialFishingValidationErrorsEnum.NoEDeliveryRegistration) {
                        this.hasNoEDeliveryRegistrationError = true;
                    }

                    if (validationError === CommercialFishingValidationErrorsEnum.InvalidPermitLisenseRegistrationNumber) {
                        if (this.isPublicApp) {
                            this.hasInvalidPermitLicenseRegistrationNumber = true;
                        }
                    }
                }

                const error = errorResponse.error as ErrorModel;
                if (error?.code === ErrorCode.DuplicatedMarksNumbers) {
                    this.duplicatedMarkNumbers = error?.messages ?? [];
                    this.form.get('fishingGearsGroup')!.setErrors({ 'duplicatedMarkNumbers': this.duplicatedMarkNumbers });
                    this.form.get('fishingGearsGroup')!.markAsTouched();
                }
                else if (error?.code === ErrorCode.DuplicatedPingersNumbers) {
                    this.duplicatedPingerNumbers = error?.messages ?? [];
                    this.form.get('fishingGearsGroup')!.setErrors({ 'duplicatedPingerNumbers': this.duplicatedPingerNumbers });
                    this.form.get('fishingGearsGroup')!.markAsTouched();
                }
            }
        }

        setTimeout(() => {
            this.validityCheckerGroup.validate();
        });
    }

    private getPermitLicensePageCodeByType(type: CommercialFishingTypesEnum): PageCodeEnum {
        switch (type) {
            case CommercialFishingTypesEnum.QuataSpeciesPermitLicense:
                return PageCodeEnum.CatchQuataSpecies;
            case CommercialFishingTypesEnum.PoundNetPermitLicense:
                return PageCodeEnum.PoundnetCommFishLic;
            case CommercialFishingTypesEnum.PermitLicense:
            default:
                return PageCodeEnum.RightToFishResource;
        }
    }

    private shouldHideBasicPaymentInfo(): boolean {
        return (this.model as PermitLicenseFishingGearsApplicationDTO)?.paymentInformation?.paymentType === null
            || (this.model as PermitLicenseFishingGearsApplicationDTO)?.paymentInformation?.paymentType === undefined
            || (this.model as PermitLicenseFishingGearsApplicationDTO)?.paymentInformation?.paymentType === '';
    }
}