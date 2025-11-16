import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type AppTheme = 'light' | 'dark';

const THEME_KEY = 'rr-theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private themeSubject = new BehaviorSubject<AppTheme>('light');
  theme$ = this.themeSubject.asObservable();

  constructor() {
    const stored = localStorage.getItem(THEME_KEY) as AppTheme | null;
    if (stored === 'light' || stored === 'dark') {
      this.themeSubject.next(stored);
    }
  }

  get theme(): AppTheme {
    return this.themeSubject.value;
  }

  setTheme(theme: AppTheme) {
    this.themeSubject.next(theme);
    localStorage.setItem(THEME_KEY, theme);
  }

  toggle() {
    this.setTheme(this.theme === 'light' ? 'dark' : 'light');
  }
}
