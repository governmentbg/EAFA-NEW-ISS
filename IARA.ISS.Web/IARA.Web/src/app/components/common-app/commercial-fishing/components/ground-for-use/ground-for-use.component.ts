import { Component, Input, OnInit, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';

import { HolderGroundForUseDTO } from '@app/models/generated/dtos/HolderGroundForUseDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { DateRangeIndefiniteData } from '@app/shared/components/date-range-indefinite/date-range-indefinite.component';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';

@Component({
    selector: 'ground-for-use',
    templateUrl: './ground-for-use.component.html'
})
export class GroudForUseComponent extends CustomFormControl<HolderGroundForUseDTO | undefined> implements OnInit {
    @Input()
    public groundForUseTypes: NomenclatureDTO<number>[] = [];

    private id: number | undefined;

    public constructor(@Self() ngControl: NgControl) {
        super(ngControl);
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            groundForUseTypeControl: new FormControl(undefined, Validators.required),
            groundForUseNumberControl: new FormControl(undefined, [Validators.required, Validators.maxLength(500)]),
            groundForUseValidityRangeControl: new FormControl(undefined, Validators.required)
        });
    }

    public writeValue(value: HolderGroundForUseDTO | undefined): void {
        if (value !== null && value !== undefined) {
            this.id = value.id;

            if (value.typeId !== null && value.typeId !== undefined) {
                const groundForUseType: NomenclatureDTO<number> = this.groundForUseTypes.find(x => x.value === value.typeId)!;
                this.form.get('groundForUseTypeControl')!.setValue(groundForUseType);
            }

            this.form.get('groundForUseNumberControl')!.setValue(value.number);
            this.form.get('groundForUseValidityRangeControl')!.setValue(new DateRangeIndefiniteData(
                {
                    range: new DateRangeData({ start: value.groundForUseValidFrom, end: value.groundForUseValidTo }),
                    indefinite: value.isGroundForUseUnlimited ?? false
                })
            );
        }
        else {
            this.id = undefined;

            this.form.reset();
        }
    }

    protected getValue(): HolderGroundForUseDTO | undefined {
        const result = new HolderGroundForUseDTO({
            id: this.id,
            typeId: this.form.get('groundForUseTypeControl')!.value?.value,
            number: this.form.get('groundForUseNumberControl')!.value
        });

        const validity: DateRangeIndefiniteData | undefined = this.form.get('groundForUseValidityRangeControl')!.value;

        if (validity !== undefined && validity !== null) {
            result.isGroundForUseUnlimited = validity.indefinite;
            result.groundForUseValidFrom = validity.range?.start;
            result.groundForUseValidTo = validity.range?.end;
        }
        else {
            result.isGroundForUseUnlimited = false;
        }

        return result;
    }
}
