import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { fuseConfig } from '@app/fuse-config';
import { FuseConfigService } from '@fuse/services/config.service';
import { FuseConfig, FuseNavigation } from '@fuse/types';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
    selector: 'horizontal-layout-1',
    templateUrl: './layout-1.component.html',
    styleUrls: ['./layout-1.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class HorizontalLayout1Component implements OnInit, OnDestroy {
    public fuseConfig: FuseConfig = fuseConfig;
    navigation: FuseNavigation[];

    // Private
    private unsubscribeAll: Subject<any>;

    /**
     * Constructor
     *
     * @param {FuseConfigService} fuseConfigService
     */
    constructor(private fuseConfigService: FuseConfigService) {
        // Set the defaults
        this.navigation = [];

        // Set the private defaults
        this.unsubscribeAll = new Subject();
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
}