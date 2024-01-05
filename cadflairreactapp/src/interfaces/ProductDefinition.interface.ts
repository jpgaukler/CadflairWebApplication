export default interface ProductDefinition {
    id: number;
    subscriptionId: number;
    categoryId?: number;
    name: string;
    description: string;
    thumbnailUri: string;
    forgeBucketKey: string;
    createdById: number;
    createdOn: string;
}
