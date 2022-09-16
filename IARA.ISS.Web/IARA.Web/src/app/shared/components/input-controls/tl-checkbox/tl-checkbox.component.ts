import { Component, ContentChild, Input, OnInit, Optional, Self, TemplateRef } from '@angular/core';
import { NgControl } from '@angular/forms';
import { pairwise, startWith } from 'rxjs/operators';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLTranslatePipe } from '../../../pipes/tl-translate.pipe';
import { BaseTLControl } from '../base-tl-control';

@Component({
    selector: 'tl-checkbox',
    templateUrl: './tl-checkbox.component.html'
})
export class TLCheckboxComponent extends BaseTLControl implements OnInit {
    @Input()
    public isThreeState: boolean = false;

    @ContentChild(TemplateRef)
    public content: any | undefined;

    public isIndeterminate: boolean = false;


    constructor(@Self() @Optional() ngControl: NgControl, fuseTranslationService: FuseTranslationLoaderService, tlTranslatePipe: TLTranslatePipe) {
        super(ngControl, fuseTranslationService, tlTranslatePipe);
    }

    public ngOnInit(): void {
        super.ngOnInit();

        this.ngControl.control?.valueChanges.subscribe({
            next: () => {
                this.ngControl.control?.markAsTouched();
            }
        });

        if (this.isThreeState) {
            if (this.ngControl.control?.value === null) {
                this.isIndeterminate = true;
            }
            this.ngControl.control?.valueChanges?.pipe(startWith(null), pairwise()).subscribe({
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