import { HttpErrorResponse } from '@angular/common/http';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { PenalDecreeTypeEnum } from '@app/enums/penal-decree-type.enum';
import { IPenalDecreesService } from '@app/interfaces/administration-app/penal-decrees.interface';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { AuanViolatedRegulationDTO } from '@app/models/generated/dtos/AuanViolatedRegulationDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PenalDecreeAuanDataDTO } from '@app/models/generated/dtos/PenalDecreeAuanDataDTO';
import { PenalDecreeEditDTO } from '@app/models/generated/dtos/PenalDecreeEditDTO';
import { PenalDecreesService } from '@app/services/administration-app/penal-decrees.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { forkJoin, Observable } from 'rxjs';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { EditPenalDecreeDialogParams } from '../models/edit-penal-decree-params.model';
import { InspectorUserNomenclatureDTO } from '@app/models/generated/dtos/InspectorUserNomenclatureDTO';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { AuanStatusEnum } from '@app/enums/auan-status.enum';

@Component({
    selector: 'edit-decree-warning',
    templateUrl: './edit-decree-warning.component.html'
})
export class EditDecreeWarningComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;

    public readonly service!: IPenalDecreesService;
    public readonly pageCode: PageCodeEnum = PageCodeEnum.PenalDecrees;
    public readonly decreeType: PenalDecreeTypeEnum = PenalDecreeTypeEnum.Warning;
    public readonly today: Date = new Date();

    public isAdding: boolean = false;
    public viewMode: boolean = false;
    public isThirdParty: boolean = false;
    public violatedRegulationsTouched: boolean = false;
    public drafter: InspectorUserNomenclatureDTO | undefined;

    public decreeNumErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.decreeNumErrorLabelText.bind(this);

    public territoryUnits: NomenclatureDTO<number>[] = [];
    public courts: NomenclatureDTO<number>[] = [];
    public users: InspectorUserNomenclatureDTO[] = [];
    public violatedRegulations: AuanViolatedRegulationDTO[] = [];

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private typeId!: number;
    private auanId: number | undefined;
    private penalDecreeId!: number | undefined;
    private model!: PenalDecreeEditDTO;

    private readonly nomenclatures: CommonNomenclatures;
    private readonly translate: FuseTranslationLoaderService;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly snackbar: TLSnackbar;

    public constructor(
        service: PenalDecreesService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        snackbar: TLSnackbar
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.snackbar = snackbar;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        this.isAdding = this.penalDecreeId === undefined || this.penalDecreeId === null;

        const nomenclatures: (NomenclatureDTO<number> | InspectorUserNomenclatureDTO)[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures)),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Courts, this.service.getCourts.bind(this.service), false),
            this.service.getInspectorUsernames()
        ).toPromise();

        this.territoryUnits = nomenclatures[0];
        this.courts = nomenclatures[1];
        this.users = nomenclatures[2];

        if (this.auanId !== undefined && this.auanId !== null) {
            this.service.getPenalDecreeAuanData(this.auanId).subscribe({
                next: (data: PenalDecreeAuanDataDTO) => {
                    this.fillAuanData(data);
                    this.isThirdParty = data.isExternal ?? false;

                    if (this.penalDecreeId === undefined || this.penalDecreeId === null) {
                        this.model = new PenalDecreeEditDTO();
                        this.model.penalDecreeStatus = AuanStatusEnum.Draft;
                    }
                    else {
                        this.service.getPenalDecree(this.penalDecreeId).subscribe({
                            next: (decree: PenalDecreeEditDTO) => {
                                this.model = decree;
                                this.fillForm();
                            }
                        });
                    }
                }
            });
        }
        else {
            this.isThirdParty = true;
            this.model = new PenalDecreeEditDTO();
        }
    }

    public ngAfterViewInit(): void {
        this.form.get('drafterControl')!.valueChanges.subscribe({
            next: (drafter: InspectorUserNomenclatureDTO | undefined) => {
                this.drafter = drafter;

                if (drafter !== undefined && drafter !== null) {
                    this.form.get('issuerPositionControl')!.setValue(drafter.issuerPosition);
                }
                else {
                    this.form.get('issuerPositionControl')!.setValue(undefined);
                }
            }
        });

        this.form.get('auanViolatedRegulationsControl')!.valueChanges.subscribe({
            next: (result: AuanViolatedRegulationDTO[] | undefined) => {
                if (result !== undefined && result !== null) {
                    this.violatedRegulations = result;
                    this.violatedRegulationsTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                }
            }
        });
    }

    public setData(data: EditPenalDecreeDialogParams | undefined, wrapperData: DialogWrapperData): void {
        if (data !== undefined && data !== null) {
            this.auanId = data.auanId;
            this.typeId = data.typeId;
            this.penalDecreeId = data.id;
            this.viewMode = data.isReadonly ?? false;
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.viewMode) {
            dialogClose();
        }

        this.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.fillModel();
            this.model.penalDecreeStatus = AuanStatusEnum.Submitted;
            CommonUtils.sanitizeModelStrings(this.model);

            this.openConfirmDialog().subscribe({
                next: (ok: boolean) => {
                    if (ok) {
                        this.saveDecree(dialogClose);
                    }
                }
            });
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (action.id === 'print') {
            if (this.viewMode) {
                this.service.downloadPenalDecree(this.penalDecreeId!).subscribe({
                    next: () => {
                        //nothing to do
                    }
                });
            }
            else {
                this.markAllAsTouched();
                this.validityCheckerGroup.validate();

                if (this.form.valid) {
                    this.fillModel();
                    this.model.penalDecreeStatus = AuanStatusEnum.Submitted;
                    CommonUtils.sanitizeModelStrings(this.model);

                    this.openConfirmDialog().subscribe({
                        next: (ok: boolean) => {
                            if (ok) {
                                if (this.penalDecreeId !== undefined && this.penalDecreeId !== null) {
                                    this.service.editPenalDecree(this.model).subscribe({
                                        next: () => {
                                            this.service.downloadPenalDecree(this.penalDecreeId!).subscribe({
                                                next: () => {
                                                    dialogClose(this.model);
                                                }
                                            });
                                        },
                                        error: (response: HttpErrorResponse) => {
                                            this.handleAddEditErrorResponse(response);
                                        }
                                    });
                                }
                                else {
                                    this.service.addPenalDecree(this.model).subscribe({
                                        next: (id: number) => {
                                            this.model.id = id;

                                            this.service.downloadPenalDecree(id).subscribe({
                                                next: () => {
                                                    dialogClose(this.model);
                                                }
                                            });
                                        },
                                        error: (response: HttpErrorResponse) => {
                                            this.handleAddEditErrorResponse(response);
                                        }
                                    });
                                }
                            }
                        }
                    });
                }
            }
        }
        else if (action.id === 'save-draft') {
            this.markAllAsTouched();
            this.validityCheckerGroup.validate();

            if (this.form.valid) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);

                this.model.penalDecreeStatus = AuanStatusEnum.Draft;
                this.saveDecree(dialogClose);
            }
        }
        else if (action.id === 'cancel-decree') {
            this.markAllAsTouched();
            this.validityCheckerGroup.validate();

            if (this.form.valid
                || (this.model.penalDecreeStatus === AuanStatusEnum.Submitted && this.viewMode)
            ) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);

                this.confirmDialog.open({
                    title: this.translate.getValue('penal-decrees.cancel-agreement-confirm-dialog-title'),
                    message: this.translate.getValue('penal-decrees.cancel-agreement-confirm-dialog-message'),
                    okBtnLabel: this.translate.getValue('penal-decrees.cancel-agreement-confirm-dialog-ok-btn-label')
                }).subscribe({
                    next: (ok: boolean) => {
                        if (ok) {
                            if (this.model.penalDecreeStatus === AuanStatusEnum.Draft) {
                                this.model.penalDecreeStatus = AuanStatusEnum.Canceled;
                                this.saveDecree(dialogClose);
                            }
                            else {
                                this.updateDecreeStatus(AuanStatusEnum.Canceled, dialogClose);
                            }
                        }
                    }
                });
            }
        }
        else if (action.id === 'more-corrections-needed' || action.id === 'activate-decree') {
            this.updateDecreeStatus(AuanStatusEnum.Draft, dialogClose);
        }
    }

    public decreeNumErrorLabelText(controlName: string, error: unknown, errorCode: string): TLError | undefined {
        if (controlName === 'decreeNumControl') {
            if (errorCode === 'decreeNumExists' && error === true) {
                return new TLError({ type: 'error', text: this.translate.getValue('penal-decrees.warning-num-already-exist-error') });
            }
        }
        return undefined;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            decreeNumControl: new FormControl(null, Validators.maxLength(20)),
            drafterControl: new FormControl(null, Validators.required),
            issuerPositionControl: new FormControl(null, Validators.maxLength(100)),
            issueDateControl: new FormControl(null, Validators.required),
            territoryUnitControl: new FormControl(null),
            effectiveDateControl: new FormControl(null),
            courtControl: new FormControl(null),

            auanControl: new FormControl(null),
            deliveryControl: new FormControl(null),

            commentsControl: new FormControl(null, Validators.maxLength(4000)),
            constatationCommentsControl: new FormControl(null, Validators.maxLength(4000)),
            evidenceCommentsControl: new FormControl(null, Validators.maxLength(4000)),

            seizedFishingGearControl: new FormControl(null),
            seizedFishControl: new FormControl(null),
            seizedApplianceControl: new FormControl(null),
            isRecurrentViolationControl: new FormControl(false),
            minorCircumstancesDescriptionControl: new FormControl(null, Validators.maxLength(4000)),

            auanViolatedRegulationsControl: new FormControl(null),
            violatedRegulationsControl: new FormControl(null),

            filesControl: new FormControl(null)
        }, this.violatedRegulationsValidator());
    }

    private fillForm(): void {
        this.form.get('decreeNumControl')!.setValue(this.model.decreeNum);
        this.form.get('issueDateControl')!.setValue(this.model.issueDate);
        this.form.get('effectiveDateControl')!.setValue(this.model.effectiveDate);
        this.form.get('drafterControl')!.setValue(this.users.find(x => x.value === this.model.issuerUserId));
        this.form.get('courtControl')!.setValue(this.courts.find(x => x.value === this.model.appealCourtId));
        this.form.get('issuerPositionControl')!.setValue(this.model.issuerPosition);

        this.form.get('isRecurrentViolationControl')!.setValue(this.model.isRecurrentViolation);
        this.form.get('commentsControl')!.setValue(this.model.comments);
        this.form.get('minorCircumstancesDescriptionControl')!.setValue(this.model.minorCircumstancesDescription);
        this.form.get('constatationCommentsControl')!.setValue(this.model.constatationComments);
        this.form.get('evidenceCommentsControl')!.setValue(this.model.evidenceComments);

        this.form.get('auanViolatedRegulationsControl')!.setValue(this.model.auanViolatedRegulations);
        this.form.get('violatedRegulationsControl')!.setValue(this.model.decreeViolatedRegulations);

        this.form.get('filesControl')!.setValue(this.model.files);

        if (this.model.seizedFish !== undefined && this.model.seizedFish !== null) {
            this.form.get('seizedFishControl')!.setValue(this.model.seizedFish);
        }

        if (this.model.seizedFishingGear !== undefined && this.model.seizedFishingGear !== null) {
            this.form.get('seizedFishingGearControl')!.setValue(this.model.seizedFishingGear);
        }

        if (this.model.seizedAppliance !== undefined && this.model.seizedAppliance !== null) {
            this.form.get('seizedApplianceControl')!.setValue(this.model.seizedAppliance);
        }

        if (this.model.deliveryData !== undefined && this.model.deliveryData !== null) {
            this.form.get('deliveryControl')!.setValue(this.model.deliveryData);
        }

        if (this.viewMode) {
            this.form.disable();
        }
    }

    private fillModel(): void {
        this.model.auanId = this.auanId;
        this.model.typeId = this.typeId;
        this.model.decreeNum = this.form.get('decreeNumControl')!.value;
        this.model.issueDate = this.form.get('issueDateControl')!.value;
        this.model.effectiveDate = this.form.get('effectiveDateControl')!.value;
        this.model.issuerPosition = this.form.get('issuerPositionControl')!.value;
        this.model.issuerUserId = this.form.get('drafterControl')!.value?.value;
        this.model.appealCourtId = this.form.get('courtControl')!.value?.value;

        this.model.isRecurrentViolation = this.form.get('isRecurrentViolationControl')!.value;
        this.model.comments = this.form.get('commentsControl')!.value;
        this.model.constatationComments = this.form.get('constatationCommentsControl')!.value;
        this.model.evidenceComments = this.form.get('evidenceCommentsControl')!.value;
        this.model.minorCircumstancesDescription = this.form.get('minorCircumstancesDescriptionControl')!.value;

        this.model.seizedFish = this.form.get('seizedFishControl')!.value;
        this.model.seizedFishingGear = this.form.get('seizedFishingGearControl')!.value;
        this.model.seizedAppliance = this.form.get('seizedApplianceControl')!.value;

        this.model.deliveryData = this.form.get('deliveryControl')!.value;

        this.model.auanViolatedRegulations = this.form.get('auanViolatedRegulationsControl')!.value;
        this.model.decreeViolatedRegulations = this.form.get('violatedRegulationsControl')!.value;

        if (this.isThirdParty && this.isAdding) {
            this.model.auanData = this.form.get('auanControl')!.value;
            this.model.auanData!.userId = this.form.get('drafterControl')!.value?.value;
            this.model.auanData!.territoryUnitId = this.form.get('territoryUnitControl')!.value?.value;
            this.model.auanData!.constatationComments = this.form.get('constatationCommentsControl')!.value;
            this.model.auanData!.violatedRegulations = this.form.get('auanViolatedRegulationsControl')!.value;
        }

        this.model.files = this.form.get('filesControl')!.value;
    }

    private fillAuanData(data: PenalDecreeAuanDataDTO): void {
        this.form.get('auanControl')!.setValue(data);
        this.form.get('territoryUnitControl')!.setValue(this.territoryUnits.find(x => x.value === data.territoryUnitId));
        this.form.get('constatationCommentsControl')!.setValue(data.constatationComments);

        if (this.isAdding) {
            setTimeout(() => {
                this.form.get('seizedFishControl')!.setValue(data.confiscatedFish);
                this.form.get('seizedApplianceControl')!.setValue(data.confiscatedAppliance);
                this.form.get('seizedFishingGearControl')!.setValue(data.confiscatedFishingGear);
                this.form.get('auanViolatedRegulationsControl')!.setValue(data.violatedRegulations);
            });
        }
    }

    private violatedRegulationsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (!this.violatedRegulations.some(x => x.isActive !== false)) {
                return { 'atLeastOneViolatedRegulationNeeded': true };
            }
            return null;
        }
    }

    private markAllAsTouched(): void {
        this.form.markAllAsTouched();

        this.violatedRegulationsTouched = true;
    }

    private openConfirmDialog(): Observable<boolean> {
        const message: string = this.isAdding
            ? this.translate.getValue('penal-decrees.complete-add-warning-confirm-dialog-message')
            : this.translate.getValue('penal-decrees.complete-edit-warning-confirm-dialog-message');

        return this.confirmDialog.open({
            title: this.translate.getValue('penal-decrees.complete-warning-confirm-dialog-title'),
            message: message,
            okBtnLabel: this.translate.getValue('penal-decrees.complete-warning-confirm-dialog-ok-btn-label')
        });
    }

    private saveDecree(dialogClose: DialogCloseCallback): void {
        if (this.penalDecreeId !== undefined && this.penalDecreeId !== null) {
            this.service.editPenalDecree(this.model).subscribe({
                next: () => {
                    dialogClose(this.model);
                },
                error: (response: HttpErrorResponse) => {
                    this.handleAddEditErrorResponse(response);
                }
            });
        }
        else {
            this.service.addPenalDecree(this.model).subscribe({
                next: (id: number) => {
                    this.model.id = id;
                    dialogClose(this.model);
                },
                error: (response: HttpErrorResponse) => {
                    this.handleAddEditErrorResponse(response);
                }
            });
        }
    }

    private updateDecreeStatus(status: AuanStatusEnum, dialogClose: DialogCloseCallback): void {
        this.service.updateDecreeStatus(this.penalDecreeId!, status).subscribe({
            next: () => {
                dialogClose(this.model);
            }
        });
    }

    private handleAddEditErrorResponse(response: HttpErrorResponse): void {
        if (response.error?.messages !== null && response.error?.messages !== undefined) {
            const messages: string[] = response.error.messages;

            if (messages.length !== 0) {
                this.snackbar.errorModel(response.error as ErrorModel);
            }
            else {
                this.snackbar.error(this.translate.getValue('service.an-error-occurred-in-the-app'));
            }
        }

        if (response.error?.code === ErrorCode.NoEDeliveryRegistration) {
            this.form.get('deliveryControl')!.setErrors({ 'hasNoEDeliveryRegistrationError': true });
            this.form.get('deliveryControl')!.markAsTouched();
            this.validityCheckerGroup.validate();
        }

        if (response.error?.code === ErrorCode.AuanNumAlreadyExists) {
            this.form.get('auanControl')!.setErrors({ 'auanNumExists': true });
            this.form.get('auanControl')!.markAsTouched();
            this.validityCheckerGroup.validate();
        }

        if (response.error?.code === ErrorCode.PenalDecreeNumAlreadyExists) {
            this.form.get('decreeNumControl')!.setErrors({ 'decreeNumExists': true });
            this.form.get('decreeNumControl')!.markAsTouched();
            this.validityCheckerGroup.validate();
        }
    }
}