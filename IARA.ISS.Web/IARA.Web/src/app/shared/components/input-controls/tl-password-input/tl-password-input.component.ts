import { Component, Optional, Self } from '@angular/core';
import { NgControl } from '@angular/forms';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';
import { BaseTLControl } from '../base-tl-control';

@Component({
    selector: 'tl-password-input',
    templateUrl: './tl-password-input.component.html',
    styleUrls: ['./tl-password-input.component.scss']
})
export class TLPasswordComponent extends BaseTLControl {

    public constructor(@Self() @Optional() ngControl: NgControl, translatePipe: TLTranslatePipe) {
        super(ngControl, translatePipe);
        this.visible = false;
    }

    public toggleVisibility(): void {
        this.visible = !this.visible;
    }

    public showPassword(): void {
        this.visible = true;
    }

    public hidePassword(): void {
        this.visible = false;
    }

    public visible: boolean;

    public autocompleteValue: string = 'new-password';
}
