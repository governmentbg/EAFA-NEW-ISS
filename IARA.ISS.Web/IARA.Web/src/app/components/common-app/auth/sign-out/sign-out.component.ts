import { AfterViewInit, Component, Inject, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { FuseConfigService } from '@fuse/services/config.service';
import { Subject, Subscription, timer } from 'rxjs';
import { finalize, takeUntil, takeWhile, tap } from 'rxjs/operators';
import { SECURITY_SERVICE_TOKEN } from '../di/auth-di.tokens';
import { ISecurityService } from '../interfaces/security-service.interface';


@Component({
    selector: 'auth-sign-out',
    templateUrl: './sign-out.component.html',
    styleUrls: ['./sign-out.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class AuthSignOutComponent implements OnInit, OnDestroy {
    countdown: number = 5;
    countdownMapping: any = {
        '=1': '# секунда',
        'other': '# секунди'
    };

    private _unsubscribeAll: Subject<any> = new Subject<any>();

    private securityService: ISecurityService;
    private router: Router;
    private fuseConfigService: FuseConfigService;
    private subscriptions: Subscription[] = [];

    public constructor(@Inject(SECURITY_SERVICE_TOKEN) securityService: ISecurityService,
        router: Router,
        fuseConfigService: FuseConfigService) {
        this.securityService = securityService;
        this.fuseConfigService = fuseConfigService;
        this.router = router;
    }

    public ngOnInit(): void {
        // Sign out
        this.subscriptions.push(this.securityService.logout().subscribe(() => {
            this.subscriptions.push(timer(1000, 1000)
                .pipe(
                    finalize(() => {
                        NomenclatureStore.instance.clearAllNomenclatures();
                        this.router.navigateByUrl('/');
                    }),
                    takeWhile(() => this.countdown > 0),
                    takeUntil(this._unsubscribeAll),
                    tap(() => this.countdown--)
                ).subscribe());
        }));

        this.fuseConfigService.hidePanels();
    }


    public ngOnDestroy(): void {
        for (const item of this.subscriptions) {
            item.unsubscribe();
        }

        this.fuseConfigService.restoreConfig();
    }
}
