import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { FuseProgressBarService } from '@fuse/components/progress-bar/progress-bar.service';

@Component({
    selector: 'fuse-progress-bar',
    templateUrl: './progress-bar.component.html',
    styleUrls: ['./progress-bar.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class FuseProgressBarComponent implements OnInit, OnDestroy {
    bufferValue: number;
    mode: 'determinate' | 'indeterminate' | 'buffer' | 'query';
    value: number;
    visible: boolean;

    // Private
    private unsubscribeAll: Subject<any>;

    /**
     * Constructor
     *
     * @param {FuseProgressBarService} fuseProgressBarService
     */
    constructor(private fuseProgressBarService: FuseProgressBarService) {
        // Set the defaults
        this.visible = false;
        this.value = 0;
        this.mode = 'determinate';
        this.bufferValue = 0;
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
        // Subscribe to the progress bar service properties

        // Buffer value
        this.fuseProgressBarService.bufferValue
            .pipe(takeUntil(this.unsubscribeAll))
            .subscribe((bufferValue) => {
                this.bufferValue = bufferValue;
            });

        // Mode
        this.fuseProgressBarService.mode
            .pipe(takeUntil(this.unsubscribeAll))
            .subscribe((mode) => {
                this.mode = mode;
            });

        // Value
        this.fuseProgressBarService.value
            .pipe(takeUntil(this.unsubscribeAll))
            .subscribe((value) => {
                this.value = value;
            });

        // Visible
        this.fuseProgressBarService.visible
            .pipe(takeUntil(this.unsubscribeAll))
            .subscribe((visible) => {
                this.visible = visible;
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
}