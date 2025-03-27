import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { PenalDecreeTypeEnum } from '@app/enums/penal-decree-type.enum';
import { IPenalDecreesService } from '@app/interfaces/administration-app/penal-decrees.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { AuanViolatedRegulationDTO } from '@app/models/generated/dtos/AuanViolatedRegulationDTO';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { PenalDecreeEditDTO } from '@app/models/generated/dtos/PenalDecreeEditDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PenalDecreesService } from '@app/services/administration-app/penal-decrees.service';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { forkJoin, Observable } from 'rxjs';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { EditPenalDecreeDialogParams } from '../models/edit-penal-decree-params.model';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { PenalDecreeAuanDataDTO } from '@app/models/generated/dtos/PenalDecreeAuanDataDTO';
import { PenalDecreeResolutionDTO } from '@app/models/generated/dtos/PenalDecreeResolutionDTO';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { InspectorUserNomenclatureDTO } from '@app/models/generated/dtos/InspectorUserNomenclatureDTO';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { PenalDecreeUtils } from '../utils/penal-decree.utils';
import { AuanStatusEnum } from '@app/enums/auan-status.enum';
import { SystemPropertiesDTO } from '@app/models/generated/dtos/SystemPropertiesDTO';
import { SystemParametersService } from '@app/services/common-app/system-parameters.service';
import { DateUtils } from '@app/shared/utils/date.utils';
import { DateDifference } from '@app/models/common/date-difference.model';
import { Moment } from 'moment';
import moment from 'moment';

@Component({
    selector: 'edit-decree-resolution',
    templateUrl: './edit-decree-resolution.component.html'
})
export class EditDecreeResolutionComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;

    public readonly service!: IPenalDecreesService;
    public readonly pageCode: PageCodeEnum = PageCodeEnum.PenalDecrees;
    public readonly decreeType: PenalDecreeTypeEnum = PenalDecreeTypeEnum.Resolution;
    public readonly today: Date = new Date();

    public maxIssueDate: Date = new Date();
    public minIssueDate: Date | undefined;
    public isAdding: boolean = false;
    public viewMode: boolean = false;
    public isThirdParty: boolean = false;
    public hasTerritoryUnit: boolean = false;
    public auanViolatedRegulationsTouched: boolean = false;
    public violatedRegulationsTouched: boolean = false;
    public canSaveAfterHours: boolean = false;
    public noConstatationComments: boolean = true;
    public noEvidenceComments: boolean = true;
    public noMaterialEvidence: boolean = true;
    public noReasons: boolean = true;
    public noMotives: boolean = true;
    public noZann: boolean = true;
    public noZra: boolean = true;

    public inspectedEnityName: string | undefined;
    public violatedRegulationsTitle: string | undefined;
    public drafter: InspectorUserNomenclatureDTO | undefined;
    public canAddAfterHours: number | undefined;

    public decreeNumErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.decreeNumErrorLabelText.bind(this);

    public territoryUnits: NomenclatureDTO<number>[] = [];
    public courts: NomenclatureDTO<number>[] = [];
    public users: InspectorUserNomenclatureDTO[] = [];

    public auanViolatedRegulations: AuanViolatedRegulationDTO[] = [];
    public decreeViolatedRegulations: AuanViolatedRegulationDTO[] = [];

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private typeId!: number;
    private auanId: number | undefined;
    private penalDecreeId!: number | undefined;
    private model!: PenalDecreeEditDTO;
    private decreeNum: string | undefined;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly translate: FuseTranslationLoaderService;
    private readonly systemParametersService: SystemParametersService;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly snackbar: TLSnackbar;

    public constructor(
        service: PenalDecreesService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        systemParametersService: SystemParametersService,
        confirmDialog: TLConfirmDialog,
        snackbar: TLSnackbar
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.systemParametersService = systemParametersService;
        this.confirmDialog = confirmDialog;
        this.snackbar = snackbar;

        this.violatedRegulationsTitle = this.translate.getValue('penal-decrees.edit-violated-regulations');

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        this.isAdding = this.penalDecreeId === undefined || this.penalDecreeId === null;

        const nomenclatures: (NomenclatureDTO<number> | InspectorUserNomenclatureDTO)[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Courts, this.service.getCourts.bind(this.service), false),
            this.service.getInspectorUsernames()
        ).toPromise();

        this.territoryUnits = nomenclatures[0];
        this.courts = nomenclatures[1];
        this.users = nomenclatures[2];

        const systemParameters: SystemPropertiesDTO = await this.systemParametersService.systemParameters();
        this.canAddAfterHours = systemParameters.addAuanAfterHours;

        if (this.auanId !== undefined && this.auanId !== null) {
            this.service.getPenalDecreeAuanData(this.auanId).subscribe({
                next: (data: PenalDecreeAuanDataDTO) => {
                    this.fillAuanData(data);
                    this.isThirdParty = data.isExternal ?? false;

                    if (this.penalDecreeId === undefined || this.penalDecreeId === null) {
                        this.model = new PenalDecreeEditDTO();
                        this.model.penalDecreeStatus = AuanStatusEnum.Draft;
                        this.form.get('decreeNumControl')!.disable();
                    }
                    else {
                        this.service.getPenalDecree(this.penalDecreeId).subscribe({
                            next: (decree: PenalDecreeEditDTO) => {
                                this.model = decree;
                                this.decreeNum = decree.decreeNum;

                                if (decree.issueDate !== undefined && decree.issueDate !== null) {

                                    // Не може да се избере дата след 31.12.2024 г. за постановленията, чиито номера не са генерирани
                                    if (decree.issueDate <= PenalDecreeUtils.AUTO_GENERATE_NUMBER_AFTER_DATE) {
                                        this.maxIssueDate = PenalDecreeUtils.AUTO_GENERATE_NUMBER_AFTER_DATE;
                                    }
                                    else {
                                        // Не може да се избере дата от предишната година за постановленията, чиито номера са генерирани
                                        // с цел да не може и да се промени номерът им
                                        this.minIssueDate = new Date(decree.issueDate.getFullYear(), 0, 1);
                                    }
                                }

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

            this.form.get('territoryUnitControl')!.setValidators(Validators.required);
            this.form.get('territoryUnitControl')!.markAsPending();
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

        this.form.get('auanControl')!.valueChanges.subscribe({
            next: (auanData: PenalDecreeAuanDataDTO | undefined) => {
                if (auanData !== undefined && auanData !== null) {
                    this.inspectedEnityName = PenalDecreeUtils.getInspectedEntityName(auanData.inspectedEntity);
                }
                else {
                    this.inspectedEnityName = undefined;
                }

                this.violatedRegulationsTitle = PenalDecreeUtils.getViolatedRegulationsTitle(this.inspectedEnityName, this.translate);
            }
        });

        if (!this.viewMode) {
            this.form.get('auanViolatedRegulationsControl')!.valueChanges.subscribe({
                next: (result: AuanViolatedRegulationDTO[] | undefined) => {
                    if (result !== undefined && result !== null) {
                        this.auanViolatedRegulations = result;
                        this.auanViolatedRegulationsTouched = true;
                        this.form.updateValueAndValidity({ onlySelf: true });
                    }
                }
            });

            this.form.get('violatedRegulationsControl')!.valueChanges.subscribe({
                next: (result: AuanViolatedRegulationDTO[] | undefined) => {
                    if (result !== undefined && result !== null) {
                        this.decreeViolatedRegulations = result;
                        this.violatedRegulationsTouched = true;
                        this.form.updateValueAndValidity({ onlySelf: true });
                    }
                }
            });

            this.form.get('noConstatationCommentsControl')!.valueChanges.subscribe({
                next: (result: boolean) => {
                    this.noConstatationComments = result;
                    if (result) {
                        this.form.get('constatationCommentsControl')!.setValue(undefined);
                        this.form.get('constatationCommentsControl')!.clearValidators();
                    }
                    else {
                        this.form.get('constatationCommentsControl')!.setValidators(Validators.required);
                    }

                    this.form.get('constatationCommentsControl')!.updateValueAndValidity({ emitEvent: false });
                }
            });

            this.form.get('noEvidenceCommentsControl')!.valueChanges.subscribe({
                next: (result: boolean) => {
                    this.noEvidenceComments = result;
                    if (result) {
                        this.form.get('evidenceCommentsControl')!.setValue(undefined);
                        this.form.get('evidenceCommentsControl')!.clearValidators();
                    }
                    else {
                        this.form.get('evidenceCommentsControl')!.setValidators(Validators.required);
                    }

                    this.form.get('evidenceCommentsControl')!.updateValueAndValidity({ emitEvent: false });
                }
            });

            this.form.get('noMaterialEvidenceControl')!.valueChanges.subscribe({
                next: (result: boolean) => {
                    this.noMaterialEvidence = result;
                    if (result) {
                        this.form.get('materialEvidenceControl')!.setValue(undefined);
                        this.form.get('materialEvidenceControl')!.clearValidators();
                    }
                    else {
                        this.form.get('materialEvidenceControl')!.setValidators(Validators.required);
                    }

                    this.form.get('materialEvidenceControl')!.updateValueAndValidity({ emitEvent: false });
                }
            });

            this.form.get('noReasonsControl')!.valueChanges.subscribe({
                next: (result: boolean) => {
                    this.noReasons = result;
                    if (result) {
                        this.form.get('reasonsControl')!.setValue(undefined);
                        this.form.get('reasonsControl')!.clearValidators();
                    }
                    else {
                        this.form.get('reasonsControl')!.setValidators(Validators.required);
                    }

                    this.form.get('reasonsControl')!.updateValueAndValidity({ emitEvent: false });
                }
            });

            this.form.get('noMotivesControl')!.valueChanges.subscribe({
                next: (result: boolean) => {
                    this.noMotives = result;
                    if (result) {
                        this.form.get('motivesControl')!.setValue(undefined);
                        this.form.get('motivesControl')!.clearValidators();
                    }
                    else {
                        this.form.get('motivesControl')!.setValidators(Validators.required);
                    }

                    this.form.get('motivesControl')!.updateValueAndValidity({ emitEvent: false });
                }
            });

            this.form.get('noZannControl')!.valueChanges.subscribe({
                next: (result: boolean) => {
                    this.noZann = result;
                    if (result) {
                        this.form.get('zannControl')!.setValue(undefined);
                        this.form.get('zannControl')!.clearValidators();
                    }
                    else {
                        this.form.get('zannControl')!.setValidators(Validators.required);
                    }

                    this.form.get('zannControl')!.updateValueAndValidity({ emitEvent: false });
                }
            });

            this.form.get('noZraControl')!.valueChanges.subscribe({
                next: (result: boolean) => {
                    this.noZra = result;
                    if (result) {
                        this.form.get('zraControl')!.setValue(undefined);
                        this.form.get('zraControl')!.clearValidators();
                    }
                    else {
                        this.form.get('zraControl')!.setValidators(Validators.required);
                    }

                    this.form.get('zraControl')!.updateValueAndValidity({ emitEvent: false });
                }
            });

            // Номерата на постановленията се генерират автоматично от 01.01.2025 г., тези на създадените преди това може да се редактират
            this.form.get('issueDateControl')!.valueChanges.subscribe({
                next: (value: Moment | null | undefined) => {
                    if (value !== undefined && value !== null) {
                        if ((moment(value)).toDate() > PenalDecreeUtils.AUTO_GENERATE_NUMBER_AFTER_DATE) {
                            if (this.isAdding) {
                                this.form.get('decreeNumControl')!.setValue(undefined);
                            }
                            else {
                                this.form.get('decreeNumControl')!.setValue(this.decreeNum);
                            }

                            this.form.get('decreeNumControl')!.disable();
                        }
                        else {
                            this.form.get('decreeNumControl')!.enable();
                            this.form.get('decreeNumControl')!.setValidators(Validators.required);
                            this.form.get('decreeNumControl')!.markAsPending();
                            this.form.get('decreeNumControl')!.updateValueAndValidity({ emitEvent: false });
                        }
                    }
                }
            });
        }
    }

    public setData(data: EditPenalDecreeDialogParams | undefined, wrapperData: DialogWrapperData): void {
        if (data !== undefined && data !== null) {
            this.auanId = data.auanId;
            this.penalDecreeId = data.id;
            this.typeId = data.typeId;
            this.canSaveAfterHours = data.canSaveAfterHours;
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
                return new TLError({ type: 'error', text: this.translate.getValue('penal-decrees.resolution-num-already-exist-error') });
            }
        }
        return undefined;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            decreeNumControl: new FormControl(null, Validators.maxLength(20)),
            drafterControl: new FormControl(null, Validators.required),
            issuerPositionControl: new FormControl(null, Validators.maxLength(100)),
            issueDateControl: new FormControl(null, [Validators.required, this.cannotAddAfterHours()]),
            effectiveDateControl: new FormControl(null),
            territoryUnitControl: new FormControl(null),
            appealCourtControl: new FormControl(null),
            appealSectorControl: new FormControl(null),
            courtControl: new FormControl(null, Validators.required),

            reasonsControl: new FormControl(null, Validators.maxLength(4000)),
            motivesControl: new FormControl(null, Validators.maxLength(4000)),
            zannControl: new FormControl(null, Validators.maxLength(500)),
            zraControl: new FormControl(null, Validators.maxLength(500)),
            materialEvidenceControl: new FormControl(null, Validators.maxLength(4000)),

            noReasonsControl: new FormControl(true),
            noMotivesControl: new FormControl(true),
            noZannControl: new FormControl(true),
            noZraControl: new FormControl(true),
            noConstatationCommentsControl: new FormControl(true),
            noMaterialEvidenceControl: new FormControl(true),
            noEvidenceCommentsControl: new FormControl(true),

            auanControl: new FormControl(null),
            constatationCommentsControl: new FormControl(null, Validators.maxLength(4000)),
            evidenceCommentsControl: new FormControl(null, Validators.maxLength(4000)),
            auanViolatedRegulationsControl: new FormControl(null),

            isRecurrentViolationControl: new FormControl(false),
            commentsControl: new FormControl(null, Validators.maxLength(4000)),

            seizedFishingGearControl: new FormControl(null),
            seizedFishControl: new FormControl(null),
            seizedApplianceControl: new FormControl(null),
            violatedRegulationsControl: new FormControl(null),

            deliveryControl: new FormControl(null),

            filesControl: new FormControl(null)
        }, [
            this.auanViolatedRegulationsValidator(),
            this.decreeViolatedRegulationsValidator()
        ]);
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
        this.form.get('auanViolatedRegulationsControl')!.setValue(this.model.auanViolatedRegulations);
        this.form.get('violatedRegulationsControl')!.setValue(this.model.decreeViolatedRegulations);

        this.noConstatationComments = CommonUtils.isNullOrWhiteSpace(this.model.constatationComments);
        this.noEvidenceComments = CommonUtils.isNullOrWhiteSpace(this.model.evidenceComments);
        this.form.get('noConstatationCommentsControl')!.setValue(this.noConstatationComments);
        this.form.get('noEvidenceCommentsControl')!.setValue(this.noEvidenceComments);

        if (!this.noConstatationComments) {
            this.form.get('constatationCommentsControl')!.setValue(this.model.constatationComments);
        }

        if (!this.noEvidenceComments) {
            this.form.get('evidenceCommentsControl')!.setValue(this.model.evidenceComments);
        }

        if (this.model.seizedAppliance !== undefined && this.model.seizedAppliance !== null) {
            this.form.get('seizedFishControl')!.setValue(this.model.seizedFish);
        }

        if (this.model.seizedAppliance !== undefined && this.model.seizedAppliance !== null) {
            this.form.get('seizedApplianceControl')!.setValue(this.model.seizedAppliance);
        }

        if (this.model.seizedFishingGear !== undefined && this.model.seizedFishingGear !== null) {
            this.form.get('seizedFishingGearControl')!.setValue(this.model.seizedFishingGear);
        }

        if (this.model.deliveryData !== undefined && this.model.deliveryData !== null) {
            this.form.get('deliveryControl')!.setValue(this.model.deliveryData);
        }

        if (this.model.resolutionData !== undefined && this.model.resolutionData !== null) {
            this.noMaterialEvidence = CommonUtils.isNullOrWhiteSpace(this.model.resolutionData.materialEvidence);
            this.noReasons = CommonUtils.isNullOrWhiteSpace(this.model.resolutionData.reasons);
            this.noMotives = CommonUtils.isNullOrWhiteSpace(this.model.resolutionData.motives);
            this.noZra = CommonUtils.isNullOrWhiteSpace(this.model.resolutionData.zra);
            this.noZann = CommonUtils.isNullOrWhiteSpace(this.model.resolutionData.zann);

            this.form.get('noMaterialEvidenceControl')!.setValue(this.noMaterialEvidence);
            this.form.get('noReasonsControl')!.setValue(this.noReasons);
            this.form.get('noMotivesControl')!.setValue(this.noMotives);
            this.form.get('noZraControl')!.setValue(this.noZra);
            this.form.get('noZannControl')!.setValue(this.noZann);

            if (!this.noMaterialEvidence) {
                this.form.get('materialEvidenceControl')!.setValue(this.model.resolutionData.materialEvidence);
            }

            if (!this.noReasons) {
                this.form.get('reasonsControl')!.setValue(this.model.resolutionData.reasons);
            }

            if (!this.noMotives) {
                this.form.get('motivesControl')!.setValue(this.model.resolutionData.motives);
            }

            if (!this.noZann) {
                this.form.get('zannControl')!.setValue(this.model.resolutionData.zann);
            }

            if (!this.noZra) {
                this.form.get('zraControl')!.setValue(this.model.resolutionData.zra);
            }
        }

        this.form.get('filesControl')!.setValue(this.model.files);

        setTimeout(() => {
            this.auanViolatedRegulations = this.model.auanViolatedRegulations ?? [];
            this.decreeViolatedRegulations = this.model.decreeViolatedRegulations ?? [];
        });

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
        this.model.isRecurrentViolation = this.form.get('isRecurrentViolationControl')!.value;
        this.model.comments = this.form.get('commentsControl')!.value;
        this.model.deliveryData = this.form.get('deliveryControl')!.value;
        this.model.files = this.form.get('filesControl')!.value;
        this.model.seizedFish = this.form.get('seizedFishControl')!.value;
        this.model.seizedAppliance = this.form.get('seizedApplianceControl')!.value;
        this.model.seizedFishingGear = this.form.get('seizedFishingGearControl')!.value;
        this.model.auanViolatedRegulations = this.form.get('auanViolatedRegulationsControl')!.value;
        this.model.decreeViolatedRegulations = this.form.get('violatedRegulationsControl')!.value;
        this.model.seizedFishingGear = this.form.get('seizedFishingGearControl')!.value;
        this.model.auanTerritoryUnitId = this.form.get('territoryUnitControl')!.value?.value;
        this.model.appealCourtId = this.form.get('courtControl')!.value?.value;

        this.model.resolutionData = new PenalDecreeResolutionDTO({
            id: this.model.resolutionData?.id,
            isActive: true
        });

        if (this.noConstatationComments) {
            this.model.constatationComments = undefined;
        }
        else {
            this.model.constatationComments = this.form.get('constatationCommentsControl')!.value;
        }

        if (this.noEvidenceComments) {
            this.model.evidenceComments = undefined;
        }
        else {
            this.model.evidenceComments = this.form.get('evidenceCommentsControl')!.value;
        }

        if (this.noReasons) {
            this.model.resolutionData.reasons = undefined;
        }
        else {
            this.model.resolutionData.reasons = this.form.get('reasonsControl')!.value;
        }

        if (this.noMotives) {
            this.model.resolutionData.motives = undefined;
        }
        else {
            this.model.resolutionData.motives = this.form.get('motivesControl')!.value;
        }

        if (this.noZann) {
            this.model.resolutionData.zann = undefined;
        }
        else {
            this.model.resolutionData.zann = this.form.get('zannControl')!.value;
        }

        if (this.noZra) {
            this.model.resolutionData.zra = undefined;
        }
        else {
            this.model.resolutionData.zra = this.form.get('zraControl')!.value;
        }

        if (this.noMaterialEvidence) {
            this.model.resolutionData.materialEvidence = undefined;
        }
        else {
            this.model.resolutionData.materialEvidence = this.form.get('materialEvidenceControl')!.value;
        }

        if (this.isThirdParty) {
            this.model.auanData = this.form.get('auanControl')!.value;
            this.model.auanData!.userId = this.form.get('drafterControl')!.value?.value;
            this.model.auanData!.territoryUnitId = this.form.get('territoryUnitControl')!.value?.value;
            this.model.auanData!.constatationComments = this.form.get('constatationCommentsControl')!.value;
            this.model.auanData!.violatedRegulations = this.form.get('auanViolatedRegulationsControl')!.value;
            this.model.auanData!.isExternal = true;
        }
    }

    private fillAuanData(data: PenalDecreeAuanDataDTO): void {
        if (data.territoryUnitId !== undefined && data.territoryUnitId !== null) {
            this.hasTerritoryUnit = true;
            this.form.get('territoryUnitControl')!.setValue(this.territoryUnits.find(x => x.value === data.territoryUnitId));
        }

        this.form.get('auanControl')!.setValue(data);

        this.inspectedEnityName = PenalDecreeUtils.getInspectedEntityName(data.inspectedEntity);
        this.violatedRegulationsTitle = PenalDecreeUtils.getViolatedRegulationsTitle(this.inspectedEnityName, this.translate);

        if (this.isAdding) {
            this.noConstatationComments = CommonUtils.isNullOrWhiteSpace(data.constatationComments);
            this.form.get('noConstatationCommentsControl')!.setValue(this.noConstatationComments);

            if (!this.noConstatationComments) {
                this.form.get('constatationCommentsControl')!.setValue(data.constatationComments);
            }

            setTimeout(() => {
                this.form.get('seizedFishControl')!.setValue(data.confiscatedFish);
                this.form.get('seizedApplianceControl')!.setValue(data.confiscatedAppliance);
                this.form.get('seizedFishingGearControl')!.setValue(data.confiscatedFishingGear);
                this.form.get('auanViolatedRegulationsControl')!.setValue(data.violatedRegulations);
            });
        }
    }

    private auanViolatedRegulationsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (!this.auanViolatedRegulations.some(x => x.isActive !== false)) {
                return { 'atLeastOneAuanViolatedRegulationNeeded': true };
            }
            return null;
        }
    }

    private decreeViolatedRegulationsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (!this.decreeViolatedRegulations.some(x => x.isActive !== false)) {
                return { 'atLeastOneViolatedRegulationNeeded': true };
            }
            return null;
        }
    }

    private cannotAddAfterHours(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === undefined || control === null) {
                return null;
            }

            if (this.form === undefined || this.form === null) {
                return null;
            }

            if (control.value === null || control.value === undefined) {
                return null;
            }

            if (this.canSaveAfterHours) {
                return null;
            }

            if (this.canAddAfterHours === undefined || this.canAddAfterHours === null) {
                return null;
            }

            const startDate: Date = (moment(control.value)).toDate();
            const now: Date = new Date();

            const difference: DateDifference | undefined = DateUtils.getDateDifference(startDate, now);

            if (difference === undefined || difference === null) {
                return null;
            }

            if (difference.minutes === 0 && difference.hours === 0 && difference.days === 0) {
                return null;
            }

            const differenceHours: number = (difference.days! * 24) + difference.hours! + (difference.minutes! / 60);

            if (differenceHours > this.canAddAfterHours) {
                return { cannotAddAfterHours: true };
            }

            return null;
        }
    }

    private markAllAsTouched(): void {
        this.form.markAllAsTouched();

        this.auanViolatedRegulationsTouched = true;
        this.violatedRegulationsTouched = true;
    }

    private openConfirmDialog(): Observable<boolean> {
        const message: string = this.isAdding
            ? this.translate.getValue('penal-decrees.complete-add-resolution-confirm-dialog-message')
            : this.translate.getValue('penal-decrees.complete-edit-resolution-confirm-dialog-message');

        return this.confirmDialog.open({
            title: this.translate.getValue('penal-decrees.complete-resolution-confirm-dialog-title'),
            message: message,
            okBtnLabel: this.translate.getValue('penal-decrees.complete-resolution-confirm-dialog-ok-btn-label')
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