import Attachment from "./Attachment.interface";
import TableValue from "./TableValue.interface";

export default interface Row {
    id: number;
    productTableId: number;
    partNumber: string;
    createdById: number;
    createdOn: string;
    tableValues: TableValue[];
    attachments: Attachment[];
}