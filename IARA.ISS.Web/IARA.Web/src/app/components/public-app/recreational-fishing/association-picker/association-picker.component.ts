import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RecreationalFishingPublicService } from '@app/services/public-app/recreational-fishing-public.service';

@Component({
    selector: 'association-picker',
    templateUrl: './association-picker.component.html',
    styleUrls: ['./association-picker.component.scss']
})
export class AssociationPickerComponent implements OnInit {
    @Output()
    public associationChosen = new EventEmitter<NomenclatureDTO<number>>();

    public associations!: NomenclatureDTO<number>[];

    private service: RecreationalFishingPublicService;

    public constructor(service: RecreationalFishingPublicService) {
        this.service = service;
    }

    public ngOnInit(): void {
        this.service.currentUserAssociations.subscribe({
            next: (assocs: NomenclatureDTO<number>[]) => {
                this.associations = assocs;

                if (this.associations.length === 1) {
                    this.associationChosen.emit(this.associations[0]);
                }
            }
        });

        this.service.getUserAssociations().subscribe();
    }

    public chooseAssociation(association: NomenclatureDTO<number>): void {
        this.associationChosen.emit(association);
    }
}