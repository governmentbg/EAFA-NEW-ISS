import { Component, OnInit, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { LetterOfAttorneyDTO } from '@app/models/generated/dtos/LetterOfAttorneyDTO';
import { CustomFormControl } from '../../utils/custom-form-control';
import { DateRangeIndefiniteData } from '../date-range-indefinite/date-range-indefinite.component';
import { DateRangeData } from '../input-controls/tl-date-range/tl-date-range.component';

@Component({
    selector: 'letter-of-attorney',
    templateUrl: './letter-of-attorney.component.html'
})
export class LetterOfAttorneyComponent extends CustomFormControl<LetterOfAttorneyDTO> implements OnInit {
    public readonly today: Date = new Date();

    public constructor(@Self() ngControl: NgControl) {
        super(ngControl);
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        if (this.isDisabled) {
            this.form.disable();
        }
    }

    public writeValue(value: LetterOfAttorneyDTO): void {
        if (value !== null && value !== undefined) {
            this.form.get('letterNumControl')!.setValue(value.letterNum);
            this.form.get('letterPeriodControl')!.setValue(new DateRangeIndefiniteData({
                range: new DateRangeData({ start: value.letterValidFrom, end: value.letterValidTo }),
                indefinite: value.isUnlimited ?? false
            }));
            this.form.get('notaryNameControl')!.setValue(value.notaryName);
        }
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            letterNumControl: new FormControl(null, [Validators.required, Validators.maxLength(50)]),
            letterPeriodControl: new FormControl(null, Validators.required),
            notaryNameControl: new FormControl(null, Validators.maxLength(500))
        });
    }

    protected getValue(): LetterOfAttorneyDTO {
        const result = new LetterOfAttorneyDTO({
            letterNum: this.form.get('letterNumControl')?.value ?? undefined,
            notaryName: this.form.get('notaryNameControl')?.value ?? undefined
        });

        const period: DateRangeIndefiniteData | undefined = this.form.get('letterPeriodControl')!.value;
        if (period !== undefined && period !== null) {
            result.isUnlimited = period.indefinite;
            result.letterValidFrom = period.range?.start;
            result.letterValidTo = period.range?.end;
        }
        else {
            result.isUnlimited = false;
        }

        return result;
    }
}
