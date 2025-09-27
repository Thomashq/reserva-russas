import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { isDevMode } from '@angular/core';

export function getBaseUrl_Version() {
  if (isDevMode())
    return "v1";
  else
    return "v1";
}

export function getBaseUrl() {
  if(isDevMode())
    return "https://localhost:7099/api/" + getBaseUrl_Version() + "/";
  else
    return "https://localhost:7099/api/" + getBaseUrl_Version() + "/";
}
const providers = [
  { provide: 'BASE_URL', useValue: getBaseUrl() }
];

bootstrapApplication(AppComponent, {
  ...appConfig,
  providers: [
    ...(appConfig.providers ?? []),
    ...providers,
  ],
}).catch(err => console.error(err));
