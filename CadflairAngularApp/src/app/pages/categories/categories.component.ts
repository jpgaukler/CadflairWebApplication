import { Component, Input, OnInit, inject } from '@angular/core';
import { CatalogService } from '../../services/catalog.service';
import Category from '../../../interfaces/Category.interface';
import Subscription from '../../../interfaces/Subscription.interface';
import ProductDefinition from '../../../interfaces/ProductDefinition.interface';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [
    CommonModule,
  ],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.css'
})
export class CategoriesComponent implements OnInit {

  // inputs
  @Input() companyName!: string;
  @Input() categoryName!: string;

  // services
  private catalogService: CatalogService = inject(CatalogService);
  private router: Router = inject(Router);

  // props
  subscription?: Subscription;
  allCategories: Category[] = [];
  allProductDefinitions: ProductDefinition[] = [];
  categories: Category[] = [];
  productDefinitions: ProductDefinition[] = [];


  async ngOnInit(): Promise<void> {
    this.subscription = await this.catalogService.getSubscriptionByCompanyName(this.companyName);

    if (!this.subscription)
      return;

    this.allCategories = await this.catalogService.getCategoriesBySubsriptionId(this.subscription?.id);
    this.allProductDefinitions = await this.catalogService.getProductDefinitionsBySubscriptionId(this.subscription?.id);

    if (this.categoryName) {
      const flatCategories = this.flattenCategories(this.allCategories);
      let category = flatCategories.find(c => c.name === this.categoryName);

      this.categories = category?.childCategories ?? [];
      this.productDefinitions = this.allProductDefinitions.filter(p => p.categoryId === category?.id) ?? [];
    }
    else {
      this.categories = this.allCategories;
      this.productDefinitions = this.allProductDefinitions.filter(p => p.categoryId === null) ?? [];
    }
  }

  flattenCategories(categories: Category[]): Category[] {
    return categories.flatMap(c => this.flattenCategories(c.childCategories)).concat(categories);
  }

  onCategoryClick(category: Category): void {
    this.router.navigate([this.subscription!.companyName, 'categories', category.name]);
  }

  onProductDefinitionClick(productDefinition: ProductDefinition): void {
    this.router.navigate([this.subscription!.companyName, 'products', productDefinition.name]);
  }


}
