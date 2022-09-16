import { Component, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { delay, filter, take, takeUntil } from 'rxjs/operators';

import { FuseConfigService } from '@fuse/services/config.service';
import { FuseNavigationService } from '@fuse/components/navigation/navigation.service';
import { FusePerfectScrollbarDirective } from '@fuse/directives/fuse-perfect-scrollbar/fuse-perfect-scrollbar.directive';
import { FuseSidebarService } from '@fuse/components/sidebar/sidebar.service';
import { FuseConfig, FuseNavigation } from '@fuse/types';

@Component({
    selector: 'navbar-vertical-style-2',
    templateUrl: './style-2.component.html',
    styleUrls: ['./style-2.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class NavbarVerticalStyle2Component implements OnInit, OnDestroy {
    fuseConfig?: FuseConfig = undefined;
    navigation: FuseNavigation[] = [];

    // Private
    private fusePerfectScrollbar?: FusePerfectScrollbarDirective = undefined;
    private unsubscribeAll: Subject<any>;

    /**
     * Constructor
     *
     * @param {FuseConfigService} fuseConfigService
     * @param {FuseNavigationService} fuseNavigationService
     * @param {FuseSidebarService} fuseSidebarService
     * @param {Router} _router
     */
    constructor(private fuseConfigService: FuseConfigService,
        private fuseNavigationService: FuseNavigationService,
        private fuseSidebarService: FuseSidebarService,
        private router: Router) {
        // Set the private defaults
        this.unsubscribeAll = new Subject();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Accessors
    // -----------------------------------------------------------------------------------------------------

    // Directive
    @ViewChild(FusePerfectScrollbarDirective, { static: true })
    set directive(theDirective: FusePerfectScrollbarDirective) {
        if (!theDirective) {
            return;
        }

        this.fusePerfectScrollbar = theDirective;

        // Update the scrollbar on collapsable item toggle
        this.fuseNavigationService.onItemCollapseToggled
            .pipe(
                delay(500),
                takeUntil(this.unsubscribeAll)
            )
            .subscribe(() => {
                this.fusePerfectScrollbar?.update();
            });

        // Scroll to the active item position
        this.router.events
            .pipe(
                filter((event) => event instanceof NavigationEnd),
                take(1)
            )
            .subscribe(() => {
                setTimeout(() => {
                    this.fusePerfectScrollbar?.scrollToElement('navbar .nav-link.active', -120);
                });
            }
            );
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On init
     */
    ngOnInit(): void {
        this.router.events
            .pipe(
                filter((event) => event instanceof NavigationEnd),
                takeUntil(this.unsubscribeAll)
            )
            .subscribe(() => {
                if (this.fuseSidebarService.getSidebar('navbar')) {
                    this.fuseSidebarService.getSidebar('navbar')?.close();
                }
            }
            );

        // Get current navigation
        this.fuseNavigationService.onNavigationChanged
            .pipe(
                filter(value => value !== null),
                takeUntil(this.unsubscribeAll)
            )
            .subscribe(() => {
                this.navigation = this.fuseNavigationService.getCurrentNavigation();
            });

        // Subscribe to the config changes
        this.fuseConfigService.config
            .pipe(takeUntil(this.unsubscribeAll))
            .subscribe((config) => {
                this.fuseConfig = config;
            });
    }

    /**
     * On destroy
     */
    ngOnDestroy(): void {
        // Unsubscribe from all subscriptions
        this.unsubscribeAll.next();
        this.unsubscribeAll.complete();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Toggle sidebar opened status
     */
    toggleSidebarOpened(): void {
        this.fuseSidebarService.getSidebar('navbar')?.toggleOpen();
    }

    /**
     * Toggle sidebar folded status
     */
    toggleSidebarFolded(): void {
        this.fuseSidebarService.getSidebar('navbar')?.toggleFold();
    }
}