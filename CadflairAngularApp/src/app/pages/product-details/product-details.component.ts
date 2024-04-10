import { Component, Input, OnChanges, SimpleChanges, inject } from '@angular/core';
import { ForgeViewerComponent } from '../../components/forge-viewer/forge-viewer.component';
import { CatalogService } from '../../services/catalog.service';
import { Router } from '@angular/router';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { CommonModule } from '@angular/common';
import { DividerModule } from 'primeng/divider';
import ProductDefinition from '../../../interfaces/ProductDefinition.interface';
import Row from '../../../interfaces/Row.interface';
import Subscription from '../../../interfaces/Subscription.interface';
import Attachment from '../../../interfaces/Attachment.interface';
import { ForgeService } from '../../services/forge.service';

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [
    ForgeViewerComponent,
    DropdownModule,
    ButtonModule,
    TableModule,
    DividerModule,
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
  private forgeService: ForgeService = inject(ForgeService);
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
    this.activeAttachment3d = this.row?.attachments.find(i => i.forgeObjectKey.includes("stp"));
    this.activeAttachment2d = this.row?.attachments.find(i => i.forgeObjectKey.includes("pdf"));
    this.activeAttachment = this.activeAttachment3d;
  }

  async onDownloadChange(forgeObjectKey:string): Promise<void> {
    if (!this.productDefinition)
      return;

    if (!forgeObjectKey) {
      this.downloadLink = undefined;
      return;
    }

    this.downloadLink = await this.forgeService.getSignedUrl(this.productDefinition?.forgeBucketKey, forgeObjectKey);
  }

  onDownloadClick(): void {
    if (!this.downloadLink)
      return;

    window.location.href = this.downloadLink; 
  }

  getDownloadOptions(): string[] {
    return this.row?.attachments.map(a => a.forgeObjectKey) ?? [];
  }

  getColumnValue(columnId: number): string {
    return this.row?.tableValues.find(i => i.columnId === columnId)?.value ?? ''
  }

  onView2dClick(): void {
    this.activeAttachment = this.activeAttachment2d;
  }

  onView3dClick(): void {
    this.activeAttachment = this.activeAttachment3d;
  }

}
