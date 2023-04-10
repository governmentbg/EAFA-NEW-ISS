import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { forkJoin } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IAuanRegisterService } from '@app/interfaces/administration-app/auan-register.interface';
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
import { AddressTypesEnum } from '@app/enums/address-types.enum';
import { AuanObjectionResolutionTypesEnum } from '@app/enums/auan-objection-resolution-types.enum';
import { AuanDeliveryDataDTO } from '@app/models/generated/dtos/AuanDeliveryDataDTO';
import { ErrorSnackbarComponent } from '@app/shared/components/error-snackbar/error-snackbar.component';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { InspDeliveryConfirmationTypesEnum } from '@app/enums/insp-delivery-confirmation-types.enum';
import { InspDeliveryTypesEnum } from '@app/enums/insp-delivery-types.enum';
import { EditAuanDialogParams } from '../models/edit-auan-dialog-params.model';
import { InspDeliveryTypesNomenclatureDTO } from '@app/models/generated/dtos/InspDeliveryTypesNomenclatureDTO';
import { InspDeliveryTypeGroupsEnum } from '@app/enums/insp-delivery-type-groups.enum';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';

@Component({
    selector: 'edit-auan',
    templateUrl: './edit-auan.component.html'
})
export class EditAuanComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;
    public auanId!: number | undefined;

    public get witnesses(): FormArray {
        return this.form.get('witnessesControl') as FormArray;
    }

    public readonly service!: IAuanRegisterService;
    public readonly pageCode: PageCodeEnum = PageCodeEnum.AuanRegister;
    public readonly companyHeadquartersType: AddressTypesEnum = AddressTypesEnum.COMPANY_HEADQUARTERS;
    public readonly correspondenceAddressType: AddressTypesEnum = AddressTypesEnum.CORRESPONDENCE;
    public readonly deliveryTypesEnum: typeof InspDeliveryTypesEnum = InspDeliveryTypesEnum;
    public readonly confirmationTypesEnum: typeof InspDeliveryConfirmationTypesEnum = InspDeliveryConfirmationTypesEnum;
    public readonly today: Date = new Date();

    public inspectionTypes: NomenclatureDTO<number>[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];
    public deliveryTypes: NomenclatureDTO<number>[] = [];
    public deliveryConfirmationTypes: NomenclatureDTO<number>[] = [];
    public drafters: NomenclatureDTO<number>[] = [];
    public objectionResolutionTypes: NomenclatureDTO<AuanObjectionResolutionTypesEnum>[] = [];

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

    public auanNumErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.auanNumErrorLabelText.bind(this);

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private inspectionId!: number;
    private model!: AuanRegisterEditDTO;

    private readonly nomenclatures: CommonNomenclatures;
    private readonly translate: FuseTranslationLoaderService;
    private readonly snackbar: MatSnackBar;

    public constructor(
        service: AuanRegisterService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        snackbar: MatSnackBar
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.snackbar = snackbar;

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

        const nomenclatures: (NomenclatureDTO<number> | InspDeliveryTypesNomenclatureDTO)[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.InspectionTypes, this.nomenclatures.getInspectionTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.InspDeliveryTypes, this.service.getAuanDeliveryTypes.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.InspDeliveryConfirmationTypes, this.service.getAuanDeliveryConfirmationTypes.bind(this.service), false),
            this.service.getInspectionDrafters(this.inspectionId)
        ).toPromise();

        this.inspectionTypes = nomenclatures[0];
        this.territoryUnits = nomenclatures[1];
        this.deliveryTypes = (nomenclatures[2] as InspDeliveryTypesNomenclatureDTO[]).filter(x => x.group === InspDeliveryTypeGroupsEnum.AUAN);
        this.deliveryConfirmationTypes = (nomenclatures[3] as InspDeliveryTypesNomenclatureDTO[]).filter(x => x.group === InspDeliveryTypeGroupsEnum.AUAN);
        this.drafters = nomenclatures[4];

        this.service.getAuanReportData(this.inspectionId).subscribe({
            next: (data: AuanReportDataDTO) => {
                this.isInspector = true;

                this.fillReportData(data);
                if (this.auanId === undefined || this.auanId === null) {
                    this.model = new AuanRegisterEditDTO();
                }
                else {
                    this.form.get('inspectedEntityControl')!.clearValidators();
                    this.form.get('inspectedEntityControl')!.updateValueAndValidity({ emitEvent: false });

                    this.form.get('drafterControl')!.disable();
                    this.form.get('territoryUnitControl')!.disable();

                    this.service.getAuan(this.auanId).subscribe({
                        next: (auan: AuanRegisterEditDTO) => {
                            this.model = auan;

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
        if (this.isAdding) {
            this.form.get('inspectedEntityControl')!.valueChanges.subscribe({
                next: (entity: AuanInspectedEntityDTO | undefined) => {
                    this.inspectedEntity = entity;

                    if (this.inspectedEntity !== null && this.inspectedEntity !== undefined) {
                        this.form.get('inspectedEntityBasicInfoControl')!.setValue(this.inspectedEntity);
                    }
                }
            });
        }

        this.form.get('deliveryTypeControl')!.valueChanges.subscribe({
            next: (type: NomenclatureDTO<number> | undefined) => {
                this.form.get('deliveryDateControl')!.clearValidators();
                this.form.get('deliveryTerritoryUnitControl')!.clearValidators();
                this.form.get('stateServiceControl')!.clearValidators();
                this.form.get('deliveryAddressControl')!.clearValidators();
                this.form.get('sentDateControl')!.clearValidators();
                this.form.get('refusalDateControl')!.clearValidators();
                this.form.get('refusalWitnessesControl')!.clearValidators();

                if (type !== undefined && type !== null) {
                    this.deliveryType = InspDeliveryTypesEnum[type.code as keyof typeof InspDeliveryTypesEnum];

                    switch (this.deliveryType) {
                        case InspDeliveryTypesEnum.Personal:
                            this.form.get('deliveryDateControl')!.setValidators(Validators.required);
                            break;
                        case InspDeliveryTypesEnum.Office:
                            this.form.get('deliveryTerritoryUnitControl')!.setValidators(Validators.required);
                            break;
                        case InspDeliveryTypesEnum.StateService:
                            this.form.get('stateServiceControl')!.setValidators([Validators.required, Validators.maxLength(200)]);
                            break;
                        case InspDeliveryTypesEnum.ByMail:
                            this.form.get('deliveryAddressControl')!.setValidators(Validators.required);
                            this.form.get('sentDateControl')!.setValidators(Validators.required);
                            break;
                        case InspDeliveryTypesEnum.Refusal:
                            this.form.get('refusalDateControl')!.setValidators(Validators.required);
                            this.form.get('refusalWitnessesControl')!.setValidators(Validators.required);
                            break;
                    }

                    this.form.get('deliveryTerritoryUnitControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('stateServiceControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('deliveryAddressControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('sentDateControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('refusalDateControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('deliveryDateControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('refusalWitnessesControl')!.updateValueAndValidity({ emitEvent: false });
                }
                else {
                    this.deliveryType = undefined;
                }
            }
        });

        this.form.get('hasObjectionControl')!.valueChanges.subscribe({
            next: (yes: boolean) => {
                this.form.get('objectionDateControl')!.clearValidators();
                this.form.get('objectionResolutionTypeControl')!.clearValidators();

                if (yes) {
                    this.form.get('objectionDateControl')!.setValidators(Validators.required);
                    this.form.get('objectionResolutionTypeControl')!.setValidators(Validators.required);
                }
                else {
                    this.form.get('objectionResolutionDateControl')!.clearValidators();
                    this.form.get('objectionResolutionNumControl')!.clearValidators();

                    this.form.get('objectionResolutionDateControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('objectionResolutionNumControl')!.updateValueAndValidity({ emitEvent: false });
                }

                this.form.get('objectionDateControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('objectionResolutionTypeControl')!.updateValueAndValidity({ emitEvent: false });
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
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

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
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
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
                if (this.form.valid) {
                    this.fillModel();
                    CommonUtils.sanitizeModelStrings(this.model);

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
            reportNumControl: new FormControl({ value: null, disabled: true }),
            drafterControl: new FormControl(null, Validators.required),
            drafterNameControl: new FormControl({ value: null, disabled: true }),
            inspectionTypeControl: new FormControl({ value: null, disabled: true }),
            territoryUnitControl: new FormControl(null),

            auanNumControl: new FormControl(null, [Validators.required, Validators.maxLength(20)]),
            draftDateControl: new FormControl(null, Validators.required),
            locationDescriptionControl: new FormControl(null, [Validators.required, Validators.maxLength(400)]),

            inspectedEntityControl: new FormControl(null),
            inspectedEntityBasicInfoControl: new FormControl(null),

            witnessesControl: new FormControl(null, Validators.required),

            constatationCommentsControl: new FormControl(null, Validators.maxLength(4000)),
            offenderCommentsControl: new FormControl(null, Validators.maxLength(4000)),

            deliveryTypeControl: new FormControl(null),
            deliveryTerritoryUnitControl: new FormControl(null),
            stateServiceControl: new FormControl(null),
            referenceNumControl: new FormControl(null, Validators.maxLength(500)),
            deliveryAddressControl: new FormControl(null),
            isEDeliveryRequestedControl: new FormControl(null),

            confirmationTypeControl: new FormControl(null),
            deliveryDateControl: new FormControl(null),
            refusalDateControl: new FormControl(null),
            refusalWitnessesControl: new FormControl(null),

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
        }, this.violatedRegulationsValidator());
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
            this.form.get('drafterControl')!.setValue(this.drafters.find(x => x.value === this.model.inspectorId));
        }
        else {
            this.form.get('drafterNameControl')!.setValue(this.model.inspectorName);
        }

        this.form.get('constatationCommentsControl')!.setValue(this.model.constatationComments);
        this.form.get('offenderCommentsControl')!.setValue(this.model.offenderComments);

        const delivery: AuanDeliveryDataDTO | undefined = this.model.deliveryData;
        if (delivery !== undefined && delivery !== null) {
            const type: InspDeliveryTypesEnum | undefined = delivery.deliveryType;
            this.form.get('isEDeliveryRequestedControl')!.setValue(delivery.isEDeliveryRequested);

            if (type !== undefined && type !== null) {
                this.form.get('deliveryTypeControl')!.setValue(this.deliveryTypes.find(x => x.code === InspDeliveryTypesEnum[type]));

                if (type === InspDeliveryTypesEnum.Office) {
                    this.form.get('deliveryTerritoryUnitControl')!.setValue(this.territoryUnits.find(x => x.value === delivery.territoryUnitId));
                }
                else if (type === InspDeliveryTypesEnum.ByMail) {
                    this.form.get('sentDateControl')!.setValue(delivery.sentDate);
                    this.form.get('deliveryAddressControl')!.setValue(delivery.address);
                }
                else if (type === InspDeliveryTypesEnum.StateService) {
                    this.form.get('stateServiceControl')!.setValue(delivery.stateService);
                    this.form.get('referenceNumControl')!.setValue(delivery.referenceNum);
                }
                else if (type === InspDeliveryTypesEnum.Refusal) {
                    this.form.get('refusalDateControl')!.setValue(delivery.refusalDate);
                    this.form.get('refusalWitnessesControl')!.setValue(delivery.refusalWitnesses);
                }
                else if (type === InspDeliveryTypesEnum.Personal) {
                    this.form.get('deliveryDateControl')!.setValue(delivery.deliveryDate);

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

        this.model.auanWitnesses = this.form.get('witnessesControl')!.value;

        this.model.inspectedEntity = this.form.get('inspectedEntityBasicInfoControl')!.value;

        this.model.constatationComments = this.form.get('constatationCommentsControl')!.value;
        this.model.offenderComments = this.form.get('offenderCommentsControl')!.value;

        if (this.isInspector) {
            this.model.inspectorId = this.form.get('drafterControl')!.value?.value;
        }

        const deliveryType: NomenclatureDTO<number> | undefined = this.form.get('deliveryTypeControl')!.value;

        if (deliveryType !== undefined && deliveryType !== null) {
            this.model.deliveryData = new AuanDeliveryDataDTO({
                id: this.model.deliveryData?.id,
                deliveryType: InspDeliveryTypesEnum[deliveryType.code as keyof typeof InspDeliveryTypesEnum],
                isDelivered: false
            });

            this.model.deliveryData.isEDeliveryRequested = this.form.get('isEDeliveryRequestedControl')!.value ?? false;

            if (this.model.deliveryData.deliveryType === InspDeliveryTypesEnum.Office) {
                this.model.deliveryData.territoryUnitId = this.form.get('deliveryTerritoryUnitControl')!.value!.value;
            }
            else if (this.model.deliveryData.deliveryType === InspDeliveryTypesEnum.ByMail) {
                this.model.deliveryData.address = this.form.get('deliveryAddressControl')!.value;
                this.model.deliveryData.sentDate = this.form.get('sentDateControl')!.value;
            }
            else if (this.model.deliveryData.deliveryType === InspDeliveryTypesEnum.StateService) {
                this.model.deliveryData.stateService = this.form.get('stateServiceControl')!.value;
                this.model.deliveryData.referenceNum = this.form.get('referenceNumControl')!.value;
            }
            else if (this.model.deliveryData.deliveryType === InspDeliveryTypesEnum.Refusal) {
                this.model.deliveryData.refusalDate = this.form.get('refusalDateControl')!.value;
                this.model.deliveryData.refusalWitnesses = this.form.get('refusalWitnessesControl')!.value;
            }
            else if (this.model.deliveryData.deliveryType === InspDeliveryTypesEnum.Personal) {
                this.model.deliveryData.deliveryDate = this.form.get('deliveryDateControl')!.value;

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
            this.form.get('deliveryTypeControl')!.setErrors({ 'hasNoEDeliveryRegistrationError': true });
            this.form.get('deliveryTypeControl')!.markAsTouched();
            this.validityCheckerGroup.validate();
        }

        if (response.error?.code === ErrorCode.AuanNumAlreadyExists) {
            this.form.get('auanNumControl')!.setErrors({ auanNumExists: true });
            this.form.get('auanNumControl')!.markAsTouched();
            this.validityCheckerGroup.validate();
        }
    }
}