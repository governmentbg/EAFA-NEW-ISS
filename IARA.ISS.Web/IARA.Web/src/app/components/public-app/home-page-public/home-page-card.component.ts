import { Component, Input } from '@angular/core';

@Component({
    selector: 'home-page-card',
    templateUrl: './home-page-card.component.html',
    styleUrls: ['./home-page-public.component.scss']
})
export class HomePageCardComponent {
    @Input()
    public url: string;

    @Input()
    public title: string;

    @Input()
    public tooltipText: string;

    @Input()
    public icon: string = 'fa-play-circle'

    @Input()
    public shouldDownload: boolean = false;

    public constructor() {
        this.url = '';
        this.title = '';
        this.tooltipText = '';
    }

    public onRedirect(): void {
        if (!this.shouldDownload) {
            window.open(this.url);
        }
        else {
            const link = document.createElement('a');
            link.download = this.title;
            link.href = this.url;
            link.click();
            link.remove();
        }
    }
}