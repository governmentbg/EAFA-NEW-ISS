﻿import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ApplicationTypeDTO } from '@app/models/generated/dtos/ApplicationTypeDTO';
import { SecurityService } from '@app/services/common-app/security.service';
import { ApplicationsPublicService } from '@app/services/public-app/applications-public.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { HomePageCardModel } from './models/home-page-card.params';

const USER_MANUAL_URL: string = '../../../assets/user-manual/IARA_ISS_Public_User_Manual.pdf';

@Component({
    selector: 'home-page-public',
    templateUrl: './home-page-public.component.html',
    styleUrls: ['./home-page-public.component.scss']
})
export class HomePagePublicComponent implements OnInit {
    public isAuthenticated: boolean = false;
    public applicationTypes: ApplicationTypeDTO[] = [];
    public cards: HomePageCardModel[] = [];

    @ViewChild('table')
    private table!: TLDataTableComponent;

    private router: Router;
    private securityService: SecurityService;
    private readonly applicationsService: ApplicationsPublicService;
    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        router: Router,
        securityService: SecurityService,
        applicationsService: ApplicationsPublicService,
        translate: FuseTranslationLoaderService
    ) {
        this.router = router;
        this.securityService = securityService;
        this.applicationsService = applicationsService;
        this.translate = translate;

        this.initializeFooterCards();

        this.securityService.isAuthenticatedEvent.subscribe({
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
            this.router.navigate(['/account/sign-in']);
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

    private initializeFooterCards(): void {
        this.cards = [
            new HomePageCardModel({
                title: this.translate.getValue('home-page-public.tickets-video'),
                tooltipText: this.translate.getValue('home-page-public.tickets-video'),
                url: 'https://www.youtube.com/watch?v=Rewqa0xUgMw'
            }),
            new HomePageCardModel({
                title: this.translate.getValue('home-page-public.associations-video'),
                tooltipText: this.translate.getValue('home-page-public.associations-video'),
                url: 'https://www.youtube.com/watch?v=zqZLrxKBnSI'
            }),
            new HomePageCardModel({
                title: this.translate.getValue('home-page-public.user-manual-pdf'),
                tooltipText: this.translate.getValue('home-page-public.user-manual-pdf'),
                url: USER_MANUAL_URL,
                icon: 'fa-file-alt',
                shouldDownload: true
            })
        ]
    }
}