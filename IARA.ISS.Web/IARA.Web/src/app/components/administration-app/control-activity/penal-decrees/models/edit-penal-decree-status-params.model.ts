import { IPenalDecreesService } from '@app/interfaces/administration-app/penal-decrees.interface';
import { PenalDecreeStatusDTO } from '@app/models/generated/dtos/PenalDecreeStatusDTO';
import { PenalDecreeTypeEnum } from '@app/enums/penal-decree-type.enum';

export class EditPenalDecreeStatusDialogParams {
    public service!: IPenalDecreesService;
    public model: PenalDecreeStatusDTO | undefined;
    public viewMode: boolean = false;
    public decreeType: PenalDecreeTypeEnum;
    public penalDecreeId!: number;

    public constructor(
        service: IPenalDecreesService,
        model: PenalDecreeStatusDTO | undefined,
        decreeType: PenalDecreeTypeEnum,
        viewMode: boolean = false,
        penalDecreeId: number
    ) {
        this.service = service;
        this.model = model;
        this.viewMode = viewMode;
        this.decreeType = decreeType;
        this.penalDecreeId = penalDecreeId;
    }
}