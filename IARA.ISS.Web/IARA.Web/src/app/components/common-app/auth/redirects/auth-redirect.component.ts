import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SECURITY_SERVICE_TOKEN } from '../di/auth-di.tokens';
import { ISecurityService } from '../interfaces/security-service.interface';

@Component({
    selector: 'redirect',
    template: '',
})
export class AuthRedirectComponent implements OnInit {

    private router: Router;
    private securityService!: ISecurityService;

    public constructor(router: Router, @Inject(SECURITY_SERVICE_TOKEN) securityService: ISecurityService) {
        
        this.router = router;
        this.securityService = securityService;
    }

    public async ngOnInit(): Promise<void> {
        
        const path: string = await this.securityService.getUserRedirectPath();
        const skip: boolean = this.router.url === '/';
        this.router.navigate([path], { skipLocationChange: skip });
    }
}