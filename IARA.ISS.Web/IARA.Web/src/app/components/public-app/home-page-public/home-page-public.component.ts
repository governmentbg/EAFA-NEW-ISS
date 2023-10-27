import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ApplicationTypeDTO } from '@app/models/generated/dtos/ApplicationTypeDTO';
import { ApplicationsPublicService } from '@app/services/public-app/applications-public.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { AuthService } from '@app/shared/services/auth.service';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { HomePageVideoModel } from './models/home-page-video.params';

@Component({
    selector: 'home-page-public',
    templateUrl: './home-page-public.component.html',
    styleUrls: ['./home-page-public.component.scss']
})
export class HomePagePublicComponent implements OnInit {
    public isAuthenticated: boolean = false;
    public applicationTypes: ApplicationTypeDTO[] = [];
    public videos: HomePageVideoModel[] = []; //TODO

    @ViewChild('table')
    private table!: TLDataTableComponent;

    private router: Router;
    private authService: AuthService;
    private readonly applicationsService: ApplicationsPublicService;
    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        router: Router,
        authService: AuthService,
        applicationsService: ApplicationsPublicService,
        translate: FuseTranslationLoaderService
    ) {
        this.router = router;
        this.authService = authService;
        this.applicationsService = applicationsService;
        this.translate = translate;

        this.authService.isAuthenticatedEvent.subscribe({
            next: (result: boolean) => {
                this.isAuthenticated = result;
            }
        });
    }

    public ngOnInit(): void {
        this.applicationsService.getApplicationTypesForChoice().subscribe((result: ApplicationTypeDTO[]) => {
            setTimeout(() => {
                this.applicationTypes = result;
            });
        });
    }

    public login(): void {
        if (!this.isAuthenticated) {
            this.authService.login();
        }
    }

    public redirectFromApplicationType(): void {
        if (this.isAuthenticated) {
            this.router.navigate(['/submitted-applications']);
        }
        else {
            this.router.navigate(['/registration']);
        }
    }

    public redirectToRegistration(): void {
        this.router.navigate(['/registration']);
    }

    public toggleExpandGroup(group: { key: number, value: ApplicationTypeDTO[] }): void {
        this.table.toggleExandGroup(group);
    }
}