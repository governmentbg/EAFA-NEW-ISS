﻿import { HttpErrorResponse } from '@angular/common/http';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ApplicationValidationErrorsEnum } from '@app/enums/application-validation-errors.enum';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { IAquacultureFacilitiesService } from '@app/interfaces/common-app/aquaculture-facilities.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ErrorModel } from '@app/models/common/exception.model';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { ApplicationSubmittedByRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedForRegixDataDTO';
import { AquacultureChangeOfCircumstancesApplicationDTO } from '@app/models/generated/dtos/AquacultureChangeOfCircumstancesApplicationDTO';
import { AquacultureChangeOfCircumstancesRegixDataDTO } from '@app/models/generated/dtos/AquacultureChangeOfCircumstancesRegixDataDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { AquacultureFacilitiesPublicService } from '@app/services/public-app/aquaculture-facilities-public.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { Observable, Subject } from 'rxjs';
import { map } from 'rxjs/operators';


@Component({
    selector: 'aquaculture-change-of-circumstances',
    templateUrl: './aquaculture-change-of-circumstances.component.html'
})
export class AquacultureChangeOfCircumstancesComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;

    public readonly pageCode: PageCodeEnum = PageCodeEnum.AquaFarmChange;

    public aquacultures: NomenclatureDTO<number>[] = [];

    public notifier: Notifier = new Notifier();
    public expectedResults: AquacultureChangeOfCircumstancesRegixDataDTO;
    public regixChecks: ApplicationRegiXCheckDTO[] = [];

    public isPublicApp: boolean = false;
    public isOnlineApplication: boolean = false;
    public isEditing: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public showRegiXData: boolean = false;
    public isReadonly: boolean = false;
    public viewMode: boolean = false;
    public isPaid: boolean = false;
    public hasDelivery: boolean = false;
    public hasNoEDeliveryRegistrationError: boolean = false;
    public hideBasicPaymentInfo: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();
    public service!: IAquacultureFacilitiesService;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private applicationId: number | undefined;
    private isApplicationHistoryMode: boolean = false;
    private applicationsService: IApplicationsService | undefined;
    private dialogRightSideActions: IActionInfo[] | undefined;
    private translate: FuseTranslationLoaderService;
    private snackbar: TLSnackbar;

    private model!: AquacultureChangeOfCircumstancesApplicationDTO | AquacultureChangeOfCircumstancesRegixDataDTO;

    public constructor(translate: FuseTranslationLoaderService, snackbar: TLSnackbar) {
        this.translate = translate;
        this.snackbar = snackbar;

        this.isPublicApp = IS_PUBLIC_APP;

        this.expectedResults = new AquacultureChangeOfCircumstancesRegixDataDTO({
            submittedBy: new ApplicationSubmittedByRegixDataDTO(),
            submittedFor: new ApplicationSubmittedForRegixDataDTO(),
            changes: []
        });
    }

    public async ngOnInit(): Promise<void> {
        this.aquacultures = await this.getAllAquacultureNomenclatures();

        // извличане на исторически данни за заявление
        if (this.isApplicationHistoryMode && this.applicationId !== undefined) {
            this.form.disable();

            if (this.applicationsService) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const application: AquacultureChangeOfCircumstancesApplicationDTO = new AquacultureChangeOfCircumstancesApplicationDTO(contentObject);
                        application.files = content.files;
                        application.applicationId = content.applicationId;

                        this.isPaid = application.isPaid!;
                        this.hasDelivery = application.hasDelivery!;
                        this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                        this.isOnlineApplication = application.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.model = application;
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
                        next: (regixData: RegixChecksWrapperDTO<AquacultureChangeOfCircumstancesRegixDataDTO>) => {
                            this.model = new AquacultureChangeOfCircumstancesRegixDataDTO(regixData.dialogDataModel);
                            this.expectedResults = new AquacultureChangeOfCircumstancesRegixDataDTO(regixData.regiXDataModel);

                            this.fillForm();
                        }
                    });
                }
                else {
                    // извличане на данни за заявление
                    this.isEditing = false;

                    this.service.getApplication(this.applicationId, this.showRegiXData, this.pageCode).subscribe({
                        next: (application: AquacultureChangeOfCircumstancesApplicationDTO) => {
                            application.applicationId = this.applicationId;
                            application.isDraft = application.isDraft ?? true;

                            this.isOnlineApplication = application.isOnlineApplication!;
                            this.isPaid = application.isPaid!;
                            this.hasDelivery = application.hasDelivery!;
                            this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new AquacultureChangeOfCircumstancesRegixDataDTO(application.regiXDataModel);
                                application.regiXDataModel = undefined;
                            }

                            if (this.isPublicApp && this.isOnlineApplication && (application.submittedBy === undefined || application.submittedBy === null)) {
                                const service = this.service as AquacultureFacilitiesPublicService;
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
        this.service = data.service as IAquacultureFacilitiesService;
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

    private buildForm(): void {
        this.form = new FormGroup({
            submittedByControl: new FormControl(null),
            submittedForControl: new FormControl(null),
            aquacultureControl: new FormControl(null, Validators.required),
            changesControl: new FormControl(null),
            deliveryDataControl: new FormControl(),
            applicationPaymentInformationControl: new FormControl(),
            filesControl: new FormControl(null)
        });
    }

    private fillForm(): void {
        this.form.get('submittedByControl')!.setValue(this.model.submittedBy);
        this.form.get('submittedForControl')!.setValue(this.model.submittedFor);
        this.form.get('changesControl')!.setValue(this.model.changes);

        const aquacultureFacilityId: number | undefined = this.model.aquacultureFacilityId;
        this.form.get('aquacultureControl')!.setValue(this.aquacultures.find(x => x.value === aquacultureFacilityId));

        if (this.model instanceof AquacultureChangeOfCircumstancesRegixDataDTO) {
            this.form.get('aquacultureControl')!.disable();
            this.fillFormRegiX();
        }
        else {
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
        this.model.changes = this.form.get('changesControl')!.value;

        if (this.model instanceof AquacultureChangeOfCircumstancesApplicationDTO) {
            this.model.aquacultureFacilityId = this.form.get('aquacultureControl')!.value?.value;
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
                        this.snackbar.errorModel(response.error as ErrorModel);
                    }
                    else {
                        this.snackbar.error(this.translate.getValue('service.an-error-occurred-in-the-app'));
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

        if (this.model instanceof AquacultureChangeOfCircumstancesApplicationDTO && !this.model.isDraft) {
            return this.service.editApplication(this.model, this.pageCode, fromSaveAsDraft);
        }
        else {
            return this.service.addApplication(this.model, this.pageCode);
        }
    }

    private getAllAquacultureNomenclatures(): Promise<NomenclatureDTO<number>[]> {
        return this.service.getAllAquacultureNomenclatures().pipe(map((aquacultures: NomenclatureDTO<number>[]) => {
            for (const aqua of aquacultures) {
                aqua.description = aqua.description!.replace('{UROR}', this.translate.getValue('aquacultures.aquaculture-dropdown-uror'));
                aqua.description = aqua.description!.replace('{REGNUM}', this.translate.getValue('aquacultures.aquaculture-dropdown-reg-num'));
            }
            return aquacultures;
        })).toPromise();
    }

    private shouldHidePaymentData(): boolean {
        return (this.model as AquacultureChangeOfCircumstancesApplicationDTO)?.paymentInformation?.paymentType === null
            || (this.model as AquacultureChangeOfCircumstancesApplicationDTO)?.paymentInformation?.paymentType === undefined
            || (this.model as AquacultureChangeOfCircumstancesApplicationDTO)?.paymentInformation?.paymentType === '';
    }
}