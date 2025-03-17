import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { forkJoin, Observable } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { AuanRegisterEditDTO } from '@app/models/generated/dtos/AuanRegisterEditDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { AuanReportDataDTO } from '@app/models/generated/dtos/AuanReportDataDTO';
import { AuanRegisterService } from '@app/services/administration-app/auan-register.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { AuanInspectedEntityDTO } from '@app/models/generated/dtos/AuanInspectedEntityDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { AuanViolatedRegulationDTO } from '@app/models/generated/dtos/AuanViolatedRegulationDTO';
import { AuanObjectionResolutionTypesEnum } from '@app/enums/auan-objection-resolution-types.enum';
import { AuanDeliveryDataDTO } from '@app/models/generated/dtos/AuanDeliveryDataDTO';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { InspDeliveryConfirmationTypesEnum } from '@app/enums/insp-delivery-confirmation-types.enum';
import { InspDeliveryTypesEnum } from '@app/enums/insp-delivery-types.enum';
import { EditAuanDialogParams } from '../models/edit-auan-dialog-params.model';
import { InspDeliveryTypesNomenclatureDTO } from '@app/models/generated/dtos/InspDeliveryTypesNomenclatureDTO';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { AuanStatusEnum } from '@app/enums/auan-status.enum';
import { AuanWitnessDTO } from '@app/models/generated/dtos/AuanWitnessDTO';
import { AuanDrafterNomenclatureDTO } from '@app/models/generated/dtos/AuanDrafterNomenclatureDTO';
import { AddressRegistrationDTO } from '@app/models/generated/dtos/AddressRegistrationDTO';
import { SystemParametersService } from '@app/services/common-app/system-parameters.service';
import { SystemPropertiesDTO } from '@app/models/generated/dtos/SystemPropertiesDTO';
import { DateUtils } from '@app/shared/utils/date.utils';
import { DateDifference } from '@app/models/common/date-difference.model';
import { Moment } from 'moment';
import moment from 'moment';

@Component({
    selector: 'edit-auan',
    templateUrl: './edit-auan.component.html'
})
export class EditAuanComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;
    public auanId!: number | undefined;

    public readonly deliveryTypesEnum: typeof InspDeliveryTypesEnum = InspDeliveryTypesEnum;
    public readonly confirmationTypesEnum: typeof InspDeliveryConfirmationTypesEnum = InspDeliveryConfirmationTypesEnum;
    public readonly service!: AuanRegisterService;
    public readonly pageCode: PageCodeEnum = PageCodeEnum.AuanRegister;
    public readonly today: Date = new Date();

    public inspectionTypes: NomenclatureDTO<number>[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];
    public objectionResolutionTypes: NomenclatureDTO<AuanObjectionResolutionTypesEnum>[] = [];

    public drafters: AuanDrafterNomenclatureDTO[] = [];
    public allDrafters: AuanDrafterNomenclatureDTO[] = [];

    public inspectedEntities: AuanInspectedEntityDTO[] = [];
    public violatedRegulations: AuanViolatedRegulationDTO[] = [];

    public inspectedEntity: AuanInspectedEntityDTO | undefined;
    public deliveryType: InspDeliveryTypesEnum | undefined;
    public confirmationType: InspDeliveryConfirmationTypesEnum | undefined;

    public isAdding: boolean = false;
    public viewMode: boolean = false;
    public isInspector: boolean = false;
    public isFromThirdPartyInspection: boolean = false;
    public violatedRegulationsTouched: boolean = false;
    public showInspectedEntity: boolean = false;
    public canAddInspectedEntity: boolean = false;
    public isFromInspection: boolean = true;
    public canSaveAfterHours: boolean = false;
    public canAddAuanAfterHours: number | undefined;
    public minDate: Date | undefined;

    public auanNumErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.auanNumErrorLabelText.bind(this);

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private inspectionId!: number;
    private model!: AuanRegisterEditDTO;

    private readonly nomenclatures: CommonNomenclatures;
    private readonly translate: FuseTranslationLoaderService;
    private readonly systemParametersService: SystemParametersService;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly snackbar: TLSnackbar;

    public constructor(
        service: AuanRegisterService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        systemParametersService: SystemParametersService,
        confirmDialog: TLConfirmDialog,
        snackbar: TLSnackbar
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.snackbar = snackbar;
        this.systemParametersService = systemParametersService;

        this.buildForm();

        this.objectionResolutionTypes = [
            new NomenclatureDTO<AuanObjectionResolutionTypesEnum>({
                value: AuanObjectionResolutionTypesEnum.Cancelled,
                displayName: this.translate.getValue('auan-register.objection-resolution-cancelled'),
                isActive: true
            }),
            new NomenclatureDTO<AuanObjectionResolutionTypesEnum>({
                value: AuanObjectionResolutionTypesEnum.PartiallyCancelled,
                displayName: this.translate.getValue('auan-register.objection-resolution-partially-cancelled'),
                isActive: true
            })
        ];
    }

    public async ngOnInit(): Promise<void> {
        this.isAdding = this.auanId === undefined || this.auanId === null;
        this.showInspectedEntity = !this.isAdding;

        const nomenclatures: (NomenclatureDTO<number> | InspDeliveryTypesNomenclatureDTO | AuanDrafterNomenclatureDTO)[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.InspectionTypes, this.nomenclatures.getInspectionTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false),
            this.service.getAllDrafters()
        ).toPromise();

        this.inspectionTypes = nomenclatures[0];
        this.territoryUnits = nomenclatures[1];
        this.drafters = nomenclatures[2];
        this.allDrafters = nomenclatures[2];

        const systemParameters: SystemPropertiesDTO = await this.systemParametersService.systemParameters();
        this.canAddAuanAfterHours = systemParameters.addAuanAfterHours;

        this.service.getAuanReportData(this.inspectionId).subscribe({
            next: (data: AuanReportDataDTO) => {
                this.isInspector = true;
                this.fillReportData(data);

                if (this.auanId === undefined || this.auanId === null) {
                    this.model = new AuanRegisterEditDTO();

                    if (this.drafters !== undefined && this.drafters !== null && this.drafters.length === 1) {
                        this.form.get('drafterControl')!.setValue(this.drafters[0]);
                    }
                }
                else {
                    this.form.get('inspectedEntityControl')!.clearValidators();
                    this.form.get('inspectedEntityControl')!.updateValueAndValidity({ emitEvent: false });

                    this.service.getAuan(this.auanId).subscribe({
                        next: (auan: AuanRegisterEditDTO) => {
                            this.model = auan;

                            if (auan.status !== AuanStatusEnum.Draft) {
                                this.form.get('drafterControl')!.disable();
                                this.form.get('territoryUnitControl')!.disable();
                            }
                            else if (auan.inspectedEntity === undefined || auan.inspectedEntity === null) {
                                this.canAddInspectedEntity = true;
                                this.form.get('inspectedEntityControl')!.setValidators(Validators.required);
                                this.form.get('inspectedEntityControl')!.updateValueAndValidity({ emitEvent: false });
                            }

                            if (auan.inspectorName !== undefined && auan.inspectorName !== null) {
                                this.isInspector = false;
                                this.form.get('drafterControl')!.clearValidators();
                                this.form.get('drafterControl')!.updateValueAndValidity({ emitEvent: false });
                            }

                            this.fillForm();
                        }
                    });
                }
            }
        });
    }

    public ngAfterViewInit(): void {
        if (!this.viewMode) {
            this.form.get('territoryUnitControl')!.valueChanges.subscribe({
                next: (value: NomenclatureDTO<number> | undefined) => {
                    if (value !== undefined && value !== null && typeof value !== 'string') {
                        this.drafters = this.allDrafters.filter(x => x.territoryUnitId === value.value);
                    }
                    else {
                        this.drafters = this.allDrafters;
                    }

                    this.drafters = this.drafters.slice();
                }
            });

            this.form.get('drafterControl')!.valueChanges.subscribe({
                next: (value: AuanDrafterNomenclatureDTO | undefined) => {
                    if (value !== undefined && value !== null && value.territoryUnitId !== undefined && value.territoryUnitId !== null) {
                        const territoryUnit: NomenclatureDTO<number> | undefined = this.form.get('territoryUnitControl')!.value;

                        if (territoryUnit === undefined || territoryUnit === null) {
                            this.form.get('territoryUnitControl')!.setValue(this.territoryUnits.find(x => x.value === value.territoryUnitId));
                        }
                    }
                }
            });

            this.form.get('inspectedEntityControl')!.valueChanges.subscribe({
                next: (entity: AuanInspectedEntityDTO | undefined) => {
                    this.inspectedEntity = entity;

                    if (this.inspectedEntity !== null && this.inspectedEntity !== undefined) {
                        this.showInspectedEntity = true;
                        this.form.get('inspectedEntityBasicInfoControl')!.setValue(this.inspectedEntity);
                    }
                    else {
                        this.showInspectedEntity = false;
                    }
                }
            });

            if (!this.isFromThirdPartyInspection) {
                this.form.get('isInspectedEntityFromInspectionControl')!.valueChanges.subscribe({
                    next: (value: boolean | undefined) => {
                        this.isFromInspection = false;
                    
                        if (value === true) {
                            this.isFromInspection = true;
                            this.form.get('inspectedEntityControl')!.setValidators(Validators.required);
                            this.form.get('inspectedEntityControl')!.setValue(this.inspectedEntities.find(x => x === this.inspectedEntity));
                        }
                        else {
                            this.showInspectedEntity = true;
                            this.form.get('inspectedEntityControl')!.clearValidators();
                            this.form.get('inspectedEntityBasicInfoControl')!.setValue(null);
                        }

                        this.form.get('inspectedEntityControl')!.updateValueAndValidity({ emitEvent: false });
                        this.form.get('inspectedEntityBasicInfoControl')!.updateValueAndValidity({ emitEvent: false });

                        this.form.get('inspectedEntityControl')!.markAsPending({ emitEvent: false });
                        this.form.get('inspectedEntityBasicInfoControl')!.markAsPending({ emitEvent: false });
                    }
                });
            }
        }

        this.form.get('auanDeliveryDataControl')!.valueChanges.subscribe({
            next: (delivery: AuanDeliveryDataDTO) => {
                if (delivery !== undefined && delivery !== null) {
                    this.deliveryType = delivery.deliveryType;
                }

                this.form.get('hasObjectionControl')!.setValue(false);
                this.form.get('hasObjectionControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.form.get('hasObjectionControl')!.valueChanges.subscribe({
            next: (yes: boolean) => {
                this.form.get('objectionDateControl')!.clearValidators();
                this.form.get('objectionResolutionTypeControl')!.clearValidators();
                this.form.get('objectionResolutionDateControl')!.clearValidators();
                this.form.get('objectionResolutionNumControl')!.clearValidators();
                this.form.get('objectionResolutionTypeControl')!.setValue(undefined);

                if (yes) {
                    this.form.get('objectionDateControl')!.setValidators(Validators.required);
                    this.form.get('objectionResolutionTypeControl')!.setValidators(Validators.required);
                }

                this.form.get('objectionDateControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('objectionResolutionTypeControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('objectionResolutionDateControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('objectionResolutionNumControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.form.get('objectionResolutionTypeControl')!.valueChanges.subscribe({
            next: (type: NomenclatureDTO<AuanObjectionResolutionTypesEnum> | undefined) => {
                this.form.get('objectionResolutionDateControl')!.clearValidators();
                this.form.get('objectionResolutionNumControl')!.clearValidators();

                if (type !== null && type !== undefined) {
                    this.form.get('objectionResolutionDateControl')!.setValidators(Validators.required);
                    this.form.get('objectionResolutionNumControl')!.setValidators([Validators.required, Validators.maxLength(50)]);
                }

                this.form.get('objectionResolutionDateControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('objectionResolutionNumControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.form.get('violatedRegulationsControl')!.valueChanges.subscribe({
            next: (result: AuanViolatedRegulationDTO[] | undefined) => {
                if (result !== undefined && result !== null) {
                    this.violatedRegulations = result;
                    this.violatedRegulationsTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                }
            }
        });
    }

    public setData(data: EditAuanDialogParams | undefined, wrapperData: DialogWrapperData): void {
        if (data !== undefined && data !== null) {
            this.inspectionId = data.inspectionId;
            this.auanId = data.id;
            this.isFromThirdPartyInspection = data.isThirdParty;
            this.canSaveAfterHours = data.canSaveAfterHours;
            this.viewMode = data.isReadonly ?? false;

            if (!this.isFromThirdPartyInspection) {
                this.form.get('inspectedEntityControl')!.setValidators(Validators.required);
                this.form.get('inspectedEntityControl')!.updateValueAndValidity({ emitEvent: false });
            }
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.viewMode) {
            dialogClose();
        }

        this.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.openConfirmDialog().subscribe({
                next: (ok: boolean) => {
                    if (ok) {
                        this.fillModel();
                        this.model.status = AuanStatusEnum.Submitted;
                        CommonUtils.sanitizeModelStrings(this.model);

                        this.saveAuan(dialogClose);
                    }
                }
            });
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (action.id === 'save-draft') {
            this.markDraftOrCancelAsTouched();
            this.validityCheckerGroup.validate();

            if (this.isDraftOrCancelValid()) {
                this.fillModel();
                this.model.status = AuanStatusEnum.Draft;
                CommonUtils.sanitizeModelStrings(this.model);

                this.saveAuan(dialogClose);
            }
        }

        if (action.id === 'cancel-auan') {
            this.markDraftOrCancelAsTouched();

            if (this.isDraftOrCancelValid() || this.viewMode) {
                this.fillModel();
                this.model.status = AuanStatusEnum.Canceled;
                CommonUtils.sanitizeModelStrings(this.model);

                this.confirmDialog.open({
                    title: this.translate.getValue('auan-register.cancel-auan-confirm-dialog-title'),
                    message: this.translate.getValue('auan-register.cancel-auan-confirm-dialog-message'),
                    okBtnLabel: this.translate.getValue('auan-register.cancel-auan-confirm-dialog-ok-btn-label')
                }).subscribe({
                    next: (ok: boolean) => {
                        if (ok) {
                            this.saveAuan(dialogClose);
                        }
                    }
                });
            }
        }

        if (this.auanId !== undefined && this.auanId !== null) {
            if (action.id === 'more-corrections-needed' || action.id === 'activate-auan') {
                this.updateAuanStatus(AuanStatusEnum.Draft, dialogClose);
            }
        }

        if (action.id === 'print') {
            if (this.viewMode) {
                this.service.downloadAuan(this.auanId!).subscribe({
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
                    this.model.status = AuanStatusEnum.Submitted;
                    CommonUtils.sanitizeModelStrings(this.model);

                    this.openConfirmDialog().subscribe({
                        next: (ok: boolean) => {
                            if (ok) {
                                if (this.auanId !== undefined && this.auanId !== null) {
                                    this.service.editAuan(this.model).subscribe({
                                        next: () => {
                                            this.service.downloadAuan(this.auanId!).subscribe({
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
                                    this.service.addAuan(this.model).subscribe({
                                        next: (id: number) => {
                                            this.model.id = id;

                                            this.service.downloadAuan(id).subscribe({
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

    public auanNumErrorLabelText(controlName: string, error: unknown, errorCode: string): TLError | undefined {
        if (controlName === 'auanNumControl') {
            if (errorCode === 'auanNumExists' && error === true) {
                return new TLError({ type: 'error', text: this.translate.getValue('auan-register.auan-num-already-exist-error') });
            }
        }
        return undefined;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            reportNumControl: new FormControl(),
            drafterControl: new FormControl(null, Validators.required),
            drafterNameControl: new FormControl({ value: null, disabled: true }),
            inspectionTypeControl: new FormControl(),
            territoryUnitControl: new FormControl(null),

            auanNumControl: new FormControl(null, [Validators.required, Validators.maxLength(20)]),
            draftDateControl: new FormControl(null, [Validators.required, this.cannotAddAfterHours()]),
            locationDescriptionControl: new FormControl(null, [Validators.required, Validators.maxLength(400)]),

            inspectedEntityControl: new FormControl(null),
            inspectedEntityBasicInfoControl: new FormControl(null, Validators.required),
            isInspectedEntityFromInspectionControl: new FormControl(true),

            witnessesControl: new FormControl(null, Validators.required),

            constatationCommentsControl: new FormControl(null, Validators.maxLength(4000)),
            offenderCommentsControl: new FormControl(null, Validators.maxLength(4000)),

            auanDeliveryDataControl: new FormControl(null),

            hasObjectionControl: new FormControl(false),
            objectionDateControl: new FormControl(null),
            objectionResolutionTypeControl: new FormControl(null),
            objectionResolutionDateControl: new FormControl(null),
            objectionResolutionNumControl: new FormControl(null),
            sentDateControl: new FormControl(null),
            confiscatedApplianceControl: new FormControl(null),
            confiscatedFishControl: new FormControl(null),
            confiscatedFishingGearControl: new FormControl(null),
            violatedRegulationsControl: new FormControl(null),

            filesControl: new FormControl(null)
        }, [this.violatedRegulationsValidator(), this.violatedRegulationFeldsValidator()]);
    }

    private fillForm(): void {
        this.form.get('auanNumControl')!.setValue(this.model.auanNum);
        this.form.get('draftDateControl')!.setValue(this.model.draftDate);
        this.form.get('locationDescriptionControl')!.setValue(this.model.locationDescription);
        this.form.get('territoryUnitControl')!.setValue(this.territoryUnits.find(x => x.value === this.model.territoryUnitId));

        this.form.get('witnessesControl')!.setValue(this.model.auanWitnesses);

        if (this.model.inspectedEntity !== undefined && this.model.inspectedEntity !== null) {
            this.inspectedEntity = this.model.inspectedEntity;
            this.form.get('inspectedEntityBasicInfoControl')!.setValue(this.model.inspectedEntity);
        }

        if (this.isInspector) {
            this.form.get('drafterControl')!.setValue(this.allDrafters.find(x => x.value === this.model.inspectorId));
        }
        else {
            this.form.get('drafterNameControl')!.setValue(this.model.inspectorName);
        }

        this.form.get('constatationCommentsControl')!.setValue(this.model.constatationComments);
        this.form.get('offenderCommentsControl')!.setValue(this.model.offenderComments);

        const delivery: AuanDeliveryDataDTO | undefined = this.model.deliveryData;
        this.form.get('auanDeliveryDataControl')!.setValue(delivery);

        if (delivery !== undefined && delivery !== null) {
            const type: InspDeliveryTypesEnum | undefined = delivery.deliveryType;

            if (type !== undefined && type !== null && type === InspDeliveryTypesEnum.Personal) {
                this.form.get('hasObjectionControl')!.setValue(this.model.hasObjection);
                if (this.model.hasObjection === true) {
                    this.form.get('objectionDateControl')!.setValue(this.model.objectionDate);

                    if (this.model.resolutionType !== undefined && this.model.resolutionType !== null) {
                        this.form.get('objectionResolutionTypeControl')!.setValue(this.objectionResolutionTypes.find(x => {
                            return x.value === this.model.resolutionType;
                        }));

                        this.form.get('objectionResolutionDateControl')!.setValue(this.model.resolutionDate);
                        this.form.get('objectionResolutionNumControl')!.setValue(this.model.resolutionNum);
                    }
                }
            }
        }

        this.form.get('confiscatedApplianceControl')!.setValue(this.model.confiscatedAppliance);
        this.form.get('confiscatedFishControl')!.setValue(this.model.confiscatedFish);
        this.form.get('confiscatedFishingGearControl')!.setValue(this.model.confiscatedFishingGear);
        this.form.get('violatedRegulationsControl')!.setValue(this.model.violatedRegulations);

        this.form.get('filesControl')!.setValue(this.model.files);

        if (this.viewMode) {
            this.form.disable();
        }
    }

    private fillModel(): void {
        this.model.inspectionId = this.inspectionId;
        this.model.auanNum = this.form.get('auanNumControl')!.value;
        this.model.draftDate = this.form.get('draftDateControl')!.value;
        this.model.locationDescription = this.form.get('locationDescriptionControl')!.value;
        this.model.territoryUnitId = this.form.get('territoryUnitControl')!.value?.value;

        const witnesses: AuanWitnessDTO[] = this.form.get('witnessesControl')!.value;

        this.model.auanWitnesses = witnesses.filter(x =>
            !CommonUtils.isNullOrWhiteSpace(x.witnessNames)
            && !CommonUtils.isNullOrEmpty(x.dateOfBirth)
            && !CommonUtils.isNullOrEmpty(x.address?.countryId)
            && !CommonUtils.isNullOrWhiteSpace(x.address?.street));

        const isInspectedEntityValid: boolean = this.form.get('inspectedEntityBasicInfoControl')!.valid === true;
        if (isInspectedEntityValid) {
            this.model.inspectedEntity = this.form.get('inspectedEntityBasicInfoControl')!.value;
        }
        else {
            this.model.inspectedEntity = undefined;
        }

        this.model.constatationComments = this.form.get('constatationCommentsControl')!.value;
        this.model.offenderComments = this.form.get('offenderCommentsControl')!.value;

        if (this.isInspector) {
            this.model.inspectorId = this.form.get('drafterControl')!.value?.value;
        }

        const deliveryData: AuanDeliveryDataDTO = this.form.get('auanDeliveryDataControl')!.value;
        if (deliveryData !== undefined && deliveryData !== null) {
            const type: InspDeliveryTypesEnum | undefined = deliveryData.deliveryType;
            this.model.deliveryData = deliveryData;

            if (type !== undefined && type !== null && type === InspDeliveryTypesEnum.Personal) {
                this.model.hasObjection = this.form.get('hasObjectionControl')!.value ?? false;

                if (this.model.hasObjection === true) {
                    this.model.objectionDate = this.form.get('objectionDateControl')!.value;
                    this.model.resolutionType = this.form.get('objectionResolutionTypeControl')!.value?.value;

                    if (this.model.resolutionType !== undefined && this.model.resolutionType !== null) {
                        this.model.resolutionDate = this.form.get('objectionResolutionDateControl')!.value;
                        this.model.resolutionNum = this.form.get('objectionResolutionNumControl')!.value;
                    }
                }
            }
        }

        this.model.confiscatedAppliance = this.form.get('confiscatedApplianceControl')!.value;
        this.model.confiscatedFish = this.form.get('confiscatedFishControl')!.value;
        this.model.confiscatedFishingGear = this.form.get('confiscatedFishingGearControl')!.value;
        this.model.violatedRegulations = this.form.get('violatedRegulationsControl')!.value;

        this.model.files = this.form.get('filesControl')!.value;
    }

    private fillReportData(data: AuanReportDataDTO): void {
        this.form.get('reportNumControl')!.setValue(data.reportNum);
        this.form.get('inspectionTypeControl')!.setValue(this.inspectionTypes.find(x => x.value === data.inspectionTypeId));
        this.form.get('territoryUnitControl')!.setValue(this.territoryUnits.find(x => x.value === data.territoryUnitId));

        this.inspectedEntities = data.inspectedEntities ?? [];
        this.patchInspectedEntitiesNomenclature();

        if (this.isAdding) {
            setTimeout(() => {
                this.form.get('violatedRegulationsControl')!.setValue(data.violatedRegulations);
            });
        }
    }

    private patchInspectedEntitiesNomenclature(): void {
        for (const entity of this.inspectedEntities) {
            entity.isActive = true;

            if (entity.isUnregisteredPerson === true) {
                entity.isUnregisteredPerson = undefined;

                entity.displayName = entity.unregisteredPerson!.firstName;
                if (entity.unregisteredPerson!.middleName !== undefined && entity.unregisteredPerson!.middleName !== null) {
                    entity.displayName = `${entity.displayName} ${entity.unregisteredPerson!.middleName}`;
                }

                if (entity.unregisteredPerson!.lastName !== undefined && entity.unregisteredPerson!.lastName !== null) {
                    entity.displayName = `${entity.displayName} ${entity.unregisteredPerson!.lastName}`;
                }

                entity.description = entity.unregisteredPerson!.egnLnc?.egnLnc;
            }
            else {
                if (entity.isPerson === true) {
                    entity.displayName = entity.person!.firstName;
                    if (entity.person!.middleName !== undefined && entity.person!.middleName !== null) {
                        entity.displayName = `${entity.displayName} ${entity.person!.middleName}`;
                    }
                    entity.displayName = `${entity.displayName} ${entity.person!.lastName}`;

                    entity.description = entity.person!.egnLnc!.egnLnc;
                }
                else if (entity.isPerson === false) {
                    entity.displayName = entity.legal!.name;
                    entity.description = entity.legal!.eik;
                }
            }
        }
    }

    private markAllAsTouched(): void {
        this.form.markAllAsTouched();
        this.violatedRegulationsTouched = true;
    }

    //В статуси "Чернова" и "Анулиран" са задължителни само полетата "Номер на АУАН", "Дата на съставяне" и "Актосъставител"
    private markDraftOrCancelAsTouched(): void {
        this.violatedRegulationsTouched = false;

        this.form.get('auanNumControl')!.markAsTouched();
        this.form.get('drafterControl')!.markAsTouched();
        this.form.get('draftDateControl')!.markAsTouched();

        if (this.violatedRegulations !== undefined && this.violatedRegulations !== null && this.violatedRegulations.some(x => x.hasErrors)) {
            this.violatedRegulationsTouched = true;
        }

        if (!this.inspectedEntityAddressesValid()) {
            this.form.get('inspectedEntityBasicInfoControl')!.markAsTouched();
        }
    }

    private isDraftOrCancelValid(): boolean {
        if (this.violatedRegulations !== undefined && this.violatedRegulations !== null && this.violatedRegulations.some(x => x.hasErrors)) {
            return false;
        }

        return (this.form.get('auanNumControl')!.valid
            && this.form.get('drafterControl')!.valid
            && this.form.get('draftDateControl')!.valid);
    }

    private inspectedEntityAddressesValid(): boolean {
        const inspectedEntity: AuanInspectedEntityDTO | undefined = this.form.get('inspectedEntityBasicInfoControl')!.value;

        if (inspectedEntity !== undefined && inspectedEntity !== null) {
            if (inspectedEntity.addresses !== undefined && inspectedEntity.addresses !== null && inspectedEntity.addresses.length > 0) {
                const invalidAddresses: AddressRegistrationDTO[] = inspectedEntity.addresses.filter(x => {
                    if (CommonUtils.isNullOrEmpty(x.countryId)) {
                        return true;
                    }

                    if (CommonUtils.isNullOrWhiteSpace(x.street)) {
                        return true;
                    }

                    return false;
                });

                if (invalidAddresses.length > 0) {
                    return false;
                }
            }
        }

        return true;
    }

    private openConfirmDialog(): Observable<boolean> {
        const message: string = this.auanId !== undefined && this.auanId !== null
            ? this.translate.getValue('auan-register.complete-edit-auan-confirm-dialog-message')
            : this.translate.getValue('auan-register.complete-add-auan-confirm-dialog-message');

        return this.confirmDialog.open({
            title: this.translate.getValue('auan-register.complete-auan-confirm-dialog-title'),
            message: message,
            okBtnLabel: this.translate.getValue('auan-register.complete-auan-confirm-dialog-ok-btn-label')
        });
    }

    private updateAuanStatus(status: AuanStatusEnum, dialogClose: DialogCloseCallback): void {
        this.service.updateAuanStatus(this.auanId!, status).subscribe({
            next: () => {
                dialogClose(this.model);
            }
        });
    }

    private saveAuan(dialogClose: DialogCloseCallback): void {
        if (this.auanId !== undefined && this.auanId !== null) {
            this.service.editAuan(this.model).subscribe({
                next: () => {
                    dialogClose(this.model);
                },
                error: (response: HttpErrorResponse) => {
                    this.handleAddEditErrorResponse(response);
                }
            });
        }
        else {
            this.service.addAuan(this.model).subscribe({
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

    private violatedRegulationsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (!this.violatedRegulations.some(x => x.isActive !== false)) {
                return { 'atLeastOneViolatedRegulationNeeded': true };
            }
            return null;
        }
    }

    private violatedRegulationFeldsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.violatedRegulations === undefined || this.violatedRegulations === null || this.violatedRegulations.length === 0) {
                return null;
            }

            if (this.violatedRegulations.some(x => x.hasErrors)) {
                return { 'invalidViolatedRegulation': true };
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

            if (this.canAddAuanAfterHours === undefined || this.canAddAuanAfterHours === null) {
                return null;
            }

            const startDate: Moment = moment(control.value);
            const now: Date = new Date();
         
            if (startDate === undefined || startDate === null) {
                return null;
            }

            const difference: DateDifference | undefined = DateUtils.getDateDifference((startDate as Moment).toDate(), now);

            if (difference === undefined || difference === null) {
                return null;
            }

            if (difference.minutes === 0 && difference.hours === 0 && difference.days === 0) {
                return null;
            }

            const differenceHours: number = (difference.days! * 24) + difference.hours! + (difference.minutes! / 60);
           
            if (differenceHours > this.canAddAuanAfterHours) {
                return { cannotAddAfterHours: true };
            }

            return null;
        }
    }

    private handleAddEditErrorResponse(response: HttpErrorResponse): void {
        if (response.error?.messages !== null && response.error?.messages !== undefined) {
            const messages: string[] = response.error.messages;

            if (messages.length !== 0) {
                this.snackbar.errorModel(response.error as ErrorModel);
            } else {
                this.snackbar.error(this.translate.getValue('service.an-error-occurred-in-the-app'));
            }
        }

        if (response.error?.code === ErrorCode.NoEDeliveryRegistration) {
            this.form.get('auanDeliveryDataControl')!.setErrors({ 'hasNoEDeliveryRegistrationError': true });
            this.form.get('auanDeliveryDataControl')!.markAsTouched();
            this.validityCheckerGroup.validate();
        }

        if (response.error?.code === ErrorCode.AuanNumAlreadyExists) {
            this.form.get('auanNumControl')!.setErrors({ auanNumExists: true });
            this.form.get('auanNumControl')!.markAsTouched();
            this.validityCheckerGroup.validate();
        }

        if (response.error?.code === ErrorCode.CannotCancelAuanWithPenalDecrees) {
            const errorMessage: string = this.translate.getValue('auan-register.cannot-cancel-auan-with-penal-decrees');
            this.snackbar.error(errorMessage);
        }
    }
}