import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable, Subject } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { IFishingCapacityService } from '@app/interfaces/common-app/fishing-capacity.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { ApplicationSubmittedByRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedForRegixDataDTO';
import { IncreaseFishingCapacityApplicationDTO } from '@app/models/generated/dtos/IncreaseFishingCapacityApplicationDTO';
import { IncreaseFishingCapacityRegixDataDTO } from '@app/models/generated/dtos/IncreaseFishingCapacityRegixDataDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { FishingCapacityPublicService } from '@app/services/public-app/fishing-capacity-public.service';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { FishingCapacityFreedActionsRegixDataDTO } from '@app/models/generated/dtos/FishingCapacityFreedActionsRegixDataDTO';
import { NewCertificateData } from '../acquired-fishing-capacity/acquired-fishing-capacity.component';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { ApplicationPaymentInformationDTO } from '@app/models/generated/dtos/ApplicationPaymentInformationDTO';
import { ErrorSnackbarComponent } from '@app/shared/components/error-snackbar/error-snackbar.component';
import { ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { ApplicationValidationErrorsEnum } from '@app/enums/application-validation-errors.enum';
import { FishingCapacityFreedActionsDTO } from '@app/models/generated/dtos/FishingCapacityFreedActionsDTO';
import { FishingCapacityRemainderActionEnum } from '@app/enums/fishing-capacity-remainder-action.enum';
import { ShipNomenclatureFilters, ShipsUtils } from '@app/shared/utils/ships.utils';

@Component({
    selector: 'increase-fishing-capacity',
    templateUrl: './increase-fishing-capacity.component.html'
})
export class IncreaseFishingCapacityComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    public readonly pageCode: PageCodeEnum = PageCodeEnum.IncreaseFishCap;

    public ships: ShipNomenclatureDTO[] = [];

    public notifier: Notifier = new Notifier();
    public expectedResults: IncreaseFishingCapacityRegixDataDTO;
    public regixChecks: ApplicationRegiXCheckDTO[] = [];

    public paymentInformation: ApplicationPaymentInformationDTO | undefined;

    public isPublicApp: boolean = false;
    public isOnlineApplication: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();
    public isEditing: boolean = false;
    public isDraft: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public showRegiXData: boolean = false;
    public isReadonly: boolean = false;
    public viewMode: boolean = false;
    public isPaid: boolean = false;
    public hasDelivery: boolean = false;
    public hasNoEDeliveryRegistrationError: boolean = false;
    public willIssueCapacityCertificates: boolean = false;
    public hideBasicPaymentInfo: boolean = false;
    public service!: IFishingCapacityService;

    public remainingTonnage: number = 0;
    public remainingPower: number = 0;
    public newCertificateData: NewCertificateData | undefined;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private applicationId: number | undefined;
    private isApplicationHistoryMode: boolean = false;
    private applicationsService: IApplicationsService | undefined;
    private dialogRightSideActions: IActionInfo[] | undefined;
    private nomenclatures: CommonNomenclatures;

    private model!: IncreaseFishingCapacityApplicationDTO | IncreaseFishingCapacityRegixDataDTO;

    private translate: FuseTranslationLoaderService;
    private snackbar: MatSnackBar;

    public constructor(
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        snackbar: MatSnackBar
    ) {
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.snackbar = snackbar;

        this.isPublicApp = IS_PUBLIC_APP;

        this.expectedResults = new IncreaseFishingCapacityRegixDataDTO({
            submittedBy: new ApplicationSubmittedByRegixDataDTO(),
            submittedFor: new ApplicationSubmittedForRegixDataDTO(),
            remainingCapacityAction: new FishingCapacityFreedActionsRegixDataDTO()
        });
    }

    public async ngOnInit(): Promise<void> {
        this.form.get('deliveryDataControl')?.valueChanges.subscribe({
            next: () => {
                this.hasNoEDeliveryRegistrationError = false;
            }
        });

        this.ships = await NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false
        ).toPromise();

        this.ships = ShipsUtils.filter(this.ships, new ShipNomenclatureFilters({
            isThirdPartyShip: false,
            hasFishingCapacity: true,
            isDestOrDereg: false
        }));

        // извличане на исторически данни за заявление
        if (this.isApplicationHistoryMode && this.applicationId !== undefined) {
            this.form.disable();

            if (this.applicationsService) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const application: IncreaseFishingCapacityApplicationDTO = new IncreaseFishingCapacityApplicationDTO(contentObject);
                        application.files = content.files;
                        application.applicationId = content.applicationId;

                        this.isPaid = application.isPaid!;
                        this.hasDelivery = application.hasDelivery!;
                        this.paymentInformation = application.paymentInformation;
                        this.hideBasicPaymentInfo = this.shouldHidePaymentData();
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
                        next: (regixData: RegixChecksWrapperDTO<IncreaseFishingCapacityRegixDataDTO>) => {
                            this.model = new IncreaseFishingCapacityRegixDataDTO(regixData.dialogDataModel);
                            this.expectedResults = new IncreaseFishingCapacityRegixDataDTO(regixData.regiXDataModel);

                            this.fillForm();
                        }
                    });
                }
                else {
                    // извличане на данни за заявление
                    this.isEditing = false;

                    this.service.getApplication(this.applicationId, this.showRegiXData, this.pageCode).subscribe({
                        next: (application: IncreaseFishingCapacityApplicationDTO) => {
                            application.applicationId = this.applicationId;
                            application.isDraft = application.isDraft ?? true;

                            this.isDraft = application.isDraft;
                            this.isPaid = application.isPaid!;
                            this.hasDelivery = application.hasDelivery!;
                            this.paymentInformation = application.paymentInformation;
                            this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                            this.isOnlineApplication = application.isOnlineApplication!;
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new IncreaseFishingCapacityRegixDataDTO(application.regiXDataModel);
                                application.regiXDataModel = undefined;
                            }

                            if (this.isPublicApp && this.isOnlineApplication && (application.submittedBy === undefined || application.submittedBy === null)) {
                                const service = this.service as FishingCapacityPublicService;
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

    public setData(data: DialogParamsModel, wrapperData: DialogWrapperData): void {
        this.applicationId = data.applicationId;
        this.isReadonly = data.isReadonly;
        this.isApplicationHistoryMode = data.isApplicationHistoryMode;
        this.viewMode = data.viewMode;
        this.showOnlyRegiXData = data.showOnlyRegiXData;
        this.showRegiXData = data.showRegiXData;
        this.applicationsService = data.applicationsService;
        this.service = data.service as IFishingCapacityService;
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
                if (action.id === 'save') {
                    return this.saveBtnClicked(action, dialogClose);
                }
            }
        }
        else {
            dialogClose();
        }
    }

    public onAcquiredGrossTonnageChanged(acquiredTonnage: number | undefined): void {
        const tonnage: number = this.form.get('grossTonnageControl')!.value ?? 0;

        acquiredTonnage ??= 0;
        this.remainingTonnage = acquiredTonnage - tonnage;
        this.remainingTonnage = this.remainingTonnage < 0 ? 0 : this.remainingTonnage;

        this.willIssueCapacityCertificates = this.remainingPower > 0 || this.remainingTonnage > 0;
    }

    public onAcquiredPowerChanged(acquiredPower: number | undefined): void {
        const power: number = this.form.get('powerControl')!.value ?? 0;

        acquiredPower ??= 0;
        this.remainingPower = acquiredPower - power;
        this.remainingPower = this.remainingPower < 0 ? 0 : this.remainingPower;

        this.willIssueCapacityCertificates = this.remainingPower > 0 || this.remainingTonnage > 0;
    }

    public onNewCertificateDataChanged(newCertificateData: NewCertificateData): void {
        this.newCertificateData = newCertificateData;
    }

    public fileTypeFilterFn(options: PermittedFileTypeDTO[]): PermittedFileTypeDTO[] {
        const pdfs: FileTypeEnum[] = [FileTypeEnum.SIGNEDAPPL, FileTypeEnum.APPLICATION_PDF];

        let result: PermittedFileTypeDTO[] = options;

        if (!this.isOnlineApplication) {
            result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        return result;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            submittedByControl: new FormControl(null),
            submittedForControl: new FormControl(null),
            shipControl: new FormControl(null, Validators.required),
            grossTonnageControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 2)]),
            powerControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 2)]),
            acquiredCapacityControl: new FormControl(null),
            actionsControl: new FormControl(null),
            deliveryDataControl: new FormControl(null),
            filesControl: new FormControl(null)
        });

        this.form.get('actionsControl')!.valueChanges.subscribe({
            next: (actions: FishingCapacityFreedActionsDTO) => {
                if (actions) {
                    this.willIssueCapacityCertificates =
                        actions.action !== FishingCapacityRemainderActionEnum.NoCertificate
                        && (this.remainingTonnage > 0 || this.remainingPower > 0);
                }
                else {
                    this.willIssueCapacityCertificates = false;
                }
            }
        });
    }

    private fillForm(): void {
        this.form.get('submittedByControl')!.setValue(this.model.submittedBy);
        this.form.get('submittedForControl')!.setValue(this.model.submittedFor);
        this.form.get('actionsControl')!.setValue(this.model.remainingCapacityAction);

        if (this.model instanceof IncreaseFishingCapacityRegixDataDTO) {
            this.fillFormRegiX();
        }
        else {
            if (this.model.shipId !== undefined && this.model.shipId !== null) {
                this.form.get('shipControl')!.setValue(ShipsUtils.get(this.ships, this.model.shipId));
            }

            this.form.get('grossTonnageControl')!.setValue(this.model.increaseGrossTonnageBy?.toFixed(2));
            this.form.get('powerControl')!.setValue(this.model.increasePowerBy?.toFixed(2));
            this.form.get('acquiredCapacityControl')!.setValue(this.model.acquiredCapacity);
            this.form.get('filesControl')!.setValue(this.model.files);

            if (this.hasDelivery === true) {
                this.form.get('deliveryDataControl')!.setValue(this.model.deliveryData);
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

        if (this.model instanceof IncreaseFishingCapacityApplicationDTO) {
            this.model.shipId = this.form.get('shipControl')!.value?.value;
            this.model.increaseGrossTonnageBy = this.form.get('grossTonnageControl')!.value;
            this.model.increasePowerBy = this.form.get('powerControl')!.value;
            this.model.acquiredCapacity = this.form.get('acquiredCapacityControl')!.value;
            this.model.files = this.form.get('filesControl')!.value;

            if (this.willIssueCapacityCertificates) {
                this.model.remainingCapacityAction = this.form.get('actionsControl')!.value;
            }
            else {
                this.model.remainingCapacityAction = undefined;
            }

            if (this.hasDelivery === true) {
                this.model.deliveryData = this.form.get('deliveryDataControl')!.value;
            }
        }
    }

    private saveApplication(dialogClose: DialogCloseCallback, saveAsDraft: boolean = false): Observable<boolean> {
        const saveOrEditDone: Subject<boolean> = new Subject<boolean>();

        this.saveOrEdit(saveAsDraft).subscribe({
            next: (id: number | void) => {
                this.hasNoEDeliveryRegistrationError = false;

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

        return saveOrEditDone;
    }

    private saveOrEdit(saveAsDraft: boolean): Observable<number | void> {
        this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model);

        if (this.model instanceof IncreaseFishingCapacityApplicationDTO && !this.model.isDraft) {
            return this.service.editApplication(this.model, this.pageCode, saveAsDraft);
        }
        return this.service.addApplication(this.model, this.pageCode);
    }

    private shouldHidePaymentData(): boolean {
        return this.paymentInformation?.paymentType === null
            || this.paymentInformation?.paymentType === undefined
            || this.paymentInformation?.paymentType === '';
    }
}