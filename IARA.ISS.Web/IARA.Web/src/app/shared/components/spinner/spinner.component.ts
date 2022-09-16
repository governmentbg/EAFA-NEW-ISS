import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
    selector: 'spinner',
    templateUrl: 'spinner.component.html'
})
export class SpinnerComponent {
    constructor(public dialogRef: MatDialogRef<SpinnerComponent>) { }
}