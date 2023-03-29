import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IPenalDecreesService } from '@app/interfaces/administration-app/penal-decrees.interface';
import { PenalDecreeEditDTO } from '@app/models/generated/dtos/PenalDecreeEditDTO';
import { PenalDecreesService } from '@app/services/administration-app/penal-decrees.service';
import { EditPenalDecreeDialogParams } from '../models/edit-penal-decree-params.model';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { PenalDecreeAuanDataDTO } from '@app/models/generated/dtos/PenalDecreeAuanDataDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { PenalDecreeTypeEnum } from '@app/enums/penal-decree-type.enum';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { forkJoin } from 'rxjs';
import { AuanViolatedRegulationDTO } from '@app/models/generated/dtos/AuanViolatedRegulationDTO';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';

@Component({
    selector: 'edit-decree-agreement',
    templateUrl: './edit-decree-agreement.component.html'
})
export class EditDecreeAgreementComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;

    public readonly service!: IPenalDecreesService;
    public readonly pageCode: PageCodeEnum = PageCodeEnum.PenalDecrees;
    public readonly decreeType: PenalDecreeTypeEnum = PenalDecreeTypeEnum.Agreement;
    public readonly today: Date = new Date();

    public isAdding: boolean = false;
    public viewMode: boolean = false;
    public isThirdParty: boolean = false;
    public violatedRegulationsTouched: boolean = false;

    public territoryUnits: NomenclatureDTO<number>[] = [];
    public users: NomenclatureDTO<number>[] = [];
    public violatedRegulations: AuanViolatedRegulationDTO[] = [];

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private typeId!: number;
    private auanId: number | undefined;
    private penalDecreeId!: number | undefined;
    private model!: PenalDecreeEditDTO;
    private readonly nomenclatures: CommonNomenclatures;

    public constructor(
        service: PenalDecreesService,
        nomenclatures: CommonNomenclatures
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        this.isAdding = this.penalDecreeId === undefined || this.penalDecreeId === null;
        this.isThirdParty = this.auanId === undefined || this.auanId === null;

        const nomenclatures: NomenclatureDTO<number>[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures)),
            this.service.getInspectorUsernames()
        ).toPromise();

        this.territoryUnits = nomenclatures[0];
        this.users = nomenclatures[1];

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
        this.form.controls.fineControl.valueChanges.subscribe({
            next: (fineAmount: number | undefined) => {
                if (fineAmount !== undefined && fineAmount !== null) {
                    const finePercent: string = (fineAmount * 0.7).toFixed(2);
                    this.form.get('finePercentControl')!.setValue(finePercent);
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
                if (this.form.valid) {
                    this.fillModel();
                    CommonUtils.sanitizeModelStrings(this.model);

                    if (this.penalDecreeId !== undefined && this.penalDecreeId !== null) {
                        this.service.editPenalDecree(this.model).subscribe({
                            next: () => {

                                this.service.downloadPenalDecree(this.penalDecreeId!).subscribe({
                                    next: () => {
                                        dialogClose(this.model);
                                    }
                                });
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
                            }
                        });
                    }
                }
            }
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            decreeNumControl: new FormControl(null, [Validators.required, Validators.maxLength(20)]),
            drafterControl: new FormControl(null, Validators.required),
            issueDateControl: new FormControl(null, Validators.required),
            issuerPositionControl: new FormControl(null),
            territoryUnitControl: new FormControl(null),
            effectiveDateControl: new FormControl(null),
            auanControl: new FormControl(null),
            auanViolatedRegulationsControl: new FormControl(null),
            violatedRegulationsControl: new FormControl(null),
            fineControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 2)]),
            finePercentControl: new FormControl({ value: null, disabled: true }),
            commentsControl: new FormControl(null, Validators.maxLength(4000)),
            constatationCommentsControl: new FormControl(null, Validators.maxLength(4000)),
            seizedFishingGearControl: new FormControl(null),
            seizedFishControl: new FormControl(null),
            seizedApplianceControl: new FormControl(null),

            filesControl: new FormControl(null)
        }, this.violatedRegulationsValidator());
    }

    private fillForm(): void {
        this.form.get('decreeNumControl')!.setValue(this.model.decreeNum);
        this.form.get('issueDateControl')!.setValue(this.model.issueDate);
        this.form.get('effectiveDateControl')!.setValue(this.model.effectiveDate);
        this.form.get('issuerPositionControl')!.setValue(this.model.issuerPosition);
        this.form.get('drafterControl')!.setValue(this.users.find(x => x.value === this.model.issuerUserId));

        this.form.get('fineControl')!.setValue(this.model.fineAmount?.toFixed(2));
        this.form.get('commentsControl')!.setValue(this.model.comments);
        this.form.get('constatationCommentsControl')!.setValue(this.model.constatationComments);

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

        this.model.fineAmount = this.form.get('fineControl')!.value;
        this.model.comments = this.form.get('commentsControl')!.value;
        this.model.constatationComments = this.form.get('constatationCommentsControl')!.value;

        this.model.seizedFish = this.form.get('seizedFishControl')!.value;
        this.model.seizedFishingGear = this.form.get('seizedFishingGearControl')!.value;
        this.model.seizedAppliance = this.form.get('seizedApplianceControl')!.value;

        this.model.auanViolatedRegulations = this.form.get('auanViolatedRegulationsControl')!.value;
        this.model.decreeViolatedRegulations = this.form.get('violatedRegulationsControl')!.value;

        if (this.isThirdParty) {
            this.model.auanData = this.form.get('auanControl')!.value;
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


    private handleAddEditErrorResponse(response: HttpErrorResponse): void {
        const error: ErrorModel = response.error as ErrorModel;

        if (error !== undefined && error !== null) {
            if (response.error?.code === ErrorCode.AuanNumAlreadyExists) {
                this.form.get('auanControl')!.setErrors({ 'auanNumExists': true });
                this.form.get('auanControl')!.markAsTouched();
                this.validityCheckerGroup.validate();
            }
        }
    }
}