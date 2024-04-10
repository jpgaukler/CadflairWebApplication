import { Component, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { PrimeNGConfig } from 'primeng/api';
import { LocalStorageService } from './services/local-storage.service';
import { ThemeService } from './services/theme.service';
import { ButtonModule } from 'primeng/button';
import { SidebarModule } from 'primeng/sidebar';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    ButtonModule,
    SidebarModule
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {

  appBarHeight: string = '60px';
  sidebarVisible: boolean = false;

  // services
  private primengConfig: PrimeNGConfig = inject(PrimeNGConfig)
  private localStorageService: LocalStorageService = inject(LocalStorageService)
  private themeService: ThemeService = inject(ThemeService)
  private router: Router = inject(Router);


  constructor() {
    this.primengConfig.ripple = true;
    this.themeService.setTheme('cadflair');

  //  const theme = this.localStorageService.getItem(this.themeService.themeSetting);

  //  if (theme) {
  //    this.themeService.setTheme(theme);
  //  }
  }


  changeTheme(theme: string) {
    this.themeService.setTheme(theme);
  }

  onHomeClick() {
    this.sidebarVisible = false;
    this.router.navigate(['/']);
  }

  onDemoClick() {
    this.sidebarVisible = false;
    this.router.navigate(['/demo/categories']);
  }

  onContactUsClick() {
    this.sidebarVisible = false;
    this.router.navigate(['/']);

    setTimeout(() => document.getElementById("contact-us")?.scrollIntoView({
      behavior: "smooth",
      block: "start",
      inline: "nearest"
    }), 500);
  }

}
