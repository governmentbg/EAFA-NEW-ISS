import { Component, EventEmitter, Output } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    selector: 'public-page-footer',
    templateUrl: './public-page-footer.component.html',
    styleUrls: ['./public-page-footer.component.scss'],
})
export class PublicPageFooterComponent {
    @Output()
    public goToSignInClicked: EventEmitter<void> = new EventEmitter<void>();

    private readonly router: Router;

    public constructor(router: Router) {
        this.router = router;
    }

    public onGoToSignInClicked(): void {
        this.goToSignInClicked.emit();
        this.router.navigate(['/account/sign-in']);
    }
}
