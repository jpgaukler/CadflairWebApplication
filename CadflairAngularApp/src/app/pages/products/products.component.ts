import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, SimpleChanges, inject } from '@angular/core';
import { CatalogService } from '../../services/catalog.service';
import { Router } from '@angular/router';
import { TableModule } from 'primeng/table';
import { MultiSelectModule } from 'primeng/multiselect';
import { ButtonModule } from 'primeng/button';
import Subscription from '../../../interfaces/Subscription.interface';
import ProductDefinition from '../../../interfaces/ProductDefinition.interface';
import Row from '../../../interfaces/Row.interface';
import TableValue from '../../../interfaces/TableValue.interface';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [
    TableModule,
    CommonModule,
    MultiSelectModule,
    ButtonModule
  ],
  templateUrl: './products.component.html',
  styleUrl: './products.component.css'
})
export class ProductsComponent implements OnChanges {
  // inputs
  @Input() companyName!: string;
  @Input() productDefinitionName!: string;

  // services
  private catalogService: CatalogService = inject(CatalogService);
  private router: Router = inject(Router);

  // props
  subscription?: Subscription;
  productDefinition?: ProductDefinition;
  columnFilters: Map<number, string[]> = new Map<number, string[]>();
  filteredRows: Row[] = [];

  async ngOnChanges(changes: SimpleChanges): Promise<void> {
    this.subscription = await this.catalogService.getSubscriptionByCompanyName(this.companyName);

    if (!this.subscription)
      return;

    this.productDefinition = await this.catalogService.getProductDefinitionsBySubscriptionIdAndProductDefinitionName(this.subscription.id, this.productDefinitionName);
    this.filteredRows = this.productDefinition?.productTable.rows ?? [];
  }

  getDistinctColumnValues(columnId: number): string[] {
    if (!this.productDefinition)
      return [];

    const values = this.productDefinition.productTable.rows.map(r => r.tableValues.find(i => i.columnId === columnId)!.value)

    // Set function returns only unique values
    return [...new Set(values)]
  }

  getColumnHeader(tableValue: TableValue): string {
    return this.productDefinition?.productTable?.columns.find(c => c.id === tableValue.columnId)?.header ?? ''
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

    this.filteredRows = filteredRows;
  }

  onRowClick(partNumber: string) {
    this.router.navigate([this.subscription!.companyName, 'products', this.productDefinition!.name, partNumber ]);
  }


}
