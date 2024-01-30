import { Component, Input, Optional, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { MatFormFieldAppearance } from '@angular/material/form-field';
import { TLTranslatePipe } from '../../../pipes/tl-translate.pipe';
import { BaseTLControl } from '../base-tl-control';

export const NOOP_VALUE_ACCESSOR: ControlValueAccessor = {
    writeValue(): void { },
    registerOnChange(): void { },
    registerOnTouched(): void { }
};

@Component({
    selector: 'tl-textarea',
    templateUrl: './tl-textarea.component.html',
})
export class TLTextareaComponent extends BaseTLControl {
    @Input()
    public appearance: MatFormFieldAppearance = 'legacy';

    @Input()
    public minRows: number = 5;

    @Input()
    public maxRows: number = 5;

    @Input()
    public value: string | undefined;

    constructor(@Self() @Optional() ngControl: NgControl, tlTranslatePipe: TLTranslatePipe) {
        super(ngControl, tlTranslatePipe);
    }
}