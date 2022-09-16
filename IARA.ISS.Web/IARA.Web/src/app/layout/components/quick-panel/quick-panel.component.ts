import { Component, ViewEncapsulation } from '@angular/core';

@Component({
    selector: 'quick-panel',
    templateUrl: './quick-panel.component.html',
    styleUrls: ['./quick-panel.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class QuickPanelComponent {
    date: Date;
    events: { title: string; detail: string }[] = [];
    notes: { title: string; detail: string }[] = [];
    settings: { notify: boolean; cloud: boolean; retro: boolean };

    /**
     * Constructor
     */
    constructor() {
        // Set the defaults
        this.date = new Date();
        this.settings = {
            notify: true,
            cloud: false,
            retro: true
        };
    }
}