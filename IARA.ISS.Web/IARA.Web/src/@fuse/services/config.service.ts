import { Inject, Injectable, InjectionToken } from '@angular/core';
import { ResolveEnd, Router } from '@angular/router';
import { Platform } from '@angular/cdk/platform';
import { BehaviorSubject, Observable } from 'rxjs';
import { filter } from 'rxjs/operators';
import * as _ from 'lodash';
import { FuseConfig } from '../types/fuse-config';

// Create the injection token for the custom settings
export const FUSE_CONFIG = new InjectionToken('fuseCustomConfig');

@Injectable({
    providedIn: 'root'
})
export class FuseConfigService {
    // Private
    private _configSubject?: BehaviorSubject<FuseConfig> = undefined;
    private readonly _defaultConfig: FuseConfig;

    /**
     * Constructor
     *
     * @param {Platform} _platform
     * @param {Router} _router
     * @param _config
     */
    constructor(
        private _platform: Platform,
        private _router: Router,
        @Inject(FUSE_CONFIG) _config: FuseConfig
    ) {
        // Set the default config from the user provided config (from forRoot)
        this._defaultConfig = _config;

        // Initialize the service
        this._init();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Accessors
    // -----------------------------------------------------------------------------------------------------

    /**
     * Set and get the config
     */
    set config(value) {
        // Get the value from the behavior subject
        let config = this._configSubject?.getValue();

        // Merge the new config
        config = _.merge({}, config, value);

        // Notify the observers
        this._configSubject?.next(config);
    }

    get config(): Observable<FuseConfig> {
        return this._configSubject?.asObservable() ?? new Observable<FuseConfig>();
    }

    /**
     * Get default config
     *
     * @returns {FuseConfig}
     */
    get defaultConfig(): FuseConfig {
        return this._defaultConfig;
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Private methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Initialize
     *
     * @private
     */
    private _init(): void {
        /**
         * Disable custom scrollbars if browser is mobile
         */
        if (this._platform.ANDROID || this._platform.IOS) {
            this._defaultConfig.customScrollbars = false;
        }

        // Set the config from the default config
        this._configSubject = new BehaviorSubject(_.cloneDeep(this._defaultConfig));

        // Reload the default layout config on every RoutesRecognized event
        // if the current layout config is different from the default one
        this._router.events
            .pipe(filter(event => event instanceof ResolveEnd))
            .subscribe(() => {
                if (!_.isEqual(this._configSubject?.getValue().layout, this._defaultConfig.layout)) {
                    // Clone the current config
                    const config = _.cloneDeep(this._configSubject?.getValue());

                    // Reset the layout from the default config
                    if (config !== undefined) {
                        (config as FuseConfig).layout = _.cloneDeep(this._defaultConfig.layout);
                        // Set the config
                        this._configSubject?.next(config as FuseConfig);
                    }
                }
            });
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Set config
     *
     * @param value
     * @param {{emitEvent: boolean}} opts
     */
    setConfig(value: any, opts = { emitEvent: true }): void {
        // Get the value from the behavior subject
        let config = this._configSubject?.getValue();
        this.prevConfig = config ?? this.defaultConfig;
        // Merge the new config
        config = _.merge({}, config, value);

        // If emitEvent option is true...
        if (opts.emitEvent === true && config !== undefined) {
            // Notify the observers
            this._configSubject?.next(config as FuseConfig);
        }
    }


    private prevConfig!: FuseConfig;
    public hidePanels(): void {
      
        this.setConfig({
            layout: {
                navbar: {
                    hidden: true
                },
                toolbar: {
                    hidden: true
                },
                footer: {
                    hidden: true
                },
                sidepanel: {
                    hidden: true
                }
            }
        });
    }

    public restoreConfig(): void {
        this.setConfig(this.prevConfig);
    }

    /**
     * Get config
     *
     * @returns {Observable<FuseConfig>}
     */
    getConfig(): Observable<FuseConfig> {
        return this._configSubject?.asObservable() ?? new Observable<FuseConfig>();
    }

    /**
     * Reset to the default config
     */
    resetToDefaults(): void {
        // Set the config from the default config
        this._configSubject?.next(_.cloneDeep(this._defaultConfig));
    }
}