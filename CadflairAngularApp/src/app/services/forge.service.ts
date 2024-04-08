import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ForgeService {

  baseUrl: string = 'https://cadflairrestapi.azurewebsites.net/api/v1';

  //async getPublicToken() {
  //  const response = await fetch(`${this.baseUrl}/forge/token`);
  //  const data = await response.json();

  //  return data;
  //}

  async getObjectUrn(bucketKey:string, objectKey:string) {
    const response = await fetch(`${this.baseUrl}/forge/buckets/${bucketKey}/objects/${objectKey}/urn`);
    const data = await response.json();

    return data;
  }

  async getSignedUrl() {

  }

}
