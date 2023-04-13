import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { AfterViewInit, Component, EventEmitter, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Observable, Subject } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { AddressRegistrationDTO } from '@app/models/generated/dtos/AddressRegistrationDTO';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { QualifiedFisherApplicationEditDTO } from '@app/models/generated/dtos/QualifiedFisherApplicationEditDTO';
import { QualifiedFisherEditDTO } from '@app/models/generated/dtos/QualifiedFisherEditDTO';
import { QualifiedFisherRegixDataDTO } from '@app/models/generated/dtos/QualifiedFisherRegixDataDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { ApplicantRelationToRecipientDTO } from '@app/models/generated/dtos/ApplicantRelationToRecipientDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IQualifiedFishersService } from '@app/interfaces/administration-app/qualified-fishers.interface';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { MaritimeEducationFishermanDialogParamsModel } from './models/maritime-education-fisherman-dialog-params.model';
import { IdentifierTypeEnum } from '@app/enums/identifier-type.enum';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { HttpErrorResponse } from '@angular/common/http';
import { QualifiedFishersPublicService } from '@app/services/public-app/qualified-fishers-public.service';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { SubmittedByRolesEnum } from '@app/enums/submitted-by-roles.enum';
import { DuplicatesEntryDTO } from '@app/models/generated/dtos/DuplicatesEntryDTO';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';
import { QualifiedFisherStatusesEnum } from '@app/enums/qualified-fisher-statuses.enum';

@Component({
    selector: 'edit-fisher-component',
    templateUrl: './edit-fisher.component.html',
})
export class EditFisherComponent implements OnInit, AfterViewInit, IDialogComponent {
    public readonly pageCode: PageCodeEnum = PageCodeEnum.CommFishLicense;

    public editForm!: FormGroup;
    public expectedResults: QualifiedFisherRegixDataDTO;
    public service!: IQualifiedFishersService;
    public modeViewOnly: boolean = false;
    public modeReadOnly: boolean = false;
    public modeApplication: boolean = false;
    public modeApplicationRegixOnly: boolean = false;
    public showRegiXData: boolean = false;
    public isWithMaritimeEducation: boolean = false;
    public isThirdCountryFisherman: boolean = false;
    public isEditing: boolean = false;
    public isEditingSubmittedBy: boolean = false;
    public hasNoEDeliveryRegistrationError: boolean = false;
    public hasPersonAlreadyFisherError: boolean = false;
    public isOnlineApplication: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();;
    public isPublicApp: boolean = false;
    public loadRegisterFromApplication: boolean = false;
    public showSubmittedFor: boolean = false;
    public hideBasicPaymentInfo: boolean = false;
    public isPaid: boolean = false;
    public hasDelivery: boolean = false;

    public notifier: Notifier = new Notifier();
    public regixChecks: ApplicationRegiXCheckDTO[] = [];
    public duplicates: DuplicatesEntryDTO[] = [];

    public territoryUnits: NomenclatureDTO<number>[] = [];
    public statuses: NomenclatureDTO<QualifiedFisherStatusesEnum>[] = [];

    public readonly qualifiedFisherStatusesEnum: typeof QualifiedFisherStatusesEnum = QualifiedFisherStatusesEnum;

    private applicationsService: IApplicationsService | undefined;
    private id: number | undefined;
    private applicationId: number | undefined;
    private model!: QualifiedFisherEditDTO | QualifiedFisherApplicationEditDTO | QualifiedFisherRegixDataDTO;
    private modeApplicationHistory: boolean = false;
    private dialogRightSideActions: IActionInfo[] | undefined;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private readonly translationService: FuseTranslationLoaderService;
    private readonly nomenclatures: CommonNomenclatures;

    public constructor(
        translationService: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
    ) {
        this.translationService = translationService;
        this.nomenclatures = nomenclatures;

        this.isPublicApp = IS_PUBLIC_APP;

        this.expectedResults = new QualifiedFisherRegixDataDTO({
            submittedByRegixData: new RegixPersonDataDTO(),
            submittedByAddresses: new Array<AddressRegistrationDTO>(),
            submittedForRegixData: new RegixPersonDataDTO(),
            submittedForAddresses: new Array<AddressRegistrationDTO>()
        });

        this.statuses = [
            new NomenclatureDTO<QualifiedFisherStatusesEnum>({
                value: QualifiedFisherStatusesEnum.Registered,
                displayName: this.translationService.getValue('qualified-fishers-page.qualified-fisher-registered'),
                isActive: true
            }),
            new NomenclatureDTO<QualifiedFisherStatusesEnum>({
                value: QualifiedFisherStatusesEnum.NoPassedExam,
                displayName: this.translationService.getValue('qualified-fishers-page.qualified-fisher-has-no-passed-exam'),
                isActive: true
            })
        ];
    }

    public setData(data: DialogParamsModel | MaritimeEducationFishermanDialogParamsModel, buttons: DialogWrapperData): void {
        this.id = data.id;
        this.applicationsService = data.applicationsService;
        this.modeViewOnly = data.viewMode;
        this.service = data.service as IQualifiedFishersService;
        this.dialogRightSideActions = buttons.rightSideActions;

        if (data instanceof DialogParamsModel) {
            this.applicationId = data.applicationId;
            this.modeApplication = data.isApplication;
            this.loadRegisterFromApplication = data.loadRegisterFromApplication;
            this.modeReadOnly = data.isReadonly;
            this.modeApplicationRegixOnly = data.showOnlyRegiXData;
            this.showRegiXData = data.showRegiXData;
            this.modeApplicationHistory = data.isApplicationHistoryMode;
            this.isWithMaritimeEducation = false;
        }
        else {
            this.modeApplication = false;
            this.modeReadOnly = false;
            this.modeApplicationRegixOnly = false;
            this.modeApplicationHistory = false;
            this.isThirdCountryFisherman = data.isThirdCountryFisherman;
            this.isWithMaritimeEducation = true;
        }

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        this.territoryUnits = await NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false
        ).toPromise();

        if (this.modeApplicationHistory && this.applicationId !== undefined) { // извличане на исторически данни за заявление
            this.editForm.disable();

            if (this.applicationsService !== null && this.applicationsService !== undefined) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const application: QualifiedFisherApplicationEditDTO = new QualifiedFisherApplicationEditDTO(contentObject);
                        application.files = content.files;
                        application.applicationId = content.applicationId;
                        this.model = application;
                        this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                        this.isPaid = application.isPaid ?? false;
                        this.hasDelivery = application.hasDelivery ?? false;
                        this.isOnlineApplication = application.isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
            else {
                throw new Error('applicationsService cannot be null/undefined in applicationsHistoryMode');
            }
        }
        else if ((this.id === undefined || this.id === null) && this.applicationId !== undefined && this.applicationId !== null && !this.modeApplication) { // извличане на данни за създаване на регистров запис от заявление
            if (this.loadRegisterFromApplication === true) {  // извличане на данни за регистър по id на заявление
                if (this.modeReadOnly || this.modeViewOnly) {
                    this.editForm.disable();
                }
                this.isEditing = true;
                this.isEditingSubmittedBy = true;

                this.service.getRegisterByApplicationId(this.applicationId, this.pageCode).subscribe({
                    next: (result: unknown) => {
                        this.model = new QualifiedFisherEditDTO(result as QualifiedFisherEditDTO);
                        this.isOnlineApplication = (result as QualifiedFisherEditDTO).isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
            else {
                this.isEditing = false;
                this.isEditingSubmittedBy = false;
                this.setRegistrationDateValidators();
                this.service.getApplicationDataForRegister(this.applicationId!).subscribe({
                    next: (result: QualifiedFisherEditDTO) => {
                        this.model = result;
                        this.isOnlineApplication = result.isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
        }
        else {
            if (this.modeReadOnly || this.modeViewOnly) {
                this.editForm.disable();
            }

            if (this.modeApplication && this.applicationId !== undefined) { // в случай на заявление
                this.isEditing = false;
                this.isEditingSubmittedBy = false;

                if (this.modeApplicationRegixOnly) { // извличане на данни за RegiX сверка от служител
                    this.service.getRegixData(this.applicationId).subscribe({
                        next: (regixData: RegixChecksWrapperDTO<QualifiedFisherRegixDataDTO>) => {
                            this.model = new QualifiedFisherRegixDataDTO(regixData.dialogDataModel);
                            this.expectedResults = new QualifiedFisherRegixDataDTO(regixData.regiXDataModel);
                            this.expectedResults.applicationId = this.applicationId;
                            this.expectedResults.id = this.model.id;
                            // TODO instantiate inner properties of expected results !!!

                            this.fillForm();
                        }
                    });
                }
                else { // извличане на данни за заявление
                    this.isEditing = false;
                    this.isEditingSubmittedBy = false;

                    this.service.getApplication(this.applicationId, this.showRegiXData).subscribe({
                        next: (application: QualifiedFisherApplicationEditDTO | null | undefined) => {
                            if (application === null || application === undefined) {
                                application = new QualifiedFisherApplicationEditDTO({ applicationId: this.applicationId });
                            }
                            else {
                                application.applicationId = this.applicationId;
                            }

                            this.model = application;
                            this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new QualifiedFisherRegixDataDTO(application.regiXDataModel);
                                application.regiXDataModel = undefined;
                            }

                            if (this.model instanceof QualifiedFisherApplicationEditDTO) {
                                this.isOnlineApplication = this.model.isOnlineApplication!;
                                this.isPaid = application.isPaid ?? false;
                                this.hasDelivery = application.hasDelivery ?? false;

                                if (this.isPublicApp && this.isOnlineApplication) {
                                    this.isEditingSubmittedBy = true;
                                    if ((this.model.submittedByRegixData === undefined || this.model.submittedByRegixData === null)
                                        && (this.model.submittedByAddresses === undefined || this.model.submittedByAddresses === null)
                                    ) {
                                        const service = this.service as QualifiedFishersPublicService;
                                        service.getCurrentUserAsSubmittedBy().subscribe({
                                            next: (submittedBy: ApplicationSubmittedByDTO) => {
                                                if (this.model instanceof QualifiedFisherApplicationEditDTO) {
                                                    this.model.submittedByRegixData = submittedBy.person;
                                                    this.model.submittedByAddresses = submittedBy.addresses;
                                                }

                                                this.fillForm();
                                            }
                                        });
                                    }
                                    else {
                                        this.fillForm();
                                    }
                                }
                                else {
                                    this.fillForm();
                                }
                            }
                        }
                    });
                }
            }
            else if (this.id !== undefined && this.id !== null) { // извличане на данни за регистров запис
                this.isEditing = true;
                this.isEditingSubmittedBy = true;
                this.setRegistrationDateValidators();
                this.service.get(this.id!).subscribe({
                    next: (fisher: QualifiedFisherEditDTO) => {
                        this.model = fisher;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
            else if (this.isWithMaritimeEducation) {
                this.model = new QualifiedFisherEditDTO({
                    isWithMaritimeEducation: true,
                    hasExam: false
                });
                this.setRegistrationDateValidators();
                this.setDiplomaControlsValidators();
            }
        }
    }

    public ngAfterViewInit(): void {
        if (!this.isWithMaritimeEducation && !this.modeApplication) {
            this.editForm.get('hasPassedExamControl')!.valueChanges.subscribe({
                next: () => {
                    this.setPassedExamControlsValidators();
                    this.changeQualifiedFisherStatus();
                }
            });
        }

        if (this.modeApplication && !this.modeApplicationRegixOnly) {
            this.editForm.get('deliveryDataControl')!.valueChanges.subscribe({
                next: () => {
                    this.hasNoEDeliveryRegistrationError = false;
                }
            });
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.editForm.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.editForm.valid) {
            if (actionInfo.id === 'print') {
                this.saveAndPrintQualifiedFisherRecord(dialogClose);
            }
            else {
                this.saveQualifiedFisherRecord(dialogClose);
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        let applicationAction: boolean = false;

        if (this.model instanceof QualifiedFisherApplicationEditDTO || this.model instanceof QualifiedFisherRegixDataDTO) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);
            applicationAction = ApplicationUtils.applicationDialogButtonClicked(new ApplicationDialogData({
                action: actionInfo,
                dialogClose: dialogClose,
                applicationId: this.applicationId!,
                model: this.model,
                readOnly: this.modeReadOnly,
                viewMode: this.modeViewOnly,
                editForm: this.editForm,
                saveFn: this.saveQualifiedFisherRecord.bind(this),
                onMarkAsTouched: () => {
                    this.validityCheckerGroup.validate();
                }
            }));
        }

        if (!this.modeReadOnly && !this.modeViewOnly && !applicationAction) { // in the case where RecordType == 'Register'
            this.editForm.markAllAsTouched();
            this.validityCheckerGroup.validate();

            if (this.editForm.valid) {
                switch (actionInfo.id) {
                    case 'save':
                    case 'print':
                        return this.saveBtnClicked(actionInfo, dialogClose);
                }
            }
        }
        else if (actionInfo.id === 'print' && (this.modeReadOnly || this.modeViewOnly) && !applicationAction) {
            this.service.downloadRegister(this.model.id!).subscribe();
        }
    }

    public fileTypeFilterFn(options: PermittedFileTypeDTO[]): PermittedFileTypeDTO[] {
        const pdfs: FileTypeEnum[] = [FileTypeEnum.SIGNEDAPPL, FileTypeEnum.APPLICATION_PDF];
        const offlines: FileTypeEnum[] = [FileTypeEnum.PAYEDFEE, FileTypeEnum.SCANNED_FORM];

        let result: PermittedFileTypeDTO[] = options;

        if (this.modeApplication || !this.isOnlineApplication) {
            result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        if (this.isOnlineApplication) {
            result = result.filter(x => !offlines.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        return result;
    }

    public downloadedPersonData(person: PersonFullDataDTO): void {
        this.editForm.get('submittedByRegixDataControl')!.setValue(person.person);
        this.editForm.get('submittedByAddressDataControl')!.setValue(person.addresses);
    }

    public downloadedSubmittedForPersonData(person: PersonFullDataDTO): void {
        this.editForm.get('submittedForRegixDataControl')!.setValue(person.person);
        this.editForm.get('submittedForAddressDataControl')!.setValue(person.addresses);
    }

    private saveQualifiedFisherRecord(dialogClose: DialogCloseCallback, fromSaveAsDraft: boolean = false): Observable<boolean> {
        const saveOrEditDone: EventEmitter<boolean> = new EventEmitter<boolean>();

        this.saveOrEdit(fromSaveAsDraft).subscribe({
            next: (id: number | void) => {
                this.hasNoEDeliveryRegistrationError = false;

                if (typeof id === 'number' && id !== undefined) {
                    this.model.id = id;
                    dialogClose(this.model);
                }
                else {
                    dialogClose(this.model);
                }

                saveOrEditDone.emit(true);
                saveOrEditDone.complete();
            },
            error: (errorResponse: HttpErrorResponse) => {
                this.handleErrorResponse(errorResponse);
            }
        });

        return saveOrEditDone.asObservable();
    }

    private saveAndPrintQualifiedFisherRecord(dialogClose: DialogCloseCallback): void {
        this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model);
        let saveOrEditObservable: Observable<boolean>;

        if (this.id !== null && this.id !== undefined) {
            saveOrEditObservable = this.service.editAndDownloadRegister(this.model);
        }
        else {
            saveOrEditObservable = this.service.addAndDownloadRegister(this.model);
        }

        saveOrEditObservable.subscribe({
            next: (downloaded: boolean) => {
                if (downloaded === true) {
                    dialogClose(this.model);
                }
            }
        });
    }

    private saveOrEdit(fromSaveAsDraft: boolean): Observable<number | void> {
        this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model)

        let saveOrEditObservable: Observable<void | number>;

        if (this.model instanceof QualifiedFisherEditDTO) {
            if (this.id !== undefined && this.id !== null) {
                saveOrEditObservable = this.service.edit(this.model);
            }
            else {
                saveOrEditObservable = this.service.add(this.model);
            }
        }
        else {
            if (this.model.id !== undefined && this.model.id !== null) {
                saveOrEditObservable = this.service.editApplication(this.model, this.pageCode, fromSaveAsDraft);
            }
            else {
                saveOrEditObservable = this.service.addApplication(this.model);
            }
        }

        return saveOrEditObservable;
    }

    private buildForm(): void {
        this.editForm = new FormGroup({
            submittedForAddressDataControl: new FormControl(undefined),
            submittedForRegixDataControl: new FormControl(undefined)
        });

        if (this.modeApplication === false) {
            this.editForm.addControl('registrationNumberControl', new FormControl({ value: null, disabled: true }));
            this.editForm.addControl('registrationDateControl', new FormControl());
            this.editForm.addControl('statusControl', new FormControl());
            this.editForm.addControl('examDateControl', new FormControl());
            this.editForm.addControl('protocolNumberControl', new FormControl(undefined, [Validators.maxLength(50)]));
            this.editForm.addControl('hasPassedExamControl', new FormControl(false));
            this.editForm.addControl('diplomaNumberControl', new FormControl(undefined, [Validators.maxLength(50)]));
            this.editForm.addControl('diplomaDateControl', new FormControl());
            this.editForm.addControl('diplomaIssuerControl', new FormControl(undefined, Validators.maxLength(200)));

            if (this.isWithMaritimeEducation === true && (this.id === null || this.id === undefined)) { // При добавяне на правоспособен рибар с диплома
                this.editForm.setValidators([this.personNotAlreadyFisher()]);

                this.editForm.get('submittedForRegixDataControl')!.valueChanges.subscribe({
                    next: () => {
                        this.hasPersonAlreadyFisherError = false;
                        this.editForm.updateValueAndValidity({ emitEvent: false });
                    }
                });
            }
        }
        else {
            this.editForm.addControl('submittedByRegixDataControl', new FormControl());
            this.editForm.addControl('submittedByAddressDataControl', new FormControl());

            if (!this.modeApplicationRegixOnly) {
                this.editForm.setValidators([this.personNotAlreadyFisher()]);

                this.editForm.get('submittedByRegixDataControl')!.valueChanges.subscribe({
                    next: () => {
                        this.hasPersonAlreadyFisherError = false;
                        this.editForm.updateValueAndValidity({ emitEvent: false });
                    }
                });

                this.editForm.get('submittedForRegixDataControl')!.valueChanges.subscribe({
                    next: () => {
                        this.hasPersonAlreadyFisherError = false;
                        this.editForm.updateValueAndValidity({ emitEvent: false });
                    }
                });

                this.editForm.addControl('applicantRelationToRecipientControl', new FormControl());

                this.editForm.get('applicantRelationToRecipientControl')!.valueChanges.subscribe({
                    next: (value: ApplicantRelationToRecipientDTO | undefined) => {
                        if (value?.role === SubmittedByRolesEnum.Personal) {
                            this.showSubmittedFor = false;
                            this.editForm.get('submittedForAddressDataControl')!.setErrors(null);
                            this.editForm.get('submittedForRegixDataControl')!.setErrors(null);
                        } else {
                            this.showSubmittedFor = true;
                        }
                    }
                });

                this.editForm.addControl('deliveryDataControl', new FormControl());
                this.editForm.addControl('applicationPaymentInformationControl', new FormControl());
            }
        }

        if (!this.modeApplicationRegixOnly) {
            this.editForm.addControl('filesControl', new FormControl());
            this.editForm.addControl('commentsControl', new FormControl(undefined, Validators.maxLength(1000)));
            this.editForm.addControl('examTerritoryUnitControl', new FormControl());
        }
    }

    private fillForm(): void {
        if (this.model instanceof QualifiedFisherApplicationEditDTO || this.model instanceof QualifiedFisherRegixDataDTO) {
            this.editForm.controls.submittedByRegixDataControl.setValue(this.model.submittedByRegixData);
            this.editForm.controls.submittedByAddressDataControl.setValue(this.model.submittedByAddresses);

            if (this.model.submittedByRole !== SubmittedByRolesEnum.Personal) {
                this.showSubmittedFor = true;
            }
            else {
                this.showSubmittedFor = false;
            }
        }

        if (this.model instanceof QualifiedFisherApplicationEditDTO) {
            this.editForm.controls.applicantRelationToRecipientControl.setValue(new ApplicantRelationToRecipientDTO({
                role: this.model.submittedByRole,
                letterOfAttorney: this.model.letterOfAttorney
            }));

            if (this.hasDelivery) {
                this.editForm.get('deliveryDataControl')!.setValue(this.model.deliveryData);
            }

            if (this.isPaid === true) {
                this.editForm.get('applicationPaymentInformationControl')!.setValue(this.model.paymentInformation);
            }
        }

        if (this.model instanceof QualifiedFisherEditDTO) {
            this.editForm.controls.registrationNumberControl.setValue(this.model.registrationNum);
            this.editForm.controls.registrationDateControl.setValue(this.model.registrationDate);

            const status: NomenclatureDTO<QualifiedFisherStatusesEnum> = this.statuses.find(x => x.value === (this.model as QualifiedFisherEditDTO).status)!;
            this.editForm.controls.statusControl.setValue(status);

            this.duplicates = this.model.duplicateEntries ?? [];
        }

        if (this.model instanceof QualifiedFisherEditDTO || this.model instanceof QualifiedFisherApplicationEditDTO) {
            if (this.isWithMaritimeEducation === true && this.model instanceof QualifiedFisherEditDTO) {
                this.editForm.controls.diplomaNumberControl.setValue(this.model.diplomaNumber);
                this.editForm.controls.diplomaDateControl.setValue(this.model.diplomaDate);
                this.editForm.controls.diplomaIssuerControl.setValue(this.model.diplomaIssuer);

                this.setDiplomaControlsValidators();
            }
            else {
                const examTerritoryUnitId: number | undefined = this.model.examTerritoryUnitId;
                this.editForm.controls.examTerritoryUnitControl.setValue(this.territoryUnits.find(x => x.value === examTerritoryUnitId));

                if (this.model instanceof QualifiedFisherEditDTO) {
                    this.editForm.controls.hasPassedExamControl.setValue(this.model.hasPassedExam ?? false);
                    this.setPassedExamControlsValidators();
                    this.editForm.controls.protocolNumberControl.setValue(this.model.examProtocolNumber);
                    this.editForm.controls.examDateControl.setValue(this.model.examDate);
                }

                this.setExamControlsValidators();
            }

            this.editForm.controls.commentsControl.setValue(this.model.comments);
            this.editForm.controls.filesControl.setValue(this.model.files);
        }

        this.isThirdCountryFisherman = this.model.submittedForRegixData?.egnLnc?.identifierType === IdentifierTypeEnum.FORID ?? false;
        this.editForm.controls.submittedForRegixDataControl.setValue(this.model.submittedForRegixData);
        this.editForm.controls.submittedForAddressDataControl.setValue(this.model.submittedForAddresses);

        if (this.model instanceof QualifiedFisherRegixDataDTO && this.modeApplicationRegixOnly) {
            this.fillFormRegiX();
        }
        else {
            if (this.showRegiXData) {
                this.fillFormRegiX();
            }
        }
    }

    private fillFormRegiX(): void {
        if ((this.model as QualifiedFisherApplicationEditDTO).applicationRegiXChecks !== undefined
            && (this.model as QualifiedFisherApplicationEditDTO).applicationRegiXChecks !== null
        ) {
            const applicationRegiXChecks: ApplicationRegiXCheckDTO[] = (this.model as QualifiedFisherApplicationEditDTO).applicationRegiXChecks!;

            setTimeout(() => {
                this.regixChecks = applicationRegiXChecks;
            });
        }

        if (!this.modeViewOnly) {
            this.notifier.start();
            this.notifier.onNotify.subscribe({
                next: () => {
                    this.editForm.markAllAsTouched();

                    if (this.modeApplicationRegixOnly) {
                        ApplicationUtils.enableOrDisableRegixCheckButtons(this.editForm, this.dialogRightSideActions);
                    }

                    this.notifier.stop();
                }
            });
        }
    }

    private fillModel(): void {
        if (this.model instanceof QualifiedFisherApplicationEditDTO || this.model instanceof QualifiedFisherRegixDataDTO) {
            this.model.submittedByRegixData = this.editForm.controls.submittedByRegixDataControl.value;
            this.model.submittedByAddresses = this.editForm.controls.submittedByAddressDataControl.value;


            if (this.model.submittedByRole === SubmittedByRolesEnum.Personal) {
                this.model.submittedForRegixData = undefined;
                this.model.submittedForAddresses = undefined;
            } else {
                this.model.submittedForRegixData = this.editForm.controls.submittedForRegixDataControl.value;
                this.model.submittedForAddresses = this.editForm.controls.submittedForAddressDataControl.value;
            }
        }
        else {
            this.model.submittedForRegixData = this.editForm.controls.submittedForRegixDataControl.value;
            this.model.submittedForAddresses = this.editForm.controls.submittedForAddressDataControl.value;
        }

        if (this.model instanceof QualifiedFisherApplicationEditDTO) {
            const relation: ApplicantRelationToRecipientDTO = this.editForm.controls.applicantRelationToRecipientControl.value;
            this.model.submittedByRole = relation.role;
            this.model.letterOfAttorney = relation.letterOfAttorney;

            if (this.hasDelivery) {
                this.model.deliveryData = this.editForm.get('deliveryDataControl')!.value;
            }

            if (this.isPaid === true) {
                this.model.paymentInformation = this.editForm.get('applicationPaymentInformationControl')!.value;
            }
        }

        if (this.model instanceof QualifiedFisherEditDTO) {
            this.model.registrationDate = this.editForm.controls.registrationDateControl.value;
        }

        if (this.model instanceof QualifiedFisherEditDTO || this.model instanceof QualifiedFisherApplicationEditDTO) {
            if (this.isWithMaritimeEducation === true && this.model instanceof QualifiedFisherEditDTO) {
                this.model.diplomaNumber = this.editForm.controls.diplomaNumberControl.value;
                this.model.diplomaDate = this.editForm.controls.diplomaDateControl.value;
                this.model.diplomaIssuer = this.editForm.controls.diplomaIssuerControl.value;
            }
            else {
                this.model.examTerritoryUnitId = this.editForm.controls.examTerritoryUnitControl.value?.value;

                if (this.model instanceof QualifiedFisherEditDTO) {
                    this.model.hasPassedExam = this.editForm.controls.hasPassedExamControl.value;
                    this.model.examProtocolNumber = this.editForm.controls.protocolNumberControl.value;
                    this.model.examDate = this.editForm.controls.examDateControl.value;
                }
            }

            if (this.model.submittedForRegixData !== null && this.model.submittedForRegixData !== undefined) {
                this.model.name = `${this.model.submittedForRegixData!.firstName} ${this.model.submittedForRegixData!.middleName} ${this.model.submittedForRegixData!.lastName}`;
                this.model.egn = this.model.submittedForRegixData!.egnLnc?.egnLnc;
            }

            this.model.comments = this.editForm.controls.commentsControl.value;
            this.model.files = this.editForm.controls.filesControl.value;
        }
    }

    private setDiplomaControlsValidators(): void {
        this.editForm.get('diplomaNumberControl')!.setValidators([Validators.required, Validators.maxLength(50)]);
        this.editForm.get('diplomaDateControl')!.setValidators([Validators.required]);
        this.editForm.get('diplomaIssuerControl')!.setValidators([Validators.required, Validators.maxLength(200)]);

        this.editForm.get('diplomaNumberControl')!.markAsPending();
        this.editForm.get('diplomaDateControl')!.markAsPending();
        this.editForm.get('diplomaIssuerControl')!.markAsPending();

        if (this.modeReadOnly || this.modeViewOnly) {
            this.editForm.get('diplomaNumberControl')!.disable({ emitEvent: false });
            this.editForm.get('diplomaDateControl')!.disable({ emitEvent: false });
            this.editForm.get('diplomaIssuerControl')!.disable({ emitEvent: false });
        }
    }

    private setExamControlsValidators(): void {
        this.editForm.get('examTerritoryUnitControl')!.setValidators([Validators.required]);

        this.editForm.get('examTerritoryUnitControl')!.markAsPending();

        if (this.modeReadOnly || this.modeViewOnly) {
            this.editForm.get('examTerritoryUnitControl')!.disable({ emitEvent: false });
        }

        if (this.model instanceof QualifiedFisherEditDTO) {
            this.editForm.get('hasPassedExamControl')!.setValidators(Validators.required); // TODO test why this validation does not work ???
        }
    }

    private setPassedExamControlsValidators(): void {
        if (this.editForm.get('hasPassedExamControl')!.value === true) {
            this.editForm.get('protocolNumberControl')!.setValidators([Validators.required, Validators.maxLength(50)]);
            this.editForm.get('examDateControl')!.setValidators([Validators.required]);

            this.editForm.get('protocolNumberControl')!.markAsPending();
            this.editForm.get('examDateControl')!.markAsPending();
        }
        else {
            this.editForm.get('protocolNumberControl')!.setValidators([Validators.maxLength(50)]);
            this.editForm.get('examDateControl')!.clearValidators();

            this.editForm.get('protocolNumberControl')!.markAsPending();
            this.editForm.get('examDateControl')!.markAsPending();
        }

        if (this.modeReadOnly || this.modeViewOnly) {
            this.editForm.get('protocolNumberControl')!.disable({ emitEvent: false });
            this.editForm.get('examDateControl')!.disable({ emitEvent: false });
        }
    }

    private setRegistrationDateValidators(): void {
        this.editForm.get('registrationDateControl')!.setValidators(Validators.required);
        this.editForm.get('registrationDateControl')!.markAsPending();

        if (this.modeReadOnly || this.modeViewOnly) {
            this.editForm.get('registrationDateControl')!.disable({ emitEvent: false });
        }
    }

    private changeQualifiedFisherStatus(): void {
        const hasPassedExam: boolean = this.editForm.get('hasPassedExamControl')!.value ?? false;

        let status: NomenclatureDTO<QualifiedFisherStatusesEnum>;

        if (hasPassedExam) {
            status = this.statuses.find(x => x.value === QualifiedFisherStatusesEnum.Registered)!;
        }
        else {
            status = this.statuses.find(x => x.value === QualifiedFisherStatusesEnum.NoPassedExam)!;
        }

        this.editForm.get('statusControl')!.setValue(status);
    }

    private handleErrorResponse(errorResponse: HttpErrorResponse): void {
        const error = errorResponse.error as ErrorModel;
        if (error?.code === ErrorCode.QualifiedFisherAlreadyExists
            && (this.model instanceof QualifiedFisherEditDTO || this.model instanceof QualifiedFisherApplicationEditDTO)
        ) {
            this.hasPersonAlreadyFisherError = true;
            this.editForm.updateValueAndValidity({ emitEvent: false });
            this.validityCheckerGroup.validate();
        }
        else if (error?.code === ErrorCode.NoEDeliveryRegistration && this.model instanceof QualifiedFisherApplicationEditDTO) {
            this.hasNoEDeliveryRegistrationError = true;
            this.editForm.markAllAsTouched();
            this.validityCheckerGroup.validate();
        }
    }

    private shouldHidePaymentData(): boolean {
        return (this.model as QualifiedFisherApplicationEditDTO)?.paymentInformation?.paymentType === null
            || (this.model as QualifiedFisherApplicationEditDTO)?.paymentInformation?.paymentType === undefined
            || (this.model as QualifiedFisherApplicationEditDTO)?.paymentInformation?.paymentType === '';
    }

    private personNotAlreadyFisher(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.hasPersonAlreadyFisherError) {
                return { 'personIsAlreadyQualifiedFisher': true };
            }

            return null;
        }
    }
}