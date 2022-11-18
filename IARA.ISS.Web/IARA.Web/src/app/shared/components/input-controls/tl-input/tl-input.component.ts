import { Component, Input, Optional, Self } from '@angular/core';
import { NgControl } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';
import { BaseTLControl } from '../base-tl-control';

type MatFormFieldAppearance = 'legacy' | 'standard' | 'fill' | 'outline';
type HTMLInputTypes = 'button' | 'checkbox' | 'color' | 'date' | 'datetime-local' | 'email' | 'file' | 'hidden' | 'image'
    | 'month' | 'number' | 'password' | 'radio' | 'range' | 'reset' | 'search' | 'submit' | 'tel' | 'text' | 'time' | 'url' | 'week';

@Component({
    selector: 'tl-input',
    templateUrl: './tl-input.component.html',
})
export class TLInputComponent extends BaseTLControl {

    @Input()
    public appearance: MatFormFieldAppearance = 'legacy';

    @Input()
    public type: HTMLInputTypes = 'text';

    @Input()
    public value: string | undefined;

    constructor(@Self() @Optional() ngControl: NgControl, fuseTranslationService: FuseTranslationLoaderService, tlTranslatePipe: TLTranslatePipe) {
        super(ngControl, fuseTranslationService, tlTranslatePipe);
    }
}