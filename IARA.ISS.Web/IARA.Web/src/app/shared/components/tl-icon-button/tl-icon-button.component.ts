import { TLIconTypes } from '@app/enums/icon-types.enum';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
    selector: 'tl-icon-button',
    templateUrl: './tl-icon-button.component.html',
    styleUrls: ['./tl-icon-button.component.scss']
})
export class TLIconButtonComponent {
    constructor() {
        this.tooltipText = '';
        this.icon = '';
    }

    @Input() public tooltipText: string;
    @Input() public icon: string;
    @Input() public size: number | undefined;
    @Input() public type: TLIconTypes | undefined;
    @Input() public iconClass: string | undefined;
    @Input() public iconColor: string | undefined;
    @Input() public disabled: boolean = false;

    @Output() public buttonClicked: EventEmitter<Event> = new EventEmitter<Event>();
    @Output() public mouseOver: EventEmitter<void> = new EventEmitter<void>();
    @Output() public mouseOut: EventEmitter<void> = new EventEmitter<void>();

    public onButtonClicked(event: Event): void {
        event.stopPropagation();
        this.buttonClicked.emit(event);
    }

    public onMouseOver(event: MouseEvent): void {
        event.stopPropagation();
        this.mouseOver.emit();
    }

    public onMouseOut(event: MouseEvent): void {
        event.stopPropagation();
        this.mouseOut.emit();
    }
}