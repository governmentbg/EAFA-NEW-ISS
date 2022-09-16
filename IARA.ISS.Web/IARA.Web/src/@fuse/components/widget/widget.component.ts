import { AfterContentInit, Component, ContentChildren, HostBinding, QueryList, Renderer2, ViewEncapsulation } from '@angular/core';
import { FuseWidgetToggleDirective } from './widget-toggle.directive';

@Component({
    selector: 'fuse-widget',
    templateUrl: './widget.component.html',
    styleUrls: ['./widget.component.scss'],
    encapsulation: ViewEncapsulation.None
})

export class FuseWidgetComponent implements AfterContentInit {
    @HostBinding('class.flipped')
    flipped = false;

    @ContentChildren(FuseWidgetToggleDirective, { descendants: true })
    toggleButtons: QueryList<FuseWidgetToggleDirective> = new QueryList<FuseWidgetToggleDirective>();

    /**
     * Constructor
     *
     * @param {Renderer2} renderer
     */
    constructor(private renderer: Renderer2) { }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * After content init
     */
    ngAfterContentInit(): void {
        // Listen for the flip button click
        setTimeout(() => {
            this.toggleButtons.forEach(flipButton => {
                this.renderer.listen(flipButton.elementRef.nativeElement, 'click', (event) => {
                    event.preventDefault();
                    event.stopPropagation();
                    this.toggle();
                });
            });
        });
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Toggle the flipped status
     */
    toggle(): void {
        this.flipped = !this.flipped;
    }
}