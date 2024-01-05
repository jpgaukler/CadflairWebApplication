export default interface Category {
    id: number;
    parentId: number | undefined;
    subscriptionId: number;
    name: string;
    description: string;
    thumbnailUri: string;
    createdById: number;
    createdOn: string;
    parentCategory: Category;
    childCategories: Category[];
}
