import { SimpleAuditDTO } from "@app/models/generated/dtos/SimpleAuditDTO";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { DateUtils } from "../../utils/date.utils";

@Component({
    selector: 'tl-audit',
    templateUrl: './tl-audit.component.html',
})
export class TLAuditComponent {
    public auditFormGroup: FormGroup;
    constructor() {
        this.auditFormGroup = new FormGroup({
            createdByControl: new FormControl(''),
            createdOnControl: new FormControl(''),
            updatedByControl: new FormControl(''),
            updatedOnControl: new FormControl('')
        });
    }

    @Output() public openAudit: EventEmitter<void> = new EventEmitter<void>();
    @Output() public openDetailedAudit: EventEmitter<void> = new EventEmitter<void>();

    @Input() public set auditInfo(value: SimpleAuditDTO) {
        if (value != undefined) {
            this.auditFormGroup.controls.createdByControl.setValue(value.createdBy);
            this.auditFormGroup.controls.createdOnControl.setValue(DateUtils.ToDisplayDateTime(value.createdOn));
            this.auditFormGroup.controls.updatedByControl.setValue(value.updatedBy);
            this.auditFormGroup.controls.updatedOnControl.setValue(DateUtils.ToDisplayDateTime(value.updatedOn));
        }
    }

    public onTogglePopover(isOpened: boolean): void {
        if (isOpened) {
            this.openAudit.emit();
        }
    }

    public emitDetailedAudit(): void {
        this.openDetailedAudit.emit();
    }
}