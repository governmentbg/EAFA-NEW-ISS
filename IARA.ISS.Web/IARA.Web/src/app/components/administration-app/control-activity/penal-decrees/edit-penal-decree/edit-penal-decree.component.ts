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

    public isAdding: boolean = false;
    public viewMode: boolean = false;
    public isThirdParty: boolean = false;
    public fishCompensationFormTouched: boolean = false;
    public violatedRegulationsTouched: boolean = false;

    public sanctionTypes: NomenclatureDTO<number>[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];
    public users: NomenclatureDTO<number>[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public courts: NomenclatureDTO<number>[] = [];
    public sectors: NomenclatureDTO<number>[] = [];

    public fishCompensations: PenalDecreeFishCompensationDTO[] = [];
    public violatedRegulations: AuanViolatedRegulationDTO[] = [];

    public sanctionType: PenalDecreeSanctionTypesEnum | undefined;

    @ViewChild('fishCompensationsTable')
    private fishCompensationsTable!: TLDataTableComponent;

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
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
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
        this.isThirdParty = this.auanId === undefined || this.auanId === null;

        const nomenclatures: (NomenclatureDTO<number>)[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.PenalDecreeSanctionTypes, this.service.getPenalDecreeSanctionTypes.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Courts, this.service.getCourts.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Sectors, this.nomenclatures.getSectors.bind(this.nomenclatures), false),
            this.service.getInspectorUsernames()
        ).toPromise();

        this.sanctionTypes = nomenclatures[0];
        this.territoryUnits = nomenclatures[1];
        this.fishes = nomenclatures[2];
        this.courts = nomenclatures[3];
        this.sectors = nomenclatures[4];
        this.users = nomenclatures[5];

        this.addSanctionControls();

        if (this.auanId !== undefined && this.auanId !== null) {
            this.form.get('territoryUnitControl')!.disable();

            this.service.getPenalDecreeAuanData(this.auanId).subscribe({
                next: (data: PenalDecreeAuanDataDTO) => {
                    this.fillAuanData(data);

                    if (this.penalDecreeId === undefined || this.penalDecreeId === null) {
                        this.model = new PenalDecreeEditDTO();
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
            this.model = new PenalDecreeEditDTO();
        }
    }

    public ngAfterViewInit(): void {
        this.form.get('auanViolatedRegulationsControl')!.valueChanges.subscribe({
            next: (result: AuanViolatedRegulationDTO[] | undefined) => {
                if (result !== undefined && result !== null) {
                    this.violatedRegulations = result;
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
    }

    public setData(data: EditPenalDecreeDialogParams | undefined, wrapperData: DialogWrapperData): void {
        if (data !== undefined && data !== null) {
            this.auanId = data.auanId;
            this.penalDecreeId = data.id;
            this.typeId = data.typeId;
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
            CommonUtils.sanitizeModelStrings(this.model);

            this.openConfirmDialog().subscribe({
                next: (ok: boolean) => {
                    if (ok) {
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
    }

    private buildForm(): void {
        this.form = new FormGroup({
            decreeNumControl: new FormControl(null, [Validators.required, Validators.maxLength(20)]),
            drafterControl: new FormControl(null, Validators.required),
            issuerPositionControl: new FormControl(null, Validators.maxLength(100)),
            issueDateControl: new FormControl(null, Validators.required),
            effectiveDateControl: new FormControl(null),
            territoryUnitControl: new FormControl(null),
            courtControl: new FormControl(null),
            sectorControl: new FormControl(null),

            auanControl: new FormControl(null),
            auanViolatedRegulationsControl: new FormControl(null),

            isRecurrentViolationControl: new FormControl(false),
            sanctionDescriptionControl: new FormControl(null, Validators.maxLength(4000)),
            fineAmountControl: new FormControl(null, TLValidators.number(0, undefined, 2)),
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
        }, this.violatedRegulationsValidator());

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
    }

    private fillForm(): void {
        this.form.get('decreeNumControl')!.setValue(this.model.decreeNum);
        this.form.get('issueDateControl')!.setValue(this.model.issueDate);
        this.form.get('effectiveDateControl')!.setValue(this.model.effectiveDate);
        this.form.get('issuerPositionControl')!.setValue(this.model.issuerPosition);
        this.form.get('drafterControl')!.setValue(this.users.find(x => x.value === this.model.issuerUserId));
        this.form.get('courtControl')!.setValue(this.courts.find(x => x.value === this.model.appealCourtId));
        this.form.get('sectorControl')!.setValue(this.sectors.find(x => x.value === this.model.appealSectorId));

        this.form.get('isRecurrentViolationControl')!.setValue(this.model.isRecurrentViolation);
        this.form.get('sanctionDescriptionControl')!.setValue(this.model.sanctionDescription);
        this.form.get('fineAmountControl')!.setValue(this.model.fineAmount?.toFixed(2));
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
            this.violatedRegulations = this.model.auanViolatedRegulations ?? [];
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
        this.model.appealSectorId = this.form.get('sectorControl')!.value?.value;

        this.model.isRecurrentViolation = this.form.get('isRecurrentViolationControl')!.value;
        this.model.sanctionDescription = this.form.get('sanctionDescriptionControl')!.value;
        this.model.fineAmount = this.form.get('fineAmountControl')!.value;
        this.model.comments = this.form.get('commentsControl')!.value;
        this.model.constatationComments = this.form.get('constatationCommentsControl')!.value;
        this.model.evidenceComments = this.form.get('evidenceCommentsControl')!.value;
        this.model.deliveryData = this.form.get('deliveryControl')!.value;

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
            this.model.auanData!.territoryUnitId = this.form.get('territoryUnitControl')!.value?.value;
            this.model.auanData!.constatationComments = this.form.get('constatationCommentsControl')!.value;
            this.model.auanData!.violatedRegulations = this.form.get('auanViolatedRegulationsControl')!.value;
        }
    }

    private fillAuanData(data: PenalDecreeAuanDataDTO): void {
        this.form.get('territoryUnitControl')!.setValue(this.territoryUnits.find(x => x.value === data.territoryUnitId));
        this.form.get('auanControl')!.setValue(data);
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
            ? this.translate.getValue('penal-decrees.complete-add-penal-decree-confirm-dialog-message')
            : this.translate.getValue('penal-decrees.complete-edit-penal-decree-confirm-dialog-message');

        return this.confirmDialog.open({
            title: this.translate.getValue('penal-decrees.complete-penal-decree-confirm-dialog-title'),
            message: message,
            okBtnLabel: this.translate.getValue('penal-decrees.complete-penal-decree-confirm-dialog-ok-btn-label')
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
    }
}