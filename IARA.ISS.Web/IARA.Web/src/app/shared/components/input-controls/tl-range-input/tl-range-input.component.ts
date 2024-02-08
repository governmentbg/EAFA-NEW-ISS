import { Component, Input, OnInit, Optional, Self } from '@angular/core';
import { NgControl } from '@angular/forms';
import { MatFormFieldAppearance } from '@angular/material/form-field';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLTranslatePipe } from '../../../pipes/tl-translate.pipe';
import { BaseTLControl } from '../base-tl-control';
import { TLError } from '../models/tl-error.model';

@Component({
    selector: 'tl-range-input',
    templateUrl: './tl-range-input.component.html'
})
export class TLRangeInputComponent extends BaseTLControl implements OnInit {
    @Input() public fromLabel!: string;
    @Input() public toLabel!: string;

    @Input() public separator!: string;

    @Input() public min: number | undefined;
    @Input() public max: number | undefined;

    @Input() public startRequired: boolean = true;
    @Input() public endRequired: boolean = true;

    @Input() public appearance: MatFormFieldAppearance = 'legacy';

    public constructor(@Self() @Optional() ngControl: NgControl, tlTranslatePipe: TLTranslatePipe) {
        super(ngControl, tlTranslatePipe);

        this.getControlErrorLabelText = this.getControlErrorLabelTextHelper.bind(this);
    }

    private getControlErrorLabelTextHelper(controlName: string, error: unknown, errorCode: string): TLError | undefined {
        const record = error as Record<string, unknown>;

        if (controlName === 'startValueControl') {
            if (errorCode === 'min') {
                return new TLError({ text: `Минималната начална стойност е ${record.min}`, type: 'error' });
            }
            if (errorCode === 'max') {
                return new TLError({ text: `Максималната начална стойност е ${record.max}`, type: 'error' });
            }
            if (errorCode === 'required' && error === true) {
                return new TLError({ text: 'Началната стойност е задължителна', type: 'error' });
            }
        }
        else if (controlName === 'endValueControl') {
            if (errorCode === 'min') {
                return new TLError({ text: `Минималната крайна стойност е ${record.min}`, type: 'error' });
            }
            if (errorCode === 'max') {
                return new TLError({ text: `Максималната крайна стойност е ${record.max}`, type: 'error' });
            }
            if (errorCode === 'required' && error === true) {
                return new TLError({ text: 'Крайната стойност е задължителна', type: 'error' });
            }
        }
        return undefined;
    }
}