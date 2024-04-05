import Column from "./Column.interface";
import Row from "./Row.interface";

export default interface ProductTable {
    id: number;
    productDefinitionId: number;
    createdById: number;
    createdOn: string;
    rows: Row[];
    columns: Column[];
}