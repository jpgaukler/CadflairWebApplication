import { Image, Text, Group, Stack, Title, Table, Loader, Anchor, MultiSelect, Skeleton, Container, Drawer, Paper, ActionIcon } from '@mantine/core';
import { useParams } from 'react-router-dom';
import Subscription from '../interfaces/Subscription.interface';
import ProductDefinition from '../interfaces/ProductDefinition.interface';
import useSWR from 'swr';
import { useEffect, useRef, useState } from 'react';
import Row from '../interfaces/Row.interface';
import { useDisclosure } from '@mantine/hooks';
import { IconChevronUp } from '@tabler/icons-react';

const fetcher = (url: string) => fetch(url).then(res => {
    console.log(res);
    return res.json();
})


export default function Products() {
    const params = useParams();
    const [opened, { open, close }] = useDisclosure(false);

    const { data: subscription } = useSWR<Subscription>(`https://cadflairrestapi.azurewebsites.net/api/v1/subscriptions/${params.companyName}`, fetcher)
    const { data: productDefinition, isLoading } = useSWR<ProductDefinition>(subscription ? `https://cadflairrestapi.azurewebsites.net/api/v1/subscriptions/${subscription.id}/product-definitions/${params.productDefinitionName}` : null, fetcher)

    const [filteredRows, setFilteredRows] = useState<Row[]>([]);
    const columnFilters = useRef(new Map<number, string[]>());

    useEffect(() => {
        setFilteredRows(productDefinition ? productDefinition.productTable.rows : []);
    }, [productDefinition]);


    function getDistinctColumnValues(columnId: number): string[] {

        if (!productDefinition?.productTable?.rows)
            return [];

        const values = productDefinition.productTable.rows.map(r => r.tableValues.find(i => i.columnId === columnId)!.value)

        // Set function returns only unique values
        return [...new Set(values)]
    }

    function handleFilterChange(columnId: number, values: string[]) {
        // clear previous filter values
        columnFilters.current.delete(columnId);

        // set new filter values
        if (values.length > 0)
            columnFilters.current.set(columnId, values);

        const filteredRows = productDefinition?.productTable.rows.filter((row) => {
            for (const tableValue of row.tableValues) { 
                const columnFilter = columnFilters.current.get(tableValue.columnId);

                if (!columnFilter)
                    continue;

                // check to see if this value is selected in the column filters
                if (!columnFilter.includes(tableValue.value))
                    return false;
            }

            return true;
        });

        setFilteredRows(filteredRows ? filteredRows : []);
    }


    return (
        <>
            <Group align="flex-start" wrap="nowrap">

                {/* filter desktop*/}
                <Stack p="sm" w="275px" pos="sticky" top={60} visibleFrom="md">
                    <Group justify="space-between">
                        <Title order={5}>Filter by</Title>
                        <Text size="xs">Count: {filteredRows.length}</Text>
                    </Group>
                    {isLoading
                        ? <>
                            {Array(10).fill(0).map((_, index) => <Skeleton key={index} h={28} mt="sm" />)}
                        </>
                        : <Stack>
                            {productDefinition?.productTable?.columns.map(col =>
                                <MultiSelect
                                    key={col.id}
                                    label={col.header}
                                    placeholder="Pick value"
                                    data={getDistinctColumnValues(col.id)}
                                    onChange={(values) => handleFilterChange(col.id, values)}
                                />
                            )}
                        </Stack>}
                </Stack>

                {/* filter mobile */}
                <Paper pos="fixed" bottom={0} left={0} right={0} bg="#ffffff" withBorder hiddenFrom="md">
                    <Group justify="space-between" p="sm">
                        <Title order={5}>Filter by</Title>
                        <ActionIcon color="black" variant="subtle" onClick={open}>
                            <IconChevronUp />
                        </ActionIcon>
                    </Group>
                </Paper>
                <Drawer
                    opened={opened}
                    onClose={close}
                    title={<Title order={5}>Filter by</Title>}
                    position="bottom"
                    hiddenFrom="md"
                    size="100%"
                >
                    {isLoading
                        ? <>
                            {Array(10).fill(0).map((_, index) => <Skeleton key={index} h={28} mt="sm" />)}
                        </>
                        : <Stack>
                            {productDefinition?.productTable?.columns.map(col =>
                                <MultiSelect
                                    key={col.id}
                                    label={col.header}
                                    placeholder="Pick value"
                                    data={getDistinctColumnValues(col.id)}
                                    onChange={(values) => handleFilterChange(col.id, values)}
                                />
                            )}
                        </Stack>}
                </Drawer>

                {/* table */}
                {isLoading
                    ? <Stack mih="70vh" w="100%" justify="center" align="center">
                        <Loader color="blue" />
                        <Text>Loading...</Text>
                    </Stack>
                    : <Container size="xl">
                        <Stack>
                            <Stack pos="sticky" top={60} bg="#ffffff" h="250px">
                                <Title order={3} my="sm">{productDefinition?.name}</Title>
                                <Group wrap="nowrap">
                                    <Image
                                        src={productDefinition?.thumbnailUri}
                                        height={130}
                                        width={75}
                                        alt="thumbnail"
                                    />
                                    <Text size="sm" c="dimmed" lineClamp={6}>{productDefinition?.description}</Text>
                                </Group>
                            </Stack>

                            <Table hiddenFrom="md">
                                <Table.Thead pos="sticky" top={250 + 60} bg="#ffffff" display="none">
                                    <Table.Tr>
                                        <Table.Th>Part Number</Table.Th>
                                        {productDefinition?.productTable?.columns.map(col =>
                                            <Table.Th key={col.id}>{col.header}</Table.Th>
                                        )}
                                    </Table.Tr>
                                </Table.Thead>
                                <Table.Tbody display="table-row-group">
                                    {filteredRows.map(row =>
                                        <Table.Tr key={row.id}>
                                            <Table.Td display="flex" style={{ display: "flex", justifyContent: "space-between" }}>
                                                <Text size="sm" fw={600}>Part Number</Text>
                                                <Anchor size="sm" href={`/${params.companyName}/products/${params.productDefinitionName}/${row.partNumber}`}>{row.partNumber}</Anchor>
                                            </Table.Td>
                                            {row.tableValues.map(tableValue =>
                                                <Table.Td key={tableValue.id} style={{ display: "flex", justifyContent: "space-between" }}>
                                                    <Text size="sm">{productDefinition?.productTable?.columns.find(c => c.id === tableValue.columnId)?.header}</Text>
                                                    <Text size="sm">{tableValue.value}</Text>
                                                </Table.Td>
                                            )}
                                        </Table.Tr>)}
                                </Table.Tbody>
                            </Table>


                            <Table visibleFrom="md">
                                <Table.Thead pos="sticky" top={250 + 60} bg="#ffffff">
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
                                        </Table.Tr>)}
                                </Table.Tbody>
                            </Table>
                        </Stack>
                    </Container>}
            </Group>
        </>
    );

}


