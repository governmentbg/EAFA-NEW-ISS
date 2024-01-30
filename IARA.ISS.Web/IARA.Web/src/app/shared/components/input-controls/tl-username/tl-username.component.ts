import { Component, EventEmitter, HostListener, Input, Optional, Output, Self } from '@angular/core';
import { NgControl } from '@angular/forms';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';
import { BaseTLControl } from '../base-tl-control';


type HTMLInputTypes = 'button' | 'checkbox' | 'color' | 'date' | 'datetime-local' | 'email' | 'file' | 'hidden' | 'image'
    | 'month' | 'number' | 'password' | 'radio' | 'range' | 'reset' | 'search' | 'submit' | 'tel' | 'text' | 'time' | 'url' | 'week';

@Component({
    selector: 'tl-username',
    templateUrl: './tl-username.component.html',
})
export class TLUsernameComponent extends BaseTLControl {
    private _maskSeparator: string | undefined;

    @Input()
    public type: HTMLInputTypes = 'text';

    /**
     * This Input should be passed only when there is NO formControl 
     * (use only for readonly <input> cases with no direct UI user interaction)
     **/
    @Input()
    public value: string | undefined;

    @Input()
    public mask: string | undefined;

    @Input()
    public get maskSeparator(): string | undefined {
        return this._maskSeparator;
    }
    public set maskSeparator(value: string | undefined) {
        this._maskSeparator = value;

        this.mask = 'separator';
    }

    @Output()
    public onClicked: EventEmitter<void> = new EventEmitter<void>();

    @HostListener('click')
    private clicked(): void {
        this.onClicked.emit();
    }

    public constructor(@Self() @Optional() ngControl: NgControl, translatePipe: TLTranslatePipe) {
        super(ngControl, translatePipe);
    }
}
