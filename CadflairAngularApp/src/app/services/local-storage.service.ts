import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {

  /**
   * Set a value in local storage. If the value is an object, don't forget to use JSON.stringify(value).
   * @param key
   * @param value If the value is an object, don't forget to use JSON.stringify(value).
   */
  setItem(key: string, value: string): void {
    localStorage.setItem(key, value);
  }

  getItem(key: string): string | null {
    return localStorage.getItem(key);
  }

  removeItem(key: string): void {
    localStorage.removeItem(key);
  }

  /** Clears all items from local storage. */
  clear(): void {
    localStorage.clear();
  }

}
