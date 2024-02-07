import { Component, ContentChild, ElementRef, Input, OnInit, Optional, Self } from '@angular/core';
import { NgControl } from '@angular/forms';
import { pairwise, startWith } from 'rxjs/operators';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';
import { BaseTLControl } from '../base-tl-control';
import { TLCheckboxTemplateComponent } from './components/tl-checkbox-template/tl-checkbox-template.component';

@Component({
    selector: 'tl-checkbox',
    templateUrl: './tl-checkbox.component.html',
    styleUrls: ['./tl-checkbox.component.scss']
})
export class TLCheckboxComponent extends BaseTLControl implements OnInit {
    @Input()
    public isThreeState: boolean = false;

    /**
     * Should be passed only when there is NO form control -> for one way binding for UI purposes
     * */
    @Input()
    public value: boolean | undefined = undefined;

    @ContentChild(TLCheckboxTemplateComponent)
    public checkboxTemplate: TLCheckboxTemplateComponent | undefined;

    public isIndeterminate: boolean = false;

    public constructor(@Self() @Optional() ngControl: NgControl, fuseTranslationService: FuseTranslationLoaderService, tlTranslatePipe: TLTranslatePipe) {
        super(ngControl, fuseTranslationService, tlTranslatePipe);
    }

    public ngOnInit(): void {
        super.ngOnInit();

        if (this.ngControl !== null && this.ngControl !== undefined) {
            this.ngControl.control?.valueChanges.subscribe({
                next: () => {
                    this.ngControl.control?.markAsTouched();
                }
            });
        }


        if (this.isThreeState) {
            if (this.ngControl.control === null || this.ngControl.control === undefined || this.ngControl.control?.value === null) {
                this.isIndeterminate = true;
            }

            this.ngControl?.control?.valueChanges?.pipe(startWith(null), pairwise()).subscribe({
                next: ([prev, next]: [boolean, boolean]) => {
                    if (prev && !next) {
                        this.isIndeterminate = true;
                        this.ngControl.control?.reset(null, { emitEvent: false });
                    }
                    else if (!prev && this.isIndeterminate) {
                        this.isIndeterminate = false;
                        this.ngControl.control?.setValue(false, { emitEvent: false });
                    }
                }
            });
        }
    }

    public onCheckboxClicked(event: PointerEvent): void {
        if (this.readonly === true) {
            event.preventDefault();
        }
    }
}