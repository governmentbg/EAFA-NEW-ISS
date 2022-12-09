import { Component, EventEmitter, Injector, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable, Subject } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { IShipsRegisterService } from '@app/interfaces/common-app/ships-register.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { ApplicationSubmittedByRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedForRegixDataDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { ShipDeregistrationApplicationDTO } from '@app/models/generated/dtos/ShipDeregistrationApplicationDTO';
import { ShipDeregistrationRegixDataDTO } from '@app/models/generated/dtos/ShipDeregistrationRegixDataDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { ApplicationPaymentInformationDTO } from '@app/models/generated/dtos/ApplicationPaymentInformationDTO';
import { ErrorSnackbarComponent } from '@app/shared/components/error-snackbar/error-snackbar.component';
import { ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { ApplicationValidationErrorsEnum } from '@app/enums/application-validation-errors.enum';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { ShipsRegisterPublicService } from '@app/services/public-app/ships-register-public.service';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { FishingCapacityFreedActionsRegixDataDTO } from '@app/models/generated/dtos/FishingCapacityFreedActionsRegixDataDTO';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { NewCertificateData } from '../../fishing-capacity/acquired-fishing-capacity/acquired-fishing-capacity.component';
import { ShipsUtils } from '@app/shared/utils/ships.utils';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { IFishingCapacityService } from '@app/interfaces/common-app/fishing-capacity.interface';
import { FishingCapacityAdministrationService } from '@app/services/administration-app/fishing-capacity-administration.service';
import { FishingCapacityPublicService } from '@app/services/public-app/fishing-capacity-public.service';

@Component({
    selector: 'ship-deregistration',
    templateUrl: './ship-deregistration.component.html'
})
export class ShipDeregistrationComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    public readonly pageCode: PageCodeEnum = PageCodeEnum.DeregShip;

    public ships: ShipNomenclatureDTO[] = [];

    public notifier: Notifier = new Notifier();
    public expectedResults: ShipDeregistrationRegixDataDTO;
    public regixChecks: ApplicationRegiXCheckDTO[] = [];

    public isPublicApp: boolean = false;
    public isOnlineApplication: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();
    public isEditing: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public showRegiXData: boolean = false;
    public isReadonly: boolean = false;
    public viewMode: boolean = false;
    public isDraft: boolean = false;
    public isPaid: boolean = false;
    public hasDelivery: boolean = false;
    public hasFishingCapacity: boolean = false;
    public hasNoEDeliveryRegistrationError: boolean = false;
    public hideBasicPaymentInfo: boolean = false;
    public service!: IShipsRegisterService;

    public fishingCapacityService: IFishingCapacityService;

    public maxTonnage: number = 0;
    public maxPower: number = 0;
    public newCertificateData: NewCertificateData | undefined;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private applicationId: number | undefined;
    private isApplicationHistoryMode: boolean = false;
    private applicationsService: IApplicationsService | undefined;
    private dialogRightSideActions: IActionInfo[] | undefined;
    private nomenclatures: CommonNomenclatures;
    private translate: FuseTranslationLoaderService;
    private snackbar: MatSnackBar;

    private model!: ShipDeregistrationApplicationDTO | ShipDeregistrationRegixDataDTO;

    public constructor(
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        snackbar: MatSnackBar,
        injector: Injector
    ) {
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.snackbar = snackbar;

        this.isPublicApp = IS_PUBLIC_APP;

        if (this.isPublicApp) {
            this.fishingCapacityService = injector.get(FishingCapacityPublicService);
        }
        else {
            this.fishingCapacityService = injector.get(FishingCapacityAdministrationService);
        }

        this.expectedResults = new ShipDeregistrationRegixDataDTO({
            submittedBy: new ApplicationSubmittedByRegixDataDTO(),
            submittedFor: new ApplicationSubmittedForRegixDataDTO(),
            freedCapacityAction: new FishingCapacityFreedActionsRegixDataDTO()
        })
    }

    public async ngOnInit(): Promise<void> {
        this.form.get('deliveryDataControl')?.valueChanges.subscribe({
            next: () => {
                this.hasNoEDeliveryRegistrationError = false;
            }
        });

        this.form.get('shipControl')?.valueChanges.subscribe({
            next: (ship: ShipNomenclatureDTO | undefined) => {
                if (ship !== undefined && ship !== null && typeof ship !== 'string') {
                    this.hasFishingCapacity = ShipsUtils.hasFishingCapacity(ship);
                    this.maxTonnage = ship.grossTonnage!;
                    this.maxPower = ship.mainEnginePower!;

                    this.newCertificateData = new NewCertificateData({
                        tonnage: Number(this.maxTonnage),
                        power: Number(this.maxPower),
                        validThreeYears: true
                    });
                }
                else {
                    this.hasFishingCapacity = false;
                    this.maxTonnage = 0;
                    this.maxPower = 0;
                    this.newCertificateData = undefined;
                }
            }
        });

        if (!this.showOnlyRegiXData) {
            this.ships = await NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false
            ).toPromise();
        }

        // извличане на исторически данни за заявление
        if (this.isApplicationHistoryMode && this.applicationId !== undefined) {
            this.form.disable();

            if (this.applicationsService) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const application: ShipDeregistrationApplicationDTO = new ShipDeregistrationApplicationDTO(contentObject);
                        application.files = content.files;
                        application.applicationId = content.applicationId;

                        this.isPaid = application.isPaid!;
                        this.hasDelivery = application.hasDelivery!;
                        this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                        this.isOnlineApplication = application.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.model = application;
                        this.isDraft = application.isDraft!;
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
                        next: (regixData: RegixChecksWrapperDTO<ShipDeregistrationRegixDataDTO>) => {
                            this.model = new ShipDeregistrationRegixDataDTO(regixData.dialogDataModel);
                            this.expectedResults = new ShipDeregistrationRegixDataDTO(regixData.regiXDataModel);

                            this.fillForm();
                        }
                    });
                }
                else {
                    // извличане на данни за заявление
                    this.isEditing = false;

                    this.service.getApplication(this.applicationId, this.showRegiXData, this.pageCode).subscribe({
                        next: (application: ShipDeregistrationApplicationDTO) => {
                            application.applicationId = this.applicationId;
                            application.isDraft = application.isDraft ?? true;

                            this.isOnlineApplication = application.isOnlineApplication!;
                            this.isDraft = application.isDraft!;
                            this.isPaid = application.isPaid!;
                            this.hasDelivery = application.hasDelivery!;
                            this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new ShipDeregistrationRegixDataDTO(application.regiXDataModel);
                                application.regiXDataModel = undefined;
                            }

                            if (this.isPublicApp && this.isOnlineApplication && (application.submittedBy === undefined || application.submittedBy === null)) {
                                const service = this.service as ShipsRegisterPublicService;
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

                            if (!this.isDraft) {
                                this.form.get('shipControl')!.disable();
                            }
                        }
                    });
                }
            }
        }
    }

    public setData(data: DialogParamsModel, wrapperData: DialogWrapperData): void {
        this.applicationId = data.applicationId;
        this.isReadonly = data.isReadonly;
        this.isApplicationHistoryMode = data.isApplicationHistoryMode;
        this.viewMode = data.viewMode;
        this.showOnlyRegiXData = data.showOnlyRegiXData;
        this.showRegiXData = data.showRegiXData;
        this.applicationsService = data.applicationsService;
        this.service = data.service as IShipsRegisterService;
        this.dialogRightSideActions = wrapperData.rightSideActions;

        this.buildForm();
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.saveApplication(dialogClose);
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model);

        const applicationAction: boolean = ApplicationUtils.applicationDialogButtonClicked(new ApplicationDialogData({
            action: action,
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
                switch (action.id) {
                    case 'save':
                        return this.saveBtnClicked(action, dialogClose);
                }
            }
        }
        else {
            dialogClose();
        }
    }

    public fileTypeFilterFn(options: PermittedFileTypeDTO[]): PermittedFileTypeDTO[] {
        const pdfs: FileTypeEnum[] = [FileTypeEnum.SIGNEDAPPL, FileTypeEnum.APPLICATION_PDF];

        let result: PermittedFileTypeDTO[] = options;

        if (!this.isOnlineApplication) {
            result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        return result;
    }

    public shipControlErrorLabelTest(controlName: string, error: unknown, errorCode: string): TLError | undefined {
        if (controlName === 'shipControl') {
            if (errorCode === 'shipDestroyedOrDeregistered' && error === true) {
                return new TLError({
                    text: this.translate.getValue('ships-register.dereg-ship-deregistered-error'),
                    type: 'error'
                });
            }

            if (errorCode === 'shipThirdParty' && error === true) {
                return new TLError({
                    text: this.translate.getValue('ships-register.dereg-ship-third-party-error'),
                    type: 'error'
                });
            }
        }
        return undefined;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            submittedByControl: new FormControl(undefined),
            submittedForControl: new FormControl(undefined),
            shipControl: new FormControl(undefined, [Validators.required, this.shipValidator()]),
            reasonControl: new FormControl(undefined, Validators.required),
            actionsControl: new FormControl(undefined),
            deliveryDataControl: new FormControl(undefined),
            applicationPaymentInformationControl: new FormControl(),
            filesControl: new FormControl(undefined)
        });
    }

    private fillForm(): void {
        this.form.get('submittedByControl')!.setValue(this.model.submittedBy);
        this.form.get('submittedForControl')!.setValue(this.model.submittedFor);
        this.form.get('actionsControl')!.setValue(this.model.freedCapacityAction);

        if (this.model instanceof ShipDeregistrationRegixDataDTO) {
            this.fillFormRegiX();
        }
        else {
            const shipId: number | undefined = this.model.shipId;
            if (shipId !== undefined && shipId !== null) {
                this.form.get('shipControl')!.setValue(ShipsUtils.get(this.ships, shipId));
            }

            this.form.get('reasonControl')!.setValue(this.model.deregistrationReason);
            this.form.get('filesControl')!.setValue(this.model.files);

            if (this.hasDelivery === true) {
                this.form.get('deliveryDataControl')!.setValue(this.model.deliveryData);
            }

            if (this.isPaid === true) {
                this.form.get('applicationPaymentInformationControl')!.setValue(this.model.paymentInformation);
            }

            if (this.showRegiXData) {
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

        if (this.model instanceof ShipDeregistrationApplicationDTO) {
            this.model.shipId = this.form.get('shipControl')!.value?.value;
            this.model.deregistrationReason = this.form.get('reasonControl')!.value;
            this.model.files = this.form.get('filesControl')!.value;

            if (this.hasFishingCapacity === true) {
                this.model.freedCapacityAction = this.form.get('actionsControl')!.value;
            }

            if (this.hasDelivery === true) {
                this.model.deliveryData = this.form.get('deliveryDataControl')!.value;
            }

            if (this.isPaid === true) {
                this.model.paymentInformation = this.form.get('applicationPaymentInformationControl')!.value;
            }
        }
        else {
            this.model.freedCapacityAction = this.form.get('actionsControl')!.value;
        }
    }

    private saveApplication(dialogClose: DialogCloseCallback, fromSaveAsDraft: boolean = false): Observable<boolean> {
        const saveOrEditDone: EventEmitter<boolean> = new EventEmitter<boolean>();

        this.saveOrEdit(fromSaveAsDraft).subscribe({
            next: (id: number | void) => {
                this.hasNoEDeliveryRegistrationError = false;

                if (typeof id === 'number' && id !== undefined) {
                    this.model.applicationId = id;
                    dialogClose(this.model);
                }
                else {
                    dialogClose(this.model);
                }

                saveOrEditDone.emit(true);
                saveOrEditDone.complete();
            },
            error: (response: HttpErrorResponse) => {
                if (response.error?.messages !== null && response.error?.messages !== undefined) {
                    const messages: string[] = response.error.messages;

                    if (messages.length !== 0) {
                        this.snackbar.openFromComponent(ErrorSnackbarComponent, {
                            data: response.error as ErrorModel,
                            duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                            panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                        });
                    }
                    else {
                        this.snackbar.openFromComponent(ErrorSnackbarComponent, {
                            data: new ErrorModel({ messages: [this.translate.getValue('service.an-error-occurred-in-the-app')] }),
                            duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                            panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                        });
                    }

                    if (messages.find(message => message === ApplicationValidationErrorsEnum[ApplicationValidationErrorsEnum.NoEDeliveryRegistration])) {
                        this.hasNoEDeliveryRegistrationError = true;
                    }
                }
            }
        });

        return saveOrEditDone.asObservable();
    }

    private saveOrEdit(fromSaveAsDraft: boolean): Observable<number | void> {
        this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model);

        if (this.model instanceof ShipDeregistrationApplicationDTO && !this.model.isDraft) {
            return this.service.editApplication(this.model, this.pageCode, fromSaveAsDraft);
        }
        else {
            return this.service.addApplication(this.model, this.pageCode);
        }
    }

    private shipValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const shipId: number | undefined = this.form?.get('shipControl')?.value?.value;
            if (shipId !== undefined && shipId !== null) {
                const ship: ShipNomenclatureDTO = ShipsUtils.get(this.ships, shipId);

                if (ShipsUtils.isDestOrDereg(ship)) {
                    return { shipDestroyedOrDeregistered: true };
                }

                if (ShipsUtils.isThirdParty(ship)) {
                    return { shipThirdParty: true };
                }
            }

            return null;
        };
    }

    private shouldHidePaymentData(): boolean {
        return (this.model as ShipDeregistrationApplicationDTO)?.paymentInformation?.paymentType === null
            || (this.model as ShipDeregistrationApplicationDTO)?.paymentInformation?.paymentType === undefined
            || (this.model as ShipDeregistrationApplicationDTO)?.paymentInformation?.paymentType === '';
    }
}