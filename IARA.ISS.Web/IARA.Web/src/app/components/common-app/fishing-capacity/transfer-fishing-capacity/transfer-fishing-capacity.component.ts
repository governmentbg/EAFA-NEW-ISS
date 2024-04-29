import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ApplicationValidationErrorsEnum } from '@app/enums/application-validation-errors.enum';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { IFishingCapacityService } from '@app/interfaces/common-app/fishing-capacity.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ErrorModel } from '@app/models/common/exception.model';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { ApplicationSubmittedByRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedForRegixDataDTO';
import { FishingCapacityCertificateNomenclatureDTO } from '@app/models/generated/dtos/FishingCapacityCertificateNomenclatureDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { TransferFishingCapacityApplicationDTO } from '@app/models/generated/dtos/TransferFishingCapacityApplicationDTO';
import { TransferFishingCapacityRegixDataDTO } from '@app/models/generated/dtos/TransferFishingCapacityRegixDataDTO';
import { FishingCapacityPublicService } from '@app/services/public-app/fishing-capacity-public.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { RequestProperties } from '@app/shared/services/request-properties';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { Observable, Subject } from 'rxjs';


@Component({
    selector: 'transfer-fishing-capacity',
    templateUrl: './transfer-fishing-capacity.component.html',
    styleUrls: ['./transfer-fishing-capacity.component.scss']
})
export class TransferFishingCapacityComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;
    public tonnageControl: FormControl;
    public powerControl: FormControl;
    public validToControl: FormControl;

    public readonly pageCode: PageCodeEnum = PageCodeEnum.TransferFishCap;

    public notifier: Notifier = new Notifier();
    public expectedResults: TransferFishingCapacityRegixDataDTO;
    public regixChecks: ApplicationRegiXCheckDTO[] = [];

    public certificates: FishingCapacityCertificateNomenclatureDTO[] = [];

    public maxGrossTonnage: number = 0;
    public maxPower: number = 0;

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
    public hasNoEDeliveryRegistrationError: boolean = false;
    public hideBasicPaymentInfo: boolean = false;
    public licenceInvalid: boolean = false;
    public service!: IFishingCapacityService;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private isApplication: boolean = true;
    private applicationId: number | undefined;
    private isApplicationHistoryMode: boolean = false;
    private applicationsService: IApplicationsService | undefined;
    private dialogRightSideActions: IActionInfo[] | undefined;

    private translate: FuseTranslationLoaderService;
    private snackbar: TLSnackbar;
    private datePipe: DatePipe;

    private model!: TransferFishingCapacityApplicationDTO | TransferFishingCapacityRegixDataDTO;

    public constructor(
        datePipe: DatePipe,
        translate: FuseTranslationLoaderService,
        snackbar: TLSnackbar
    ) {
        this.datePipe = datePipe;
        this.translate = translate;
        this.snackbar = snackbar;

        this.expectedResults = new TransferFishingCapacityRegixDataDTO({
            submittedBy: new ApplicationSubmittedByRegixDataDTO(),
            submittedFor: new ApplicationSubmittedForRegixDataDTO(),
            holders: []
        });

        this.isPublicApp = IS_PUBLIC_APP;

        this.tonnageControl = new FormControl({ value: '', disabled: true });
        this.powerControl = new FormControl({ value: '', disabled: true });
        this.validToControl = new FormControl({ value: '', disabled: true });
    }

    public async ngOnInit(): Promise<void> {
        this.form.get('deliveryDataControl')?.valueChanges.subscribe({
            next: () => {
                this.hasNoEDeliveryRegistrationError = false;
            }
        });

        this.certificates = await this.service.getAllCapacityCertificateNomenclatures().toPromise();

        // извличане на исторически данни за заявление
        if (this.isApplicationHistoryMode && this.applicationId !== undefined) {
            this.form.disable();

            if (this.applicationsService) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const application: TransferFishingCapacityApplicationDTO = new TransferFishingCapacityApplicationDTO(contentObject);
                        application.files = content.files;
                        application.applicationId = content.applicationId;

                        this.isPaid = application.isPaid!;
                        this.hasDelivery = application.hasDelivery!;
                        this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                        this.isOnlineApplication = application.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.model = application;
                        this.isDraft = application.isDraft ?? true;
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
        else {
            if (this.isReadonly || this.viewMode) {
                this.form.disable();
            }

            if (this.applicationId !== undefined) {
                // извличане на данни за RegiX справка от служител
                this.isEditing = false;

                if (this.showOnlyRegiXData) {
                    this.service.getRegixData(this.applicationId, this.pageCode).subscribe({
                        next: (regixData: RegixChecksWrapperDTO<TransferFishingCapacityRegixDataDTO>) => {
                            this.model = new TransferFishingCapacityRegixDataDTO(regixData.dialogDataModel);
                            this.expectedResults = new TransferFishingCapacityRegixDataDTO(regixData.regiXDataModel);

                            this.fillForm();
                        }
                    });
                }
                else {
                    // извличане на данни за заявление
                    this.isEditing = false;

                    this.service.getApplication(this.applicationId, this.showRegiXData, this.pageCode).subscribe({
                        next: (application: TransferFishingCapacityApplicationDTO) => {
                            application.applicationId = this.applicationId;
                            application.isDraft = application.isDraft ?? true;

                            this.isPaid = application.isPaid!;
                            this.hasDelivery = application.hasDelivery!;
                            this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                            this.isOnlineApplication = application.isOnlineApplication!;
                            this.isDraft = application.isDraft;
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new TransferFishingCapacityRegixDataDTO(application.regiXDataModel);
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

    public ngAfterViewInit(): void {
        if (!this.showOnlyRegiXData) {
            this.form.get('capacityCertificateControl')!.valueChanges.subscribe({
                next: (licence: FishingCapacityCertificateNomenclatureDTO) => {
                    if (licence !== undefined && licence !== null && licence instanceof NomenclatureDTO) {
                        this.maxGrossTonnage = licence.grossTonnage ?? 0;
                        this.maxPower = licence.power ?? 0;

                        this.tonnageControl.setValue(licence.grossTonnage!.toFixed(2));
                        this.powerControl.setValue(licence.power!.toFixed(2));
                        this.validToControl.setValue(this.datePipe.transform(licence.validTo!, 'dd.MM.yyyy'));

                        this.licenceInvalid = licence.validTo!.getTime() < Date.now();
                    }
                    else {
                        this.maxGrossTonnage = 0;
                        this.maxPower = 0;

                        this.validToControl.setValue('');
                        this.tonnageControl.setValue('');
                        this.powerControl.setValue('');

                        this.licenceInvalid = false;
                    }
                }
            });
        }
    }

    public setData(data: DialogParamsModel, wrapperData: DialogWrapperData): void {
        this.isApplication = data.isApplication;
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
        this.markAllAsTouched();
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
            this.markAllAsTouched();
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

    public fileTypeFilterFn(options: PermittedFileTypeDTO[]): PermittedFileTypeDTO[] {
        const pdfs: FileTypeEnum[] = [FileTypeEnum.SIGNEDAPPL, FileTypeEnum.APPLICATION_PDF];
        const offlines: FileTypeEnum[] = [FileTypeEnum.PAYEDFEE, FileTypeEnum.SCANNED_FORM];

        let result: PermittedFileTypeDTO[] = options;

        if (!this.isOnlineApplication) {
            result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        if (this.isOnlineApplication) {
            if (this.isApplication && !this.isReadonly) {
                result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
            }

            result = result.filter(x => !offlines.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        return result;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            submittedByControl: new FormControl(undefined),
            submittedForControl: new FormControl(undefined),
            capacityCertificateControl: new FormControl(undefined, Validators.required),
            holdersControl: new FormControl(undefined),
            deliveryDataControl: new FormControl(undefined),
            applicationPaymentInformationControl: new FormControl(),
            filesControl: new FormControl(undefined)
        });
    }

    private fillForm(): void {
        this.form.get('submittedByControl')!.setValue(this.model.submittedBy);
        this.form.get('submittedForControl')!.setValue(this.model.submittedFor);
        this.form.get('holdersControl')!.setValue(this.model.holders);

        if (this.model instanceof TransferFishingCapacityRegixDataDTO) {
            this.fillFormRegiX();
        }
        else {
            const capacityCertificateId: number | undefined = this.model.capacityCertificateId;
            if (capacityCertificateId !== undefined && capacityCertificateId !== null) {
                this.form.get('capacityCertificateControl')!.setValue(this.certificates.find(x => x.value === capacityCertificateId));
            }
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
        this.model.holders = this.form.get('holdersControl')!.value;

        if (this.model instanceof TransferFishingCapacityApplicationDTO) {
            this.model.capacityCertificateId = this.form.get('capacityCertificateControl')!.value?.value;
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

                        this.snackbar.errorModel(response.error as ErrorModel, RequestProperties.DEFAULT);
                    }
                    else {
                        this.snackbar.error(this.translate.getValue('service.an-error-occurred-in-the-app'), RequestProperties.DEFAULT.showExceptionDurationErr, RequestProperties.DEFAULT.showExceptionColorClassErr);
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

        if (this.model instanceof TransferFishingCapacityApplicationDTO && !this.isDraft) {
            if (this.isApplication) {
                return this.service.editApplication(this.model, this.pageCode, saveAsDraft);
            }
            return this.service.completeTransferFishingCapacityApplication(this.model);
        }
        return this.service.addApplication(this.model, this.pageCode);
    }

    private markAllAsTouched(): void {
        this.form.markAllAsTouched();
        this.form.get('holdersControl')!.updateValueAndValidity();
    }

    private shouldHidePaymentData(): boolean {
        return (this.model as TransferFishingCapacityApplicationDTO)?.paymentInformation?.paymentType === null
            || (this.model as TransferFishingCapacityApplicationDTO)?.paymentInformation?.paymentType === undefined
            || (this.model as TransferFishingCapacityApplicationDTO)?.paymentInformation?.paymentType === '';
    }
}