import { PageCodeEnum } from "@app/enums/page-code.enum";

export class ChooseApplicationDialogParams {

    public pageCodes!: PageCodeEnum[];

    public constructor(obj?: Partial<ChooseApplicationDialogParams>) {
        Object.assign(this, obj);
    }
}
