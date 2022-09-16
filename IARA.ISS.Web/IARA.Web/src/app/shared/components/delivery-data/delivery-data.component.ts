import { Component, Input, OnInit, Optional, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { forkJoin, Subscription } from 'rxjs';

import { DeliveryTypesEnum } from '@app/enums/delivery-types.enum';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ApplicationBaseDeliveryDTO } from '@app/models/generated/dtos/ApplicationBaseDeliveryDTO';
import { ApplicationDeliveryTypeDTO } from '@app/models/generated/dtos/ApplicationDeliveryTypeDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { AddressRegistrationDTO } from '@app/models/generated/dtos/AddressRegistrationDTO';
import { AddressTypesEnum } from '@app/enums/address-types.enum';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';

@Component({
    selector: 'delivery-data',
    templateUrl: './delivery-data.component.html'
})
export class DeliveryDataComponent extends CustomFormControl<ApplicationBaseDeliveryDTO> implements OnInit {
    @Input()
    public pageCode!: PageCodeEnum;

    public deliveryTypes: ApplicationDeliveryTypeDTO[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];

    public readonly deliveryTypesEnum: typeof DeliveryTypesEnum = DeliveryTypesEnum;
    public readonly addressTypesEnum: typeof AddressTypesEnum = AddressTypesEnum;
    public chosenDeliveryTypeCode: DeliveryTypesEnum | undefined;

    private nomenclatures: CommonNomenclatures;
    private allDeliveryTypes: ApplicationDeliveryTypeDTO[] = [];

    private readonly loader: FormControlDataLoader;

    @ViewChild(ValidityCheckerGroupDirective)
    protected validityCheckerGroup!: ValidityCheckerGroupDirective;

    public constructor(
        @Self() ngControl: NgControl,
        @Optional() @Self() validityChecker: ValidityCheckerDirective,
        nomenclatures: CommonNomenclatures
    ) {
        super(ngControl, true, validityChecker);
        this.nomenclatures = nomenclatures;

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.form.updateValueAndValidity();
                this.validityCheckerGroup.validate();
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
        this.loader.load();
    }

    public writeValue(value: ApplicationBaseDeliveryDTO | undefined): void {
        if (value !== undefined && value !== null) {
            this.loader.load(() => {
                this.fillForm(value);
            });
        }
        else {
            this.fillForm(value);
        }
    }

    protected buildForm(): AbstractControl {
        const group: FormGroup = new FormGroup({
            deliveryTypeControl: new FormControl(undefined, Validators.required),
            deliveryAddressControl: new FormControl(undefined),
            deliveryEmailControl: new FormControl(undefined, Validators.maxLength(256)),
            deliveryTerritoryUnitControl: new FormControl(undefined)
        });

        group.get('deliveryTypeControl')!.valueChanges.subscribe({
            next: (value: ApplicationDeliveryTypeDTO) => {
                if (value !== null && value !== undefined) {
                    this.chosenDeliveryTypeCode = DeliveryTypesEnum[value.code as keyof typeof DeliveryTypesEnum];
                }
                else {
                    this.chosenDeliveryTypeCode = undefined;
                }

                switch (this.chosenDeliveryTypeCode) {
                    case DeliveryTypesEnum.ByMail:
                        this.clearErrors('deliveryEmailControl', 'deliveryTerritoryUnitControl');

                        this.form.get('deliveryAddressControl')!.setValidators(Validators.required);
                        break;
                    case DeliveryTypesEnum.CopyOnEmail:
                        this.clearErrors('deliveryAddressControl', 'deliveryTerritoryUnitControl');

                        this.form.get('deliveryEmailControl')!.setValidators([Validators.maxLength(256), Validators.required]);
                        break;
                    case DeliveryTypesEnum.Personal:
                        this.loader.load(() => {
                            this.clearErrors('deliveryEmailControl', 'deliveryAddressControl');

                            this.form.get('deliveryTerritoryUnitControl')!.setValidators(Validators.required);
                        });
                        break;
                    case DeliveryTypesEnum.eDelivery:
                    case DeliveryTypesEnum.NoDelivery:
                    case undefined:
                        this.clearErrors('deliveryEmailControl', 'deliveryAddressControl', 'deliveryTerritoryUnitControl');
                        break;
                }

                this.form.updateValueAndValidity({ emitEvent: false });
            }
        });

        return group;
    }

    protected getValue(): ApplicationBaseDeliveryDTO {
        let deliveryAddress: AddressRegistrationDTO | undefined;
        let territoryUnitId: number | undefined;
        let emailAddress: string | undefined;

        switch (this.chosenDeliveryTypeCode) {
            case DeliveryTypesEnum.ByMail:
                deliveryAddress = this.form.get('deliveryAddressControl')!.value;
                break;
            case DeliveryTypesEnum.CopyOnEmail:
                emailAddress = this.form.get('deliveryEmailControl')!.value;
                break;
            case DeliveryTypesEnum.Personal:
                territoryUnitId = this.form.get('deliveryTerritoryUnitControl')!.value?.value
                break;
        }

        return new ApplicationBaseDeliveryDTO({
            deliveryTypeId: this.form.get('deliveryTypeControl')!.value?.value,
            deliveryTeritorryUnitId: territoryUnitId,
            deliveryAddress: deliveryAddress,
            deliveryEmailAddress: emailAddress
        });
    }

    private fillForm(deliveryData: ApplicationBaseDeliveryDTO | undefined): void {
        if (deliveryData !== null && deliveryData !== undefined) {
            this.form.get('deliveryTypeControl')!.setValue(this.deliveryTypes.find(x => x.value === deliveryData.deliveryTypeId));

            switch (this.chosenDeliveryTypeCode) {
                case DeliveryTypesEnum.ByMail:
                    this.form.get('deliveryAddressControl')!.setValue(deliveryData.deliveryAddress);
                    break;
                case DeliveryTypesEnum.CopyOnEmail:
                    this.form.get('deliveryEmailControl')!.setValue(deliveryData.deliveryEmailAddress);
                    break;
                case DeliveryTypesEnum.Personal:
                    this.loader.load(() => {
                        this.form.get('deliveryTerritoryUnitControl')!.setValue(this.territoryUnits.find(x => x.value === deliveryData.deliveryTeritorryUnitId));
                    });
                    break;
            }
        }
        else {
            this.form.reset();
        }
    }

    private getNomenclatures(): Subscription {
        const subscription: Subscription = forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.DeliveryTypes, this.nomenclatures.getDeliveryTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false)
        ).subscribe({
            next: (nomenclatures: NomenclatureDTO<number>[][]) => {
                this.allDeliveryTypes = nomenclatures[0];
                this.deliveryTypes = this.allDeliveryTypes.filter(x => x.pageCode === this.pageCode);

                this.territoryUnits = nomenclatures[1];

                this.loader.complete();
            }
        });

        return subscription;
    }

    private clearErrors(...controls: string[]): void {
        for (const control of controls) {
            this.form.get(control)!.reset();
            this.form.get(control)!.clearValidators();
            this.form.get(control)!.setErrors(null);
        }
    }
}
