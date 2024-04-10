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

  async getObjectUrn(bucketKey: string, objectKey: string): Promise<any> {
    const response = await fetch(`${this.baseUrl}/forge/buckets/${bucketKey}/objects/${objectKey}/urn`);
    const data = await response.json();

    if (response.status !== 200)
      return undefined;

    return data;
  }

  async getSignedUrl(forgeBucketKey: string, forgeObjectKey: string): Promise<string> {
    const response = await fetch(`${this.baseUrl}/forge/buckets/${forgeBucketKey}/objects/${forgeObjectKey}/signed-url`);
    const data = await response.json();
    const url: string = data.url;

    if (response.status !== 200)
      return '';

    return url;
  }

}
