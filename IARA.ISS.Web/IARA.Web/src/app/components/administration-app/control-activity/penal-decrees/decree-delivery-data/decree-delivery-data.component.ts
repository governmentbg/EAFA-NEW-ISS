import { AfterViewInit, Component, Input, OnInit, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { PenalDecreeDeliveryDataDTO } from '@app/models/generated/dtos/PenalDecreeDeliveryDataDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { InspDeliveryTypesEnum } from '@app/enums/insp-delivery-types.enum';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { IPenalDecreesService } from '@app/interfaces/administration-app/penal-decrees.interface';
import { PenalDecreesService } from '@app/services/administration-app/penal-decrees.service';
import { forkJoin, Subscription } from 'rxjs';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { InspDeliveryConfirmationTypesEnum } from '@app/enums/insp-delivery-confirmation-types.enum';
import { InspDeliveryTypesNomenclatureDTO } from '@app/models/generated/dtos/InspDeliveryTypesNomenclatureDTO';
import { InspDeliveryTypeGroupsEnum } from '@app/enums/insp-delivery-type-groups.enum';
import { AuanWitnessDTO } from '@app/models/generated/dtos/AuanWitnessDTO';

@Component({
    selector: 'decree-delivery-data',
    templateUrl: './decree-delivery-data.component.html'
})
export class DecreeDeliveryDataComponent extends CustomFormControl<PenalDecreeDeliveryDataDTO> implements OnInit, AfterViewInit {
    @Input() public viewMode!: boolean;

    public deliveryTypes: NomenclatureDTO<number>[] = [];
    public confirmationTypes: NomenclatureDTO<number>[] = [];

    public readonly deliveryTypesEnum: typeof InspDeliveryTypesEnum = InspDeliveryTypesEnum;
    public readonly confirmationTypesEnum: typeof InspDeliveryConfirmationTypesEnum = InspDeliveryConfirmationTypesEnum;

    public delivery: PenalDecreeDeliveryDataDTO | undefined;
    public deliveryType: InspDeliveryTypesEnum | undefined;
    public confirmationType: InspDeliveryConfirmationTypesEnum | undefined;
    public isDelivered: boolean = false;
    public readonly today: Date = new Date();

    private readonly service: IPenalDecreesService;

    private readonly loader: FormControlDataLoader;

    public constructor(
        @Self() ngControl: NgControl,
        service: PenalDecreesService
    ) {
        super(ngControl);

        this.service = service;

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
        this.loader.load();
    }

    public ngAfterViewInit(): void {
        this.form.get('deliveryTypeControl')!.valueChanges.subscribe({
            next: (type: NomenclatureDTO<number> | undefined) => {
                if (type !== undefined && type !== null) {
                    this.form.get('deliveryDateControl')!.setValidators(Validators.required);
                    this.form.get('deliveryDateControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('deliverySentDateControl')!.clearValidators();

                    this.deliveryType = InspDeliveryTypesEnum[type.code as keyof typeof InspDeliveryTypesEnum];

                    if (this.deliveryType === InspDeliveryTypesEnum.DecreeReturn || this.deliveryType === InspDeliveryTypesEnum.DecreeTag) {
                        this.form.get('deliverySentDateControl')?.setValidators(Validators.required);
                    }

                    this.form.get('deliverySentDateControl')!.updateValueAndValidity({ emitEvent: false });
                }
                else {
                    this.deliveryType = undefined;
                }
            }
        });

        this.form.get('confirmationTypeControl')!.valueChanges.subscribe({
            next: (type: NomenclatureDTO<number> | undefined) => {
                if (type !== null && type !== undefined) {
                    this.confirmationType = InspDeliveryConfirmationTypesEnum[type.code as keyof typeof InspDeliveryConfirmationTypesEnum];
                    this.isDelivered = true;

                    this.form.get('witnessesControl')!.clearValidators();

                    if (this.confirmationType === InspDeliveryConfirmationTypesEnum.RefusalDecree) {
                        this.form.get('witnessesControl')?.setValidators(Validators.required);
                    }

                    this.form.get('witnessesControl')!.updateValueAndValidity({ emitEvent: false });
                }
                else {
                    this.confirmationType = undefined;
                    this.isDelivered = false;
                }
            }
        });
    }

    public writeValue(value: PenalDecreeDeliveryDataDTO): void {
        if (value !== undefined && value !== null) {
            this.delivery = value;

            this.loader.load(() => {
                this.fillForm(value);
            });
        }
        this.fillForm(value);
    }

    protected getValue(): PenalDecreeDeliveryDataDTO {
        let sentDate: Date | undefined = undefined;
        let refusalWitnesses: AuanWitnessDTO[] = [];

        if (this.deliveryType !== null && this.deliveryType !== undefined) {
            if (this.deliveryType === InspDeliveryTypesEnum.DecreeReturn || this.deliveryType === InspDeliveryTypesEnum.DecreeTag) {
                sentDate = this.form.get('deliverySentDateControl')!.value;
            }

            if (this.confirmationType === InspDeliveryConfirmationTypesEnum.RefusalDecree) {
                refusalWitnesses = this.form.get('witnessesControl')!.value;
            }
        }

        return new PenalDecreeDeliveryDataDTO({
            id: this.delivery?.id,
            deliveryType: this.deliveryType,
            confirmationType: this.confirmationType,
            isDelivered: this.isDelivered,
            deliveryDate: this.form.get('deliveryDateControl')!.value,
            referenceNum: this.form.get('deliverySentControl')!.value,
            sentDate: sentDate,
            refusalWitnesses: refusalWitnesses
        });
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            deliveryTypeControl: new FormControl(null),
            deliveryDateControl: new FormControl(null),
            witnessesControl: new FormControl(null),
            deliverySentDateControl: new FormControl(null),
            deliverySentControl: new FormControl(null, Validators.maxLength(200)),
            confirmationTypeControl: new FormControl(null)
        });
    }

    private getNomenclatures(): Subscription {
        const subscribtion: Subscription = forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.InspDeliveryTypes, this.service.getAuanDeliveryTypes.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.InspDeliveryConfirmationTypes, this.service.getAuanDeliveryConfirmationTypes.bind(this.service), false)
        ).subscribe({
            next: (nomenclatures: (NomenclatureDTO<number> | InspDeliveryTypesNomenclatureDTO)[][]) => {
                this.deliveryTypes = (nomenclatures[0] as InspDeliveryTypesNomenclatureDTO[]).filter(x => x.group === InspDeliveryTypeGroupsEnum.PD);
                this.confirmationTypes = (nomenclatures[1] as InspDeliveryTypesNomenclatureDTO[]).filter(x => x.group === InspDeliveryTypeGroupsEnum.PD);
            }
        });

        return subscribtion;
    }

    private fillForm(delivery: PenalDecreeDeliveryDataDTO): void {
        if (delivery !== undefined && delivery !== null) {
            const type: InspDeliveryTypesEnum | undefined = delivery.deliveryType;

            this.form.get('deliveryDateControl')!.setValue(delivery.deliveryDate);

            if (delivery.confirmationType !== undefined && delivery.confirmationType !== null) {
                this.form.get('confirmationTypeControl')!.setValue(this.confirmationTypes.find(x => x.code === InspDeliveryConfirmationTypesEnum[delivery.confirmationType!]));
            }

            if (type !== undefined && type !== null) {
                this.form.get('deliveryTypeControl')!.setValue(this.deliveryTypes.find(x => x.code === InspDeliveryTypesEnum[type]));

                if (type === InspDeliveryTypesEnum.DecreeReturn || type === InspDeliveryTypesEnum.DecreeTag) {
                    this.form.get('deliverySentDateControl')!.setValue(delivery.sentDate);
                    this.form.get('deliverySentControl')!.setValue(delivery.referenceNum);
                }

                if (this.confirmationType === InspDeliveryConfirmationTypesEnum.RefusalDecree) {
                    this.form.get('witnessesControl')!.setValue(delivery.refusalWitnesses);
                }
            }
        }
    }
}