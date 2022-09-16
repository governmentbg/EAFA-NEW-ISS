import { Component, EventEmitter, Injector, Input, OnChanges, OnInit, Output, Self, SimpleChange, SimpleChanges } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup, NgControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { AcquiredFishingCapacityDTO } from '@app/models/generated/dtos/AcquiredFishingCapacityDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { AcquiredCapacityMannerEnum } from '@app/enums/acquired-capacity-manner.enum';
import { IFishingCapacityService } from '@app/interfaces/common-app/fishing-capacity.interface';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { FishingCapacityPublicService } from '@app/services/public-app/fishing-capacity-public.service';
import { FishingCapacityAdministrationService } from '@app/services/administration-app/fishing-capacity-administration.service';
import { FishingCapacityCertificateNomenclatureDTO } from '@app/models/generated/dtos/FishingCapacityCertificateNomenclatureDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';

export class NewCertificateData {
    public tonnage!: number;
    public power!: number;
    public validTo: Date | undefined;
    public validThreeYears: boolean = false;

    public constructor(obj?: Partial<NewCertificateData>) {
        Object.assign(this, obj);
    }
}

@Component({
    selector: 'acquired-fishing-capacity',
    templateUrl: './acquired-fishing-capacity.component.html'
})
export class AcquiredFishingCapacityComponent extends CustomFormControl<AcquiredFishingCapacityDTO> implements OnInit, OnChanges {
    @Input() public minGrossTonnage: number | undefined;
    @Input() public maxGrossTonnage: number | undefined;
    @Input() public minPower: number | undefined;
    @Input() public maxPower: number | undefined;

    @Output() public onGrossTonnageChanged: EventEmitter<number> = new EventEmitter<number>();
    @Output() public onPowerChanged: EventEmitter<number> = new EventEmitter<number>();
    @Output() public onNewCertificateDataChange: EventEmitter<NewCertificateData | undefined> = new EventEmitter<NewCertificateData | undefined>();

    public readonly manners: typeof AcquiredCapacityMannerEnum = AcquiredCapacityMannerEnum;

    public types: NomenclatureDTO<AcquiredCapacityMannerEnum>[] = [];
    public licenses: FishingCapacityCertificateNomenclatureDTO[] = [];

    public disableAddLicenceBtn: boolean = false;

    public get certificatesArray(): FormArray {
        return this.form.get('certificatesArray') as FormArray;
    }

    private get grossTonnageValidators(): ValidatorFn[] {
        return [Validators.required, TLValidators.number(this.minGrossTonnage ?? 0, this.maxGrossTonnage ?? undefined, 2)];
    }

    private get powerValidators(): ValidatorFn[] {
        return [Validators.required, TLValidators.number(this.minPower ?? 0, this.maxPower ?? undefined, 2)];
    }

    private allLicences: FishingCapacityCertificateNomenclatureDTO[] = [];

    private service!: IFishingCapacityService;
    private translate: FuseTranslationLoaderService;

    private readonly loader: FormControlDataLoader;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        injector: Injector
    ) {
        super(ngControl);
        this.translate = translate;

        if (IS_PUBLIC_APP) {
            this.service = injector.get(FishingCapacityPublicService);
        }
        else {
            this.service = injector.get(FishingCapacityAdministrationService);
        }

        this.types = [
            new NomenclatureDTO<AcquiredCapacityMannerEnum>({
                value: AcquiredCapacityMannerEnum.Ranking,
                displayName: this.translate.getValue('fishing-capacity.acquired-ranking'),
                isActive: true
            }),
            new NomenclatureDTO<AcquiredCapacityMannerEnum>({
                value: AcquiredCapacityMannerEnum.FreeCapLicence,
                displayName: this.translate.getValue('fishing-capacity.acquired-free-capacity-licence'),
                isActive: true
            })
        ];

        this.setSubscribers();
        this.loader = new FormControlDataLoader(this.getLicences.bind(this));
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
        this.loader.load();
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const minGrossTonnage: SimpleChange | undefined = changes['minGrossTonnage'];
        const minPower: SimpleChange | undefined = changes['minPower'];

        if (minGrossTonnage || minPower) {
            const manner: AcquiredCapacityMannerEnum | undefined = this.form.get('typeControl')!.value?.value;

            if (manner === AcquiredCapacityMannerEnum.Ranking) {
                this.form.get('grossTonnageControl')!.setValidators(this.grossTonnageValidators);
                this.form.get('grossTonnageControl')!.markAsPending({ emitEvent: false });

                this.form.get('powerControl')!.setValidators(this.powerValidators);
                this.form.get('powerControl')!.markAsPending({ emitEvent: false });
            }
            else if (manner === AcquiredCapacityMannerEnum.FreeCapLicence) {
                this.certificatesArray.updateValueAndValidity();
            }

            this.form.updateValueAndValidity();

            if (this.isDisabled) {
                this.form.disable({ emitEvent: false });
            }
        }
    }

    public writeValue(value: AcquiredFishingCapacityDTO): void {
        if (value !== undefined && value !== null && value.acquiredManner !== undefined && value.acquiredManner !== null) {
            this.form.get('typeControl')!.setValue(this.types.find(x => x.value === value.acquiredManner));

            if (value.acquiredManner === AcquiredCapacityMannerEnum.Ranking) {
                this.form.get('grossTonnageControl')!.setValue(value.grossTonnage?.toFixed(2));
                this.form.get('powerControl')!.setValue(value.power?.toFixed(2));
            }
            else if (value.acquiredManner === AcquiredCapacityMannerEnum.FreeCapLicence) {
                if ((value.capacityLicenceIds ?? []).length > 0) {
                    this.loader.load(() => {
                        this.certificatesArray.clear();

                        for (const certificateId of value.capacityLicenceIds!) {
                            const control: FormControl = new FormControl(this.licenses.find(x => x.value === certificateId), Validators.required);
                            this.certificatesArray.push(control);
                        }

                        this.emitOnNewCertificateDataChange();
                    });
                }
                else {
                    this.certificatesArray.clear();
                    this.certificatesArray.push(new FormControl(undefined, Validators.required));
                }
            }
        }
        else {
            this.form.get('typeControl')!.setValue(this.types.find(x => x.value === AcquiredCapacityMannerEnum.Ranking));
            this.form.get('grossTonnageControl')!.setValue(null);
            this.form.get('powerControl')!.setValue(null);
        }
    }

    public addEmptyLicence(): void {
        this.certificatesArray.push(new FormControl(undefined, Validators.required));
    }

    public removeLicence(index: number): void {
        this.certificatesArray.removeAt(index);
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            typeControl: new FormControl(null, Validators.required),
            grossTonnageControl: new FormControl(null),
            powerControl: new FormControl(null),
            certificatesArray: new FormArray([])
        });
    }

    protected getValue(): AcquiredFishingCapacityDTO {
        const result = new AcquiredFishingCapacityDTO({
            acquiredManner: this.form.get('typeControl')!.value?.value ?? undefined
        });

        if (result.acquiredManner === AcquiredCapacityMannerEnum.Ranking) {
            result.grossTonnage = this.form.get('grossTonnageControl')!.value ?? undefined;
            result.power = this.form.get('powerControl')!.value ?? undefined;
            result.capacityLicenceIds = undefined;
        }
        else if (result.acquiredManner === AcquiredCapacityMannerEnum.FreeCapLicence) {
            result.grossTonnage = undefined;
            result.power = undefined;

            const certificates = (this.certificatesArray.value as FishingCapacityCertificateNomenclatureDTO[]) ?? [];
            result.capacityLicenceIds = certificates
                .filter(x => x !== undefined && x !== null)
                .map(x => x.value!);
        }

        return result;
    }

    private setSubscribers(): void {
        this.form.get('typeControl')!.valueChanges.subscribe({
            next: (type: NomenclatureDTO<AcquiredCapacityMannerEnum>) => {
                if (type !== undefined && type !== null) {
                    if (type.value === AcquiredCapacityMannerEnum.Ranking) {
                        this.form.get('grossTonnageControl')!.setValidators(this.grossTonnageValidators);
                        this.form.get('grossTonnageControl')!.markAsPending({ emitEvent: false });

                        this.form.get('powerControl')!.setValidators(this.powerValidators);
                        this.form.get('powerControl')!.markAsPending({ emitEvent: false });

                        for (const control of this.certificatesArray.controls) {
                            control.clearValidators();
                            control.updateValueAndValidity();
                        }

                        this.certificatesArray.clearValidators();
                        this.certificatesArray.updateValueAndValidity();
                    }
                    else if (type.value === AcquiredCapacityMannerEnum.FreeCapLicence) {
                        this.form.get('grossTonnageControl')!.clearValidators();
                        this.form.get('grossTonnageControl')!.updateValueAndValidity({ emitEvent: false });

                        this.form.get('powerControl')!.clearValidators();
                        this.form.get('powerControl')!.updateValueAndValidity({ emitEvent: false });

                        if (this.certificatesArray.length === 0) {
                            this.certificatesArray.push(new FormControl(undefined, Validators.required));
                        }
                        else {
                            for (const control of this.certificatesArray.controls) {
                                control.setValidators(Validators.required);
                                control.markAsPending({ emitEvent: false });
                            }
                        }

                        this.certificatesArray.setValidators(this.licencesValidator());
                        this.certificatesArray.updateValueAndValidity();
                    }

                    this.form.updateValueAndValidity({ emitEvent: false });

                    const [tonnageSum, powerSum] = this.getCapacitySums();
                    this.onGrossTonnageChanged.emit(tonnageSum);
                    this.onPowerChanged.emit(powerSum);

                    this.emitOnNewCertificateDataChange();

                    if (this.isDisabled) {
                        this.form.disable({ emitEvent: false });
                    }
                }
            }
        });

        this.form.get('grossTonnageControl')!.valueChanges.subscribe({
            next: (value: number) => {
                this.onGrossTonnageChanged.emit(value);
                this.emitOnNewCertificateDataChange();
            }
        });

        this.form.get('powerControl')!.valueChanges.subscribe({
            next: (value: number) => {
                this.onPowerChanged.emit(value);
                this.emitOnNewCertificateDataChange();
            }
        });

        this.certificatesArray.valueChanges.subscribe({
            next: (certs: FishingCapacityCertificateNomenclatureDTO[]) => {
                if (this.form.get('typeControl')!.value?.value === AcquiredCapacityMannerEnum.FreeCapLicence) {
                    const licences: FishingCapacityCertificateNomenclatureDTO[] = certs.filter(x => x !== undefined && x !== null);
                    const tonnageSum: number = licences.map(x => x.grossTonnage!).reduce((a: number, b: number) => a + b, 0);
                    const powerSum: number = licences.map(x => x.power!).reduce((a: number, b: number) => a + b, 0);

                    if (this.minGrossTonnage !== undefined && this.minGrossTonnage !== null && this.minPower !== undefined && this.minPower !== null) {
                        this.disableAddLicenceBtn = tonnageSum >= this.minGrossTonnage && powerSum >= this.minPower;
                    }
                    else {
                        this.disableAddLicenceBtn = false;
                    }

                    const licenceIds: number[] = licences.map(x => x.value!);
                    this.licenses = this.allLicences.filter(x => !licenceIds.includes(x.value!));

                    this.onGrossTonnageChanged.emit(tonnageSum);
                    this.onPowerChanged.emit(powerSum);

                    this.emitOnNewCertificateDataChange();

                    if (this.isDisabled) {
                        this.form.disable({ emitEvent: false });
                    }
                }
            }
        });

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.certificatesArray.updateValueAndValidity();
                this.form.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    private licencesValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.minGrossTonnage !== undefined && this.minGrossTonnage !== null && this.minPower !== undefined && this.minPower !== null) {
                let licences: FishingCapacityCertificateNomenclatureDTO[] = this.certificatesArray.value;
                licences = licences.filter(x => x !== undefined && x !== null);

                if (licences.length > 0) {
                    const tonnageSum: number = licences.map(x => x.grossTonnage!).reduce((a: number, b: number) => a + b, 0);
                    const powerSum: number = licences.map(x => x.power!).reduce((a: number, b: number) => a + b, 0);

                    if (tonnageSum < this.minGrossTonnage || powerSum < this.minPower) {
                        return { 'licencesCapacityNotEnough': true };
                    }
                }
            }

            return null;
        };
    }

    private getCapacitySums(): [number | undefined, number | undefined] {
        if (this.form.get('typeControl')!.value?.value === AcquiredCapacityMannerEnum.Ranking) {
            return [this.form.get('grossTonnageControl')!.value, this.form.get('powerControl')!.value];
        }
        else if (this.form.get('typeControl')!.value?.value === AcquiredCapacityMannerEnum.FreeCapLicence) {
            const licences = (this.certificatesArray.value as FishingCapacityCertificateNomenclatureDTO[]).filter(x => x !== undefined && x !== null);
            const tonnageSum: number = licences.map(x => x.grossTonnage!).reduce((a: number, b: number) => a + b, 0);
            const powerSum: number = licences.map(x => x.power!).reduce((a: number, b: number) => a + b, 0);

            return [tonnageSum, powerSum];
        }

        return [undefined, undefined];

    }

    private emitOnNewCertificateDataChange(): void {
        const data: NewCertificateData = new NewCertificateData();

        const manner: AcquiredCapacityMannerEnum | undefined = this.form.get('typeControl')!.value?.value;
        let [tonnage, power] = this.getCapacitySums();

        if (this.minGrossTonnage === undefined || this.minGrossTonnage === null || this.minPower === undefined || this.minPower === null) {
            this.onNewCertificateDataChange.emit(undefined);
        }
        else if (manner === undefined || manner === null || tonnage === undefined || tonnage === null || power === undefined || power === null) {
            this.onNewCertificateDataChange.emit(undefined);
        }
        else {
            tonnage = Number(tonnage);
            power = Number(power);

            if (tonnage < Number(this.minGrossTonnage) || power < Number(this.minPower)) {
                this.onNewCertificateDataChange.emit(undefined);
            }
            else {
                if (manner === AcquiredCapacityMannerEnum.Ranking) {
                    data.validThreeYears = true;
                }
                else if (manner === AcquiredCapacityMannerEnum.FreeCapLicence) {
                    const licences = (this.certificatesArray.value as FishingCapacityCertificateNomenclatureDTO[]).filter(x => x !== undefined && x !== null);
                    data.validTo = new Date(Math.max(...licences.map(x => x.validTo!.getTime())));
                }

                data.power = power - Number(this.minPower);
                data.tonnage = tonnage - Number(this.minGrossTonnage);
                
                this.onNewCertificateDataChange.emit(data);
            }
        }
    }

    private getLicences(): Subscription {
        const subscription: Subscription = this.service.getAllCapacityCertificateNomenclatures().subscribe({
            next: (licences: FishingCapacityCertificateNomenclatureDTO[]) => {
                this.allLicences = this.licenses = licences;

                this.loader.complete();
            }
        });

        return subscription;
    }
}