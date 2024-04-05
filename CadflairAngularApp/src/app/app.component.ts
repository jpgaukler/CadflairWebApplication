import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PrimeNGConfig } from 'primeng/api';
import { LocalStorageService } from './services/local-storage.service';
import { ThemeService } from './services/theme.service';
import { ButtonModule } from 'primeng/button';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    ButtonModule
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  appBarHeight = '60px';

  private primengConfig: PrimeNGConfig = inject(PrimeNGConfig)
  private localStorageService: LocalStorageService = inject(LocalStorageService)
  private themeService: ThemeService = inject(ThemeService)


  constructor() {
    this.primengConfig.ripple = true;

    const theme = this.localStorageService.getItem(this.themeService.themeSetting);

    if (theme)
      this.themeService.setTheme(theme);
  }


  changeTheme(theme: string) {
    this.themeService.setTheme(theme);
  }

}
