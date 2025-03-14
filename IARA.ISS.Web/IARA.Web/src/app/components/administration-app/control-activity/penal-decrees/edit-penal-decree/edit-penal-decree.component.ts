import { HttpErrorResponse } from '@angular/common/http';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IPenalDecreesService } from '@app/interfaces/administration-app/penal-decrees.interface';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { PenalDecreeEditDTO } from '@app/models/generated/dtos/PenalDecreeEditDTO';
import { PenalDecreesService } from '@app/services/administration-app/penal-decrees.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { PenalDecreeAuanDataDTO } from '@app/models/generated/dtos/PenalDecreeAuanDataDTO';
import { EditPenalDecreeDialogParams } from '../models/edit-penal-decree-params.model';
import { PenalDecreeSanctionTypesEnum } from '@app/enums/penal-decree-sanction-types.enum';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { forkJoin, Observable } from 'rxjs';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PenalDecreeStatusTypesEnum } from '@app/enums/penal-decree-status-types.enum';
import { PenalDecreeTypeEnum } from '@app/enums/penal-decree-type.enum';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { PenalDecreeFishCompensationDTO } from '@app/models/generated/dtos/PenalDecreeFishCompensationDTO';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { AuanViolatedRegulationDTO } from '@app/models/generated/dtos/AuanViolatedRegulationDTO';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { InspectorUserNomenclatureDTO } from '@app/models/generated/dtos/InspectorUserNomenclatureDTO';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { AuanInspectedEntityDTO } from '@app/models/generated/dtos/AuanInspectedEntityDTO';
import { PenalDecreeUtils } from '../utils/penal-decree.utils';
import { AuanStatusEnum } from '@app/enums/auan-status.enum';
import { SystemPropertiesDTO } from '@app/models/generated/dtos/SystemPropertiesDTO';
import { SystemParametersService } from '@app/services/common-app/system-parameters.service';
import { DateUtils } from '@app/shared/utils/date.utils';
import { DateDifference } from '@app/models/common/date-difference.model';
import { Moment } from 'moment';
import moment from 'moment';

@Component({
    selector: 'edit-penal-decree',
    templateUrl: './edit-penal-decree.component.html'
})
export class EditPenalDecreeComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;
    public fishCompensationForm!: FormGroup;

    public readonly service!: IPenalDecreesService;
    public readonly pageCode: PageCodeEnum = PageCodeEnum.PenalDecrees;
    public readonly decreeType: PenalDecreeTypeEnum = PenalDecreeTypeEnum.PenalDecree;
    public readonly decreeStatusTypesEnum: typeof PenalDecreeStatusTypesEnum = PenalDecreeStatusTypesEnum;
    public readonly today: Date = new Date();

    public maxIssueDate: Date = new Date();
    public minIssueDate: Date | undefined;
    public isAdding: boolean = false;
    public viewMode: boolean = false;
    public isThirdParty: boolean = false;
    public hasTerritoryUnit: boolean = false;
    public hasCompensationAmount: boolean = false;
    public fishCompensationFormTouched: boolean = false;
    public auanViolatedRegulationsTouched: boolean = false;
    public violatedRegulationsTouched: boolean = false;
    public canSaveAfterHours: boolean = false;

    public inspectedEnityName: string | undefined;
    public violatedRegulationsTitle: string | undefined;
    public drafter: InspectorUserNomenclatureDTO | undefined;
    public canAddAfterHours: number | undefined;

    public decreeNumErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.decreeNumErrorLabelText.bind(this);

    public sanctionTypes: NomenclatureDTO<number>[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public courts: NomenclatureDTO<number>[] = [];
    public users: InspectorUserNomenclatureDTO[] = [];

    public fishCompensations: PenalDecreeFishCompensationDTO[] = [];
    public auanViolatedRegulations: AuanViolatedRegulationDTO[] = [];
    public decreeViolatedRegulations: AuanViolatedRegulationDTO[] = [];

    public sanctionType: PenalDecreeSanctionTypesEnum | undefined;

    @ViewChild('fishCompensationsTable')
    private fishCompensationsTable!: TLDataTableComponent;

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
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.PenalDecreeSanctionTypes, this.service.getPenalDecreeSanctionTypes.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Courts, this.service.getCourts.bind(this.service), false),
            this.service.getInspectorUsernames()
        ).toPromise();

        this.sanctionTypes = nomenclatures[0];
        this.territoryUnits = nomenclatures[1];
        this.fishes = nomenclatures[2];
        this.courts = nomenclatures[3];
        this.users = nomenclatures[4];

        this.addSanctionControls();

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

            this.fishCompensationForm.get('countControl')!.valueChanges.subscribe({
                next: (event: RecordChangedEventArgs<PenalDecreeFishCompensationDTO>) => {
                    this.fishCompensationFormTouched = true;

                    this.fishCompensationForm.updateValueAndValidity({ onlySelf: true });
                }
            });

            this.fishCompensationForm.get('weightControl')!.valueChanges.subscribe({
                next: (event: RecordChangedEventArgs<PenalDecreeFishCompensationDTO>) => {
                    this.fishCompensationFormTouched = true;

                    this.fishCompensationForm.updateValueAndValidity({ onlySelf: true });
                }
            });

            // Номерата на постановленията се генерират автоматично от 01.01.2025 г., тези на създадените преди това може да се редактират
            this.form.get('issueDateControl')!.valueChanges.subscribe({
                next: (value: Moment | null | undefined) => {
                    if (value !== undefined && value !== null) {
                        if (value.toDate() > PenalDecreeUtils.AUTO_GENERATE_NUMBER_AFTER_DATE) {
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
                                this.model.penalDecreeStatus = AuanStatusEnum.Submitted;

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
                return new TLError({ type: 'error', text: this.translate.getValue('penal-decrees.penal-decree-num-already-exist-error') });
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
            courtControl: new FormControl(null, Validators.required),

            auanControl: new FormControl(null),
            auanViolatedRegulationsControl: new FormControl(null),

            isRecurrentViolationControl: new FormControl(false),
            sanctionDescriptionControl: new FormControl(null, Validators.maxLength(4000)),
            fineAmountControl: new FormControl(null, TLValidators.number(0, undefined, 2)),
            compensationAmountControl: new FormControl(null, TLValidators.number(0, undefined, 2)),
            commentsControl: new FormControl(null, Validators.maxLength(4000)),
            constatationCommentsControl: new FormControl(null, Validators.maxLength(4000)),
            evidenceCommentsControl: new FormControl(null, Validators.maxLength(4000)),

            seizedFishingGearControl: new FormControl(null),
            seizedFishControl: new FormControl(null),
            seizedApplianceControl: new FormControl(null),
            violatedRegulationsControl: new FormControl(null),
            fishCompensationViolatedRegulationsControl: new FormControl(null),

            deliveryControl: new FormControl(null),

            filesControl: new FormControl(null)
        }, [
            this.decreeViolatedRegulationsValidator(),
            this.auanViolatedRegulationsValidator()
        ]);

        this.fishCompensationForm = new FormGroup({
            fishIdControl: new FormControl(null, Validators.required),
            weightControl: new FormControl(null, TLValidators.number(0)),
            countControl: new FormControl(null, TLValidators.number(1, undefined, 0)),
            totalPriceControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 2)]),
            unitPriceControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 2)]),
            turbotSizeGroupIdControl: new FormControl(null),
        }, this.fishCountValidator());
    }

    private addSanctionControls(): void {
        for (const sanction of this.sanctionTypes) {
            const controlName: string = sanction.code! + 'Control';
            this.form.addControl(controlName, new FormControl());
        }

        const compensationControlName: string = PenalDecreeSanctionTypesEnum[PenalDecreeSanctionTypesEnum.Compensation] + 'Control';
        this.form.get(compensationControlName)!.valueChanges.subscribe({
            next: (value: boolean) => {
                this.hasCompensationAmount = value;

                if (!this.hasCompensationAmount) {
                    this.form.get('compensationAmountControl')!.setValue(undefined);
                }

                this.form.get('compensationAmountControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    private fillForm(): void {
        this.form.get('decreeNumControl')!.setValue(this.model.decreeNum);
        this.form.get('issueDateControl')!.setValue(this.model.issueDate);
        this.form.get('effectiveDateControl')!.setValue(this.model.effectiveDate);
        this.form.get('drafterControl')!.setValue(this.users.find(x => x.value === this.model.issuerUserId));
        this.form.get('courtControl')!.setValue(this.courts.find(x => x.value === this.model.appealCourtId));
        this.form.get('issuerPositionControl')!.setValue(this.model.issuerPosition);

        this.form.get('isRecurrentViolationControl')!.setValue(this.model.isRecurrentViolation);
        this.form.get('sanctionDescriptionControl')!.setValue(this.model.sanctionDescription);
        this.form.get('fineAmountControl')!.setValue(this.model.fineAmount?.toFixed(2));
        this.form.get('compensationAmountControl')!.setValue(this.model.compensationAmount?.toFixed(2));
        this.form.get('commentsControl')!.setValue(this.model.comments);

        this.form.get('auanViolatedRegulationsControl')!.setValue(this.model.auanViolatedRegulations);
        this.form.get('violatedRegulationsControl')!.setValue(this.model.decreeViolatedRegulations);
        this.form.get('fishCompensationViolatedRegulationsControl')!.setValue(this.model.fishCompensationViolatedRegulations);
        this.form.get('constatationCommentsControl')!.setValue(this.model.constatationComments);
        this.form.get('evidenceCommentsControl')!.setValue(this.model.evidenceComments);

        if (this.model.seizedFish !== undefined && this.model.seizedFish !== null) {
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

        this.form.get('filesControl')!.setValue(this.model.files);

        if (this.model.sanctionTypeIds) {
            for (const sanctionId of this.model.sanctionTypeIds) {
                for (const sanction of this.sanctionTypes) {
                    const controlName: string = sanction.code! + 'Control';

                    if (sanction.value === sanctionId) {
                        this.form.get(controlName)!.setValue(true);
                    }
                }
            }
        }

        setTimeout(() => {
            this.fishCompensations = this.model.fishCompensations ?? [];
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
        this.model.appealCourtId = this.form.get('courtControl')!.value?.value;
        this.model.auanTerritoryUnitId = this.form.get('territoryUnitControl')!.value?.value;

        this.model.isRecurrentViolation = this.form.get('isRecurrentViolationControl')!.value;
        this.model.sanctionDescription = this.form.get('sanctionDescriptionControl')!.value;
        this.model.fineAmount = this.form.get('fineAmountControl')!.value;
        this.model.comments = this.form.get('commentsControl')!.value;
        this.model.constatationComments = this.form.get('constatationCommentsControl')!.value;
        this.model.evidenceComments = this.form.get('evidenceCommentsControl')!.value;
        this.model.deliveryData = this.form.get('deliveryControl')!.value;

        if (this.hasCompensationAmount) {
            this.model.compensationAmount = this.form.get('compensationAmountControl')!.value;
        }
        else {
            this.model.compensationAmount = undefined;
        }

        this.model.files = this.form.get('filesControl')!.value;

        this.model.seizedFish = this.form.get('seizedFishControl')!.value;
        this.model.seizedAppliance = this.form.get('seizedApplianceControl')!.value;
        this.model.seizedFishingGear = this.form.get('seizedFishingGearControl')!.value;
        this.model.auanViolatedRegulations = this.form.get('auanViolatedRegulationsControl')!.value;
        this.model.decreeViolatedRegulations = this.form.get('violatedRegulationsControl')!.value;
        this.model.fishCompensationViolatedRegulations = this.form.get('fishCompensationViolatedRegulationsControl')!.value;

        this.model.fishCompensations = this.getFishCompensationsFromTable();

        this.model.sanctionTypeIds = [];

        for (const sanction of this.sanctionTypes) {
            const controlName: string = sanction.code! + 'Control';
            if (this.form.get(controlName)!.value === true) {
                this.model.sanctionTypeIds!.push(sanction.value!);
            }
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
        this.form.get('constatationCommentsControl')!.setValue(data.constatationComments);
        this.inspectedEnityName = PenalDecreeUtils.getInspectedEntityName(data.inspectedEntity);
        this.violatedRegulationsTitle = PenalDecreeUtils.getViolatedRegulationsTitle(this.inspectedEnityName, this.translate);

        if (this.isAdding) {
            setTimeout(() => {
                this.form.get('seizedFishControl')!.setValue(data.confiscatedFish);
                this.form.get('seizedApplianceControl')!.setValue(data.confiscatedAppliance);
                this.form.get('seizedFishingGearControl')!.setValue(data.confiscatedFishingGear);
                this.form.get('auanViolatedRegulationsControl')!.setValue(data.violatedRegulations);
            });
        }
    }

    private getFishCompensationsFromTable(): PenalDecreeFishCompensationDTO[] {
        const rows = this.fishCompensationsTable.rows as PenalDecreeFishCompensationDTO[];

        return rows.map(x => new PenalDecreeFishCompensationDTO({
            id: x.id,
            fishId: x.fishId,
            weight: x.weight,
            count: x.count,
            totalPrice: x.totalPrice,
            unitPrice: x.unitPrice,
            isActive: x.isActive ?? true
        }));
    }

    private fishCountValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.fishCompensationsTable !== undefined && this.fishCompensationsTable !== null) {
                const count: number | undefined = this.fishCompensationForm.get('countControl')!.value;
                const weight: number | undefined = this.fishCompensationForm.get('weightControl')!.value;

                if ((count === undefined || count === null || Number(count) === 0) && (weight === undefined || weight === null || Number(weight) === 0)) {
                    return { 'fishCountValidationError': true };
                }
            }
            return null;
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
            ? this.translate.getValue('penal-decrees.complete-add-penal-decree-confirm-dialog-message')
            : this.translate.getValue('penal-decrees.complete-edit-penal-decree-confirm-dialog-message');

        return this.confirmDialog.open({
            title: this.translate.getValue('penal-decrees.complete-penal-decree-confirm-dialog-title'),
            message: message,
            okBtnLabel: this.translate.getValue('penal-decrees.complete-penal-decree-confirm-dialog-ok-btn-label')
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
            },
            error: (response: HttpErrorResponse) => {
                this.handleAddEditErrorResponse(response);
            }
        });
    }

    private handleAddEditErrorResponse(response: HttpErrorResponse): void {
        if (response.error?.messages !== null && response.error?.messages !== undefined) {
            const messages: string[] = response.error.messages;

            if (messages.length !== 0) {
                this.snackbar.errorModel(response.error as ErrorModel, RequestProperties.DEFAULT);
            }
            else {
                this.snackbar.errorModel(new ErrorModel({ messages: [this.translate.getValue('service.an-error-occurred-in-the-app')] }), RequestProperties.DEFAULT);
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

        if (response.error?.code === ErrorCode.CannotCancelDecreeWithPenalPoints) {
            const errorMessage: string = this.translate.getValue('penal-decrees.cannot-cancel-decree-with-penal-points');
            this.snackbar.error(errorMessage);
        }
    }
}