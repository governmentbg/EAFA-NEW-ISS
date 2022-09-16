import { Injectable } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';

@Injectable({
    providedIn: 'root'
})
export class FuseMatSidenavHelperService {
    sidenavInstances: MatSidenav[];

    /**
     * Constructor
     */
    constructor() {
        this.sidenavInstances = [];
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Accessors
    // -----------------------------------------------------------------------------------------------------

    /**
     * Set sidenav
     *
     * @param {any} id
     * @param {any} instance
     */
    setSidenav(id: any, instance: any): void {
        this.sidenavInstances[id] = instance;
    }

    /**
     * Get sidenav
     *
     * @param {any} id
     * @returns {any}
     */
    getSidenav(id: any): any {
        return this.sidenavInstances[id];
    }
}