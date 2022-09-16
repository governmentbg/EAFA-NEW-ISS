import { DialogWrapperComponent } from '@app/shared/components/dialog-wrapper/dialog-wrapper.component';
import { SpinnerService } from '@app/shared/components/spinner/spinner.service';
import { Component, Injector, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { merge, Subscription } from 'rxjs';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DialogInputData, SampleDialogComponent, SampleDialogDataService, SampleDialogResponseService } from './sample-dialog/sample-dialog.component';

@Component({
    selector: 'sample',
    templateUrl: './sample.component.html',
    styleUrls: ['./sample.component.scss']
})
export class SampleComponent implements OnDestroy {
    public testFormGroup: FormGroup;

    public dialogResponseSub?: Subscription;
    public dialogResponseStr?: string;

    constructor(public translationLoader: FuseTranslationLoaderService,
        public spinnerService: SpinnerService,
        private formBuilder: FormBuilder,
        public matDialog: MatDialog,
        private injector: Injector,
        private dialogFormResponseService: SampleDialogResponseService) {
        this.testFormGroup = this.formBuilder.group({
            test: []
        });
    }

    public ngOnDestroy(): void {
        if (this.dialogResponseSub) {
            this.dialogResponseSub.unsubscribe();
        }
    }

    public onFileUploadComplete(data: any): void {
        console.log(data);
    }

    public dialogClosedHandler(): void {
        console.log('dialog has been closed');
    }

    public dialogSavedHandler(): void {
        console.log('dialog has been saved');
    }

    public openDialog(): void {
        const dialogData: DialogInputData = { name: 'Alice' };

        const injector = Injector.create({
            providers: [
                {
                    provide: SampleDialogDataService,
                    useValue: {
                        dialogData
                    }
                }
            ],
            parent: this.injector
        });

        //const data: DialogData<SampleDialogComponent> = {
        //		title: 'Dialog Title',
        //		component: SampleDialogComponent,
        //		TCtor: SampleComponent,
        //		headerCancelButton: { tooltip: '', callback: this.dialogClosedHandler }, // undefined,
        //		injector,
        //		leftSideActionsCollection: [{
        //				id: 'cancel-button-id',
        //				translateValue: 'cancel',
        //				callback: this.dialogClosedHandler,
        //				customClass: 'save-button',
        //				color: 'primary'
        //		}],
        //		rightSideActionsCollection: [{
        //				id: 'save-button-id',
        //				translateValue: 'save',
        //				callback: this.dialogSavedHandler,
        //				customClass: 'save-button',
        //				color: 'accent'
        //		}]
        //};

        //const dialogRef = this.matDialog.open(DialogWrapperComponent, {
        //		width: '400px',
        //		data
        //});

        //this.dialogResponseSub = merge(
        //		this.dialogFormResponseService.dialogResponse$,
        //		dialogRef.afterClosed()).subscribe(response => {
        //				dialogRef.close(); // This closes the dialog box
        //				if (this.dialogResponseSub) {
        //						this.dialogResponseSub.unsubscribe();
        //				}
        //				if (response) {
        //						this.dialogResponseStr = response.name + ' likes to eat ' + response.favouriteFood;
        //				}
        //		});
    }
}