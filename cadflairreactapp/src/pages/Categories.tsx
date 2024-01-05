import { Image, Text, Group, Grid, Paper, Stack, Title } from '@mantine/core';
import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import Category from '../interfaces/Category.interface';
import ProductDefinition from '../interfaces/ProductDefinition.interface';
import Subscription from '../interfaces/Subscription.interface';



export default function Categories() {
    const params = useParams();
    const navigate = useNavigate();
    const [categories, setCategories] = useState<Category[]>([]);
    const [productDefinitions, setProductDefinitions] = useState<ProductDefinition[]>([]);
    const [subscription, setSubscription] = useState<Subscription>();

    useEffect(() => {

        // get subscription from api
        fetch(`/api/v1/subscriptions/${params.companyName}`)
            .then((response) => response.json())
            .then((subscription:Subscription) => {
                //console.log(subscription);
                setSubscription(subscription);

                if (params.categoryName) {
                    // get single category from api
                    fetch(`/api/v1/subscriptions/${subscription.id}/categories/${params.categoryName}`)
                        .then((response) => response.json())
                        .then((category: Category) => {
                            //console.log(category);
                            setCategories(category.childCategories);

                            // get product definitions for this category
                            fetch(`/api/v1/subscriptions/${subscription.id}/categories/${category.id}/product-definitions`)
                                .then((response) => response.json())
                                .then((productDefinitions: ProductDefinition[]) => {
                                    console.log(productDefinitions);
                                    setProductDefinitions(productDefinitions);
                                })
                                .catch((err) => {
                                    console.log(err.message);
                                });
                        })
                        .catch((err) => {
                            console.log(err.message);
                        });

                }
                else {
                    // get root categories from api
                    fetch(`/api/v1/subscriptions/${subscription.id}/categories/`)
                        .then((response) => response.json())
                        .then((categories: Category[]) => {
                            //console.log(categories);
                            setCategories(categories);
                            setProductDefinitions([]);
                        })
                        .catch((err) => {
                            console.log(err.message);
                        });
                }
            })
            .catch((err) => {
                console.log(err.message);
            });
    }, [params.categoryName, params.companyName, params.subscriptionId]);

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
                {productDefinitions?.map(productDefinition =>
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


