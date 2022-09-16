import { Component, Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { FormControl, FormGroup } from '@angular/forms';

@Injectable({
    providedIn: 'root'
})
export class SampleDialogDataService {
    public dialogData?: DialogInputData;
}

@Injectable({
    providedIn: 'root'
})
export class SampleDialogResponseService {
    private dialogResponseSubject = new Subject<DialogResponse>();

    dialogResponse$ = this.dialogResponseSubject.asObservable();

    public sendResponse(name: string, favouriteFood: string): void {
        this.dialogResponseSubject.next({
            name,
            favouriteFood
        });
    }

    public cancel(): void {
        this.dialogResponseSubject.next();
    }
}

export interface DialogInputData {
    name: string;
}

export interface DialogResponse {
    name: string;
    favouriteFood: string;
}

@Component({
    selector: 'sample-dialog',
    templateUrl: './sample-dialog.component.html'
})
export class SampleDialogComponent {
    public dialogData = this.dialogFormDataService.dialogData as DialogInputData;
    public dialogForm: FormGroup;

    constructor(
        private dialogFormDataService: SampleDialogDataService,
        private dialogFormResponseService: SampleDialogResponseService) {
        this.dialogForm = new FormGroup({
            faveFood: new FormControl()
        });
    }

    public submitDialogForm(): void {
        if (this.dialogForm.controls.faveFood.value && this.dialogForm.controls.faveFood.value !== '') {
            this.dialogFormResponseService.sendResponse(this.dialogData.name, this.dialogForm.controls.faveFood.value);
        } else {
            alert('Please tell us your favourite food!');
        }
    }

    public cancelDialogForm(): void {
        this.dialogFormResponseService.cancel();
    }
}