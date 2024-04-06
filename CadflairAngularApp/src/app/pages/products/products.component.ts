import { Component, Input, OnInit, inject } from '@angular/core';
import { CatalogService } from '../../services/catalog.service';
import { Router } from '@angular/router';
import { TableModule } from 'primeng/table';
import Subscription from '../../../interfaces/Subscription.interface';
import ProductDefinition from '../../../interfaces/ProductDefinition.interface';
import { CommonModule } from '@angular/common';
import Row from '../../../interfaces/Row.interface';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [
    TableModule,
    CommonModule
  ],
  templateUrl: './products.component.html',
  styleUrl: './products.component.css'
})
export class ProductsComponent implements OnInit {

  // inputs
  @Input() companyName!: string;
  @Input() productDefinitionName!: string;

  // services
  private catalogService: CatalogService = inject(CatalogService);
  private router: Router = inject(Router);

  // props
  subscription?: Subscription;
  productDefinition?: ProductDefinition;
  columnFilters: Map<number, string[]> = new Map<number, string[]>;
  filteredRows: Row[] = [];



  async ngOnInit(): Promise<void> {
    this.subscription = await this.catalogService.getSubscriptionByCompanyName(this.companyName);

    if (!this.subscription)
      return;

    this.productDefinition = await this.catalogService.getProductDefinitionsBySubscriptionIdAndProductDefinitionName(this.subscription.id, this.productDefinitionName);
    this.filteredRows = this.productDefinition?.productTable.rows ?? [];
  }

  onFilterChange(columnId: number, values: string[]): void {
    // clear previous filter values
    this.columnFilters.delete(columnId);

    // set new filter values
    if (values.length > 0)
      this.columnFilters.set(columnId, values);

    if (!this.productDefinition)
      return;

    const filteredRows = this.productDefinition.productTable.rows.filter((row) => {
      for (const tableValue of row.tableValues) {
        const columnFilter = this.columnFilters.get(tableValue.columnId);

        if (!columnFilter)
          continue;

        // check to see if this value is selected in the column filters
        if (!columnFilter.includes(tableValue.value))
          return false;
      }

      return true;
    });


  }


}
