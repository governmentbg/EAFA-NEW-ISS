import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';
import { combineLatest, Observable, Subject } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { IFishingCapacityService } from '@app/interfaces/common-app/fishing-capacity.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { ApplicationSubmittedByRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedForRegixDataDTO';
import { ReduceFishingCapacityApplicationDTO } from '@app/models/generated/dtos/ReduceFishingCapacityApplicationDTO';
import { ReduceFishingCapacityRegixDataDTO } from '@app/models/generated/dtos/ReduceFishingCapacityRegixDataDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { FishingCapacityFreedActionsRegixDataDTO } from '@app/models/generated/dtos/FishingCapacityFreedActionsRegixDataDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { FishingCapacityPublicService } from '@app/services/public-app/fishing-capacity-public.service';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { NewCertificateData } from '../acquired-fishing-capacity/acquired-fishing-capacity.component';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { ErrorSnackbarComponent } from '@app/shared/components/error-snackbar/error-snackbar.component';
import { ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { ApplicationValidationErrorsEnum } from '@app/enums/application-validation-errors.enum';
import { ApplicationPaymentInformationDTO } from '@app/models/generated/dtos/ApplicationPaymentInformationDTO';
import { FishingCapacityFreedActionsDTO } from '@app/models/generated/dtos/FishingCapacityFreedActionsDTO';
import { FishingCapacityRemainderActionEnum } from '@app/enums/fishing-capacity-remainder-action.enum';
import { ShipNomenclatureFilters, ShipsUtils } from '@app/shared/utils/ships.utils';

@Component({
    selector: 'reduce-fishing-capacity',
    templateUrl: './reduce-fishing-capacity.component.html'
})
export class ReduceFishingCapacityComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    public readonly pageCode: PageCodeEnum = PageCodeEnum.ReduceFishCap;

    public ships: ShipNomenclatureDTO[] = [];

    public notifier: Notifier = new Notifier();
    public expectedResults: ReduceFishingCapacityRegixDataDTO;
    public regixChecks: ApplicationRegiXCheckDTO[] = [];

    public paymentInformation: ApplicationPaymentInformationDTO | undefined;

    public isPublicApp: boolean = false;
    public isOnlineApplication: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();
    public isEditing: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public isReadonly: boolean = false;
    public viewMode: boolean = false;
    public isDraft: boolean = false;
    public isPaid: boolean = false;
    public hasDelivery: boolean = false;
    public hasNoEDeliveryRegistrationError: boolean = false;
    public willIssueCapacityCertificates: boolean = false;
    public service!: IFishingCapacityService;

    public maxGrossTonnage: number = 0;
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

    private model!: ReduceFishingCapacityApplicationDTO | ReduceFishingCapacityRegixDataDTO;

    public constructor(
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        snackbar: MatSnackBar
    ) {
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.snackbar = snackbar;

        this.isPublicApp = IS_PUBLIC_APP;

        this.expectedResults = new ReduceFishingCapacityRegixDataDTO({
            submittedBy: new ApplicationSubmittedByRegixDataDTO(),
            submittedFor: new ApplicationSubmittedForRegixDataDTO(),
            freedCapacityAction: new FishingCapacityFreedActionsRegixDataDTO()
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
                        const application: ReduceFishingCapacityApplicationDTO = new ReduceFishingCapacityApplicationDTO(contentObject);
                        application.files = content.files;
                        application.applicationId = content.applicationId;

                        this.isPaid = application.isPaid!;
                        this.hasDelivery = application.hasDelivery!;
                        this.paymentInformation = application.paymentInformation;
                        this.isOnlineApplication = application.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.model = application;
                        this.isDraft = application.isDraft ?? false;
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
                        next: (regixData: RegixChecksWrapperDTO<ReduceFishingCapacityRegixDataDTO>) => {
                            this.model = new ReduceFishingCapacityRegixDataDTO(regixData.dialogDataModel);
                            this.expectedResults = new ReduceFishingCapacityRegixDataDTO(regixData.regiXDataModel);

                            this.fillForm();
                        }
                    });
                }
                else {
                    // извличане на данни за заявление
                    this.isEditing = false;

                    this.service.getApplication(this.applicationId, this.pageCode).subscribe({
                        next: (application: ReduceFishingCapacityApplicationDTO) => {
                            application.applicationId = this.applicationId;
                            application.isDraft = application.isDraft ?? true;

                            this.isOnlineApplication = application.isOnlineApplication!;
                            this.isDraft = application.isDraft;
                            this.isPaid = application.isPaid!;
                            this.hasDelivery = application.hasDelivery!;
                            this.paymentInformation = application.paymentInformation;
                            this.refreshFileTypes.next();

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
            reduceCapacityTonnageControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 2)]),
            reduceCapacityPowerControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 2)]),
            actionsControl: new FormControl(null),
            deliveryDataControl: new FormControl(null),
            filesControl: new FormControl(null)
        });

        this.form.get('shipControl')!.valueChanges.subscribe({
            next: (ship: ShipNomenclatureDTO | string | undefined) => {
                if (ship && typeof ship !== 'string') {
                    this.form.get('reduceCapacityTonnageControl')!.setValidators([Validators.required, TLValidators.number(0, ship.grossTonnage, 2)]);
                    this.form.get('reduceCapacityPowerControl')!.setValidators([Validators.required, TLValidators.number(0, ship.mainEnginePower, 2)]);
                }
                else {
                    this.form.get('reduceCapacityTonnageControl')!.setValidators([Validators.required, TLValidators.number(0, undefined, 2)]);
                    this.form.get('reduceCapacityPowerControl')!.setValidators([Validators.required, TLValidators.number(0, undefined, 2)]);
                }

                this.form.get('reduceCapacityTonnageControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('reduceCapacityPowerControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });

        combineLatest(
            this.form.get('reduceCapacityTonnageControl')!.valueChanges,
            this.form.get('reduceCapacityPowerControl')!.valueChanges
        ).subscribe({
            next: ([tonnage, power]: [number | undefined, number | undefined]) => {
                if (tonnage !== undefined && tonnage !== null && power !== undefined && power !== null) {
                    this.maxGrossTonnage = Number(tonnage);
                    this.maxPower = Number(power);

                    this.newCertificateData = new NewCertificateData({
                        tonnage: Number(tonnage),
                        power: Number(power),
                        validThreeYears: true
                    });
                }
                else {
                    this.maxGrossTonnage = 0;
                    this.maxPower = 0;
                    this.newCertificateData = undefined;
                }
            }
        });

        this.form.get('actionsControl')!.valueChanges.subscribe({
            next: (actions: FishingCapacityFreedActionsDTO) => {
                if (actions) {
                    this.willIssueCapacityCertificates = actions.action !== FishingCapacityRemainderActionEnum.NoCertificate;
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
        this.form.get('actionsControl')!.setValue(this.model.freedCapacityAction);

        if (this.model instanceof ReduceFishingCapacityRegixDataDTO) {
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
                        ApplicationUtils.enableOrDisableRegixCheckButtons(this.form, this.dialogRightSideActions);
                        this.notifier.stop();
                    }
                });
            }
        }
        else {
            if (this.model.shipId !== undefined && this.model.shipId !== null) {
                this.form.get('shipControl')!.setValue(ShipsUtils.get(this.ships, this.model.shipId));
            }

            this.form.get('reduceCapacityTonnageControl')!.setValue(this.model.reduceGrossTonnageBy?.toFixed(2));
            this.form.get('reduceCapacityPowerControl')!.setValue(this.model.reducePowerBy?.toFixed(2));
            this.form.get('filesControl')!.setValue(this.model.files);

            if (this.hasDelivery === true) {
                this.form.get('deliveryDataControl')!.setValue(this.model.deliveryData);
            }
        }
    }

    private fillModel(): void {
        this.model.submittedBy = this.form.get('submittedByControl')!.value;
        this.model.submittedFor = this.form.get('submittedForControl')!.value;
        this.model.freedCapacityAction = this.form.get('actionsControl')!.value;

        if (this.model instanceof ReduceFishingCapacityApplicationDTO) {
            this.model.shipId = this.form.get('shipControl')!.value?.value;
            this.model.reduceGrossTonnageBy = this.form.get('reduceCapacityTonnageControl')!.value;
            this.model.reducePowerBy = this.form.get('reduceCapacityPowerControl')!.value;
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

        if (this.model instanceof ReduceFishingCapacityApplicationDTO && !this.isDraft) {
            return this.service.editApplication(this.model, this.pageCode, saveAsDraft);
        }
        return this.service.addApplication(this.model, this.pageCode);
    }
}