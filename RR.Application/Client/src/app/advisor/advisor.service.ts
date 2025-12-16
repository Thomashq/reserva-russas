import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { inject } from "@angular/core/testing";


@Injectable ({ providedIn: 'root'})

export class AdvisorService {
  constructor(
    public http: HttpClient,
    @Inject('BASE_URL') private baseurlApi: string
  ){ }

}
