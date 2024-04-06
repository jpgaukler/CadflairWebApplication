import { Injectable } from '@angular/core';
import Category from '../../interfaces/Category.interface';
import ProductDefinition from '../../interfaces/ProductDefinition.interface';
import Subscription from '../../interfaces/Subscription.interface';

@Injectable({
  providedIn: 'root'
})
export class CatalogService {

  baseUrl: string = 'https://cadflairrestapi.azurewebsites.net/api/v1';

  async getSubscriptionByCompanyName(companyName:string): Promise<Subscription | undefined> {
    const response = await fetch(`${this.baseUrl}/subscriptions/${companyName}`);

    if (response.status !== 200)
      return undefined;

    const subscription = await response.json();

    return subscription;
  }


  async getCategoriesBySubsriptionId(subscriptionId:number): Promise<Category[]> {
    const response = await fetch(`${this.baseUrl}/subscriptions/${subscriptionId}/categories`);

    if (response.status !== 200)
      return [];

    const categories = await response.json();

    return categories;
  }

  async getProductDefinitionsBySubscriptionId(subscriptionId:number): Promise<ProductDefinition[]> {
    const response = await fetch(`${this.baseUrl}/subscriptions/${subscriptionId}/product-definitions`);

    if (response.status !== 200)
      return [];

    const productDefinitions = await response.json();

    return productDefinitions;
  }

  async getProductDefinitionsBySubscriptionIdAndProductDefinitionName(subscriptionId:number, productDefinitionName:string): Promise<ProductDefinition | undefined> {
    const response = await fetch(`${this.baseUrl}/subscriptions/${subscriptionId}/product-definitions/${productDefinitionName}`);

    if (response.status !== 200)
      return undefined

    const productDefinition = await response.json();
    return productDefinition;
  }

}
