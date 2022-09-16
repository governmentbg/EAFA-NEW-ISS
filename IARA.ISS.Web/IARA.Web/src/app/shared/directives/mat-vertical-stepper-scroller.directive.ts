import { Directive, HostListener, Input } from '@angular/core';
import { MatStepper } from '@angular/material/stepper';

@Directive({
    selector: '[matVerticalStepperScroller]'
})
export class MatVerticalStepperScrollerDirective {
    @Input()
    public scrollToCurrentStep: boolean = false;

    private stepper: MatStepper;

    public constructor(stepper: MatStepper) {
        this.stepper = stepper;
    }

    @HostListener('animationDone')
    public selectionChanged(): void {
        if (this.scrollToCurrentStep) {
            const stepId: string = this.stepper._getStepLabelId(this.stepper.selectedIndex);
            const stepElement: HTMLElement | null = document.getElementById(stepId);

            if (stepElement !== null) {
                stepElement.scrollIntoView(true); // maybe switch to a smooth animation
            }
        }
        else {
            setTimeout(() => {
                // simulate a click to body to scroll to top
                document.querySelector('body')!.click();
            });
        }
    }
}