import { Image, Text, Group, Stack, Title, Table, Loader, Box, Select } from '@mantine/core';
import { useParams } from 'react-router-dom';
import Subscription from '../interfaces/Subscription.interface';
import ProductDefinition from '../interfaces/ProductDefinition.interface';
import useSWR from 'swr';
import { useEffect, useState } from 'react';
import Row from '../interfaces/Row.interface';
import ForgeViewer from '../components/ForgeViewer';

const fetcher = (url: string) => fetch(url).then(res => {
    console.log(res);
    return res.json();
})

export default function ProductDetails() {
    const params = useParams();

    const { data: subscription } = useSWR<Subscription>(`/api/v1/subscriptions/${params.companyName}`, fetcher)
    const { data: productDefinition, isLoading } = useSWR<ProductDefinition>(subscription ? `/api/v1/subscriptions/${subscription.id}/product-definitions/${params.productDefinitionName}` : null, fetcher)

    const [row, setRow] = useState<Row>();

    useEffect(() => {
        if (!productDefinition) {
            setRow(undefined);
            return;
        }

        const matchingRow = productDefinition.productTable.rows.find(r => r.partNumber === params.partNumber);
        matchingRow ? setRow(matchingRow) : setRow(undefined);
    }, [params.partNumber, productDefinition]);


    if (isLoading)
        return (
            <>
                <Stack h="100%" mih="90dvh" justify="center" align="center">
                    <Loader color="blue" />
                    <Text>Loading...</Text>
                </Stack>
            </>
        );

    return (
        <>
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

                <Select
                    label="Downloads"
                    placeholder="Pick value"
                    data={row?.attachments.map(a => a.forgeObjectKey)}
                />
            </Group>

            <Group w="100%" grow>
                <Box h="500px">
                    <ForgeViewer bucketKey={productDefinition?.forgeBucketKey} objectKey={row?.attachments[0].forgeObjectKey} />
                </Box>

                <Box bg="blue">
                    <Table>
                        <Table.Tbody>
                            {row?.tableValues.map(tableValue =>
                                <Table.Tr key={tableValue.id}>
                                    <Table.Td>
                                        <Text fw="500">
                                            {productDefinition?.productTable.columns.find(c => c.id === tableValue.columnId)?.header}
                                        </Text>
                                    </Table.Td>
                                    <Table.Td>
                                        <Text>
                                            {tableValue.value}
                                        </Text>
                                    </Table.Td>
                                </Table.Tr>
                            )}
                        </Table.Tbody>
                    </Table>
                </Box>
            </Group>
        </>
    );
}


