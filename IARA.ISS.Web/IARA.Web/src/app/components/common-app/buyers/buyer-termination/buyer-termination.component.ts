import { HttpErrorResponse } from '@angular/common/http';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable, Subject } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { ApplicationValidationErrorsEnum } from '@app/enums/application-validation-errors.enum';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { IBuyersService } from '@app/interfaces/common-app/buyers.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { ApplicationPaymentInformationDTO } from '@app/models/generated/dtos/ApplicationPaymentInformationDTO';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { ApplicationSubmittedByRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedForRegixDataDTO';
import { BuyerTerminationApplicationDTO } from '@app/models/generated/dtos/BuyerTerminationApplicationDTO';
import { BuyerTerminationRegixDataDTO } from '@app/models/generated/dtos/BuyerTerminationRegixDataDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { BuyersPublicService } from '@app/services/public-app/buyers-public.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { ErrorSnackbarComponent } from '@app/shared/components/error-snackbar/error-snackbar.component';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { RequestProperties } from '@app/shared/services/request-properties';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';

@Component({
    selector: 'buyer-termination',
    templateUrl: './buyer-termination.component.html'
})
export class BuyerTerminationComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;

    public pageCode!: PageCodeEnum;

    public buyers: NomenclatureDTO<number>[] = [];

    public notifier: Notifier = new Notifier();
    public expectedResults: BuyerTerminationRegixDataDTO;
    public regixChecks: ApplicationRegiXCheckDTO[] = [];

    public paymentInformation: ApplicationPaymentInformationDTO | undefined;

    public isEditing: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public showRegiXData: boolean = false;
    public isOnlineApplication: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();
    public isReadonly: boolean = false;
    public viewMode: boolean = false;
    public isPaid: boolean = false;
    public hasDelivery: boolean = false;
    public hasNoEDeliveryRegistrationError: boolean = false;
    public buyerDoesNotExistError: boolean = false;
    public isPublicApp: boolean;
    public hideBasicPaymentInfo: boolean = false;
    public service!: IBuyersService;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private applicationId: number | undefined;
    private isApplicationHistoryMode: boolean = false;
    private applicationsService: IApplicationsService | undefined;
    private dialogRightSideActions: IActionInfo[] | undefined;
    private translate: FuseTranslationLoaderService;
    private snackbar: MatSnackBar;

    private model!: BuyerTerminationApplicationDTO | BuyerTerminationRegixDataDTO;

    public constructor(translate: FuseTranslationLoaderService, snackbar: MatSnackBar) {
        this.translate = translate;
        this.snackbar = snackbar;

        this.expectedResults = new BuyerTerminationRegixDataDTO({
            submittedBy: new ApplicationSubmittedByRegixDataDTO(),
            submittedFor: new ApplicationSubmittedForRegixDataDTO()
        });

        this.isPublicApp = IS_PUBLIC_APP;
    }

    public async ngOnInit(): Promise<void> {
        if (!this.isPublicApp) {
            if (this.pageCode === PageCodeEnum.TermFirstSaleBuyer) {
                this.buyers = await this.service.getAllBuyersNomenclatures().toPromise();
            }
            else if (this.pageCode === PageCodeEnum.TermFirstSaleCenter) {
                this.buyers = await this.service.getAllFirstSaleCentersNomenclatures().toPromise();
            }
        }

        // извличане на исторически данни за заявление
        if (this.isApplicationHistoryMode && this.applicationId !== undefined) {
            this.form.disable();

            if (this.applicationsService) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const application: BuyerTerminationApplicationDTO = new BuyerTerminationApplicationDTO(contentObject);
                        application.files = content.files;
                        application.applicationId = content.applicationId;

                        this.isPaid = application.isPaid!;
                        this.hasDelivery = application.hasDelivery!;
                        this.paymentInformation = application.paymentInformation;
                        this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                        this.isOnlineApplication = application.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.model = application;
                        this.model.isOnline = this.isPublicApp;

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
                        next: (regixData: RegixChecksWrapperDTO<BuyerTerminationRegixDataDTO>) => {
                            this.model = new BuyerTerminationRegixDataDTO(regixData.dialogDataModel);
                            this.model.isOnline = this.isPublicApp;

                            this.expectedResults = new BuyerTerminationRegixDataDTO(regixData.regiXDataModel);

                            this.fillForm();
                        }
                    });
                }
                else {
                    // извличане на данни за заявление
                    this.isEditing = false;

                    this.service.getApplication(this.applicationId, this.showRegiXData, this.pageCode).subscribe({
                        next: (application: BuyerTerminationApplicationDTO) => {
                            application.applicationId = this.applicationId;
                            application.isDraft = application.isDraft ?? true;

                            this.isPaid = application.isPaid!;
                            this.hasDelivery = application.hasDelivery!;
                            this.paymentInformation = application.paymentInformation;
                            this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                            this.isOnlineApplication = application.isOnlineApplication!;
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new BuyerTerminationRegixDataDTO(application.regiXDataModel);
                                application.regiXDataModel = undefined;
                            }

                            if (this.isPublicApp && this.isOnlineApplication && (application.submittedBy === undefined || application.submittedBy === null)) {
                                const service = this.service as BuyersPublicService;
                                service.getCurrentUserAsSubmittedBy().subscribe({
                                    next: (submittedBy: ApplicationSubmittedByDTO) => {
                                        application.submittedBy = submittedBy;
                                        this.model = application;
                                        this.model.isOnline = this.isPublicApp;

                                        this.fillForm();
                                    }
                                });
                            }
                            else {
                                this.model = application;
                                this.model.isOnline = this.isPublicApp;

                                this.fillForm();
                            }
                        }
                    });
                }
            }
        }
    }

    public ngAfterViewInit(): void {
        this.form.get('deliveryDataControl')?.valueChanges.subscribe({
            next: () => {
                this.hasNoEDeliveryRegistrationError = false;
            }
        });

        if (this.isPublicApp === true) {
            this.form.get('buyerUrorrNumberControl')!.valueChanges.subscribe({
                next: () => {
                    this.buyerDoesNotExistError = false;
                }
            });
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
        this.service = data.service as IBuyersService;
        this.pageCode = data.pageCode;
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

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
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
            reasonControl: new FormControl(null, Validators.required),
            deliveryDataControl: new FormControl(),
            filesControl: new FormControl(null)
        });

        if (this.isPublicApp === true) {
            this.form.addControl('buyerUrorrNumberControl', new FormControl(null, Validators.required));
        }
        else {
            this.form.addControl('buyerControl', new FormControl(null, Validators.required));
        }
    }

    private fillForm(): void {
        this.form.get('submittedByControl')!.setValue(this.model.submittedBy);
        this.form.get('submittedForControl')!.setValue(this.model.submittedFor);

        if (this.model instanceof BuyerTerminationRegixDataDTO) {
            this.fillFormRegiX();
        }
        else {
            if (this.isPublicApp === true) {
                this.form.get('buyerUrorrNumberControl')!.setValue(this.model.buyerUrorrNumber);
            }
            else {
                const buyerId: number | undefined = this.model.buyerId;
                this.form.get('buyerControl')!.setValue(this.buyers.find(x => x.value === buyerId));
            }

            this.form.get('reasonControl')!.setValue(this.model.deregistrationReason);
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
        this.model.pageCode = this.pageCode;
        this.model.submittedBy = this.form.get('submittedByControl')!.value;
        this.model.submittedFor = this.form.get('submittedForControl')!.value;

        if (this.model instanceof BuyerTerminationApplicationDTO) {
            if (this.isPublicApp === true) {
                this.model.buyerUrorrNumber = this.form.get('buyerUrorrNumberControl')!.value;
            }
            else {
                this.model.buyerId = this.form.get('buyerControl')!.value?.value;
            }

            this.model.deregistrationReason = this.form.get('reasonControl')!.value;
            this.model.files = this.form.get('filesControl')!.value;

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
                    const messages: string[] = response.error?.messages;
                    const error: ErrorModel | undefined = response.error as ErrorModel;

                    if (error !== null && error !== undefined) {
                        if (messages.length !== 0) {
                            this.snackbar.openFromComponent(ErrorSnackbarComponent, {
                                data: response.error as ErrorModel,
                                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                            });
                        }

                        if (messages.find(message => message === ApplicationValidationErrorsEnum[ApplicationValidationErrorsEnum.NoEDeliveryRegistration])) {
                            this.hasNoEDeliveryRegistrationError = true;
                        }

                        if (error.code === ErrorCode.BuyerDoesNotExist) {
                            this.buyerDoesNotExistError = true;
                        }
                    }
                    else {
                        this.snackbar.openFromComponent(ErrorSnackbarComponent, {
                            data: new ErrorModel({ messages: [this.translate.getValue('service.an-error-occurred-in-the-app')] }),
                            duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                            panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                        });
                    }
                }
            }
        });

        return saveOrEditDone.asObservable();
    }

    private saveOrEdit(fromSaveAsDraft: boolean): Observable<number | void> {
        this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model);

        if (this.model instanceof BuyerTerminationApplicationDTO && !this.model.isDraft) {
            return this.service.editApplication(this.model, this.pageCode, fromSaveAsDraft);
        }
        else {
            return this.service.addApplication(this.model, this.pageCode);
        }
    }

    private shouldHidePaymentData(): boolean {
        return this.paymentInformation?.paymentType === null
            || this.paymentInformation?.paymentType === undefined
            || this.paymentInformation?.paymentType === '';
    }
}