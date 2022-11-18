import {
    ChangeDetectorRef, Component, ElementRef, EventEmitter, HostBinding, HostListener, Input, OnDestroy, OnInit, Output, Renderer2, ViewEncapsulation
} from '@angular/core';
import { animate, AnimationBuilder, AnimationPlayer, style } from '@angular/animations';
import { MediaObserver } from '@angular/flex-layout';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { FuseSidebarService } from './sidebar.service';
import { FuseMatchMediaService } from '@fuse/services/match-media.service';
import { FuseConfigService } from '@fuse/services/config.service';
import { MenuService } from '../../../app/shared/services/menu.service';

@Component({
    selector: 'fuse-sidebar',
    templateUrl: './sidebar.component.html',
    styleUrls: ['./sidebar.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class FuseSidebarComponent implements OnInit, OnDestroy {
    // Name
    @Input()
    name: string;

    // Key
    @Input()
    key: string;

    // Position
    @Input()
    position: 'left' | 'right';

    // Open
    @HostBinding('class.open')
    opened: boolean;

    // Locked Open
    @Input()
    lockedOpen: string;

    // isLockedOpen
    @HostBinding('class.locked-open')
    isLockedOpen: boolean;

    // Folded width
    @Input()
    foldedWidth: number;

    // Folded auto trigger on hover
    @Input()
    foldedAutoTriggerOnHover: boolean;

    // Folded unfolded
    @HostBinding('class.unfolded')
    unfolded: boolean;

    // Invisible overlay
    @Input()
    invisibleOverlay: boolean;

    // Folded changed
    @Output()
    foldedChanged: EventEmitter<boolean>;

    // Opened changed
    @Output()
    openedChanged: EventEmitter<boolean>;

    // Private
    private _folded: boolean;
    private wasActive: boolean;
    private wasFolded: boolean;
    private backdrop: HTMLElement | null = null;
    private player?: AnimationPlayer = undefined;
    private unsubscribeAll: Subject<any>;

    @HostBinding('class.animations-enabled')
    private animationsEnabled: boolean;

    /**
     * Constructor
     *
     * @param {AnimationBuilder} animationBuilder
     * @param {ChangeDetectorRef} changeDetectorRef
     * @param {ElementRef} elementRef
     * @param {FuseConfigService} fuseConfigService
     * @param {FuseMatchMediaService} fuseMatchMediaService
     * @param {FuseSidebarService} fuseSidebarService
     * @param {MediaObserver} mediaObserver
     * @param {Renderer2} renderer
     */
    constructor(private animationBuilder: AnimationBuilder,
        private changeDetectorRef: ChangeDetectorRef,
        private elementRef: ElementRef,
        private fuseConfigService: FuseConfigService,
        private fuseMatchMediaService: FuseMatchMediaService,
        private fuseSidebarService: FuseSidebarService,
        private mediaObserver: MediaObserver,
        private renderer: Renderer2,
        private menuService: MenuService) {
        // Set the defaults
        this.unfolded = false;
        this.isLockedOpen = false;
        this.wasFolded = false;
        this.wasActive = false;
        this._folded = false;
        this.lockedOpen = '';
        this.key = '';
        this.name = '';
        this.foldedAutoTriggerOnHover = true;
        this.foldedWidth = 64;
        this.foldedChanged = new EventEmitter();
        this.openedChanged = new EventEmitter();
        this.opened = false;
        this.position = 'left';
        this.invisibleOverlay = false;

        // Set the private defaults
        this.animationsEnabled = false;
        this.folded = false;
        this.unsubscribeAll = new Subject();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Accessors
    // -----------------------------------------------------------------------------------------------------

    /**
     * Folded
     *
     * @param {boolean} value
     */
    @Input()
    set folded(value: boolean) {
        // Set the folded
        this._folded = value;

        // Return if the sidebar is closed
        if (!this.opened) {
            return;
        }

        // Programmatically add/remove padding to the element
        // that comes after or before based on the position
        let sibling, styleRule;

        const styleValue = this.foldedWidth + 'px';

        // Get the sibling and set the style rule
        if (this.position === 'left') {
            sibling = this.elementRef.nativeElement.nextElementSibling;
            styleRule = 'padding-left';
        }
        else {
            sibling = this.elementRef.nativeElement.previousElementSibling;
            styleRule = 'padding-right';
        }

        // If there is no sibling, return...
        if (!sibling) {
            return;
        }

        // If folded...
        if (value) {
            // Fold the sidebar
            this.fold();

            // Set the folded width
            this.renderer.setStyle(this.elementRef.nativeElement, 'width', styleValue);
            this.renderer.setStyle(this.elementRef.nativeElement, 'min-width', styleValue);
            this.renderer.setStyle(this.elementRef.nativeElement, 'max-width', styleValue);

            // Set the style and class
            this.renderer.setStyle(sibling, styleRule, styleValue);
            this.renderer.addClass(this.elementRef.nativeElement, 'folded');
        }
        // If unfolded...
        else {
            // Unfold the sidebar
            this.unfold();

            // Remove the folded width
            this.renderer.removeStyle(this.elementRef.nativeElement, 'width');
            this.renderer.removeStyle(this.elementRef.nativeElement, 'min-width');
            this.renderer.removeStyle(this.elementRef.nativeElement, 'max-width');

            // Remove the style and class
            this.renderer.removeStyle(sibling, styleRule);
            this.renderer.removeClass(this.elementRef.nativeElement, 'folded');
        }

        // Emit the 'foldedChanged' event
        this.foldedChanged.emit(this.folded);
    }

    get folded(): boolean {
        return this._folded;
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On init
     */
    ngOnInit(): void {
        // Subscribe to config changes
        this.fuseConfigService.config
            .pipe(takeUntil(this.unsubscribeAll))
            .subscribe(() => {
            });

        // Register the sidebar
        this.fuseSidebarService.register(this.name, this);

        // Setup visibility
        this._setupVisibility();

        // Setup position
        this._setupPosition();

        // Setup lockedOpen
        this._setupLockedOpen();

        // Setup folded
        this._setupFolded();
    }

    /**
     * On destroy
     */
    ngOnDestroy(): void {
        // If the sidebar is folded, unfold it to revert modifications
        if (this.folded) {
            this.unfold();
        }

        // Unregister the sidebar
        this.fuseSidebarService.unregister(this.name);

        // Unsubscribe from all subscriptions
        this.unsubscribeAll.next();
        this.unsubscribeAll.complete();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Private methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Setup the visibility of the sidebar
     *
     * @private
     */
    private _setupVisibility(): void {
        // Remove the existing box-shadow
        this.renderer.setStyle(this.elementRef.nativeElement, 'box-shadow', 'none');

        // Make the sidebar invisible
        this.renderer.setStyle(this.elementRef.nativeElement, 'visibility', 'hidden');
    }

    /**
     * Setup the sidebar position
     *
     * @private
     */
    private _setupPosition(): void {
        // Add the correct class name to the sidebar
        // element depending on the position attribute
        if (this.position === 'right') {
            this.renderer.addClass(this.elementRef.nativeElement, 'right-positioned');
        }
        else {
            this.renderer.addClass(this.elementRef.nativeElement, 'left-positioned');
        }
    }

    /**
     * Setup the lockedOpen handler
     *
     * @private
     */
    private _setupLockedOpen(): void {
        // Return if the lockedOpen wasn't set
        if (!this.lockedOpen) {
            // Return
            return;
        }

        // Set the wasActive for the first time
        this.wasActive = false;

        // Set the wasFolded
        this.wasFolded = this.folded;

        // Show the sidebar
        this.showSidebar();

        // Act on every media change
        this.fuseMatchMediaService.onMediaChange
            .pipe(takeUntil(this.unsubscribeAll))
            .subscribe(() => {
                // Get the active status
                const isActive = this.mediaObserver.isActive(this.lockedOpen);

                // If the both status are the same, don't act
                if (this.wasActive === isActive) {
                    return;
                }

                // Activate the lockedOpen
                if (isActive) {
                    // Set the lockedOpen status
                    this.isLockedOpen = true;

                    // Show the sidebar
                    this.showSidebar();

                    // Force the the opened status to true
                    this.opened = true;
                    this.menuService.folded.next(this.folded);

                    // Emit the 'openedChanged' event
                    this.openedChanged.emit(this.opened);

                    // If the sidebar was folded, forcefully fold it again
                    if (this.wasFolded) {
                        // Enable the animations
                        this.enableAnimations();

                        // Fold
                        this.folded = true;

                        // Mark for check
                        this.changeDetectorRef.markForCheck();
                    }

                    // Hide the backdrop if any exists
                    this.hideBackdrop();
                }
                // De-Activate the lockedOpen
                else {
                    // Set the lockedOpen status
                    this.isLockedOpen = false;

                    // Unfold the sidebar in case if it was folded
                    this.unfold();

                    // Force the the opened status to close
                    this.opened = false;

                    // Emit the 'openedChanged' event
                    this.openedChanged.emit(this.opened);
                    this.menuService.opened.next(this.opened);

                    // Hide the sidebar
                    this._hideSidebar();
                }

                // Store the new active status
                this.wasActive = isActive;
            });
    }

    /**
     * Setup the initial folded status
     *
     * @private
     */
    private _setupFolded(): void {
        // Return, if sidebar is not folded
        if (!this.folded) {
            return;
        }

        // Return if the sidebar is closed
        if (!this.opened) {
            return;
        }

        // Programmatically add/remove padding to the element
        // that comes after or before based on the position
        let sibling,
            styleRule;

        const styleValue = this.foldedWidth + 'px';

        // Get the sibling and set the style rule
        if (this.position === 'left') {
            sibling = this.elementRef.nativeElement.nextElementSibling;
            styleRule = 'padding-left';
        }
        else {
            sibling = this.elementRef.nativeElement.previousElementSibling;
            styleRule = 'padding-right';
        }

        // If there is no sibling, return...
        if (!sibling) {
            return;
        }

        // Fold the sidebar
        this.fold();

        // Set the folded width
        this.renderer.setStyle(this.elementRef.nativeElement, 'width', styleValue);
        this.renderer.setStyle(this.elementRef.nativeElement, 'min-width', styleValue);
        this.renderer.setStyle(this.elementRef.nativeElement, 'max-width', styleValue);

        // Set the style and class
        this.renderer.setStyle(sibling, styleRule, styleValue);
        this.renderer.addClass(this.elementRef.nativeElement, 'folded');
    }

    /**
     * Show the backdrop
     *
     * @private
     */
    private showBackdrop(): void {
        // Create the backdrop element
        this.backdrop = this.renderer.createElement('div');

        // Add a class to the backdrop element
        this.backdrop?.classList?.add('fuse-sidebar-overlay');

        // Add a class depending on the invisibleOverlay option
        if (this.invisibleOverlay) {
            this.backdrop?.classList?.add('fuse-sidebar-overlay-invisible');
        }

        // Append the backdrop to the parent of the sidebar
        this.renderer.appendChild(this.elementRef.nativeElement.parentElement, this.backdrop);

        // Create the enter animation and attach it to the player
        this.player =
            this.animationBuilder
                .build([
                    animate('300ms ease', style({ opacity: 1 }))
                ]).create(this.backdrop);

        // Play the animation
        this.player?.play();

        // Add an event listener to the overlay
        this.backdrop?.addEventListener('click', () => {
            this.close();
        }
        );

        // Mark for check
        this.changeDetectorRef.markForCheck();
    }

    /**
     * Hide the backdrop
     *
     * @private
     */
    private hideBackdrop(): void {
        if (!this.backdrop) {
            return;
        }

        // Create the leave animation and attach it to the player
        this.player =
            this.animationBuilder
                .build([
                    animate('300ms ease', style({ opacity: 0 }))
                ]).create(this.backdrop);

        // Play the animation
        this.player?.play();

        // Once the animation is done...
        this.player?.onDone(() => {
            // If the backdrop still exists...
            if (this.backdrop) {
                // Remove the backdrop
                this.backdrop?.parentNode?.removeChild(this.backdrop);
                this.backdrop = null;
            }
        });

        // Mark for check
        this.changeDetectorRef.markForCheck();
    }

    /**
     * Change some properties of the sidebar
     * and make it visible
     *
     * @private
     */
    private showSidebar(): void {
        // Remove the box-shadow style
        this.renderer.removeStyle(this.elementRef.nativeElement, 'box-shadow');

        // Make the sidebar invisible
        this.renderer.removeStyle(this.elementRef.nativeElement, 'visibility');

        // Mark for check
        this.changeDetectorRef.markForCheck();
    }

    /**
     * Change some properties of the sidebar
     * and make it invisible
     *
     * @private
     */
    private _hideSidebar(delay = true): void {
        const delayAmount = delay ? 300 : 0;

        // Add a delay so close animation can play
        setTimeout(() => {
            // Remove the box-shadow
            this.renderer.setStyle(this.elementRef.nativeElement, 'box-shadow', 'none');

            // Make the sidebar invisible
            this.renderer.setStyle(this.elementRef.nativeElement, 'visibility', 'hidden');
        }, delayAmount);

        // Mark for check
        this.changeDetectorRef.markForCheck();
    }

    /**
     * Enable the animations
     *
     * @private
     */
    private enableAnimations(): void {
        // Return if animations already enabled
        if (this.animationsEnabled) {
            return;
        }

        // Enable the animations
        this.animationsEnabled = true;

        // Mark for check
        this.changeDetectorRef.markForCheck();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Open the sidebar
     */
    open(): void {
        if (this.opened || this.isLockedOpen) {
            return;
        }

        // Enable the animations
        this.enableAnimations();

        // Show the sidebar
        this.showSidebar();

        // Show the backdrop
        this.showBackdrop();

        // Set the opened status
        this.opened = true;

        // Emit the 'openedChanged' event
        this.openedChanged.emit(this.opened);
        this.menuService.opened.next(this.opened);

        // Mark for check
        this.changeDetectorRef.markForCheck();
    }

    /**
     * Close the sidebar
     */
    close(): void {
        if (!this.opened || this.isLockedOpen) {
            return;
        }

        // Enable the animations
        this.enableAnimations();

        // Hide the backdrop
        this.hideBackdrop();

        // Set the opened status
        this.opened = false;

        // Emit the 'openedChanged' event
        this.openedChanged.emit(this.opened);
        this.menuService.opened.next(this.opened);

        // Hide the sidebar
        this._hideSidebar();

        // Mark for check
        this.changeDetectorRef.markForCheck();
    }

    /**
     * Toggle open/close the sidebar
     */
    toggleOpen(): void {
        if (this.opened) {
            this.close();
        }
        else {
            this.open();
        }
    }

    /**
     * Mouseenter
     */
    @HostListener('mouseenter')
    onMouseEnter(): void {
        // Only work if the auto trigger is enabled
        if (!this.foldedAutoTriggerOnHover) {
            return;
        }

        this.unfoldTemporarily();
    }

    /**
     * Mouseleave
     */
    @HostListener('mouseleave')
    onMouseLeave(): void {
        // Only work if the auto trigger is enabled
        if (!this.foldedAutoTriggerOnHover) {
            return;
        }

        this.foldTemporarily();
    }

    /**
     * Fold the sidebar permanently
     */
    fold(): void {
        this.menuService.folded.next(this.folded);

        // Only work if the sidebar is not folded
        if (this.folded) {
            return;
        }

        // Enable the animations
        this.enableAnimations();

        // Fold
        this.folded = true;

        // Mark for check
        this.changeDetectorRef.markForCheck();
    }

    /**
     * Unfold the sidebar permanently
     */
    unfold(): void {
        this.menuService.folded.next(this.folded);

        // Only work if the sidebar is folded
        if (!this.folded) {
            return;
        }

        // Enable the animations
        this.enableAnimations();

        // Unfold
        this.folded = false;

        // Mark for check
        this.changeDetectorRef.markForCheck();
    }

    /**
     * Toggle the sidebar fold/unfold permanently
     */
    toggleFold(): void {
        if (this.folded) {
            this.unfold();
        }
        else {
            this.fold();
        }
    }

    /**
     * Fold the temporarily unfolded sidebar back
     */
    foldTemporarily(): void {
        // Only work if the sidebar is folded
        if (!this.folded) {
            return;
        }

        // Enable the animations
        this.enableAnimations();

        // Fold the sidebar back
        this.unfolded = false;

        // Set the folded width
        const styleValue = this.foldedWidth + 'px';

        this.renderer.setStyle(this.elementRef.nativeElement, 'width', styleValue);
        this.renderer.setStyle(this.elementRef.nativeElement, 'min-width', styleValue);
        this.renderer.setStyle(this.elementRef.nativeElement, 'max-width', styleValue);

        // Mark for check
        this.changeDetectorRef.markForCheck();
    }

    /**
     * Unfold the sidebar temporarily
     */
    unfoldTemporarily(): void {
        // Only work if the sidebar is folded
        if (!this.folded) {
            return;
        }

        // Enable the animations
        this.enableAnimations();

        // Unfold the sidebar temporarily
        this.unfolded = true;

        // Remove the folded width
        this.renderer.removeStyle(this.elementRef.nativeElement, 'width');
        this.renderer.removeStyle(this.elementRef.nativeElement, 'min-width');
        this.renderer.removeStyle(this.elementRef.nativeElement, 'max-width');

        // Mark for check
        this.changeDetectorRef.markForCheck();
    }
}