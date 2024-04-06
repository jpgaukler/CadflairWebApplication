import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { CategoriesComponent } from './pages/categories/categories.component';
import { ProductsComponent } from './pages/products/products.component';

export const routes: Routes = [
  { path: '', component: HomeComponent  },
  { path: ':companyName/categories', component: CategoriesComponent  },
  { path: ':companyName/categories/:categoryName', component: CategoriesComponent  },
  { path: ':companyName/products/:productDefinitionName', component: ProductsComponent  },
  //{ path: '**', component: PageNotFoundComponent },

];
