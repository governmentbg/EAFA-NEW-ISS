import { Component, Input } from "@angular/core";
import { NewsDTO } from "@app/models/generated/dtos/NewsDTO";
import { Router } from '@angular/router';
import { CommonUtils } from '../../../shared/utils/common.utils';

@Component({
    selector: 'news-card',
    templateUrl: './news-card.component.html',
    styleUrls: ['./news-card.component.scss']
})
export class NewsCardComponent {
    @Input() public news!: NewsDTO;

    private router: Router;

    public constructor(router: Router) {
        this.router = router;
    }

    public detailedNewsClicked(): void {
        const newsId: number | undefined = Number(this.news.id);

        if (!CommonUtils.isNumberNullOrNaN(newsId)) {
            this.router.navigateByUrl(`news/${newsId}`);
        }
    }
}