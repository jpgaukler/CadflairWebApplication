import { Image, Text, Group, Grid, Paper, Stack, Title, Loader } from '@mantine/core';
import { useNavigate, useParams } from 'react-router-dom';
import Category from '../interfaces/Category.interface';
import ProductDefinition from '../interfaces/ProductDefinition.interface';
import Subscription from '../interfaces/Subscription.interface';
import useSWR from 'swr';
import { useEffect, useState } from 'react';



const fetcher = (url: string) => fetch(url).then(res => {
    console.log(res);
    return res.json();
})

function flattenCategories(categories:Category[]): Category[] {
    return categories.flatMap(c => flattenCategories(c.childCategories)).concat(categories);
}


export default function Categories() {
    const params = useParams();
    const navigate = useNavigate();

    const { data: subscription } = useSWR<Subscription>(`https://cadflairrestapi.azurewebsites.net/api/v1/subscriptions/${params.companyName}`, fetcher)
    const { data: allCategories, isLoading:categoriesLoading } = useSWR<Category[]>(subscription ? `https://cadflairrestapi.azurewebsites.net/api/v1/subscriptions/${subscription.id}/categories/` : null, fetcher)
    const { data: allProductDefinitions, isLoading:productsLoading } = useSWR<ProductDefinition[]>(subscription ? `https://cadflairrestapi.azurewebsites.net/api/v1/subscriptions/${subscription.id}/product-definitions` : null, fetcher)

    const [categories, setCategories] = useState<Category[]>([]);
    const [productDefinitions, setProductDefinitions] = useState<ProductDefinition[]>([]);

    useEffect(() => {
        if (!allCategories) {
            setCategories([]);
            return;
        }

        if (params.categoryName) {
            const flatCategories = flattenCategories(allCategories);
            const category = flatCategories.find(c => c.name === params.categoryName);
            category ? setCategories(category.childCategories) : setCategories([]);

            const productDefs = allProductDefinitions ? allProductDefinitions.filter(p => p.categoryId === category?.id) : [];
            setProductDefinitions(productDefs);
        }
        else {
            setCategories(allCategories);

            const productDefs = allProductDefinitions ? allProductDefinitions.filter(p => p.categoryId === null) : [];
            setProductDefinitions(productDefs);
        }

    }, [params.categoryName, allCategories, allProductDefinitions]);

    if (categoriesLoading || productsLoading)
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
                {productDefinitions.map(productDefinition =>
                    <Grid.Col key={productDefinition.id} span={{ base: 12, md: 6, lg: 3 }}>
                        <Paper shadow="xs" p="sm" onClick={() => handleProductDefinitionClick(productDefinition)}>
                            <Group>
                                <Image
                                    src={productDefinition.thumbnailUri}
                                    height={130}
                                    width={75}
                                    alt="Norway"
                                />
                                <Stack>
                                    <Text fw={500}>{productDefinition.name}</Text>
                                    <Text size="sm" c="dimmed">{productDefinition.description}</Text>
                                </Stack>
                            </Group>
                        </Paper>
                    </Grid.Col>
                )}
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


