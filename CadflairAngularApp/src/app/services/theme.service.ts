import { DOCUMENT } from '@angular/common';
import { Injectable, inject } from '@angular/core';
import { LocalStorageService } from './local-storage.service';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {

  private document: Document = inject(DOCUMENT);
  private localStorageService: LocalStorageService = inject(LocalStorageService);

  themeSetting: string = 'app-theme-setting';

  setTheme(themeName: string) {
    let themeLink: HTMLLinkElement = this.document.getElementById('app-theme') as HTMLLinkElement;

    if (themeLink)
      themeLink.href = themeName + '.css';

    this.localStorageService.setItem(this.themeSetting, themeName);
  }

}
