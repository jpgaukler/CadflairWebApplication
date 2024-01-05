export default interface Subscription {
    id: number;
    subscriptionTypeId: number;
    companyName: string;
    subdirectoryName: string;
    createdById: number;
    createdOn: string;
    ownerId: number;
    expiresOn: string;
}

