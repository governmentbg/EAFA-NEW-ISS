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
import { MatSnackBar } from '@angular/material/snack-bar';
import { PenalDecreesService } from '@app/services/administration-app/penal-decrees.service';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { forkJoin } from 'rxjs';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { EditPenalDecreeDialogParams } from '../models/edit-penal-decree-params.model';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorSnackbarComponent } from '@app/shared/components/error-snackbar/error-snackbar.component';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { PenalDecreeAuanDataDTO } from '@app/models/generated/dtos/PenalDecreeAuanDataDTO';
import { PenalDecreeResolutionDTO } from '@app/models/generated/dtos/PenalDecreeResolutionDTO';

@Component({
    selector: 'edit-decree-resolution',
    templateUrl: './edit-decree-resolution.component.html'
})
export class EditDecreeResolutionComponent implements OnInit, AfterViewInit, IDialogComponent{
    public form!: FormGroup;

    public readonly service!: IPenalDecreesService;
    public readonly pageCode: PageCodeEnum = PageCodeEnum.PenalDecrees;
    public readonly decreeType: PenalDecreeTypeEnum = PenalDecreeTypeEnum.Resolution;
    public readonly today: Date = new Date();

    public isAdding: boolean = false;
    public viewMode: boolean = false;
    public isThirdParty: boolean = false;
    public violatedRegulationsTouched: boolean = false;

    public territoryUnits: NomenclatureDTO<number>[] = [];
    public users: NomenclatureDTO<number>[] = [];
    public courts: NomenclatureDTO<number>[] = [];
    public sectors: NomenclatureDTO<number>[] = [];

    public violatedRegulations: AuanViolatedRegulationDTO[] = [];

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private typeId!: number;
    private auanId: number | undefined;
    private penalDecreeId!: number | undefined;
    private model!: PenalDecreeEditDTO;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly translate: FuseTranslationLoaderService;
    private readonly snackbar: MatSnackBar;

    public constructor(
        service: PenalDecreesService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        snackbar: MatSnackBar
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.snackbar = snackbar;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        this.isAdding = this.penalDecreeId === undefined || this.penalDecreeId === null;
        this.isThirdParty = this.auanId === undefined || this.auanId === null;

        const nomenclatures: (NomenclatureDTO<number>)[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Courts, this.service.getCourts.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Sectors, this.nomenclatures.getSectors.bind(this.nomenclatures), false),
            this.service.getInspectorUsernames()
        ).toPromise();

        this.territoryUnits = nomenclatures[0];
        this.courts = nomenclatures[1];
        this.sectors = nomenclatures[2];
        this.users = nomenclatures[3];

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
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            decreeNumControl: new FormControl(null, [Validators.required, Validators.maxLength(20)]),
            drafterControl: new FormControl(null, Validators.required),
            issuerPositionControl: new FormControl(null),
            issueDateControl: new FormControl(null, Validators.required),
            effectiveDateControl: new FormControl(null),
            territoryUnitControl: new FormControl(null),
            appealCourtControl: new FormControl(null),
            appealSectorControl: new FormControl(null),
            courtControl: new FormControl(null),
            sectorControl: new FormControl(null),

            reasonsControl: new FormControl(null, Validators.maxLength(4000)),
            motivesControl: new FormControl(null, Validators.maxLength(4000)),
            zannControl: new FormControl(null, Validators.maxLength(500)),
            zraControl: new FormControl(null, Validators.maxLength(500)),
            materialEvidenceControl: new FormControl(null, Validators.maxLength(4000)),

            auanControl: new FormControl(null),
            constatationCommentsControl: new FormControl(null, Validators.maxLength(4000)),
            auanViolatedRegulationsControl: new FormControl(null),

            isRecurrentViolationControl: new FormControl(false),
            commentsControl: new FormControl(null, Validators.maxLength(4000)),

            seizedFishingGearControl: new FormControl(null),
            seizedFishControl: new FormControl(null),
            seizedApplianceControl: new FormControl(null),
            violatedRegulationsControl: new FormControl(null),

            deliveryControl: new FormControl(null),

            filesControl: new FormControl(null)
        }, this.violatedRegulationsValidator());
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
        this.form.get('commentsControl')!.setValue(this.model.comments);
        this.form.get('constatationCommentsControl')!.setValue(this.model.constatationComments);
        this.form.get('auanViolatedRegulationsControl')!.setValue(this.model.auanViolatedRegulations);
        this.form.get('violatedRegulationsControl')!.setValue(this.model.decreeViolatedRegulations);

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
            this.form.get('reasonsControl')!.setValue(this.model.resolutionData.reasons);
            this.form.get('motivesControl')!.setValue(this.model.resolutionData.motives);
            this.form.get('zannControl')!.setValue(this.model.resolutionData.zann);
            this.form.get('zraControl')!.setValue(this.model.resolutionData.zra);
            this.form.get('materialEvidenceControl')!.setValue(this.model.resolutionData.materialEvidence);
        }

        this.form.get('filesControl')!.setValue(this.model.files);

        setTimeout(() => {
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
        this.model.isRecurrentViolation = this.form.get('isRecurrentViolationControl')!.value;
        this.model.comments = this.form.get('commentsControl')!.value;
        this.model.constatationComments = this.form.get('constatationCommentsControl')!.value;
        this.model.deliveryData = this.form.get('deliveryControl')!.value;
        this.model.files = this.form.get('filesControl')!.value;
        this.model.seizedFish = this.form.get('seizedFishControl')!.value;
        this.model.seizedAppliance = this.form.get('seizedApplianceControl')!.value;
        this.model.seizedFishingGear = this.form.get('seizedFishingGearControl')!.value;
        this.model.auanViolatedRegulations = this.form.get('auanViolatedRegulationsControl')!.value;
        this.model.decreeViolatedRegulations = this.form.get('violatedRegulationsControl')!.value;
        this.model.appealCourtId = this.form.get('courtControl')!.value?.value;
        this.model.appealSectorId = this.form.get('sectorControl')!.value?.value;

        this.model.resolutionData = new PenalDecreeResolutionDTO({
            id: this.model.resolutionData?.id,
            isActive: true
        });

        this.model.resolutionData.reasons = this.form.get('reasonsControl')!.value;
        this.model.resolutionData.motives = this.form.get('motivesControl')!.value;
        this.model.resolutionData.zann = this.form.get('zannControl')!.value;
        this.model.resolutionData.zra = this.form.get('zraControl')!.value;
        this.model.resolutionData.materialEvidence = this.form.get('materialEvidenceControl')!.value;

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