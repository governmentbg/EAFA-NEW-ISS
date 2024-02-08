import { ChangeDetectionStrategy, Component, Inject } from '@angular/core';
import { MatSnackBarRef, MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';
import { ErrorModel } from '@app/models/common/exception.model';

@Component({
    selector: 'tl-error-snackbar',
    templateUrl: './error-snackbar.component.html',
    styleUrls: ['./error-snackbar.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ErrorSnackbarComponent {
    public constructor(@Inject(MAT_SNACK_BAR_DATA) data: ErrorModel, snackRef: MatSnackBarRef<ErrorSnackbarComponent>) {
        this.model = data;
        this.snackRef = snackRef;
    }

    public model: ErrorModel;
    private snackRef: MatSnackBarRef<ErrorSnackbarComponent>;

    public closeSnackbar(): void {
        this.snackRef.dismiss();
    }
}
