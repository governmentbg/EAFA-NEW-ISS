import { Component, Inject, OnInit } from '@angular/core';
import { MatSnackBarRef, MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';

@Component({
  selector: 'app-warning-snackbar',
  templateUrl: './warning-snackbar.component.html',
  styleUrls: [ './warning-snackbar.component.scss'  ]
})
export class WarningSnackbarComponent {

    constructor(@Inject(MAT_SNACK_BAR_DATA) data: string,
        snackRef: MatSnackBarRef<WarningSnackbarComponent>) {
        this.message = data;
        this.snackRef = snackRef;
    }

    public message: string;
    private snackRef: MatSnackBarRef<WarningSnackbarComponent>

    public closeSnackbar(): void {
        this.snackRef.dismiss();
    }

}
