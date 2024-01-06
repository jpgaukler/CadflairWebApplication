import { Image, Text, Group, Grid, Paper, Stack, Title } from '@mantine/core';
import { useNavigate, useParams } from 'react-router-dom';
import Category from '../interfaces/Category.interface';
import ProductDefinition from '../interfaces/ProductDefinition.interface';
import Subscription from '../interfaces/Subscription.interface';
import useSWR from 'swr';



const fetcher = (url: string) => fetch(url).then(res => {
    console.log(res);
    return res.json();
})


export default function Categories() {
    const params = useParams();
    const navigate = useNavigate();

    const { data: subscription } = useSWR<Subscription>(`/api/v1/subscriptions/${params.companyName}`, fetcher)
    const { data: categories, error, isLoading } = useSWR<Category[]>(subscription ? `/api/v1/subscriptions/${subscription.id}/categories/` : null, fetcher)
    //const { data: categories, error, isLoading } = useSWR<Category[]>(subscription ? `/api/v1/subscriptions/${subscription.id}/categories/${params.categoryName ? params.categoryName : ""}` : null, fetcher)
    //const { data:productDefinitions, error, isLoading } = useSWR<ProductDefinition[]>(`/api/v1/subscriptions/${subscription.id}/categories/${category.id}/product-definitions`, fetcher)


    return (
        <>
            <h1>Categories Page</h1>
            <Title order={1}>{subscription?.companyName}</Title>
            <Grid gutter="xs">
                <Grid.Col span={12}  >
                    <Title order={5}>Categories</Title>
                </Grid.Col>
                {categories?.map(category =>
                    <Grid.Col key={category.id} span={{ base: 12, md: 6, lg: 3 }}>
                        <Paper shadow="xs" p="sm" onClick={() => handleCategoryClick(category)}>
                            <Group>
                                <Image
                                    src={category.thumbnailUri}
                                    height={130}
                                    width={75}
                                    alt="thumbnail image"
                                />
                                <Stack>
                                    <Text fw={500}>{category.name}</Text>
                                    <Text size="sm" c="dimmed">{category.description}</Text>
                                </Stack>
                            </Group>
                        </Paper>
                    </Grid.Col>
                )}

                <Grid.Col span={12}  >
                    <Title order={5}>Product Definitions</Title>
                </Grid.Col>
            {/*    {productDefinitions?.map(productDefinition =>*/}
            {/*        <Grid.Col key={productDefinition.id} span={{ base: 12, md: 6, lg: 3 }}>*/}
            {/*            <Paper shadow="xs" p="sm" onClick={() => handleProductDefinitionClick(productDefinition)}>*/}
            {/*                <Group>*/}
            {/*                    <Image*/}
            {/*                        src={productDefinition.thumbnailUri}*/}
            {/*                        height={130}*/}
            {/*                        width={75}*/}
            {/*                        alt="Norway"*/}
            {/*                    />*/}
            {/*                    <Stack>*/}
            {/*                        <Text fw={500}>{productDefinition.name}</Text>*/}
            {/*                        <Text size="sm" c="dimmed">{productDefinition.description}</Text>*/}
            {/*                    </Stack>*/}
            {/*                </Group>*/}
            {/*            </Paper>*/}
            {/*        </Grid.Col>*/}
            {/*    )}*/}
            </Grid>
        </>
    );

    function handleCategoryClick(category: Category) {
        navigate(`/${params.companyName}/categories/${category.name}`);
    }

    function handleProductDefinitionClick(productDefinition: ProductDefinition) {
        navigate(`/${params.companyName}/products/${productDefinition.name}`);
    }

}


