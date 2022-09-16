import { Component, EventEmitter, Input, Output } from "@angular/core";

@Component({
    selector: 'tl-expansion-panel',
    templateUrl: './tl-expansion-panel.component.html',
    styleUrls: ['./tl-expansion-panel.component.scss']
})
export class TLExpansionPanelComponent {

    @Input()
    public fontSizeEm: number = 1.3;

    /**
     * Resource name for the resource to be shown in the help component
     * **/
    @Input()
    public tooltipResourceName: string = '';

    /**
     * Content of the `mat-panel-title` tag of the expansion panel, which is part of the `mat-expansion-panel-header`.
     * */
    @Input()
    public title: string = '';

    /**
     * Content of the `mat-panel-description` tag of the expansion panel, which is part of the `mat-expansion-panel-header`.
     * */
    @Input()
    public description: string = '';

    /**
     * Whether the AccordionItem is disabled. `false` by default
     * */
    @Input()
    public disabled: boolean = false;

    /**
     * Whether the AccordionItem is expanded. `true` by default
     * */
    @Input()
    public expanded: boolean = true;

    /**
     * Whether the toggle indicator should be hidden. `false` by default
     * */
    @Input()
    public hideToggle: boolean = false;

    /**
     * The position of the expansion indicator. `after` by default
     * */
    @Input()
    public togglePosition: 'after' | 'before' = 'after';

    /**
     * Height of the header while the panel is collapsed. `48px` by default
     * */
    @Input()
    public collapsedHeaderHeight: string = '48px';

    /**
     *
     * Height of the header while the panel is expanded. `64px` by default
     * */
    @Input()
    public expandedHeaderHeight: string = '64px';

    /**
     * Indicates whether some control in the expansion panel is invalid. If so, the title becomes red
     * */
    @Input()
    public hasError: boolean = false;

    /**
     * An event emitted after the body's collapse animation happens.
     * */
    @Output()
    public afterCollapse: EventEmitter<void> = new EventEmitter<void>();

    /**
     * An event emitted after the body's expansion animation happens.
     * */
    @Output()
    public afterExpand: EventEmitter<void> = new EventEmitter<void>();

    /**
     * Event emitted every time the AccordionItem is closed.
     * */
    @Output()
    public closed: EventEmitter<void> = new EventEmitter<void>();

    /**
     * Event emitted when the AccordionItem is destroyed.
     * */
    @Output()
    public destroyed: EventEmitter<void> = new EventEmitter<void>();

    /**
     * Event emitted every time the AccordionItem is opened.
     * */
    @Output()
    public opened: EventEmitter<void> = new EventEmitter<void>();

    public onCollapse(): void {
        this.expanded = false;
        this.afterCollapse.emit();
    }

    public onExpand(): void {
        this.expanded = true;
        this.afterExpand.emit();
    }
}