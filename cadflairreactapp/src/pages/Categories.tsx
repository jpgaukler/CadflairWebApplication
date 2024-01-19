import { Image, Text, Group, Grid, Paper, Stack, Loader, Breadcrumbs, Anchor } from '@mantine/core';
import { useNavigate, useParams } from 'react-router-dom';
import Category from '../interfaces/Category.interface';
import ProductDefinition from '../interfaces/ProductDefinition.interface';
import Subscription from '../interfaces/Subscription.interface';
import useSWR from 'swr';
import { useEffect, useState } from 'react';


interface BreadcrumbItem {
    text: string;
    href: string;
}

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
    const [breadcrumbItems, setBreadcrumbItems] = useState<BreadcrumbItem[]>([]);

    useEffect(() => {
        if (!allCategories) {
            setCategories([]);
            return;
        }

        const breadcrumbItems: BreadcrumbItem[] = [];

        if (params.categoryName) {
            const flatCategories = flattenCategories(allCategories);
            let category = flatCategories.find(c => c.name === params.categoryName);
            category ? setCategories(category.childCategories) : setCategories([]);

            // set product definitions
            const productDefs = allProductDefinitions ? allProductDefinitions.filter(p => p.categoryId === category?.id) : [];
            setProductDefinitions(productDefs);

            // assemble breadcrumb items by climbing up the tree, adding parent categories
            if (category) {
                while (category) {
                    breadcrumbItems.push({ text: category.name, href: `/${params.companyName}/categories/${category.name}/` });
                    category = flatCategories.find(c => c.id === category?.parentId);
                }
            }
        }
        else {
            // set categories
            setCategories(allCategories);

            // set product definitions
            const productDefs = allProductDefinitions ? allProductDefinitions.filter(p => p.categoryId === null) : [];
            setProductDefinitions(productDefs);
        }

        // add root breadcrumb
        breadcrumbItems.push({ text: "All Categories", href: `/${params.companyName}/categories/` });

        // reverse the list so the breadcrumbs are displayed from the top down
        breadcrumbItems.reverse();

        setBreadcrumbItems(breadcrumbItems);

    }, [params.categoryName, allCategories, allProductDefinitions, params.companyName]);

    function handleCategoryClick(category: Category) {
        navigate(`/${params.companyName}/categories/${category.name}`);
    }

    function handleProductDefinitionClick(productDefinition: ProductDefinition) {
        navigate(`/${params.companyName}/products/${productDefinition.name}`);
    }


    if (categoriesLoading || productsLoading)
        return (
            <>
                <Stack mih="70vh" justify="center" align="center">
                    <Loader />
                    <Text>Loading...</Text>
                </Stack>
            </>
        );

    return (
        <>
            <Grid gutter="xs" px="md">
                <Grid.Col span={12} py="lg">
                    <Breadcrumbs>
                        {breadcrumbItems.map((item, index) => <Anchor href={item.href} key={index}>{item.text}</Anchor>)}
                    </Breadcrumbs>
                </Grid.Col>
                {/*<Grid.Col span={12}  >*/}
                {/*    <Title order={5}>Categories</Title>*/}
                {/*</Grid.Col>*/}

                {categories?.map(category =>
                    <Grid.Col key={category.id} span={{ base: 12, md: 6, lg: 3 }}>
                        <Paper p="sm" onClick={() => handleCategoryClick(category)} withBorder style={{cursor:"pointer"}}>
                            <Group wrap="nowrap">
                                <Image
                                    src={category.thumbnailUri}
                                    height={130}
                                    width={75}
                                    alt="thumbnail image"
                                />
                                <Stack>
                                    <Text fw={500}>{category.name}</Text>
                                    <Text size="sm" c="dimmed" lineClamp={4}>{category.description}</Text>
                                </Stack>
                            </Group>
                        </Paper>
                    </Grid.Col>
                )}

                {/*<Grid.Col span={12}  >*/}
                {/*    <Title order={5}>Product Definitions</Title>*/}
                {/*</Grid.Col>*/}

                {productDefinitions.map(productDefinition =>
                    <Grid.Col key={productDefinition.id} span={{ base: 12, md: 6, lg: 3 }}>
                        <Paper p="sm" onClick={() => handleProductDefinitionClick(productDefinition)} withBorder style={{cursor:"pointer"}}>
                            <Group wrap="nowrap">
                                <Image
                                    src={productDefinition.thumbnailUri}
                                    height={130}
                                    width={75}
                                    alt="thumbnail image"
                                />
                                <Stack>
                                    <Text fw={500}>{productDefinition.name}</Text>
                                    <Text size="sm" c="dimmed" lineClamp={4}>{productDefinition.description}</Text>
                                </Stack>
                            </Group>
                        </Paper>
                    </Grid.Col>
                )}
            </Grid>
        </>
    );

}


