import { HttpErrorResponse } from '@angular/common/http';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Observable, Subject } from 'rxjs';

import { FileTypeEnum } from '@app/enums/file-types.enum';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { ApplicationPaymentInformationDTO } from '@app/models/generated/dtos/ApplicationPaymentInformationDTO';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { ApplicationSubmittedByRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedForRegixDataDTO';
import { DuplicatesApplicationDTO } from '@app/models/generated/dtos/DuplicatesApplicationDTO';
import { DuplicatesApplicationRegixDataDTO } from '@app/models/generated/dtos/DuplicatesApplicationRegixDataDTO';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { IDuplicatesRegisterService } from '@app/interfaces/common-app/duplicates-register.interface';
import { DuplicatesRegisterPublicService } from '@app/services/public-app/duplicates-register-public.service';
import { DuplicatesRegisterEditDTO } from '@app/models/generated/dtos/DuplicatesRegisterEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { BuyerDuplicateDataDTO } from '@app/models/generated/dtos/BuyerDuplicateDataDTO';
import { PermitDuplicateDataDTO } from '@app/models/generated/dtos/PermitDuplicateDataDTO';
import { PermitLicenseDuplicateDataDTO } from '@app/models/generated/dtos/PermitLicenseDuplicateDataDTO';
import { QualifiedFisherDuplicateDataDTO } from '@app/models/generated/dtos/QualifiedFisherDuplicateDataDTO';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { PrintConfigurationsComponent } from '@app/components/common-app/applications/components/print-configurations/print-configurations.component';
import { PrintConfigurationParameters } from '@app/components/common-app/applications/models/print-configuration-parameters.model';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';

@Component({
    selector: 'duplicates-application',
    templateUrl: './duplicates-application.component.html'
})
export class DuplicatesApplicationComponent implements OnInit, AfterViewInit, IDialogComponent {
    public readonly buyerPageCodes: PageCodeEnum[] = [
        PageCodeEnum.DupFirstSaleBuyer
    ];

    public readonly firstSalePageCodes: PageCodeEnum[] = [
        PageCodeEnum.DupFirstSaleCenter
    ];

    public readonly permitPageCodes: PageCodeEnum[] = [
        PageCodeEnum.DupCommFish,
        PageCodeEnum.DupRightToFishThirdCountry,
        PageCodeEnum.DupPoundnetCommFish
    ];

    public readonly permitLicencePageCodes: PageCodeEnum[] = [
        PageCodeEnum.DupRightToFishResource,
        PageCodeEnum.DupPoundnetCommFishLic,
        PageCodeEnum.DupCatchQuataSpecies
    ];

    public readonly fisherPageCodes: PageCodeEnum[] = [
        PageCodeEnum.CompetencyDup
    ];

    public form!: FormGroup;

    public pageCode!: PageCodeEnum;

    public notifier: Notifier = new Notifier();
    public expectedResults: DuplicatesApplicationRegixDataDTO;
    public regixChecks: ApplicationRegiXCheckDTO[] = [];

    public isPublicApp: boolean = false;
    public isOnlineApplication: boolean = false;
    public isApplication: boolean = false;
    public loadRegisterFromApplication: boolean = false;
    public isEditing: boolean = false;
    public isRegisterEntry: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public showRegiXData: boolean = false;
    public isReadonly: boolean = false;
    public viewMode: boolean = false;
    public isPaid: boolean = false;
    public hasDelivery: boolean = false;
    public hideBasicPaymentInfo: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();
    public service!: IDuplicatesRegisterService;

    public hasNoEDeliveryRegistrationError: boolean = false;
    public buyerDoesNotExistError: boolean = false;
    public permitDoesNotExistError: boolean = false;
    public permitLicenceDoesNotExistError: boolean = false;
    public fisherDoesNotExistError: boolean = false;

    public buyers: NomenclatureDTO<number>[] = [];
    public fishers: NomenclatureDTO<number>[] = [];
    public permits: NomenclatureDTO<number>[] = [];
    public permitLicences: NomenclatureDTO<number>[] = [];

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private id: number | undefined;
    private applicationId: number | undefined;
    private isApplicationHistoryMode: boolean = false;
    private applicationsService: IApplicationsService | undefined;
    private dialogRightSideActions: IActionInfo[] | undefined;

    private model!: DuplicatesApplicationDTO | DuplicatesApplicationRegixDataDTO | DuplicatesRegisterEditDTO;

    private readonly printConfigurationsDialog: TLMatDialog<PrintConfigurationsComponent>;
    private readonly translate: FuseTranslationLoaderService;

    public constructor(printConfigurationsDialog: TLMatDialog<PrintConfigurationsComponent>, translate: FuseTranslationLoaderService) {
        this.isPublicApp = IS_PUBLIC_APP;
        this.printConfigurationsDialog = printConfigurationsDialog;
        this.translate = translate;

        this.expectedResults = new DuplicatesApplicationRegixDataDTO({
            submittedBy: new ApplicationSubmittedByRegixDataDTO(),
            submittedFor: new ApplicationSubmittedForRegixDataDTO(),
        });
    }

    public async ngOnInit(): Promise<void> {
        if (!this.isPublicApp) {
            if (this.buyerPageCodes.includes(this.pageCode) || this.firstSalePageCodes.includes(this.pageCode)) {
                this.buyers = await this.service.getRegisteredBuyers().toPromise();
            }
            else if (this.permitPageCodes.includes(this.pageCode)) {
                this.permits = await this.service.getPermits().toPromise();
            }
            else if (this.permitLicencePageCodes.includes(this.pageCode)) {
                this.permitLicences = await this.service.getPermitLicenses().toPromise();
            }
            else if (this.fisherPageCodes.includes(this.pageCode)) {
                this.fishers = await this.service.getQualifiedFishers().toPromise();
            }
        }

        // извличане на исторически дани за заявление
        if (this.isApplicationHistoryMode && this.applicationId !== undefined) {
            this.form.disable();

            if (this.applicationsService) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const application: DuplicatesApplicationDTO = new DuplicatesApplicationDTO(contentObject);
                        application.files = content.files;
                        application.applicationId = content.applicationId;

                        this.isPaid = application.isPaid!;
                        this.hasDelivery = application.hasDelivery!;
                        this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                        this.isOnlineApplication = application.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.model = application;
                        this.fillForm();
                    }
                });
            }
        }
        else if (this.applicationId !== undefined && this.id === undefined && !this.isApplication) {
            // извличане на данни за регистров запис от id на заявление
            if (this.loadRegisterFromApplication === true) {
                this.isEditing = true;

                this.service.getRegisterByApplicationId(this.applicationId, this.pageCode).subscribe({
                    next: (aquaculture: unknown) => {
                        this.model = aquaculture as DuplicatesRegisterEditDTO;
                        this.isOnlineApplication = (aquaculture as DuplicatesRegisterEditDTO).isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
            else {
                // извличане на данни за създаване на регистров запис от заявление
                this.isEditing = false;

                this.service.getApplicationDataForRegister(this.applicationId, this.pageCode).subscribe({
                    next: (aquaculture: DuplicatesRegisterEditDTO) => {
                        this.model = aquaculture;
                        this.isOnlineApplication = aquaculture.isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
        }
        else {
            if (this.isApplication && this.applicationId !== undefined) {
                // извличане на данни за RegiX справка от служител
                this.isEditing = false;

                if (this.showOnlyRegiXData) {
                    this.service.getRegixData(this.applicationId, this.pageCode).subscribe({
                        next: (regixData: RegixChecksWrapperDTO<DuplicatesApplicationRegixDataDTO>) => {
                            this.model = new DuplicatesApplicationRegixDataDTO(regixData.dialogDataModel);
                            this.expectedResults = new DuplicatesApplicationRegixDataDTO(regixData.regiXDataModel);

                            this.fillForm();
                        }
                    });
                }
                else {
                    // извличане на данни за заявление
                    this.isEditing = false;

                    this.service.getApplication(this.applicationId, this.showRegiXData, this.pageCode).subscribe({
                        next: (application: DuplicatesApplicationDTO) => {
                            application.applicationId = this.applicationId;

                            this.isOnlineApplication = application.isOnlineApplication!;
                            this.isPaid = application.isPaid!;
                            this.hasDelivery = application.hasDelivery!;
                            this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new DuplicatesApplicationRegixDataDTO(application.regiXDataModel);
                                application.regiXDataModel = undefined;
                            }

                            if (this.isPublicApp && this.isOnlineApplication && (application.submittedBy === undefined || application.submittedBy === null)) {
                                const service = this.service as DuplicatesRegisterPublicService;
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
            else if (this.id !== undefined) {
                // извличане на данни за регистров запис
                this.isEditing = true;
                this.isRegisterEntry = true;

                this.service.getDuplicateRegister(this.id).subscribe({
                    next: (aquaculture: DuplicatesRegisterEditDTO) => {
                        this.model = aquaculture;
                        this.isOnlineApplication = aquaculture.isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
        }

        if (this.isReadonly || this.viewMode) {
            this.form.disable();
        }
    }

    public ngAfterViewInit(): void {
        this.form.get('deliveryDataControl')?.valueChanges.subscribe({
            next: () => {
                this.hasNoEDeliveryRegistrationError = false;
            }
        });

        this.form.get('buyerUrorrControl')?.valueChanges.subscribe({
            next: () => {
                this.buyerDoesNotExistError = false;
            }
        });

        this.form.get('permitRegNumControl')?.valueChanges.subscribe({
            next: () => {
                this.permitDoesNotExistError = false;
            }
        });

        this.form.get('permitLicenceRegNumControl')?.valueChanges.subscribe({
            next: () => {
                this.permitLicenceDoesNotExistError = false;
            }
        });

        this.form.get('fisherControl')?.valueChanges.subscribe({
            next: () => {
                this.fisherDoesNotExistError = false;
            }
        });
    }

    public setData(data: DialogParamsModel, wrapperData: DialogWrapperData): void {
        this.id = data.id;
        this.applicationId = data.applicationId;
        this.isReadonly = data.isReadonly;
        this.isApplication = data.isApplication;
        this.isApplicationHistoryMode = data.isApplicationHistoryMode;
        this.viewMode = data.viewMode;
        this.showOnlyRegiXData = data.showOnlyRegiXData;
        this.showRegiXData = data.showRegiXData;
        this.applicationsService = data.applicationsService;
        this.pageCode = data.pageCode;
        this.loadRegisterFromApplication = data.loadRegisterFromApplication;
        this.service = data.service as IDuplicatesRegisterService;
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
        if (actionInfo.id === 'print') {
            const getPrintConfig: Observable<PrintConfigurationParameters> = this.getPrintConfigurations();

            getPrintConfig.subscribe({
                next: (configuration: PrintConfigurationParameters | undefined) => {
                    if (configuration !== null && configuration !== undefined) {
                        this.service.downloadDuplicate(this.id!, configuration).subscribe({
                            next: () => {
                                // nothing to do
                            }
                        });
                    }
                }
            });
        }
        else {
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
                        this.saveBtnClicked(actionInfo, dialogClose);
                    }
                    else if (actionInfo.id === 'save-print') {
                        const getPrintConfig: Observable<PrintConfigurationParameters> = this.getPrintConfigurations();

                        getPrintConfig.subscribe({
                            next: (configuration: PrintConfigurationParameters | undefined) => {
                                if (configuration !== null && configuration !== undefined) {
                                    this.service.addAndDownloadDuplicateRegister(this.model, configuration).subscribe({
                                        next: () => {
                                            dialogClose(this.model);
                                        }
                                    });
                                }
                            }
                        });
                    }
                }
            }
            else {
                dialogClose();
            }
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

    private getPrintConfigurations(): Observable<PrintConfigurationParameters> {
        return this.printConfigurationsDialog.open({
            TCtor: PrintConfigurationsComponent,
            translteService: this.translate,
            title: this.translate.getValue('aquacultures.print-configurations-dialog-title'),
            headerCancelButton: { cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); } },
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translate.getValue('aquacultures.choose-settings-and-print')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translate.getValue('common.cancel'),
            }
        }, '900px');
    }

    private buildForm(): void {
        this.form = new FormGroup({
            submittedByControl: new FormControl(null),
            submittedForControl: new FormControl(null),
            reasonControl: new FormControl(null, [Validators.required, Validators.maxLength(4000)]),
            deliveryDataControl: new FormControl(),
            applicationPaymentInformationControl: new FormControl(),
            filesControl: new FormControl(null)
        });

        if (this.isPublicApp) {
            if (this.buyerPageCodes.includes(this.pageCode) || this.firstSalePageCodes.includes(this.pageCode)) {
                this.form.addControl('buyerUrorrControl', new FormControl(null, [Validators.required, Validators.maxLength(50)]));
            }
            else if (this.permitPageCodes.includes(this.pageCode)) {
                this.form.addControl('permitRegNumControl', new FormControl(null, [Validators.required, Validators.maxLength(50)]));
            }
            else if (this.permitLicencePageCodes.includes(this.pageCode)) {
                this.form.addControl('permitLicenceRegNumControl', new FormControl(null, [Validators.required, Validators.maxLength(50)]));
            }
            else if (this.fisherPageCodes.includes(this.pageCode)) {
                this.form.addControl('fisherControl', new FormControl(null, Validators.required));
            }
        }
        else {
            if (this.buyerPageCodes.includes(this.pageCode) || this.firstSalePageCodes.includes(this.pageCode)) {
                this.form.addControl('buyerControl', new FormControl(null, Validators.required));
            }
            else if (this.permitPageCodes.includes(this.pageCode)) {
                this.form.addControl('permitControl', new FormControl(null, Validators.required));
            }
            else if (this.permitLicencePageCodes.includes(this.pageCode)) {
                this.form.addControl('permitLicenceControl', new FormControl(null, Validators.required));
            }
            else if (this.fisherPageCodes.includes(this.pageCode)) {
                this.form.addControl('fisherRegisterControl', new FormControl(null, Validators.required));
            }
        }
    }

    private fillForm(): void {
        if (this.model instanceof DuplicatesApplicationDTO) {
            this.fillFormApplication(this.model);

            if (this.showRegiXData) {
                this.fillFormRegiX(this.model);
            }
        }
        else if (this.model instanceof DuplicatesApplicationRegixDataDTO) {
            this.form.get('submittedByControl')!.setValue(this.model.submittedBy);
            this.form.get('submittedForControl')!.setValue(this.model.submittedFor);

            this.fillFormRegiX(this.model);
        }
        else if (this.model instanceof DuplicatesRegisterEditDTO) {
            this.fillFormRegister(this.model);
        }
    }

    private fillFormApplication(model: DuplicatesApplicationDTO): void {
        this.form.get('submittedByControl')!.setValue(model.submittedBy);
        this.form.get('submittedForControl')!.setValue(model.submittedFor);
        this.form.get('reasonControl')!.setValue(model.reason);

        if (this.isPublicApp) {
            if (this.buyerPageCodes.includes(this.pageCode) || this.firstSalePageCodes.includes(this.pageCode)) {
                this.form.get('buyerUrorrControl')!.setValue(model.buyer?.buyerUrorrNumber);
            }
            else if (this.permitPageCodes.includes(this.pageCode)) {
                this.form.get('permitRegNumControl')!.setValue(model.permit?.permitRegistrationNumber);
            }
            else if (this.permitLicencePageCodes.includes(this.pageCode)) {
                this.form.get('permitLicenceRegNumControl')!.setValue(model.permitLicence?.permitLicenceRegistrationNumber);
            }
            else if (this.fisherPageCodes.includes(this.pageCode)) {
                this.form.get('fisherControl')!.setValue(model.qualifiedFisher?.qualifiedFisher);
            }
        }
        else {
            if (this.buyerPageCodes.includes(this.pageCode) || this.firstSalePageCodes.includes(this.pageCode)) {
                this.form.get('buyerControl')!.setValue(this.buyers.find(x => x.value === model.buyer?.buyerId));
            }
            else if (this.permitPageCodes.includes(this.pageCode)) {
                this.form.get('permitControl')!.setValue(this.permits.find(x => x.value === model.permit?.permitId));
            }
            else if (this.permitLicencePageCodes.includes(this.pageCode)) {
                this.form.get('permitLicenceControl')!.setValue(this.permitLicences.find(x => x.value === model.permitLicence?.permitLicenceId));
            }
            else if (this.fisherPageCodes.includes(this.pageCode)) {
                this.form.get('fisherRegisterControl')!.setValue(this.fishers.find(x => x.value === model.qualifiedFisher?.qualifiedFisherId));
            }
        }

        if (this.hasDelivery === true) {
            this.form.get('deliveryDataControl')!.setValue(model.deliveryData);
        }

        if (this.isPaid === true) {
            this.form.get('applicationPaymentInformationControl')!.setValue((this.model as DuplicatesApplicationDTO).paymentInformation);
        }

        this.form.get('filesControl')!.setValue(model.files);
    }

    private fillFormRegiX(model: DuplicatesApplicationRegixDataDTO): void {
        if (model.applicationRegiXChecks !== undefined && model.applicationRegiXChecks !== null) {
            const checks: ApplicationRegiXCheckDTO[] = model.applicationRegiXChecks;

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

    private fillFormRegister(model: DuplicatesRegisterEditDTO): void {
        this.form.get('submittedForControl')!.setValue(model.submittedFor);
        this.form.get('reasonControl')!.setValue(model.reason);

        if (this.buyerPageCodes.includes(this.pageCode) || this.firstSalePageCodes.includes(this.pageCode)) {
            this.form.get('buyerControl')!.setValue(this.buyers.find(x => x.value === model.buyerId));
        }
        else if (this.permitPageCodes.includes(this.pageCode)) {
            this.form.get('permitControl')!.setValue(this.permits.find(x => x.value === model.permitId));
        }
        else if (this.permitLicencePageCodes.includes(this.pageCode)) {
            this.form.get('permitLicenceControl')!.setValue(this.permitLicences.find(x => x.value === model.permitLicenceId));
        }
        else if (this.fisherPageCodes.includes(this.pageCode)) {
            this.form.get('fisherRegisterControl')!.setValue(this.fishers.find(x => x.value === model.qualifiedFisherId));
        }

        this.form.get('filesControl')!.setValue(model.files);
    }

    private fillModel(): void {
        if (this.model instanceof DuplicatesApplicationDTO) {
            this.fillModelApplication(this.model);
        }
        else if (this.model instanceof DuplicatesApplicationRegixDataDTO) {
            this.fillModelRegix(this.model);
        }
        else if (this.model instanceof DuplicatesRegisterEditDTO) {
            this.fillModelRegister(this.model);
        }
    }

    private fillModelApplication(model: DuplicatesApplicationDTO): void {
        model.submittedBy = this.form.get('submittedByControl')!.value;
        model.submittedFor = this.form.get('submittedForControl')!.value;
        model.reason = this.form.get('reasonControl')!.value;

        model.pageCode = this.pageCode;

        if (this.isPublicApp) {
            if (this.buyerPageCodes.includes(this.pageCode) || this.firstSalePageCodes.includes(this.pageCode)) {
                model.buyer = new BuyerDuplicateDataDTO({
                    isOnline: this.isPublicApp,
                    buyerUrorrNumber: this.form.get('buyerUrorrControl')!.value
                });
            }
            else if (this.permitPageCodes.includes(this.pageCode)) {
                model.permit = new PermitDuplicateDataDTO({
                    isOnline: this.isPublicApp,
                    permitRegistrationNumber: this.form.get('permitRegNumControl')!.value
                });
            }
            else if (this.permitLicencePageCodes.includes(this.pageCode)) {
                model.permitLicence = new PermitLicenseDuplicateDataDTO({
                    isOnline: this.isPublicApp,
                    permitLicenceRegistrationNumber: this.form.get('permitLicenceRegNumControl')!.value
                });
            }
            else if (this.fisherPageCodes.includes(this.pageCode)) {
                model.qualifiedFisher = new QualifiedFisherDuplicateDataDTO({
                    isOnline: this.isPublicApp,
                    qualifiedFisher: this.form.get('fisherControl')!.value
                });
            }
        }
        else {
            if (this.buyerPageCodes.includes(this.pageCode) || this.firstSalePageCodes.includes(this.pageCode)) {
                model.buyer = new BuyerDuplicateDataDTO({
                    isOnline: this.isPublicApp,
                    buyerId: this.form.get('buyerControl')!.value?.value
                });
            }
            else if (this.permitPageCodes.includes(this.pageCode)) {
                model.permit = new PermitDuplicateDataDTO({
                    isOnline: this.isPublicApp,
                    permitId: this.form.get('permitControl')!.value?.value
                });
            }
            else if (this.permitLicencePageCodes.includes(this.pageCode)) {
                model.permitLicence = new PermitLicenseDuplicateDataDTO({
                    isOnline: this.isPublicApp,
                    permitLicenceId: this.form.get('permitLicenceControl')!.value?.value
                });
            }
            else if (this.fisherPageCodes.includes(this.pageCode)) {
                model.qualifiedFisher = new QualifiedFisherDuplicateDataDTO({
                    isOnline: this.isPublicApp,
                    qualifiedFisherId: this.form.get('fisherRegisterControl')!.value?.value
                });
            }
        }

        if (this.hasDelivery === true) {
            model.deliveryData = this.form.get('deliveryDataControl')!.value;
        }

        if (this.isPaid === true) {
            (this.model as DuplicatesApplicationDTO).paymentInformation = this.form.get('applicationPaymentInformationControl')!.value;
        }

        model.files = this.form.get('filesControl')!.value;
    }

    private fillModelRegix(model: DuplicatesApplicationRegixDataDTO): void {
        model.submittedBy = this.form.get('submittedByControl')!.value;
        model.submittedFor = this.form.get('submittedForControl')!.value;
    }

    private fillModelRegister(model: DuplicatesRegisterEditDTO): void {
        model.submittedFor = this.form.get('submittedForControl')!.value;
        model.reason = this.form.get('reasonControl')!.value;

        model.pageCode = this.pageCode;

        if (this.buyerPageCodes.includes(this.pageCode) || this.firstSalePageCodes.includes(this.pageCode)) {
            model.buyerId = this.form.get('buyerControl')!.value?.value;
        }
        else if (this.permitPageCodes.includes(this.pageCode)) {
            model.permitId = this.form.get('permitControl')!.value?.value;
        }
        else if (this.permitLicencePageCodes.includes(this.pageCode)) {
            model.permitLicenceId = this.form.get('permitLicenceControl')!.value?.value;
        }
        else if (this.fisherPageCodes.includes(this.pageCode)) {
            model.qualifiedFisherId = this.form.get('fisherRegisterControl')!.value?.value;
        }

        model.files = this.form.get('filesControl')!.value;
    }

    private saveApplication(dialogClose: DialogCloseCallback, saveAsDraft: boolean = false): Observable<boolean> {
        const saveOrEditDone: Subject<boolean> = new Subject<boolean>();

        this.saveOrEdit(saveAsDraft).subscribe({
            next: (id: number | void) => {
                this.hasNoEDeliveryRegistrationError = false;
                this.buyerDoesNotExistError = false;
                this.permitDoesNotExistError = false;
                this.permitLicenceDoesNotExistError = false;
                this.fisherDoesNotExistError = false;

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
                this.handleSaveErrorResponse(response);
            }
        });

        return saveOrEditDone.asObservable();
    }

    private saveOrEdit(fromSaveAsDraft: boolean): Observable<number | void> {
        this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model);

        if (this.model instanceof DuplicatesRegisterEditDTO) {
            return this.service.addDuplicateRegister(this.model);
        }
        else {
            if (this.model.id !== undefined && this.model.id !== null) {
                return this.service.editApplication(this.model, this.pageCode, fromSaveAsDraft);
            }
            return this.service.addApplication(this.model, this.pageCode);
        }
    }

    private handleSaveErrorResponse(response: HttpErrorResponse): void {
        if (response.error !== null && response.error !== undefined) {
            const error: ErrorModel = response.error;

            if (this.model instanceof DuplicatesApplicationDTO) {
                if (error.code === ErrorCode.NoEDeliveryRegistration) {
                    this.hasNoEDeliveryRegistrationError = true;
                    this.validityCheckerGroup.validate();
                }
                else if (error.code === ErrorCode.BuyerDoesNotExist) {
                    this.buyerDoesNotExistError = true;
                    this.validityCheckerGroup.validate();
                }
                else if (error.code === ErrorCode.PermitDoesNotExist) {
                    this.permitDoesNotExistError = true;
                    this.validityCheckerGroup.validate();
                }
                else if (error.code === ErrorCode.PermitLicenceDoesNotExist) {
                    this.permitLicenceDoesNotExistError = true;
                    this.validityCheckerGroup.validate();
                }
                else if (error.code === ErrorCode.QualifiedFisherDoesNotExist) {
                    this.fisherDoesNotExistError = true;
                    this.validityCheckerGroup.validate();
                }
            }
        }
    }

    private shouldHidePaymentData(): boolean {
        return (this.model as DuplicatesApplicationDTO)?.paymentInformation?.paymentType === null
            || (this.model as DuplicatesApplicationDTO)?.paymentInformation?.paymentType === undefined
            || (this.model as DuplicatesApplicationDTO)?.paymentInformation?.paymentType === '';
    }
}