import { AfterViewInit, Component, Input, OnInit, Optional, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { forkJoin, Subscription } from 'rxjs';
import { InspDeliveryConfirmationTypesEnum } from '@app/enums/insp-delivery-confirmation-types.enum';
import { InspDeliveryTypeGroupsEnum } from '@app/enums/insp-delivery-type-groups.enum';
import { InspDeliveryTypesEnum } from '@app/enums/insp-delivery-types.enum';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { AuanDeliveryDataDTO } from '@app/models/generated/dtos/AuanDeliveryDataDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspDeliveryTypesNomenclatureDTO } from '@app/models/generated/dtos/InspDeliveryTypesNomenclatureDTO';
import { AuanRegisterService } from '@app/services/administration-app/auan-register.service';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { AddressTypesEnum } from '@app/enums/address-types.enum';

@Component({
    selector: 'auan-delivery-data',
    templateUrl: './auan-delivery-data.component.html'
})
export class AuanDeliveryDataComponent extends CustomFormControl<AuanDeliveryDataDTO> implements OnInit, AfterViewInit {
    @Input() public viewMode!: boolean;

    public territoryUnits: NomenclatureDTO<number>[] = [];
    public deliveryTypes: NomenclatureDTO<number>[] = [];
    public deliveryConfirmationTypes: NomenclatureDTO<number>[] = [];

    public readonly deliveryTypesEnum: typeof InspDeliveryTypesEnum = InspDeliveryTypesEnum;
    public readonly confirmationTypesEnum: typeof InspDeliveryConfirmationTypesEnum = InspDeliveryConfirmationTypesEnum;
    public readonly companyHeadquartersType: AddressTypesEnum = AddressTypesEnum.COMPANY_HEADQUARTERS;
    public readonly correspondenceAddressType: AddressTypesEnum = AddressTypesEnum.CORRESPONDENCE;
    public readonly today: Date = new Date();

    public delivery: AuanDeliveryDataDTO | undefined;
    public deliveryType: InspDeliveryTypesEnum | undefined;
    public confirmationType: InspDeliveryConfirmationTypesEnum | undefined;
    public isDelivered: boolean = false;

    private readonly nomenclatures: CommonNomenclatures;
    private readonly service: AuanRegisterService;
    private readonly loader: FormControlDataLoader;

    public constructor(
        @Self() ngControl: NgControl,
        @Self() @Optional() validityChecker: ValidityCheckerDirective,
        service: AuanRegisterService,
        nomenclatures: CommonNomenclatures
    ) {
        super(ngControl, true, validityChecker);

        this.service = service;
        this.nomenclatures = nomenclatures;

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
        this.loader.load();
    }

    public ngAfterViewInit(): void {
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
                }
                else {
                    this.deliveryType = undefined;
                }

                this.form.get('deliveryTerritoryUnitControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('stateServiceControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('deliveryAddressControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('sentDateControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('refusalDateControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('deliveryDateControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('refusalWitnessesControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    public writeValue(value: AuanDeliveryDataDTO): void {
        if (value !== undefined && value !== null) {
            this.delivery = value;

            this.loader.load(() => {
                this.fillForm(value);
            });
        }

        this.fillForm(value);
    }

    protected getValue(): AuanDeliveryDataDTO {
        let delivery: AuanDeliveryDataDTO = new AuanDeliveryDataDTO();

        const deliveryType: NomenclatureDTO<number> | undefined = this.form.get('deliveryTypeControl')!.value;

        if (deliveryType !== undefined && deliveryType !== null) {
            delivery = new AuanDeliveryDataDTO({
                id: this.delivery?.id,
                deliveryType: InspDeliveryTypesEnum[deliveryType.code as keyof typeof InspDeliveryTypesEnum],
                isDelivered: false
            });

            delivery.isEDeliveryRequested = this.form.get('isEDeliveryRequestedControl')!.value ?? false;

            if (delivery.deliveryType === InspDeliveryTypesEnum.Office) {
                delivery.territoryUnitId = this.form.get('deliveryTerritoryUnitControl')!.value?.value;
            }
            else if (delivery.deliveryType === InspDeliveryTypesEnum.ByMail) {
                delivery.address = this.form.get('deliveryAddressControl')!.value;
                delivery.sentDate = this.form.get('sentDateControl')!.value;
            }
            else if (delivery.deliveryType === InspDeliveryTypesEnum.StateService) {
                delivery.stateService = this.form.get('stateServiceControl')!.value;
                delivery.referenceNum = this.form.get('referenceNumControl')!.value;
            }
            else if (delivery.deliveryType === InspDeliveryTypesEnum.Refusal) {
                delivery.refusalDate = this.form.get('refusalDateControl')!.value;
                delivery.refusalWitnesses = this.form.get('refusalWitnessesControl')!.value;
            }
            else if (delivery.deliveryType === InspDeliveryTypesEnum.Personal) {
                delivery.deliveryDate = this.form.get('deliveryDateControl')!.value;
            }
        }

        return delivery;
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
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
            sentDateControl: new FormControl(null)
        });
    }

    private getNomenclatures(): Subscription {
        const subscribtion: Subscription = forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.InspDeliveryTypes, this.service.getDeliveryTypes.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.InspDeliveryConfirmationTypes, this.service.getAuanDeliveryConfirmationTypes.bind(this.service), false)
        ).subscribe({
            next: (nomenclatures: (NomenclatureDTO<number> | InspDeliveryTypesNomenclatureDTO)[][]) => {
                this.territoryUnits = nomenclatures[0];
                this.deliveryTypes = (nomenclatures[1] as InspDeliveryTypesNomenclatureDTO[]).filter(x => x.group === InspDeliveryTypeGroupsEnum.AUAN);
                this.deliveryConfirmationTypes = (nomenclatures[2] as InspDeliveryTypesNomenclatureDTO[]).filter(x => x.group === InspDeliveryTypeGroupsEnum.AUAN);
            }
        });

        return subscribtion;
    }

    private fillForm(delivery: AuanDeliveryDataDTO): void {
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
                }
            }
        }
    }
}