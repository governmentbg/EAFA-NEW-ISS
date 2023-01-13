import { Component, Input } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
    selector: 'spinner',
    templateUrl: 'spinner.component.html'
})
export class SpinnerComponent {
    @Input()
    public diameter: number = 100;

    @Input()
    public color: string = 'accent'

    public constructor(public dialogRef: MatDialogRef<SpinnerComponent>) { }
}