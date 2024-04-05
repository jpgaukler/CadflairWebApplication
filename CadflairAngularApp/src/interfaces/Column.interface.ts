import TableValue from "./TableValue.interface";

export default interface Column {
    id: number;
    productTableId: number;
    header: string;
    sortOrder: number;
    createdById: number;
    createdOn: string;
    tableValues: TableValue[];
}