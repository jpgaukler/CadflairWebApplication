import { Image, Text, Group, Grid, Paper, Stack, Title, Table } from '@mantine/core';
import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import Subscription from '../interfaces/Subscription.interface';
import ProductDefinition from '../interfaces/ProductDefinition.interface';
import ProductTable from '../interfaces/ProductTable.interface';

export default function Products() {
    const params = useParams();
    const navigate = useNavigate();
    const [productDefinition, setProductDefinition] = useState<ProductDefinition>();
    const [productTable, setProductTable] = useState<ProductTable>();
    const [subscription, setSubscription] = useState<Subscription>();

    useEffect(() => {

        // get subscription from api
        fetch(`/api/v1/subscriptions/${params.companyName}`)
            .then((response) => response.json())
            .then((subscription:Subscription) => {
                //console.log(subscription);
                setSubscription(subscription);

                // get product definition
                fetch(`/api/v1/subscriptions/${subscription.id}/product-definitions/${params.productDefinitionName}`)
                    .then((response) => response.json())
                    .then((data) => {
                        console.log(data);
                        setProductDefinition(data.productDefinition);
                        setProductTable(data.productTable);
                    })
                    .catch((err) => {
                        console.log(err.message);
                    });
                //_productTable = await DataServicesManager.McMasterService.GetProductTableByProductDefinitionId(_productDefinition.Id);

            })
            .catch((err) => {
                console.log(err.message);
            });
    }, [params.companyName, params.productDefinitionName]);

    return (
        <>
            <h1>Product Definitions Page</h1>
            <Title order={1}>{subscription?.companyName}</Title>


                <Group>
                    <Image
                        src={productDefinition?.thumbnailUri}
                        height={130}
                        width={75}
                        alt="Norway"
                    />
                    <Stack>
                        <Text fw={500}>{productDefinition?.name}</Text>
                        <Text size="sm" c="dimmed">{productDefinition?.description}</Text>
                    </Stack>
                </Group>

            <Table>
                <Table.Thead>
                    <Table.Tr>
                        {productTable?.columns.map(col =>
                            <Table.Th key={col.id}>{col.header}</Table.Th>
                        )}
                    </Table.Tr>
                </Table.Thead>
                <Table.Tbody>
                    {productTable?.rows.map(row =>
                        <Table.Tr key={row.id}>
                            {row.tableValues.map(tableValue => 
                                <Table.Td key={tableValue.id}>{tableValue.value}</Table.Td>
                            )}
                        </Table.Tr>
                    )}
                </Table.Tbody>
            </Table>

            <Grid gutter="xs">
            </Grid>
        </>
    );

}


