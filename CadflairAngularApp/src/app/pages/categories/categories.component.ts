import { Component, Input, OnChanges, OnInit, SimpleChanges, inject } from '@angular/core';
import { CatalogService } from '../../services/catalog.service';
import Category from '../../../interfaces/Category.interface';
import Subscription from '../../../interfaces/Subscription.interface';
import ProductDefinition from '../../../interfaces/ProductDefinition.interface';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [
    CommonModule,
    BreadcrumbModule
  ],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.css'
})
export class CategoriesComponent implements OnChanges {

  // inputs
  @Input() companyName!: string;
  @Input() categoryName!: string;

  // services
  private catalogService: CatalogService = inject(CatalogService);
  private router: Router = inject(Router);

  // props
  breadcrumbItems: MenuItem[] | undefined;
  subscription?: Subscription;
  allCategories: Category[] = [];
  allProductDefinitions: ProductDefinition[] = [];
  categories: Category[] = [];
  productDefinitions: ProductDefinition[] = [];

  async ngOnChanges(changes: SimpleChanges): Promise<void> {
    this.subscription = await this.catalogService.getSubscriptionByCompanyName(this.companyName);

    if (!this.subscription)
      return;

    const breadcrumbItems: MenuItem[] = [];
    this.allCategories = await this.catalogService.getCategoriesBySubsriptionId(this.subscription?.id);
    this.allProductDefinitions = await this.catalogService.getProductDefinitionsBySubscriptionId(this.subscription?.id);

    if (this.categoryName) {
      const flatCategories = this.flattenCategories(this.allCategories);
      let category = flatCategories.find(c => c.name === this.categoryName);

      this.categories = category?.childCategories ?? [];
      this.productDefinitions = this.allProductDefinitions.filter(p => p.categoryId === category?.id) ?? [];

      // assemble breadcrumb items by climbing up the tree, adding parent categories
      if (category) {
        while (category) {
          breadcrumbItems.push({ label: category.name, routerLink: `/${this.companyName}/categories/${category.name}/` });
          category = flatCategories.find(c => c.id === category?.parentId);
        }
      }
    }
    else {
      this.categories = this.allCategories;
      this.productDefinitions = this.allProductDefinitions.filter(p => p.categoryId === null) ?? [];
    }

    // add root breadcrumb
    breadcrumbItems.push({ label: "All Categories", routerLink: `/${this.companyName}/categories/` });

    // reverse the list so the breadcrumbs are displayed from the top down
    breadcrumbItems.reverse();

    this.breadcrumbItems = breadcrumbItems;
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
