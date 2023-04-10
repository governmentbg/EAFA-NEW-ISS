import { AfterViewInit, Component, DoCheck, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, FormGroup, NgControl, ValidationErrors, Validator, Validators } from '@angular/forms';
import { forkJoin, Subscription } from 'rxjs';

import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { FishingGearMarkDTO } from '@app/models/generated/dtos/FishingGearMarkDTO';
import { FishingGearNomenclatureDTO } from '@app/models/generated/dtos/FishingGearNomenclatureDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { PrefixInputDTO } from '@app/models/generated/dtos/PrefixInputDTO';

@Component({
    selector: 'fishing-gear-simple',
    templateUrl: './fishing-gear-simple.component.html'
})
export class FishingGearSimpleComponent implements OnInit, AfterViewInit, DoCheck, ControlValueAccessor, Validator {
    public form!: FormGroup;
    public marksFormGroup!: FormGroup;

    public isDisabled: boolean = false;
    public marksError: boolean = false;

    public fishingGearTypes: FishingGearNomenclatureDTO[] = [];
    public fishingGearMarkStatuses: NomenclatureDTO<number>[] = [];

    public marks: FishingGearMarkDTO[] = [];

    @ViewChild('marksTable')
    private marksTable!: TLDataTableComponent;

    private nomenclatures: CommonNomenclatures;

    private ngControl: NgControl;
    private onChanged: (value: FishingGearDTO | undefined) => void;
    private onTouched: (value: FishingGearDTO | undefined) => void;

    private gear: FishingGearDTO | undefined;

    private readonly loader: FormControlDataLoader;

    public constructor(@Self() ngControl: NgControl, nomenclatures: CommonNomenclatures) {
        this.ngControl = ngControl;
        this.nomenclatures = nomenclatures;

        this.ngControl.valueAccessor = this;

        this.onChanged = (value: FishingGearDTO | undefined) => { return value; };
        this.onTouched = (value: FishingGearDTO | undefined) => { return value; };

        this.buildForm();

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public ngOnInit(): void {
        this.loader.load();

        if (this.ngControl.control) {
            this.ngControl.control.validator = this.validate.bind(this);
        }
    }

    public ngAfterViewInit(): void {
        this.form.valueChanges.subscribe({
            next: () => {
                this.onChanged(this.getValue());
            }
        });

        this.marksTable.recordChanged.subscribe({
            next: () => {
                this.onChanged(this.getValue());
            }
        });
    }

    public ngDoCheck(): void {
        if (this.ngControl?.control?.touched) {
            this.form.markAllAsTouched();
        }
    }

    public writeValue(obj: FishingGearDTO | undefined): void {
        if (obj !== undefined && obj !== null) {
            this.gear = obj;
            this.marks = obj.marks ?? [];

            this.form.get('gearCountControl')!.setValue(obj.count);
            this.form.get('gearEyeSizeControl')!.setValue(obj.netEyeSize);
            this.form.get('gearHookCountControl')!.setValue(obj.hookCount);
            this.form.get('gearLengthControl')!.setValue(obj.length);
            this.form.get('gearHeightControl')!.setValue(obj.height);
            this.form.get('descriptionControl')!.setValue(obj.description);

            this.loader.load(() => {
                this.form.get('gearTypeControl')!.setValue(this.fishingGearTypes.find(x => x.value === obj!.typeId));
            });
        }
    }

    public registerOnChange(fn: (value: FishingGearDTO | undefined) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: FishingGearDTO | undefined) => void): void {
        this.onTouched = fn;
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
        if (isDisabled) {
            this.form.disable();
        }
        else {
            this.form.enable();
        }
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        const errors: ValidationErrors = {};
        for (const key of Object.keys(this.form.controls)) {
            const controlErrors: ValidationErrors | null = this.form.controls[key].errors;
            if (controlErrors !== null) {
                errors[key] = controlErrors;
            }
        }
        return Object.keys(errors).length === 0 ? null : errors;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            gearTypeControl: new FormControl(),
            gearCountControl: new FormControl(),
            gearEyeSizeControl: new FormControl(),
            gearHookCountControl: new FormControl(),
            gearLengthControl: new FormControl(),
            gearHeightControl: new FormControl(),
            descriptionControl: new FormControl(),
        });

        this.marksFormGroup = new FormGroup({
            numberControl: new FormControl(null, Validators.required),
            statusIdControl: new FormControl(null, Validators.required),
        });
    }

    private getValue(): FishingGearDTO {
        const newGear: FishingGearDTO = new FishingGearDTO({
            typeId: this.form.controls.gearTypeControl.value?.value,
            count: this.form.controls.gearCountControl.value,
            netEyeSize: this.form.controls.gearEyeSizeControl.value,
            hookCount: this.form.controls.gearHookCountControl.value,
            length: this.form.controls.gearLengthControl.value,
            height: this.form.controls.gearHeightControl.value,
            description: this.form.controls.descriptionControl.value,
            marks: this.marksTable.rows.map(x => {
                return new FishingGearMarkDTO({
                    statusId: x.statusId,
                    fullNumber: new PrefixInputDTO({
                        prefix: x.fullNumber?.prefix,
                        inputValue: x.fullNumber?.inputValue
                    }),
                    isActive: x.isActive
                });
            }),
            marksNumbers: this.marksTable.rows.map(x => x.number).join(', '),
            isActive: this.gear?.isActive ?? true
        });

        if (this.gear !== undefined && this.gear !== null) {
            if (newGear.count === this.gear.count
                && newGear.netEyeSize === this.gear.netEyeSize
                && newGear.hookCount === this.gear.hookCount
                && newGear.length === this.gear.length
                && newGear.height === this.gear.height
                && newGear.description === this.gear.description) {
                const toRemove = this.gear.marks?.filter(x => !newGear.marks?.some(y => y.fullNumber?.prefix === x.fullNumber?.prefix && y.fullNumber?.inputValue === x.fullNumber?.inputValue && y.statusId === x.statusId));
                if (toRemove !== undefined) {
                    this.gear.marks = this.gear.marks?.filter(x => !toRemove?.some(y => y === x));
                }

                const toAdd = newGear.marks?.filter(x => !this.gear!.marks?.some(y => y.fullNumber?.prefix === x.fullNumber?.prefix && y.fullNumber?.inputValue === x.fullNumber?.inputValue && y.statusId === x.statusId));
                if (toAdd !== undefined) {
                    this.gear.marks = this.gear.marks?.concat(toAdd);
                }

                this.gear.marksNumbers = this.gear.marks?.map(x => `${x.fullNumber?.prefix ?? ''}${x.fullNumber?.inputValue}`).join(', ');

                return this.gear;
            }
        }

        return newGear;
    }

    private getNomenclatures(): Subscription {
        const subscription: Subscription = forkJoin(
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.FishingGear, this.nomenclatures.getFishingGear.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.FishingGearMarkStatuses, this.nomenclatures.getFishingGearMarkStatuses.bind(this.nomenclatures), false
            )
        ).subscribe({
            next: (nomenclatures: NomenclatureDTO<number>[][]) => {
                this.fishingGearTypes = nomenclatures[0];
                this.fishingGearMarkStatuses = nomenclatures[1];

                this.loader.complete();
            }
        });

        return subscription;
    }
}
