import { Component, HostBinding, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { fuseAnimations } from '@fuse/animations';
import { FuseConfigService } from '@fuse/services/config.service';
import { FuseNavigationService } from '@fuse/components/navigation/navigation.service';
import { FuseSidebarService } from '@fuse/components/sidebar/sidebar.service';
import { FuseConfig } from '../../types';

@Component({
    selector: 'fuse-theme-options',
    templateUrl: './theme-options.component.html',
    styleUrls: ['./theme-options.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class FuseThemeOptionsComponent implements OnInit, OnDestroy {
    fuseConfig?: FuseConfig = undefined;
    form: FormGroup;

    @HostBinding('class.bar-closed')
    barClosed: boolean;

    // Private
    private unsubscribeAll: Subject<any>;

    /**
     * Constructor
     *
     * @param {DOCUMENT} document
     * @param {FormBuilder} _formBuilder
     * @param {FuseConfigService} _fuseConfigService
     * @param {FuseNavigationService} _fuseNavigationService
     * @param {FuseSidebarService} _fuseSidebarService
     * @param {Renderer2} _renderer
     */
    constructor(private formBuilder: FormBuilder,
        private fuseConfigService: FuseConfigService,
        private fuseNavigationService: FuseNavigationService,
        private fuseSidebarService: FuseSidebarService) {
        // Set the defaults
        this.barClosed = true;

        // Set the private defaults
        this.unsubscribeAll = new Subject();

        // Build the config form
        // noinspection TypeScriptValidateTypes
        this.form = this.formBuilder.group({
            colorTheme: new FormControl(),
            customScrollbars: new FormControl(),
            layout: this.formBuilder.group({
                style: new FormControl(),
                width: new FormControl(),
                navbar: this.formBuilder.group({
                    primaryBackground: new FormControl(),
                    secondaryBackground: new FormControl(),
                    folded: new FormControl(),
                    hidden: new FormControl(),
                    position: new FormControl(),
                    variant: new FormControl()
                }),
                toolbar: this.formBuilder.group({
                    background: new FormControl(),
                    customBackgroundColor: new FormControl(),
                    hidden: new FormControl(),
                    position: new FormControl()
                }),
                footer: this.formBuilder.group({
                    background: new FormControl(),
                    customBackgroundColor: new FormControl(),
                    hidden: new FormControl(),
                    position: new FormControl()
                }),
                sidepanel: this.formBuilder.group({
                    hidden: new FormControl(),
                    position: new FormControl()
                })
            })
        });
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On init
     */
    ngOnInit(): void {
        // Subscribe to the config changes
        this.fuseConfigService.config
            .pipe(takeUntil(this.unsubscribeAll))
            .subscribe((config) => {
                // Update the stored config
                this.fuseConfig = config;

                // Set the config form values without emitting an event
                // so that we don't end up with an infinite loop
                this.form.setValue(config, { emitEvent: false });
            });

        // Subscribe to the specific form value changes (layout.style)
        this.form.get('layout.style')?.valueChanges
            .pipe(takeUntil(this.unsubscribeAll))
            .subscribe((value) => {
                // Reset the form values based on the
                // selected layout style
                this.resetFormValues(value);
            });

        // Subscribe to the form value changes
        this.form.valueChanges
            .pipe(takeUntil(this.unsubscribeAll))
            .subscribe((config) => {
                // Update the config
                this.fuseConfigService.config = config;
            });
    }

    /**
     * On destroy
     */
    ngOnDestroy(): void {
        // Unsubscribe from all subscriptions
        this.unsubscribeAll.next();
        this.unsubscribeAll.complete();

        // Remove the custom function menu
        this.fuseNavigationService.removeNavigationItem('custom-function');
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Private methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Reset the form values based on the
     * selected layout style
     *
     * @param value: string
     * @private
     */
    private resetFormValues(value: string): void {
        switch (value) {
            // Vertical Layout #1
            case 'vertical-layout-1':
                {
                    this.form.patchValue({
                        layout: {
                            width: 'fullwidth',
                            navbar: {
                                primaryBackground: 'fuse-navy-700',
                                secondaryBackground: 'fuse-navy-900',
                                folded: false,
                                hidden: false,
                                position: 'left',
                                variant: 'vertical-style-1'
                            },
                            toolbar: {
                                background: 'fuse-white-500',
                                customBackgroundColor: false,
                                hidden: false,
                                position: 'below-static'
                            },
                            footer: {
                                background: 'fuse-navy-900',
                                customBackgroundColor: true,
                                hidden: true,
                                position: 'below-static'
                            },
                            sidepanel: {
                                hidden: false,
                                position: 'right'
                            }
                        }
                    });

                    break;
                }

            // Vertical Layout #2
            case 'vertical-layout-2':
                {
                    this.form.patchValue({
                        layout: {
                            width: 'fullwidth',
                            navbar: {
                                primaryBackground: 'fuse-navy-700',
                                secondaryBackground: 'fuse-navy-900',
                                folded: false,
                                hidden: false,
                                position: 'left',
                                variant: 'vertical-style-1'
                            },
                            toolbar: {
                                background: 'fuse-white-500',
                                customBackgroundColor: false,
                                hidden: false,
                                position: 'below'
                            },
                            footer: {
                                background: 'fuse-navy-900',
                                customBackgroundColor: true,
                                hidden: true,
                                position: 'below'
                            },
                            sidepanel: {
                                hidden: false,
                                position: 'right'
                            }
                        }
                    });

                    break;
                }

            // Vertical Layout #3
            case 'vertical-layout-3':
                {
                    this.form.patchValue({
                        layout: {
                            width: 'fullwidth',
                            navbar: {
                                primaryBackground: 'fuse-navy-700',
                                secondaryBackground: 'fuse-navy-900',
                                folded: false,
                                hidden: false,
                                position: 'left',
                                layout: 'vertical-style-1'
                            },
                            toolbar: {
                                background: 'fuse-white-500',
                                customBackgroundColor: false,
                                hidden: false,
                                position: 'above-static'
                            },
                            footer: {
                                background: 'fuse-navy-900',
                                customBackgroundColor: true,
                                hidden: true,
                                position: 'above-static'
                            },
                            sidepanel: {
                                hidden: false,
                                position: 'right'
                            }
                        }
                    });

                    break;
                }

            // Horizontal Layout #1
            case 'horizontal-layout-1':
                {
                    this.form.patchValue({
                        layout: {
                            width: 'fullwidth',
                            navbar: {
                                primaryBackground: 'fuse-navy-700',
                                secondaryBackground: 'fuse-navy-900',
                                folded: false,
                                hidden: false,
                                position: 'top',
                                variant: 'vertical-style-1'
                            },
                            toolbar: {
                                background: 'fuse-white-500',
                                customBackgroundColor: false,
                                hidden: false,
                                position: 'above'
                            },
                            footer: {
                                background: 'fuse-navy-900',
                                customBackgroundColor: true,
                                hidden: true,
                                position: 'above-fixed'
                            },
                            sidepanel: {
                                hidden: false,
                                position: 'right'
                            }
                        }
                    });

                    break;
                }
        }
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Toggle sidebar open
     *
     * @param key: string
     */
    toggleSidebarOpen(key: string): void {
        this.fuseSidebarService.getSidebar(key)?.toggleOpen();
    }
}