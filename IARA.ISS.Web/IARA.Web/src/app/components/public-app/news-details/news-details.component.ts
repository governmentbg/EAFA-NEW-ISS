import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { NewsDetailsDTO } from '../../../models/generated/dtos/NewsDetailsDTO';
import { NewsPublicService } from '../../../services/public-app/news-public.service';

@Component({
    selector: 'news-details',
    templateUrl: './news-details.component.html'
})
export class NewsDetailsComponent implements OnInit, OnDestroy {
    private routeSubscription!: Subscription;
    private route: ActivatedRoute;
    private newsService: NewsPublicService;
    private router: Router;

    public news!: NewsDetailsDTO;

    public constructor(route: ActivatedRoute, newsService: NewsPublicService, router: Router) {
        this.route = route;
        this.router = router;
        this.newsService = newsService;
    }

    public ngOnInit(): void {
        this.routeSubscription = this.route.params.subscribe(params => {
            const id: number = parseInt(params['id']);

            this.newsService.getPublishedNews(id).subscribe({
                next: (result: NewsDetailsDTO) => {
                    this.news = result;
                },
                error: (e: HttpErrorResponse) => {
                    this.goToNews();
                }
            });
        });
    }

    public ngOnDestroy(): void {
        this.routeSubscription.unsubscribe();
    }

    public fileNameClicked(fileId: number, fileName: string): void {
        this.newsService.downloadFile(fileId, fileName);
    }

    private goToNews(): void {
        this.router.navigateByUrl('/news');
    }
}