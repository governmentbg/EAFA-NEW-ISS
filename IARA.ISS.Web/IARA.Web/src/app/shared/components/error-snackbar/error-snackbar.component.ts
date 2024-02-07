import { ErrorModel } from '@app/models/common/exception.model';
import { Component, Inject } from '@angular/core';
import { MatSnackBarRef, MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';

@Component({
    selector: 'tl-error-snackbar',
    templateUrl: './error-snackbar.component.html'
})
export class ErrorSnackbarComponent {
    constructor(@Inject(MAT_SNACK_BAR_DATA) data: ErrorModel, snackRef: MatSnackBarRef<ErrorSnackbarComponent>) {
        this.model = data;
        this.snackRef = snackRef;
    }

    public model: ErrorModel;
    private snackRef: MatSnackBarRef<ErrorSnackbarComponent>;

    public closeSnackbar() {
        this.snackRef.dismiss();
    }
}