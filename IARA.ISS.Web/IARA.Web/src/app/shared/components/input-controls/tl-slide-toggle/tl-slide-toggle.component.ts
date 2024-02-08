import { Component, Input, Optional, Self } from '@angular/core';
import { NgControl } from '@angular/forms';
import { TLTranslatePipe } from '../../../pipes/tl-translate.pipe';
import { BaseTLControl } from '../base-tl-control';

@Component({
    selector: 'tl-slide-toggle',
    templateUrl: './tl-slide-toggle.component.html'
})
export class TLSlideToggleComponent extends BaseTLControl {
    @Input()
    public color: string = 'accent';

    @Input()
    public labelPosition: 'below' | 'after' = 'after';

    constructor(@Self() @Optional() ngControl: NgControl, tlTranslatePipe: TLTranslatePipe) {
        super(ngControl, tlTranslatePipe);
    }
}