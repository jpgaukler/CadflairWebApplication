import { Image, Text, Group, Grid, Paper, Stack, Title, Table, Loader, Anchor, Drawer, MultiSelect } from '@mantine/core';
import { useParams } from 'react-router-dom';
import Subscription from '../interfaces/Subscription.interface';
import ProductDefinition from '../interfaces/ProductDefinition.interface';
import useSWR from 'swr';
import { useEffect, useState } from 'react';
import Row from '../interfaces/Row.interface';

const fetcher = (url: string) => fetch(url).then(res => {
    console.log(res);
    return res.json();
})


export default function Products() {
    const params = useParams();

    const { data: subscription } = useSWR<Subscription>(`/api/v1/subscriptions/${params.companyName}`, fetcher)
    const { data: productDefinition, isLoading } = useSWR<ProductDefinition>(subscription ? `/api/v1/subscriptions/${subscription.id}/product-definitions/${params.productDefinitionName}` : null, fetcher)

    const [filteredRows, setFilteredRows] = useState<Row[]>([]);
    const columnFilters: Map<number, string[]> = new Map();

    useEffect(() => {
        setFilteredRows(productDefinition ? productDefinition.productTable.rows : []);
    }, [productDefinition]);


    if (isLoading)
        return (
            <>
                <Stack h="100%" mih="90dvh" justify="center" align="center">
                    <Loader color="blue" />
                    <Text>Loading...</Text>
                </Stack>
            </>
        );

    function getDistinctColumnValues(columnId: number): string[] {

        if (!productDefinition?.productTable?.rows)
            return [];

        const values = productDefinition.productTable.rows.map(r => r.tableValues.find(i => i.columnId === columnId)!.value)

        // Set function returns only unique values
        return [...new Set(values)]
    }

    function handleFilterChange(columnId: number, values: string[]) {
        // clear previous filter values
        columnFilters.delete(columnId);

        // set new filter values
        if (values.length > 0)
            columnFilters.set(columnId, values);

        console.log(columnFilters);

        const filteredRows = productDefinition?.productTable.rows.filter((row) => {
            for (const tableValue of row.tableValues) { 
                const columnFilter = columnFilters.get(tableValue.columnId);

                if (!columnFilter)
                    continue;

                //if (row.partNumber === "ANSI-B300-0.5B") {
                //    console.log(`filter:`);
                //    console.log(columnFilter);
                //    console.log(`table Value:`);
                //    console.log(tableValue.value);
                //}

                // check to see if this value is selected in the column filters
                if (!columnFilter.includes(tableValue.value))
                    return false;
            }

            return true;
        });

        console.log(filteredRows);
        setFilteredRows(filteredRows ? filteredRows : []);
    }


    return (
        <>
            <Grid>
                <Grid.Col span={2}>
                    <Title order={4}>Filter</Title>
                    <Stack>
                        {productDefinition?.productTable?.columns.map(col =>
                            <MultiSelect
                                key={col.id}
                                label={col.header}
                                placeholder="Pick value"
                                data={getDistinctColumnValues(col.id)}
                                onChange={(values) => handleFilterChange(col.id, values)}
                            />
                        )}
                    </Stack>
                </Grid.Col>
                <Grid.Col span={10}>
                    <h1>Product Definitions Page</h1>
                    <Title order={1}>{subscription?.companyName}</Title>

                    <Group>
                        <Image
                            src={productDefinition?.thumbnailUri}
                            height={130}
                            width={75}
                            alt="thumbnail"
                        />
                        <Stack>
                            <Text fw={500}>{productDefinition?.name}</Text>
                            <Text size="sm" c="dimmed">{productDefinition?.description}</Text>
                        </Stack>
                    </Group>

                    <Table>
                        <Table.Thead>
                            <Table.Tr>
                                <Table.Th>Part Number</Table.Th>
                                {productDefinition?.productTable?.columns.map(col =>
                                    <Table.Th key={col.id}>{col.header}</Table.Th>
                                )}
                            </Table.Tr>
                        </Table.Thead>
                        <Table.Tbody>
                            {filteredRows.map(row =>
                                <Table.Tr key={row.id}>
                                    <Table.Td>
                                        <Anchor href={`/${params.companyName}/products/${params.productDefinitionName}/${row.partNumber}`}>{row.partNumber}</Anchor>
                                    </Table.Td>
                                    {row.tableValues.map(tableValue =>
                                        <Table.Td key={tableValue.id}>{tableValue.value}</Table.Td>
                                    )}
                                </Table.Tr>
                            )}
                        </Table.Tbody>
                    </Table>
                </Grid.Col>
            </Grid>
        </>
    );

}


