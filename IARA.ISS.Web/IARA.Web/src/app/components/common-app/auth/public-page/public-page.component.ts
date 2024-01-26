import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { FuseConfigService } from '@fuse/services/config.service';

@Component({
    selector: 'public-page',
    templateUrl: './public-page.component.html',
    styleUrls: ['./public-page.component.scss'],
})
export class PublicPageComponent implements OnInit, OnDestroy {
    constructor(private fuseConfigService: FuseConfigService) {

    }

    public ngOnInit(): void {
        this.fuseConfigService.hidePanels();
    }

    public ngOnDestroy(): void {
        this.fuseConfigService.restoreConfig();
    }



}
