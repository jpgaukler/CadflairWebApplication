import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    ButtonModule,
    CardModule,
    RouterLink
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

  logoPath: string = 'assets/cadflair_logo.svg';

  // services
  private router: Router = inject(Router);

  onDemoClick(): void {
    this.router.navigate(['demo','categories']);
  }
}
