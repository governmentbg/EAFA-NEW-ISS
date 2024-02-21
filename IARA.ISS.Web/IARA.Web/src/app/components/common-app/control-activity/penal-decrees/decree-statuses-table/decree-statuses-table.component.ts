import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PenalDecreeStatusEditDTO } from '@app/models/generated/dtos/PenalDecreeStatusEditDTO';
import { PenalDecreeStatusRecordChanged } from '@app/components/administration-app/control-activity/penal-decrees/models/penal-decree-status-record-changed.model';

@Component({
    selector: 'decree-statuses-table',
    templateUrl: './decree-statuses-table.component.html'
})
export class DecreeStatusesTableComponent {
    @Input()
    public rows: PenalDecreeStatusEditDTO[] = [];

    @Input()
    public showAddButton: boolean = false;

    @Input()
    public canEditPenalDecreeStatuses: boolean = false;

    @Input()
    public canDeletePenalDecreeStatuses: boolean = false;

    @Input()
    public canRestorePenalDecreeStatuses: boolean = false;

    @Input()
    public recordsPerPage: number = 10;

    @Output()
    public onActiveRecordChanged: EventEmitter<PenalDecreeStatusRecordChanged> = new EventEmitter<PenalDecreeStatusRecordChanged>();

    @Output()
    public onEditPenalDecreeStatus: EventEmitter<PenalDecreeStatusEditDTO> = new EventEmitter<PenalDecreeStatusEditDTO>();

    @Output()
    public onViewPenalDecreeStatus: EventEmitter<PenalDecreeStatusEditDTO> = new EventEmitter<PenalDecreeStatusEditDTO>();

    @Output()
    public onDeletePenalDecreeStatus: EventEmitter<PenalDecreeStatusEditDTO> = new EventEmitter<PenalDecreeStatusEditDTO>();

    @Output()
    public onRestorePenalDecreeStatus: EventEmitter<PenalDecreeStatusEditDTO> = new EventEmitter<PenalDecreeStatusEditDTO>();

    public activeRecordChangedEvent(status: PenalDecreeStatusEditDTO, viewMode: boolean): void {
        this.onActiveRecordChanged.emit(
            new PenalDecreeStatusRecordChanged({
                status: status,
                viewMode: viewMode
            })
        );
    }

    public editPenalDecreeStatus(status: PenalDecreeStatusEditDTO | undefined, viewMode: boolean): void {
        if (viewMode) {
            this.onViewPenalDecreeStatus.emit(status);
        }
        else {
            this.onEditPenalDecreeStatus.emit(status);
        }
    }

    public deletePenalDecreeStatus(status: PenalDecreeStatusEditDTO): void {
        this.onDeletePenalDecreeStatus.emit(status);
    }

    public undoDeletePenalDecreeStatus(status: PenalDecreeStatusEditDTO): void {
        this.onRestorePenalDecreeStatus.emit(status);
    }
}