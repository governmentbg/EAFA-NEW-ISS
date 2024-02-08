import { MainNavigation } from '@app/shared/navigation/base/main.navigation';
import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { NavigationEnd, Router, RouterEvent } from '@angular/router';
import { FuseConfigService } from '@fuse/services/config.service';
import { NgxPermissionsService } from 'ngx-permissions';
import { Subject } from 'rxjs';
import { takeUntil, pairwise, filter } from 'rxjs/operators';

@Component({
    selector: 'vertical-layout-1',
    templateUrl: './layout-1.component.html',
    styleUrls: ['./layout-1.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class VerticalLayout1Component implements OnInit, OnDestroy {
    fuseConfig: any;
    navigation: any;

    public isUserRegistrationModulePage: boolean = false;

    // Private
    private _unsubscribeAll: Subject<any>;

    /**
     * Constructor
     *
     * @param {FuseConfigService} _fuseConfigService
     */
    constructor(
        private _fuseConfigService: FuseConfigService,
        private router: Router,
        private ngxPermissionsService: NgxPermissionsService
    ) {
        this.navigation = [];
        // Set the defaults
        this.navigation = [];
        // Set the private defaults
        this._unsubscribeAll = new Subject();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On init
     */
    ngOnInit(): void {
        // Subscribe to config changes
        this._fuseConfigService.config
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((config) => {
                this.fuseConfig = config;
            });

        this.router.events.subscribe((event: any) => {
            if (event instanceof NavigationEnd) {
                const currentRoute = event.url;
                this.isUserRegistrationModulePage = currentRoute.includes('/registration') || currentRoute.includes('/merge-profiles') || currentRoute.includes('/terms-and-conditions') || currentRoute.includes('/successful-registration') || currentRoute.includes('/redirect');
            }
        });
    }

    /**
     * On destroy
     */
    ngOnDestroy(): void {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next();
        this._unsubscribeAll.complete();
    }
}