import { Component, Input } from '@angular/core';

@Component({
    selector: 'home-page-video',
    templateUrl: './home-page-video.component.html',
    styleUrls: ['./home-page-public.component.scss']
})
export class HomePageVideoComponent {
    @Input()
    public url: string;

    @Input()
    public title: string;

    @Input()
    public tooltipText: string;

    public constructor() {
        this.url = '';
        this.title = '';
        this.tooltipText = '';
    }

    public redirectToVid(): void {
        window.open(this.url);
    }
}