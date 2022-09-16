
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class NewsImageDTO {
    public constructor(obj?: Partial<NewsImageDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public newsId?: number;

    @StrictlyTyped(String)
    public image?: string;
}