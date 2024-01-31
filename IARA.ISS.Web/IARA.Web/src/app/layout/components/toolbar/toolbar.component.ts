import { Component, Inject, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivationEnd, Router } from '@angular/router';
import { SECURITY_SERVICE_TOKEN } from '@app/components/common-app/auth/di/auth-di.tokens';
import { ISecurityService } from '@app/components/common-app/auth/interfaces/security-service.interface';
import { StorageTypes } from '@app/shared/enums/storage-types.enum';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { ITLNavigation } from '@app/shared/navigation/base/tl-navigation.interface';
import { StorageService } from '@app/shared/services/local-storage.service';
import { MessageService } from '@app/shared/services/message.service';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { FuseSidebarService } from '@fuse/components/sidebar/sidebar.service';
import { FuseConfigService } from '@fuse/services/config.service';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { FuseNavigation } from '@fuse/types/fuse-navigation';
import { TranslateService } from '@ngx-translate/core';
import * as _ from 'lodash';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
    selector: 'toolbar',
    templateUrl: './toolbar.component.html',
    styleUrls: ['./toolbar.component.scss'],
    encapsulation: ViewEncapsulation.None
})

export class ToolbarComponent implements OnInit, OnDestroy {
    horizontalNavbar: boolean;
    rightNavbar: boolean;
    hiddenNavbar: boolean;
    languages: { id: string; title: string; flag: string }[];
    navigation: FuseNavigation[];
    selectedLanguage?: { id: string; title: string; flag: string };
    userStatusOptions: { title: string; icon: string; color: string }[];

    public pageTitle: string = '';
    public usernames: string = '';
    public userPhoto: string = "assets/images/avatars/profile.jpg";
    public isInternalApp: boolean = !IS_PUBLIC_APP;

    // Private
    private _unsubscribeAll: Subject<any>;

    public showUserTickets: boolean = false;
    public isAuthenticated: boolean = false;
    /**
     * Constructor
     *
     * @param {FuseConfigService} _fuseConfigService
     * @param {FuseSidebarService} _fuseSidebarService
     * @param {TranslateService} _translateService
     */
    constructor(private _fuseConfigService: FuseConfigService,
        private fuseSidebarService: FuseSidebarService,
        private translateService: TranslateService,
        private translationLoader: FuseTranslationLoaderService,
        private router: Router,
        @Inject(SECURITY_SERVICE_TOKEN) private securityService: ISecurityService,
        private messageService: MessageService) {
        this.securityService.isAuthenticatedEvent.subscribe((result: boolean) => {
            this.isAuthenticated = result;
        });

        router.events.subscribe({
            next: (event) => {
                if (event instanceof ActivationEnd) {
                    if (CommonUtils.isNullOrEmpty(this.messageService.getMessageCurrentValue())) {
                        const resource = ((event as ActivationEnd).snapshot.data as ITLNavigation).translate;
                        this.pageTitle = this.translationLoader.getValue(resource);
                    }
                }
            }
        });

        this.messageService.getMessage().subscribe(message => {
            if (!CommonUtils.isNullOrUndefined(message)) {
                this.pageTitle = message!.text ?? '';
            }
        });

        // Set the defaults
        this.horizontalNavbar = false;
        this.rightNavbar = false;
        this.hiddenNavbar = false;
        this.userStatusOptions = [
            {
                title: 'Online',
                icon: 'icon-checkbox-marked-circle',
                color: '#4CAF50'
            },
            {
                title: 'Away',
                icon: 'icon-clock',
                color: '#FFC107'
            },
            {
                title: 'Do not Disturb',
                icon: 'icon-minus-circle',
                color: '#F44336'
            },
            {
                title: 'Invisible',
                icon: 'icon-checkbox-blank-circle-outline',
                color: '#BDBDBD'
            },
            {
                title: 'Offline',
                icon: 'icon-checkbox-blank-circle-outline',
                color: '#616161'
            }
        ];

        this.languages = [
            {
                id: 'en',
                title: 'English',
                flag: 'us'
            },
            {
                id: 'bg',
                title: 'Български',
                flag: 'bg'
            }
        ];
        this.navigation = [];

        // Set the private defaults
        this._unsubscribeAll = new Subject();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On init
     */
    ngOnInit(): void {
        // Subscribe to the config changes
        this._fuseConfigService.config
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe((settings) => {
                this.horizontalNavbar = settings.layout.navbar.position === 'top';
                this.rightNavbar = settings.layout.navbar.position === 'right';
                this.hiddenNavbar = settings.layout.navbar.hidden === true;
            });

        // Set the selected language from default languages
        this.selectedLanguage = _.find(this.languages, { id: this.translateService.currentLang });

        //this.authService.userRegistrationInfoEvent.subscribe({
        //    next: (userInfo: UserAuthDTO | null) => {
        //        if (userInfo !== null) {
        //            if (userInfo.id) {
        //                this.showUserTickets = IS_PUBLIC_APP && this.permissions.has(PermissionsEnum.TicketsPublicRead);

        //                this.authService.getUserPhoto(userInfo.id).subscribe((photo: string) => {
        //                    if (photo) {
        //                        this.userPhoto = photo;
        //                    }
        //                });
        //            }

        //            this.usernames = `${userInfo.firstName} ${userInfo.lastName}`;
        //        }
        //    }
        //});
    }

    public logout(): void {
        this.router.navigate(['/account/sign-out']);
    }

    public login(): void {
        this.router.navigate(['account']);
    }

    public goToMyProfile(): void {
        this.router.navigate(['/my-profile']);
    }

    /**
     * On destroy
     */
    ngOnDestroy(): void {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next();
        this._unsubscribeAll.complete();
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

    /**
     * Search
     *
     * @param value
     */
    search(value: string): void {
        // Do your search here...
        console.log(value);
    }

    /**
     * Set the language
     *
     * @param lang: { id: string; title: string; flag: string }
     */
    async setLanguage(lang: { id: string; title: string; flag: string }): Promise<void> {
        // Set the selected language
        this.selectedLanguage = lang;
        StorageService.getStorage(StorageTypes.Local).addOrUpdate('lang', this.selectedLanguage.id);
        location.reload();
        // Use the selected language for translations
        //const web = new WebTranslateLoader(this.requestService);
        //const translation = await web.getTranslation(this.selectedLanguage.id).toPromise();
        //const locales = [new Translation(this.selectedLanguage.id, translation)];
        //this.translationLoader.loadTranslations(locales[0]);
        //this.translateService.use(this.selectedLanguage.id);
    }

    private getUsernamesFromLocalStorage(): string {
        const names: string | null = localStorage.getItem('names');
        return names === null ? '' : names;
    }
}