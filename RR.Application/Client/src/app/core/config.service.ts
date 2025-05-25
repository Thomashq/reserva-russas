import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.dev';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {

  constructor() { }
  readonly apiUrl: string = environment.apiUrl;
}
