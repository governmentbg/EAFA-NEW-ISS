import { Component, Input, OnChanges, Optional, Self, SimpleChanges } from '@angular/core';
import { NgControl } from '@angular/forms';
import { FloatLabelType, MatFormFieldAppearance } from '@angular/material/form-field';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';
import { BaseTLControl } from '../base-tl-control';

export type HTMLInputTypes = 'button' | 'checkbox' | 'color' | 'date' | 'datetime-local' | 'email' | 'file' | 'hidden' | 'image'
    | 'month' | 'number' | 'password' | 'radio' | 'range' | 'reset' | 'search' | 'submit' | 'tel' | 'text' | 'time' | 'url' | 'week';

@Component({
    selector: 'tl-input',
    templateUrl: './tl-input.component.html',
})
export class TLInputComponent extends BaseTLControl implements OnChanges {

    @Input()
    public prefixText?: string;

    @Input()
    public appearance: MatFormFieldAppearance = 'legacy';

    @Input()
    public type: HTMLInputTypes = 'text';

    @Input()
    public floatLabel: FloatLabelType = 'auto';

    @Input()
    public value: string | undefined;

    private lastFloatLabel: FloatLabelType = 'auto';

    public constructor(
        @Self() @Optional() ngControl: NgControl,
        tlTranslatePipe: TLTranslatePipe
    ) {
        super(ngControl, tlTranslatePipe);
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if ('floatLabel' in changes) {
            this.lastFloatLabel = this.floatLabel;
            this.setFloatLabel();
        }

        if ('prefixText' in changes) {
            this.setFloatLabel();
        }
    }

    private setFloatLabel(): void {
        if (this.prefixText) {
            this.floatLabel = 'always';
        }
        else {
            this.floatLabel = this.lastFloatLabel;
        }
    }
}