import { TLIconTypes } from '@app/enums/icon-types.enum';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { MdePopoverTrigger } from '@material-extended/mde';

@Component({
    selector: 'tl-popover-button',
    templateUrl: './tl-popover.component.html',
    styleUrls: ['./tl-popover.component.scss']
})
export class TLPopoverComponent {
    @Input() public tooltipText: string;
    @Input() public icon: string;
    @Input() public iconSize: number | undefined;
    @Input() public iconType: TLIconTypes | undefined;
    @Input() public iconClass: string | undefined = 'warn-color';
    @Input() public iconColor: string | undefined;
    @Input() public overrideOpen: boolean = false;
    @Output() public togglePopover: EventEmitter<boolean> = new EventEmitter<boolean>();

    @ViewChild(MdePopoverTrigger)
    private trigger: MdePopoverTrigger | undefined;

    private isOpened: boolean = false;

    public constructor() {
        this.tooltipText = '';
        this.icon = '';
    }

    public onButtonClicked(): void {
        if (this.trigger !== undefined) {
            if (!this.overrideOpen) {
                this.trigger.togglePopover();
                this.isOpened = !this.isOpened;
            }
            this.togglePopover.emit(this.isOpened);
        }
    }

    public closePopover(close: boolean): void {
        if (close === true) {
            if (this.trigger !== undefined) {
                this.trigger.closePopover();
                this.isOpened = false;
                this.togglePopover.emit(this.isOpened);
            }
        }
    }
}