import { Component, Input, OnChanges, SimpleChanges, inject } from '@angular/core';
import { ForgeViewerComponent } from '../../components/forge-viewer/forge-viewer.component';
import { CatalogService } from '../../services/catalog.service';
import { Router } from '@angular/router';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { CommonModule } from '@angular/common';
import ProductDefinition from '../../../interfaces/ProductDefinition.interface';
import Row from '../../../interfaces/Row.interface';
import Subscription from '../../../interfaces/Subscription.interface';
import Attachment from '../../../interfaces/Attachment.interface';

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [
    ForgeViewerComponent,
    DropdownModule,
    ButtonModule,
    TableModule,
    CommonModule
  ],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.css'
})
export class ProductDetailsComponent implements OnChanges {

  // inputs
  @Input() companyName!: string;
  @Input() productDefinitionName!: string;
  @Input() partNumber!: string;

  // services
  private catalogService: CatalogService = inject(CatalogService);
  private router: Router = inject(Router);

  // props
  subscription?: Subscription;
  productDefinition?: ProductDefinition;
  row?: Row;
  activeAttachment?: Attachment;
  activeAttachment3d?: Attachment;
  activeAttachment2d?: Attachment;
  downloadLink?: string;

  async ngOnChanges(changes: SimpleChanges): Promise<void> {
    this.subscription = await this.catalogService.getSubscriptionByCompanyName(this.companyName);

    if (!this.subscription)
      return;

    this.productDefinition = await this.catalogService.getProductDefinitionsBySubscriptionIdAndProductDefinitionName(this.subscription.id, this.productDefinitionName);

    if (!this.productDefinition)
      return;

    const matchingRow = this.productDefinition.productTable.rows.find(r => r.partNumber === this.partNumber);
    this.row = matchingRow;
    this.activeAttachment = this.row?.attachments.find(i => i.forgeObjectKey.includes("stp"));
  }

  onDownloadChange(): void {

  }

  getDownloadOptions(): string[] {
    return this.row?.attachments.map(a => a.forgeObjectKey) ?? [];
  }

}
