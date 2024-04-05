import { Injectable } from '@angular/core';
import Category from '../../interfaces/Category.interface';
import ProductDefinition from '../../interfaces/ProductDefinition.interface';

@Injectable({
  providedIn: 'root'
})
export class CatalogService {

  url: string = 'https://cadflair.com/';


  getCategoriesBySubsriptionId(): Category[] {

    return [];
  }

  getProductDefinitionsBySubscriptionId(): ProductDefinition[] {

    return [];
  }

}
